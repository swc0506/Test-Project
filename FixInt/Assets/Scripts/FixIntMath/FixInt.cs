using System;
using System.Globalization;

public struct FixInt : IEquatable<FixInt>,IComparable<FixInt>
{
    public const Int64 MaxValue = 9223372036854775807;

    public const Int64 MinValue = -9223372036854775808;

    public static readonly FixInt One = new FixInt(1);
    public static readonly FixInt Zero = new FixInt(0);
    
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
    public const int MUTIPLE = 1024;

    public long Value => value;
    public int IntValue => (int)value;

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
    
    #region 外部方法

    /// <summary>
    /// 是否等于判断
    /// </summary>
    /// <param name="f1"></param>
    /// <returns></returns>
    public readonly bool Equals(FixInt f1)
    {
        return value == f1.value;
    }
    /// <summary>
    /// 是否等于判断
    /// </summary>
    /// <param name="obj">任意数值类型</param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        return value == ((FixInt)obj).value;
    }
    /// <summary>
    /// 获取唯一的HashCode码
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return value.GetHashCode();
    }

    public override string ToString()
    {
        return RawFloat.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// 将当前实例与另一个对象进行比较，返回值表示当前实例的值是大于另一个实例的值还是小于 
    /// </summary>
    /// <param name="f1"></param>
    /// <returns>返回值：小于0 表示当前实例小于目标值，等于0说明当前值与目标值相等，大于0表示当前值大于目标值</returns>
    public readonly int CompareTo(FixInt f1)
    {
        return value.CompareTo(f1.value);
    }
    /// <summary>
    /// 将当前实例与另一个对象进行比较，返回值表示当前实例的值是大于另一个实例的值还是小于 
    /// </summary>
    /// <param name="f1"></param>
    /// <returns>返回值：小于0 表示当前实例小于目标值，等于0说明当前值与目标值相等，大于0表示当前值大于目标值</returns>
    public readonly int CompareTo(object f1)
    {
        return value.CompareTo(((FixInt)f1).value);
    }
    #endregion
}
