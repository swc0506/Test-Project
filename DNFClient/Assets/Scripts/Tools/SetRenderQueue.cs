using UnityEngine;
using System.Collections.Generic;
[ExecuteInEditMode]
public class SetRenderQueue : MonoBehaviour 
{
    public int mRendererQueue;
	Renderer[] mRd;
    List<Material> mMatLst = new List<Material>();
    void Start()
	{
		mRd = GetComponentsInChildren<Renderer>();


        if (mRd != null && mRd.Length > 0)
        {
            for (int i = 0, icnt = mRd.Length; i < icnt; ++i)
            {
                Renderer tmpR = mRd[i];
                if (null == tmpR) continue;
                if (null == tmpR.sharedMaterials) continue;

                for (int j = 0, jcnt = tmpR.sharedMaterials.Length; j < jcnt; ++j)
                {
                    Material tmpM = tmpR.sharedMaterials[j];

                    if (null == tmpM)
                    {
                        //Debug.Log("Game object: \"" + gameObject.name + "\" Material missing!");
                        continue;
                    }

                    mMatLst.Add(tmpM);
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () 
	{
		//有问题，临时注掉
		// return;
		if(mRd != null && mRd.Length>0)
		{

            for(int i = 0,icnt = mMatLst.Count;i<icnt;++i)
            {
                Material tmpM = mMatLst[i];
                if (null == tmpM) continue;
                if (null != tmpM.shader)
                    tmpM.renderQueue = tmpM.shader.renderQueue + mRendererQueue;
                //else
                //    Debug.Log("Game object: \"" + gameObject.name + "\" Material missing!");
            }


            //for(int i = 0,icnt = mRd.Length;i<icnt;++i)
            //{
            //    Renderer tmpR = mRd[i];
            //    if(null == tmpR) continue;
            //    if (null == tmpR.sharedMaterials) continue;
            //
            //    for (int j = 0 ,jcnt = tmpR.sharedMaterials.Length;j<jcnt;++j)
            //    {
            //        Material tmpM = tmpR.sharedMaterials[j];
            //
            //        if (null == tmpM)
            //        {
            //            Debug.Log("Game object: \"" + gameObject.name + "\" Material missing!");
            //            continue;
            //        }
            //        if (null != tmpM.shader)
            //            tmpM.renderQueue = tmpM.shader.renderQueue + mRendererQueue;
            //        else
            //            Debug.Log("Game object: \"" + gameObject.name + "\" Material missing!");
            //    }
            //}

			//foreach(Renderer tmpR in mRd)
			//{
			//	if(tmpR!=null && tmpR.sharedMaterials!=null && tmpR.sharedMaterials.Length>0)
			//	{
			//		foreach(Material tmpM in tmpR.sharedMaterials)
			//		{
			//			if (null == tmpM)
			//			{
			//				Debug.Log("Game object: \"" + gameObject.name + "\" Material missing!");
			//				continue;
			//			}
            //            if(null != tmpM.shader)
            //                tmpM.renderQueue =tmpM.shader.renderQueue+mRendererQueue;
            //            else
			//			    Debug.Log("Game object: \"" + gameObject.name + "\" Material missing!");
			//		}
			//	}
			//}
		}
	
	}
}
