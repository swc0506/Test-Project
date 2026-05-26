using UnityEngine;
using UnityEngine.Events;

public class DmgCheck : MonoBehaviour
{
    // 检查回调
    public event UnityAction<int> checkCallback;
    
    public void Check(int id)
    {
        checkCallback?.Invoke(id);
    }
}
