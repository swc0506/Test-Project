using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
[ExecuteInEditMode]
#endif
public class XCameraScrollScript : MonoBehaviour {

    public GameObject CameraObject;
    public float fSpeed = 0;
    public float fOffset = 0;
    public float fTargetX;
    float fVec;

    [HideInInspector]
    [System.NonSerialized]
    protected Vector3 Pos;

    [HideInInspector]
    [System.NonSerialized]
    public bool  bPauseScroll = true;

    // Use this for initialization
    void Start () {
        if( CameraObject )
        {
            Pos = gameObject.transform.localPosition;
        } 
        else
        {
            CameraObject = GameObject.Find("Main Camera");
            Pos = gameObject.transform.localPosition;
        }

        fTargetX = CameraObject.transform.position.x;
    }
	
    public void Init () {
        if (CameraObject)
        {
            Pos = gameObject.transform.localPosition;
        }
    }

    public void UpdateForce()
    {
        if (CameraObject)
        {
            //newPos.x += CameraObject.transform.position.x * fSpeed + fOffset;
            //Vector3 delta = new Vector3(CameraObject.transform.position.x * fSpeed + fOffset, 0, 0);
            //delta = gameObject.transform.InverseTransformVector(delta);
            //gameObject.transform.localPosition = gameObject.transform.localPosition + delta;
            Pos = gameObject.transform.localPosition;
            fOffset = CameraObject.transform.position.x;
        }
    }

    public void ForceUpdate()
    {
        if (CameraObject/* && CameraObject.transform.hasChanged*/)
        {
            fTargetX = CameraObject.transform.position.x;
            Vector3 delta = new Vector3((fTargetX - fOffset) * fSpeed, 0, 0);
            delta = gameObject.transform.InverseTransformVector(delta);
            gameObject.transform.localPosition = Pos + delta;
        }
    }
	// Update is called once per frame 
	void Update () {

#if UNITY_EDITOR
        if(bPauseScroll && EditorApplication.isPlaying == false)
        {
            return;
        }
#endif
        if (CameraObject/* && CameraObject.transform.hasChanged*/)
        {
            fTargetX = Mathf.SmoothDamp(fTargetX, CameraObject.transform.position.x, ref fVec, 0.03f);
            //newPos.x += CameraObject.transform.position.x * fSpeed + fOffset;
            Vector3 delta = new Vector3((fTargetX - fOffset) * fSpeed, 0, 0);
            delta = gameObject.transform.InverseTransformVector(delta);
            gameObject.transform.localPosition = Pos + delta;
        }
	}
}
