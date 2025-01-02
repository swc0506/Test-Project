using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public enum E_Node_Type
{
    Walk,
    Stop
}

public enum E_Node_Color
{
    Green,
    Purple
}

public class AStarNode
{
    //格子坐标
    public int x;
    public int y;
    
    public float f;
    //离起点距离
    public float g;
    //离终点距离
    public float h;
    //父对象
    public AStarNode father;
    //反链父对象
    public AStarNode reFather;
    //格子类型
    public E_Node_Type type;
    public E_Node_Color color;
    
    public AStarNode(int x, int y, E_Node_Type type)
    {
        this.x = x;
        this.y = y;
        this.type = type;
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        
        AStarNode usr = obj as AStarNode;
        if (usr == null)
            return false;
        else
        {
            return Equals(usr);
        }
    }

    public override int GetHashCode()
    {
        return this.x.GetHashCode() ^ this.y.GetHashCode();
    }

    private bool Equals(AStarNode node)
    {
        if (node == null) return false;
        return (node.x == this.x && node.y == this.y);
    }
}
