using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAStar : MonoBehaviour
{
    public int beginX = -3;
    public int beginY = 5;
    public int offsetX = 2;
    public int offsetY = -2;
    public int mapW = 5;
    public int mapH = 5;

    public Material red;
    public Material yellow;
    public Material purple;
    public Material green;
    public Material normal;

    private Dictionary<string, GameObject> _cubes = new Dictionary<string, GameObject>();
    private Vector2 _beginPos = Vector2.right * -1;
    private Vector2 _endPos;
    private List<AStarNode> _list = new List<AStarNode>();
    
    // Start is called before the first frame update
    void Start()
    {
        AStarMgr.Instance.InitMapInfo(mapW, mapH);
        for (int i = 0; i < mapW; ++i)
        {
            for (int j = 0; j < mapH; ++j)
            {
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.transform.position = new Vector3(beginX + i * offsetX, beginY + j * offsetY, 0);

                obj.name = i + "_" + j;
                _cubes.Add(obj.name, obj);
                
                AStarNode node = AStarMgr.Instance.nodes[i, j];
                if (node.type == E_Node_Type.Stop)
                {
                    obj.GetComponent<MeshRenderer>().material = red;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if(_list != null)
                _list.Clear();
            _beginPos = Vector2.right * -1;
            InitCube();
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            //射线检测
            RaycastHit info;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out info, 1000))
            {
                if (_beginPos == Vector2.right * -1)
                {
                    //清理路径
                    if (_list != null)
                    {
                        for (int i = 0; i < _list.Count; ++i)
                        {
                            _cubes[_list[i].x + "_" + _list[i].y].GetComponent<MeshRenderer>().material = normal;
                        }
                    }
                    
                    string[] strs = info.collider.gameObject.name.Split('_');
                    info.collider.gameObject.GetComponent<MeshRenderer>().material = yellow;
                    _beginPos = new Vector2(int.Parse(strs[0]), int.Parse(strs[1]));
                }
                else
                {
                    string[] strs = info.collider.gameObject.name.Split('_');
                    info.collider.gameObject.GetComponent<MeshRenderer>().material = purple;
                    _endPos = new Vector2(int.Parse(strs[0]), int.Parse(strs[1]));
                    
                    //寻路
                    _list = AStarMgr.Instance.FindPath(_beginPos, _endPos);
                    _cubes[(int)_beginPos.x + "_" + (int)_beginPos.y].GetComponent<MeshRenderer>().material = normal;
                    if (_list != null)
                    {
                        for (int i = 0; i < _list.Count; ++i)
                        {
                            _cubes[_list[i].x + "_" + _list[i].y].GetComponent<MeshRenderer>().material = _list[i].color == E_Node_Color.Green ? green : purple;
                        }
                    }
                    _beginPos = Vector2.right * -1;
                }
            }
        }
    }

    private void InitCube()
    {
        AStarMgr.Instance.RangeMap();
        for (int i = 0; i < mapW; ++i)
        {
            for (int j = 0; j < mapH; ++j)
            {
                AStarNode node = AStarMgr.Instance.nodes[i, j];
                if (node.type == E_Node_Type.Stop)
                {
                    _cubes[i + "_" + j].GetComponent<MeshRenderer>().material = red;
                }
                else
                {
                    _cubes[i + "_" + j].GetComponent<MeshRenderer>().material = normal;
                }
            }
        }
    }
}
