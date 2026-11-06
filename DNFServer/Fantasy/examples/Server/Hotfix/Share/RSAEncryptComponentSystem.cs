using Fantasy;
using Model.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotfix.Share
{
    
    /// <summary>
    /// Rsa加密解密逻辑类
    /// </summary>
    public static class RSAEncryptComponentSystem
    {
        /// <summary>
        /// 加密密码
        /// </summary>
        /// <param name="self"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static string EncryptPassWorld(this RSAEncryptComponent self,string pass)
        {
           return RSAEncryptHelper.RSAEncrypt(self.publicKey,pass);
        }
        /// <summary>
        /// 解密密码
        /// </summary>
        /// <param name="self"></param>
        /// <param name="pass"></param>
        /// <param name="encryptPass"></param>
        /// <returns></returns>
        public static bool VerifyPassWorld(this RSAEncryptComponent self, string pass,string encryptPass)
        {
            string dePass=  RSAEncryptHelper.RSADecrypt(self.privateKey, encryptPass);
            return string.Equals(pass, dePass);
        }

        /// <summary>
        /// 生成Token
        /// </summary>
        /// <param name="self"></param>
        /// <param name="accountId"></param>
        /// <param name="sceneCfgid"></param>
        /// <returns></returns>
        public static string GenerateToken(this RSAEncryptComponent self, long accountId,long sceneCfgid)
        {
            string encryptString = $"{accountId},{sceneCfgid}";
            string token = RSAEncryptHelper.RSAEncrypt(self.publicKey, encryptString); 
            Log.Info("Encrypt Token:"+ token);
            return token;
        }
        /// <summary>
        /// 校验Token
        /// </summary>
        /// <param name="self"></param>
        /// <param name="token"></param>
        /// <param name="accountId"></param>
        /// <param name="sceneCfgid"></param>
        /// <returns></returns>
        public static bool VerifyToken(this RSAEncryptComponent self,string token, long accountId, long sceneCfgid)
        {
            if (string.IsNullOrEmpty(token))
            {
             return false;
            }
            Log.Info("deDecrypt Token:" + token);
            string originString = $"{accountId},{sceneCfgid}";
            string deDecryptString = RSAEncryptHelper.RSADecrypt(self.privateKey, token);
            return string.Equals(originString, deDecryptString);
        }
    }
}
