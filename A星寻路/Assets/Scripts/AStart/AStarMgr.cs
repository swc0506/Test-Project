using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AStarMgr
{
    private static AStarMgr _instance;

    public static AStarMgr Instance
    {
        get
        {
            if (_instance == null)
                _instance = new AStarMgr();
            return _instance;
        }
    }

    private int _mapW;
    private int _mapH;
    private Vector2 _direction;

    //所有格子坐标
    public AStarNode[,] nodes;

    //开启列表
    private List<AStarNode> _openList;
    //双向列表
    private List<AStarNode> _reOpenList;

    //关闭列表
    private List<AStarNode> _closeList;
    //双向列表
    private List<AStarNode> _reCloseList;
    
    /// <summary>
    /// 初始化地图 -- 随机阻挡
    /// </summary>
    /// <param name="w"></param>
    /// <param name="h"></param>
    public void InitMapInfo(int w, int h)
    {
        this._mapW = w;
        this._mapH = h;
        nodes = new AStarNode[w, h];
        _openList = new List<AStarNode>();
        _closeList = new List<AStarNode>();
        _reOpenList = new List<AStarNode>();
        _reCloseList = new List<AStarNode>();
        
        RangeMap();
    }

    public void RangeMap()
    {
        for (int i = 0; i < _mapW; ++i)
        {
            for (int j = 0; j < _mapH; ++j)
            {
                AStarNode node = new AStarNode(i, j, Random.Range(0, 100) < 20 ? E_Node_Type.Stop : E_Node_Type.Walk);
                nodes[i, j] = node;
            }
        }
    }

    /// <summary>
    /// A星寻路
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <returns></returns>
    public List<AStarNode> FindPath(Vector2 startPos, Vector2 endPos)
    {
        if (startPos.x < 0 || startPos.x > _mapW ||
            startPos.y < 0 || startPos.y > _mapH ||
            endPos.x < 0 || endPos.x > _mapW ||
            endPos.y < 0 || endPos.y > _mapH)
        {
            Debug.Log("超出地图");
            return null;
        }
        
        AStarNode starANode = nodes[(int)startPos.x, (int)startPos.y];
        AStarNode endANode = nodes[(int)endPos.x, (int)endPos.y];
        AStarNode starBNode = nodes[(int)endPos.x, (int)endPos.y];
        AStarNode endBNode = nodes[(int)startPos.x, (int)startPos.y];
        
        AStarNode meetANode;
        AStarNode meetBNode;

        if (starANode.type == E_Node_Type.Stop || endANode.type == E_Node_Type.Stop)
        {
            Debug.Log("节点为类型为不可通行");
            return null;
        }

        _closeList.Clear();
        _openList.Clear();
        _reCloseList.Clear();
        _reOpenList.Clear();
        
        starANode.father = null;
        starANode.f = 0;
        starANode.g = 0;
        starANode.h = 0;
        starBNode.reFather = null;
        starBNode.f = 0;
        starBNode.g = 0;
        starBNode.h = 0;
        _closeList.Add(starANode);
        _reCloseList.Add(starBNode);

        while (true)
        {
            FindNearlyNode(starANode, endANode);
            FindNearlyNode(starBNode, endBNode, true);
            if (_openList.Count == 0 || _reOpenList.Count == 0)
            {
                Debug.Log("死路");
                return null;
            }
            _openList.Sort(SortOpenList);
            _reOpenList.Sort(SortOpenList);
            starANode = _openList[0];
            starBNode = _reOpenList[0];
            
            meetANode = _closeList.Find(node => node.x == starBNode.x && node.y == starBNode.y);
            meetBNode = _reCloseList.Find(node => node.x == starANode.x && node.y == starANode.y);
            
            if (meetANode != null)
                return PaintLine(meetANode, starBNode);
            else if(meetBNode != null)
                return PaintLine(starANode, meetBNode);
            
            _closeList.Add(starANode);
            _reCloseList.Add(starBNode);
            _openList.RemoveAt(0);
            _reOpenList.RemoveAt(0);

            // if (starANode == endANode)
            // {
            //     Debug.Log("非双向A星");
            //     List<AStarNode> path = new List<AStarNode>();
            //     while (starANode.father != null)
            //     {
            //         starANode.color = E_Node_Color.Green;
            //         path.Add(starANode.father);
            //         starANode = starANode.father;
            //     }
            //
            //     path.Reverse();
            //     return path;
            // }
        }
    }

    private void FindNearlyNode(AStarNode starNode, AStarNode endNode, bool isRe = false)
    {
        FindNearlyNodeToOpen(starNode.x - 1, starNode.y - 1, 1.4f, starNode, endNode, isRe);
        FindNearlyNodeToOpen(starNode.x, starNode.y - 1, 1f, starNode, endNode, isRe);
        FindNearlyNodeToOpen(starNode.x + 1, starNode.y - 1, 1.4f, starNode, endNode, isRe);
        FindNearlyNodeToOpen(starNode.x - 1, starNode.y, 1f, starNode, endNode, isRe);
        FindNearlyNodeToOpen(starNode.x + 1, starNode.y, 1f, starNode, endNode, isRe);
        FindNearlyNodeToOpen(starNode.x - 1, starNode.y + 1, 1.4f, starNode, endNode, isRe);
        FindNearlyNodeToOpen(starNode.x, starNode.y + 1, 1f, starNode, endNode, isRe);
        FindNearlyNodeToOpen(starNode.x + 1, starNode.y + 1, 1.4f, starNode, endNode, isRe);
    }

    private void FindNearlyNodeToOpen(int x, int y, float g, AStarNode father, AStarNode end, bool isRe = false)
    {
        if (x < 0 || x >= _mapW ||
            y < 0 || y >= _mapH)
            return;
        
        AStarNode node = nodes[x, y];
        if (isRe)
        {
            if (node == null || node.type == E_Node_Type.Stop ||
                _reCloseList.Contains(node) || _reOpenList.Contains(node))
                return;
        }
        else
        {
            if (node == null || node.type == E_Node_Type.Stop ||
                _closeList.Contains(node) || _openList.Contains(node))
                return;
        }
        
        //计算f值
        if (isRe)
            node.reFather = father;
        else
            node.father = father;
        node.g = father.g + g;

        float disX = Mathf.Abs(end.x - node.x);
        float disY = Mathf.Abs(end.y - node.y);

        float d;
        //曼哈顿距离
        //d = disX + disY;
        
        //直线距离
        //var distance = Mathf.Sqrt(Mathf.Pow(disX, 2) + Mathf.Pow(disY, 2));
        //d = distance;
        
        //切比雪夫距离
        float minXY = Mathf.Min(disX, disY);
        d = disX + disY + (Mathf.Sqrt(2) - 2) * minXY;

        //加权
        float w = 1.0f;
        //动态加权
        if (d > 4)
            w = 2.0f;
        else
            w = 0.8f;

        node.h = d * w;
        node.f = node.g + w * node.h;
        
        if (isRe)
            _reOpenList.Add(node);
        else
            _openList.Add(node);
    }

    private List<AStarNode> PaintLine(AStarNode aNode, AStarNode bNode)
    {
        List<AStarNode> pathPre = new List<AStarNode>();
        List<AStarNode> pathLast = new List<AStarNode>();

        aNode.color = E_Node_Color.Green;
        pathPre.Add(aNode);
        while (aNode.father != null)
        {
            aNode = aNode.father;
            aNode.color = E_Node_Color.Green;
            pathPre.Add(aNode);
        }

        while (bNode.reFather != null)
        {
            bNode = bNode.reFather;
            bNode.color = E_Node_Color.Purple;
            pathLast.Add(bNode);
        }
        
        pathPre.Reverse();
        pathPre.AddRange(pathLast);
        return pathPre;
    }
    
    private int SortOpenList(AStarNode a, AStarNode b)
    {
        if (a.f > b.f)
            return 1;
        else
            return -1;
    }
}
