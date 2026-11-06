using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AlertBox : MonoBehaviour {

    public Text title;
    public Text msg;

    public void SetMessage(string msg)
    {
        this.msg.text = msg;
    }

    public void Close()
    {
        Destroy(gameObject);
    }

}
