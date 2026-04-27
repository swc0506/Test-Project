using UnityEngine;
using ZM.AssetFrameWork;

public class ExShopItem : MonoBehaviour
{
    public Transform gameItemParent;
    public GameObject loading;
    private GameObject mItemObj;

    public void SetData(int itemId)
    {
        AssetsFrame.InstantiateAndLoad("Assets/Demo/GameItem/" + itemId + "/" + itemId, LoadItemObjComplete, ItemObjLoading);
    }

    private void LoadItemObjComplete(GameObject itemObj, object param1, object param2)
    {
        loading.SetActive(false);
        if (itemObj != null)
        {
            itemObj.SetActive(true);
            itemObj.transform.SetParent(gameItemParent);
            itemObj.transform.localPosition = Vector3.zero;
            itemObj.transform.localRotation = Quaternion.identity;
            itemObj.transform.localScale = Vector3.one;
            mItemObj = itemObj;
        }
    }

    private void ItemObjLoading()
    {
        loading.SetActive(true);
    }

    public void Release()
    {
        if (mItemObj != null)
        {
            AssetsFrame.Release(mItemObj, true);
            mItemObj = null;
        }
        AssetsFrame.Release(gameObject, true);
    }
}
