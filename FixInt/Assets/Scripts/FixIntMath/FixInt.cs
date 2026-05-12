using System;
using System.Globalization;
using UnityEngine;

public struct FixInt
{
    /// <summary>
    /// 当前定点数对应的真实数值
    /// </summary>
    private readonly long value;
    
    /// <summary>
    /// 左移或又移次数
    /// </summary>
    private const int SHIFT = 10;

    /// <summary>
    /// 放大倍率
    /// </summary>
    private const int MUTIPLE = 1024;

    public long Value => value;
    /// <summary>
    /// 真实数值只能用于表现
    /// </summary>
    public float RawFloat => (float)(Math.Round(value / 1024.0f * 100) / 100); //精度为两位

    public double RawDouble => (double)value / 1024.0d;
    
    public int RawInt => (int)(value >> SHIFT);

    public FixInt(float value)
    {
        this.value = (long)(value * MUTIPLE);
    }
    
    public FixInt(double value)
    {
        this.value = (long)(value * MUTIPLE);
    }
    
    public FixInt(int value)
    {
        this.value = value << SHIFT;
    }
    
    /// <summary>
    /// 直接使用long值初始化定点数
    /// </summary>
    /// <param name="value"></param>
    public FixInt(long value)
    {
        this.value = value;
    }
    
    public override string ToString()
    {
        return RawFloat.ToString(CultureInfo.InvariantCulture);
    }

    #region 隐式转换

    /// <summary>
    /// float赋值时转换
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static implicit operator FixInt(float v)
    {
        return new FixInt(v);
    }
    
    public static implicit operator FixInt(int v)
    {
        return new FixInt(v);
    }
    
    public static implicit operator FixInt(long v)
    {
        return new FixInt(v);
    }
    
    public static implicit operator FixInt(double v)
    {
        return new FixInt(v);
    }

    #endregion

    #region 显式转换
    
    public static explicit operator float(FixInt v)
    {
        return v.RawFloat;
    }
    
    public static explicit operator long(FixInt v)
    {
        return v.value;
    }
    
    public static explicit operator int(FixInt v)
    {
        return v.RawInt;
    }
    
    public static explicit operator double(FixInt v)
    {
        return v.RawDouble;
    }

    #endregion

    #region 运算符

    public static FixInt operator +(FixInt a, FixInt b)
    {
        return new FixInt(a.value + b.value);
    }
    
    public static FixInt operator +(FixInt a, int b)
    {
        return a + (FixInt)b;
    }
    
    public static FixInt operator +(FixInt a, float b)
    {
        return a + (FixInt)b;
    }
    
    public static FixInt operator +(FixInt a, double b)
    {
        return a + (FixInt)b;
    }
    
    public static FixInt operator -(FixInt a, FixInt b)
    {
        return new FixInt(a.value - b.value);
    }
    
    public static FixInt operator -(FixInt a, int b)
    {
        return a - (FixInt)b;
    }
    
    public static FixInt operator -(FixInt a, float b)
    {
        return a - (FixInt)b;
    }
    
    public static FixInt operator -(FixInt a, double b)
    {
        return a - (FixInt)b;
    }
    
    public static FixInt operator *(FixInt a, FixInt b)
    {
        return new FixInt((a.value * b.value) >> SHIFT);
    }
    
    public static FixInt operator *(FixInt a, int b)
    {
        return a * (FixInt)b;
    }
    
    public static FixInt operator *(FixInt a, float b)
    {
        return a * (FixInt)b;
    }
    
    public static FixInt operator *(FixInt a, double b)
    {
        return a * (FixInt)b;
    }
    
    public static FixInt operator /(FixInt a, FixInt b)
    {
        long va = a.value << SHIFT;
        return new FixInt(va / b.value);
    }
    
    public static FixInt operator /(FixInt a, int b)
    {
        return a / (FixInt)b;
    }
    
    public static FixInt operator /(FixInt a, float b)
    {
        return a / (FixInt)b;
    }
    
    public static FixInt operator /(FixInt a, double b)
    {
        return a / (FixInt)b;
    }

    #region 操作符 == != >= <= > < % - << >>
    
    public static bool operator ==(FixInt f1, FixInt f2)
    {
        return f1.value == f2.value;
    }
    public static bool operator ==(FixInt f1, int f2)
    {
        return f1 == (FixInt)f2;
    }
    public static bool operator ==(FixInt f1, float f2)
    {
        return f1 == (FixInt)f2;
    }
    public static bool operator ==(FixInt f1, double f2)
    {
        return f1 == (FixInt)f2;
    }
    
    public static bool operator !=(FixInt f1, FixInt f2)
    {
        return f1.value != f2.value;
    }
    public static bool operator !=(FixInt f1, int f2)
    {
        return f1 != (FixInt)f2;
    }
    public static bool operator !=(FixInt f1, float f2)
    {
        return f1 != (FixInt)f2;
    }
    public static bool operator !=(FixInt f1, double f2)
    {
        return f1 != (FixInt)f2;
    }
    
    public static bool operator >=(FixInt f1, FixInt f2)
    {
        return f1.value >= f2.value;
    }
    public static bool operator >=(FixInt f1, int f2)
    {
        return f1 >= (FixInt)f2;
    }
    public static bool operator >=(FixInt f1, float f2)
    {
        return f1 >= (FixInt)f2;
    }
    public static bool operator >=(FixInt f1, double f2)
    {
        return f1 >= (FixInt)f2;
    }
    
    public static bool operator <=(FixInt f1, FixInt f2)
    {
        return f1.value <= f2.value;
    }
    public static bool operator <=(FixInt f1, int f2)
    {
        return f1 <= (FixInt)f2;
    }
    public static bool operator <=(FixInt f1, float f2)
    {
        return f1 <= (FixInt)f2;
    }
    public static bool operator <=(FixInt f1, double f2)
    {
        return f1 <= (FixInt)f2;
    }
    
    public static bool operator >(FixInt f1, FixInt f2)
    {
        return f1.value > f2.value;
    }
    public static bool operator >(FixInt f1, int f2)
    {
        return f1 > (FixInt)f2;
    }
    public static bool operator >(FixInt f1, float f2)
    {
        return f1 > (FixInt)f2;
    }
    public static bool operator >(FixInt f1, double f2)
    {
        return f1 > (FixInt)f2;
    }
    
    public static bool operator <(FixInt f1, FixInt f2)
    {
        return f1.value < f2.value;
    }
    public static bool operator <(FixInt f1, int f2)
    {
        return f1 < (FixInt)f2;
    }
    public static bool operator <(FixInt f1, float f2)
    {
        return f1 < (FixInt)f2;
    }
    public static bool operator <(FixInt f1, double f2)
    {
        return f1 < (FixInt)f2;
    }
    
    public static FixInt operator %(FixInt f1, FixInt f2)
    {
        return new FixInt(f1.value % f2.value);
    }
    public static FixInt operator -(FixInt f1)
    {
        return new FixInt(-f1.value);
    }
    public static FixInt operator <<(FixInt f1, int count)
    {
        return new FixInt(f1.value << count);
    }
    public static FixInt operator >>(FixInt f1, int count)
    {
        return new FixInt(f1.value >> count);
    }

    #endregion

    #endregion
}
