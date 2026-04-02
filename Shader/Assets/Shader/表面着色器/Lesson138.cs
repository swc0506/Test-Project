using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson138 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识点一 表面着色器中的输出结构体指什么
        //我们上节课主要学习了编译指令
        //#pragma surface 表面函数名 光照模型 可选额外参数
        //其中在讲解表面函数名时提到，需要按规则声明函数
        //函数的参数有三种固定格式
        //1. void 表面函数名(Input IN, inout SurfaceOutput o)
        //2. void 表面函数名(Input IN, inout SurfaceOutputStandard o)
        //3. void 表面函数名(Input IN, inout SurfaceOutputStandardSpecular o)
        //而其中的SurfaceOutput、SurfaceOutputStandard、SurfaceOutputStandardSpecular结构体
        //就是我们的输出结构体

        //我们可以利用上节课中学习的输入结构体Input中的数据在表面函数中进行计算后
        //将计算结果赋值存储在输出结构体中
        //之后Unity会利用我们输出的数据作为光照模型函数的输入来进行各种光照相关的计算
        //这三个输出结构体是由Unity提前声明好的，不能自己增加和减少
        //如果我们没有对其中的某些变量赋值，会使用默认值
        #endregion

        #region 知识点二 输出结构体中有哪些成员
        //SurfaceOutput结构体（在Unity内置文件 Lighting.cginc 中声明）
        //当我们在编译指令中使用 Lambert 和 BlinnPhong 光照模型时，就需要使用该结构体
        //struct SurfaceOutput
        //{
        //    fixed3 Albedo;    //漫反射
        //    fixed3 Normal;    //切线空间法线
        //    fixed3 Emission;  //自发光：一般Unity会在片元着色器最后输出前 在输出颜色上直接叠加自发光颜色
        //    half Specular;    //镜面反射指数，范围0~1
        //    fixed Gloss;      //镜面反射强度
        //    镜面相关的两个参数，如果使用了BlinnPhong光照模型
        //    会使用该公式计算高光反射强度：float spec = pow(nh, s.Specular*128) * s.Gloss;
        //    fixed Alpha;      //透明通道：如果开启了透明度的话，会用该值和颜色进行混合
        //}

        //SurfaceOutputStandard结构体（在Unity内置文件 UnityPBSLighting.cginc 中声明）
        //当我们在编译指令中使用 Standard 光照模型时，就需要使用该结构体
        //我们一般称它为金属工作流输出结构体
        //struct SurfaceOutputStandard
        //{
        //    fixed3 Albedo;    //漫反射
        //    fixed3 Normal;    //切线空间法线
        //    fixed3 Emission;  //自发光
        //    half Metallic;    //0表示非金属，1表示金属
        //    half Smoothness;  //0表示最粗糙，1表示最光滑
        //    half Occlusion;   //环境光遮蔽，默认为1
        //    fixed Alpha;      //透明通道      
        //}

        //SurfaceOutputStandardSpecular结构体（在Unity内置文件 UnityPBSLighting.cginc 中声明）
        //当我们在编译指令中使用 StandardSpecular 光照模型时，就需要使用该结构体
        //我们一般称它为高光工作流输出结构体
        //struct SurfaceOutputStandardSpecular
        //{
        //    fixed3 Albedo;    //漫反射
        //    fixed3 Normal;    //切线空间法线
        //    fixed3 Emission;  //自发光
        //    half Smoothness;  //0表示最粗糙，1表示最光滑
        //    half Occlusion;   //环境光遮蔽，默认为1
        //    fixed Alpha;      //透明通道      
        //}
        #endregion

        #region 知识点三 表面着色器的渲染流程
        //通过这几节课的讲解，我们已经了解了
        //编译指令、结构体、自定义函数（编译指令中的表面函数、光照模型、顶点函数、最终颜色修改函数）
        //我们只需要了解他们的具体作用便可以知道如何编写表面着色器
        //那现在我们通过一张图来了解下
        //表面着色器最终的渲染流程是如何把他们串联起来的
        #endregion

        #region 知识点四 为什么不一开始学习表面着色器
        //表面着色器是顶点/片元着色器的封装，要了解了顶点/片元着色器，才能了解表面着色器的本质原理
        //学会了更“底层”的顶点/片元着色器编写，以后学习其它的Shader语言，才会更加得心应手！
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


