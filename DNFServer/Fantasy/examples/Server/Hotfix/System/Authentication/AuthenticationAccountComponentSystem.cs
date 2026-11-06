using Fantasy;
using Hotfix.Share;
using Model;
using Model.Authentication;
using Model.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix
{
    /// <summary>
    /// 账号业务逻辑类
    /// </summary>
    public static class AuthenticationAccountComponentSystem
    {
        public static async FTask<uint> RegisterAccount(this AuthenticationAccountComponent self,string account,string passworld)
        {
            //1.验证账号密码是否为空
            if (string.IsNullOrEmpty(account)|| string.IsNullOrEmpty(passworld))
            {
                return 1000;//注册失败 账号或密码为空
            }
            //如果账号和密码都不为空情况下，首先在缓存中查询，
            int accountHashCode=account.GetHashCode();


            //2 .1 aaa  2..aaa 
            //1.加异步携程锁

            //结束条件：1.逻辑执行完成，2.超时

            //解锁：   self.Scene.CoroutineLockComponent.UnLock(lockComponent.CoroutineLockQueue);

            //举个例子，晚上唐老师、我 和初见老师去找肉饼谈事情，但是唐老师是先到的，然后是我，最后是初见老师到的。
            //这个时候由于唐老师是先到的，所以他和肉饼就去屋里谈事情去了，并且把门锁住了，不让我们进，所以我们只能排队在外面等待他们谈完事情把锁解开我们才能进
            //过了一会，唐老师事情谈完了，于是把锁解开走了，那个这时候，由于我是比初见老师早到的，所以就轮到我去和肉饼谈事情了。
            //我进去之后，同样为了防止初见老师进来把我们事情打乱，我也把门关上并上锁，于是初见老师只能在外面等待我事情谈完并解锁出去后，他才能进去谈他的事情。以此循环
            //总结 从无序变有序，从异步变成同步
            using (await self.Scene.CoroutineLockComponent.Lock(accountHashCode, accountHashCode,"Register Account Lock"))
            {
                //2.如果缓存中已经存在这个账号，说明该账号已经被注册过了，返回注册失败 提升性能
                if (self.accountCacheDic.ContainsKey(accountHashCode))
                {
                    return 1001;//该账号已经注册
                }
                //3.二次校验数据库中是否有这个账号
                IDateBase dateBase = self.Scene.World.DateBase;
                bool isExist = await dateBase.Exist<Account>(d => d.account == account);
                if (isExist)
                {
                    return 1001;//该账号已经注册
                }

                //4.进行账号创建
                RSAEncryptComponent rsaComponent = self.Scene.GetComponent<RSAEncryptComponent>();
                Account accountData = Entity.Create<Account>(self.Scene, true);
                accountData.account = account;
                accountData.passWorld = rsaComponent.EncryptPassWorld(passworld);
                accountData.createTime = TimeHelper.Now;

                //5.把账号数据放到缓存中
                self.accountCacheDic.Add(accountHashCode, accountData);
                //6.把该账号储存到数据库中
                await dateBase.Save(accountData);
                //表示注册成功的响应码
                return 0;
            }
        }


        /// <summary>
        /// 判断账户是否能够进行登录
        /// </summary>
        /// <param name="self"></param>
        /// <param name="account"></param>
        /// <param name="passworld"></param>
        /// <returns></returns>
        public static async FTask<(uint erorCode,Account? accountData)> LoginAccount(this AuthenticationAccountComponent self, string account, string passworld)
        {
            //1.验证账号密码是否为空
            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(passworld))
            {
                return (1005,null);//注册失败 账号或密码为空
            }
            //如果账号和密码都不为空情况下，首先在缓存中查询，
            int accountHashCode = account.GetHashCode();

            //总结 从无序变有序，从异步变成同步
            using (await self.Scene.CoroutineLockComponent.Lock(accountHashCode, accountHashCode, "Login Account Lock"))
            {
                IDateBase dateBase = self.Scene.World.DateBase;
                Account accountData = null;
                //2.如果缓存中已经存在这个账号，说明该账号已经被注册过了，返回注册失败 提升性能
                if (!self.accountCacheDic.TryGetValue(accountHashCode, out accountData))
                {

                    //2.二次校验数据库中是否有这个账号
                    accountData= await dateBase.First<Account>(d => d.account == account);
                    if (accountData==null)
                    {
                        return (1006, null);//数据库中不存在这个账号
                    }
                }

                //验证账号密码是否正确

                RSAEncryptComponent rsaComponent = self.Scene.GetComponent<RSAEncryptComponent>();
                if (!rsaComponent.VerifyPassWorld(passworld, accountData.passWorld))
                {
                    //密码不正确
                    return (1006, null);//数据库中不存在这个账号
                }
                //记录上次登录的时间
                accountData.loginTime = TimeHelper.Now;

                //5.把账号数据放到缓存中
                if (accountData==null)
                  self.accountCacheDic.Add(accountHashCode, accountData);

                //6.把该账号储存到数据库中
                await dateBase.Save(accountData);
                //表示登录成功的响应码
                return (0, accountData);
            }
        }
    }
}
