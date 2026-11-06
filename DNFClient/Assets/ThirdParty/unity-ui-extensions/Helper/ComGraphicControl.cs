using UnityEngine;
using System.ComponentModel;

public class ComGraphicControl : MonoBehaviour
{
    public enum GraphicControlEnum
    {
        [Description("高配关闭")]
        High = 0,
        [Description("中配关闭")]
        Mid  = 1,
        [Description("低配关闭")]
        Low  = 2,
        [Description("超低配关闭")]
        VeryLow = 3,

        Defalut = Low
    }

    public GraphicControlEnum controlEnum = GraphicControlEnum.Defalut;

    public string getViewString()
    {
        string viewString = "[LOD]";

        if( (controlEnum == GraphicControlEnum.High) )
        {
            viewString += "|H";
        }
        else if( (controlEnum ==  GraphicControlEnum.Mid) )
        {
            viewString += "|M";
        }
        else if( (controlEnum == GraphicControlEnum.Low) )
        {
            viewString += "|L";
        }
        else if( (controlEnum == GraphicControlEnum.VeryLow))
        {
            viewString += "|V";
        }
        return viewString;
    }
}