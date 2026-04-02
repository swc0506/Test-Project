using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson137 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点一 表面着色器中的输入结构体指什么
        //我们上节课主要学习了编译指令
        //#pragma surface 表面函数名 光照模型 可选额外参数
        //其中在讲解表面函数名时提到，需要按规则声明函数
        //函数的参数有三种固定格式
        //1. void 表面函数名(Input IN, inout SurfaceOutput o)
        //2. void 表面函数名(Input IN, inout SurfaceOutputStandard o)
        //3. void 表面函数名(Input IN, inout SurfaceOutputStandardSpecular o)
        //而其中的这个Input结构体，就是我们的输入结构体
        //它是由我们自己声明的，是数据的来源，我们需要用到这些数据进行逻辑处理
        #endregion

        #region 知识点二 输入结构体中我们能声明哪些成员
        //Input结构体虽然是需要我们自己声明的结构体
        //但是我们只要在其中按照规定声明成员变量，便能获取到指定的表面属性
        //注意：
        //1.如果我们在可选额外参数中自定义了顶点修改函数，该结构体还会是顶点修改函数的输出结构体
        //2.除了规范好的参数，我们还可以自己添加自定义参数，比如在自定义顶点修改函数中进行赋值
        //3.在顶点/片元着色器中，这些参数往往需要我们手动计算，而在表面着色器中你可以理解为Unity内部已经
        //  帮助我们计算好了，直接拿来用即可
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}