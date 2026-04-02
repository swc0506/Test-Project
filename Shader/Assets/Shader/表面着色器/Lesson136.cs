using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson136 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #region 知识回顾 表面着色器中的 编译指令
        //表面着色器中
        //最为关键的几个部分为：
        //1.编译指令
        //2.结构体
        //3.自定义函数

        //编译指令是表面着色器中用来和Unity沟通的重要方式
        //通过编译指令，我们可以告诉Unity需要让他做什么和不做什么
        //因为通过上节课我们知道表面着色器只需要实现少量代码
        //大部分代码（比如光照、阴影、反射、折射等）都交给Unity自动生成
        #endregion

        #region 知识点一 表面着色器中 编译指令 的基本构成
        //#pragma surface 表面函数名 光照模型 可选额外参数
        //它是又4个部分

        //1.固定部分 #pragma surface 
        //2.表面函数名
        //3.光照模型
        //4.可选额外参数

        //构成的
        #endregion

        #region 知识点二 第一部分 固定写法
        //#pragma surface 表面函数名 光照模型 可选额外参数
        //中
        //#pragma surface 是固定写法
        //是用于指明该编译指令是用于表面着色器的
        //在他后面我们必须填写表面函数和光照模型
        //还可以填写一些可选参数来控制表面着色器的行为
        #endregion

        #region 知识点三 第二部分 表面函数名
        //#pragma surface 表面函数名 光照模型 可选额外参数
        //中
        //表面函数名可以随意取名，但是需要在后面的代码中有对应名字的函数
        //函数的参数有三种固定格式
        //1. void 表面函数名(Input IN, inout SurfaceOutput o)
        //2. void 表面函数名(Input IN, inout SurfaceOutputStandard o)
        //3. void 表面函数名(Input IN, inout SurfaceOutputStandardSpecular o)
        //其中 Input 为输入结构体，其中可以得到各种表面属性
        //SurfaceOutput、SurfaceOutputStandard 和 SurfaceOutputStandardSpecular
        //是Unity内置的写好的用于输出的结构体，他们分别用于不同的工作流，可以配合不同的光照模型使用
        //我们之后就可以利用Input结构体中的数据进行计算，计算得到的结构赋值给输出结构体o中的成员
        //之后会自动传递给光照函数进行下一步计算（如果我们不自定义，Unity会自动生成计算代码）
        #endregion

        #region 知识点四 第三部分 光照模型
        //#pragma surface 表面函数名 光照模型 可选额外参数
        //中
        //光照模型是用来计算物体表面的光照效果的
        //Unity内置了基于物理的光照模型函数 Standard 和 StandardSpecular
        //还有简单的非基于物理的光照模型函数 Lambert 和 BlinnPhong
        //我们可以直接填写他们来应用对应的光照计算
        //同样我们也可以自定义光照模型计算相关
        //只需要在代码中按照格式书写以下函数

        //不依赖视角的光照模型，比如漫反射
        //half4 Lighting自定义名字 (SurfaceOutput s, half3 lightDir, half atten)
        //依赖视角的光照模型，比如高光反射
        //half4 Lighting自定义名字 (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
        //然后只需要在光照模型处填写 自定义名字 即可
        //就会自动调用函数中的逻辑来处理光照相关逻辑了
        #endregion

        #region 知识点五 第四部分 可选额外参数
        //#pragma surface 表面函数名 光照模型 可选额外参数
        //中
        //可选额外参数包含了很多非常有用的指令类型
        //我们着重提及几个可选参数：
        //1.自定义顶点修改函数 vertex:顶点函数名
        //  void 顶点函数名(inout appdata_full v)
        //2.最终颜色修改函数 finalcolor:自定义最终颜色修改函数名
        //  void 自定义最终颜色修改函数名(Input IN, SurfaceOutput o, inout fixed4 color)
        //3.阴影相关
        //  addshadow 为一些进行了顶点动画、透明度测试的物体显示的进行阴影投射，避免通过FallBack无法准确处理
        //  fullforwardshadows 在前向渲染路径中支持所有光源类型和阴影
        //  noshadow 禁用阴影
        //4.透明相关
        //  alphatest:变量名 利用它可以使用指定名字的变量来剔除不满足条件的片元
        //5.光照
        //  noambient 不使用任何环境光和光照探针
        //  novertexlights 不使用任何逐顶点光照
        //  noforwardadd 去掉所有前向渲染中的额外pass，只会支持一个逐像素平行光，其他光源按照逐顶点huoSH来计算
        //  nofog 关闭雾效处理
        //  nolightmap 关闭光照烘焙处理
        //6.控制代码生成
        //  默认情况下，表面着色器自动生成的代码包含前向渲染路径、延迟渲染路径使用的Pass
        //  这会让Shader文件较大，如果我们明确表面着色器只会在某些渲染路径中使用
        //  可以使用
        //  exclude_path:deferred(排除延迟渲染路径)
        //  exclude_path:forward（排除前向渲染路径）
        //  exclude_path:prepass（排除预通道渲染路径）
        //  来明确告诉Unity，不需要为某些渲染路径生成代码
        #endregion

        #region 总结
        //表面着色器中的编译指令
        //是用于定义着色器的基本行为、配置光照模型，优化渲染流程的
        //#pragma surface 表面函数名 光照模型 可选额外参数

        //#pragma surface：
        //固定写法，表明是表面着色器的编译指令

        //表面函数名：
        //指定一个表面着色器函数的名称，这个函数负责计算物体表面的颜色、法线、反射等特性。
        //Unity会自动生成对应的顶点着色器和片段着色器代码，然后结合指定的光照模型来处理表面渲染。

        //光照模型：
        //指定表面着色器使用的光照计算模型，Unity提供了几种预定义的光照模型，也可以自定义

        //可选额外参数：
        //允许你传递一些额外的编译选项
        //这些选项可以自定义顶点计算、自定义最终颜色计算、控制是否计算阴影、控制透明测试和混合等等
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
