//#if MAP
using System;
using UnityEngine;

namespace Core.Map
{
    public class Node : IEquatable<Node>
    {
        public int IndexRow { get; private set; }
        public int IndexColumn { get; private set; }
        public Vector3 Pos { get; private set; }

        public Node(int indexRow, int indexColumn)
        {
            IndexRow = indexRow;
            IndexColumn = indexColumn;
        }

        public bool Equals(Node other)
        {
            return IndexRow == other.IndexRow && IndexColumn == other.IndexColumn;
        }

        public virtual void SetPosition(Vector3 pos)
        {
            Pos = pos;
        }
    }
}
//#endif