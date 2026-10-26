///------------------
/// 壹叶成名
///------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class  MultipleBloodBars : MonoBehaviour
{
    //如果只有一条血，那么一条血就是所有的血量，如果有多条血，那么一条血就设定为一个固定值

    public Image currentBar;                         //当前血条
    public Image middleBar;                         //过渡血条
    public Image nextBar;                           //下一血条
    public Text countText;                          //剩下的血条数text

    private int count;                              //剩下的血条数(不包括当前血量)
    private float nowBlood;                         //在一条血中的当前血量，如：100/1000则为100  
    private float oneBarBlood = 10000f;             //一条血的容量，如：100/1000则为1000     

    private int colorIndex = 0;
    public Sprite[] colors;                          //血条的颜色，注意Alpha值，默认为0

    private float slowSpeed = 0.1f;                 //受到重伤时( >oneBarBlood)或者处于加血状态，当前血条的流动速度  
    private float quickSpeed = 1f;                  //受到轻伤时( <oneBarBlood)，当前血条的流动速度  
    private float speed;                            //当前血条采用的速度  
    private float middleBarSpeed = 0.1f;            //过渡血条的流动速度  

    private float nowTargetValue;                   //当前血条移动的目标点 
    private float middleTargetValue;                //过渡血条移动的目标点 
    private bool isBloodMove = false;               //控制血条的移动  

    void Update()
    {
        MoveNowBar();                               //当前血条的流动 
        MoveMiddleBar();                            //过渡血条的流动  
        //if (Input.GetMouseButtonDown(0))
        //{
        //    ChangeBlood(-3000);
        //}
        //else if (Input.GetMouseButtonDown(1))
        //{
        //    ChangeBlood(-15000);
        //}
    }
    private void Awake()
    {

    }
    private void Start()
    {
        //初始每一管血条的颜色
        //colors = new Color[] { new Color(0, 1, 1, 1), new Color(0, 1, 0, 1), new Color(0, 0, 1, 1),new Color(1, 1, 0, 1) };

        //InitBlood(2000000);

    }
    /// <summary>  
    /// 传入总血量，初始化血条  
    /// </summary>  
    /// <param name="number"></param>  
    public void InitBlood(float number)
    {
        if (number == 0)
            return;
        
        count = (int)(number / oneBarBlood);//剩下的血条数
        nowBlood = number % oneBarBlood;//最后一条血的当前血量
        if (nowBlood == 0)  //如果最后一条血的血量刚好充满剩余血条数减一
        {
            nowBlood = oneBarBlood;
            count--;
        }

        colorIndex = count % colors.Length;
        currentBar.sprite = colors[colorIndex];
        currentBar.fillAmount = nowBlood / oneBarBlood;

        if (count != 0)
        {
            int nextColorIndex = (colorIndex - 1 + colors.Length) % colors.Length;
                nextBar.sprite = colors[nextColorIndex];
                nextBar.gameObject.SetActive(true);
        }
        else
        {
            nextBar.gameObject.SetActive(false);
        }

        middleBar.gameObject.SetActive(false);

        countText.text ="x"+ (count+1);
    }

    /// <summary>  
    /// 血量变化，并根据伤害判断是否使用过渡血条  
    /// </summary>  
    /// <param name="number"></param>  
    public void ChangeBlood(float number)
    {
        nowBlood += number;
        nowTargetValue = nowBlood / oneBarBlood;
        isBloodMove = true;

        if ((number < 0) && (Mathf.Abs(number) <= oneBarBlood))//处于受伤状态并且伤害量较低时  
        {
            speed = quickSpeed;
            middleBar.gameObject.SetActive(true);
            middleBar.transform.SetSiblingIndex(nextBar.transform.GetSiblingIndex() + 1);
            middleBar.fillAmount = currentBar.fillAmount;
            middleTargetValue = nowTargetValue;
        }
        else//处于受伤状态并且伤害量较大时，或者处于加血状态  
        {
            speed = slowSpeed;
            middleBar.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 普通血条的流动 
    /// </summary>
    void MoveNowBar()
    {
        if (!isBloodMove) return;

        currentBar.fillAmount = Mathf.Lerp(currentBar.fillAmount, nowTargetValue, speed);

        if (Mathf.Abs(currentBar.fillAmount - nowTargetValue) <= 0.01f)//到达目标点  
            isBloodMove = false;
        if (count == 0)
            nextBar.gameObject.SetActive(false);
        else
            nextBar.gameObject.SetActive(true);

        if (currentBar.fillAmount >= nowTargetValue)
            SubBlood();
        else
            AddBlood();
    }

    /// <summary>
    /// 过渡血条的流动  
    /// </summary>
    void MoveMiddleBar()
    {
        //受到轻伤时( <oneBarBlood)，才会出现过渡血条
        if (speed == quickSpeed)
        {
            middleBar.fillAmount = Mathf.Lerp(middleBar.fillAmount, middleTargetValue, middleBarSpeed);
            if (Mathf.Abs(middleBar.fillAmount - 0) < 0.01f)
            {
                middleBar.transform.SetSiblingIndex(nextBar.transform.GetSiblingIndex() + 1);
                middleBar.fillAmount = 1;
                middleTargetValue++;
            }
        }
    }

    void AddBlood()
    {
        float subValue = Mathf.Abs(currentBar.fillAmount - 1);
        if (subValue <= 0.0f)//到达1  
        {
            count++;
            countText.text = "x" + (count + 1);

            currentBar.fillAmount = 0;
            nowTargetValue -= 1;
            nowBlood -= oneBarBlood;

            nextBar.sprite = colors[colorIndex];

            colorIndex++;
            colorIndex %= colors.Length;
            currentBar.sprite = colors[colorIndex];
        }
    }

    void SubBlood()
    {
        float subValue = Mathf.Abs(currentBar.fillAmount - 0);
        if (subValue <= 0.0f)//到达0  
        {
            //当前血条已经流动完，将过渡血条放置最前
            middleBar.transform.SetSiblingIndex(nextBar.transform.GetSiblingIndex() + 2);

            if (count <= 0)
            {
                middleBar.gameObject.SetActive(false);
                Destroy(gameObject);
                return;
            };
            count--;
            countText.text = "x" + (count + 1);

            currentBar.fillAmount = 1;
            nowTargetValue += 1;
            nowBlood += oneBarBlood;

            colorIndex--;
            colorIndex += colors.Length;
            colorIndex %= colors.Length;
            if (count ==0)
            {
                //currentBar.color = Color.red;//当血量为最后一条时 设置血条为红色
            }
            else 
            {
                currentBar.sprite = colors[colorIndex];//根据自设颜色进替换
            }
            int nextColorIndex = colorIndex - 1 + colors.Length;
            nextColorIndex %= colors.Length;
            if (count == 1)
            {
                //nextBar.color = Color.red; //设置最后一个血条的显示为红色
            }
            else 
            {
                nextBar.sprite = colors[nextColorIndex];//根据自设颜色进替换
            }
        }
    }
}
