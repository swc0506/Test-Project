public enum QualityEnum
{
    White = 0,
    Green = 1,
    Blue = 2,
    Purple = 3,
    Orange = 4,
    Red = 5,
}

public class HeroData
{
    public HeroData()
    {

    }
    public int id;
    public string name;
    public QualityEnum quality;
    public string nameChinese;
    public int type;
    public string skillDes;
    
    public int seatId;//位置 座位 id
    public int[] skillidArr;//技能数组
    public int hp;//声明值
    public int atk;//攻击力
    public int def;//防御力
    public int agl;//敏捷
    public int atkRage;//攻击怒气值
    public int takeDamageRage; //受击怒气值
    public int maxRage;//最大怒气
}