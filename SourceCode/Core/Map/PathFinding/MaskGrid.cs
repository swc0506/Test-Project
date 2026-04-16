//#if MAP

using System.Collections.Generic;
using UnityEngine;

namespace Core.Map
{
    public enum DiagonalMovement
    {
        None,
        Always,
        Never,
        IfAtMostOneObstacle,
        OnlyWhenNoObstacles,
    }

    public class MaskNode : Node
    {
        public bool IsWalkable { get; private set; }
        private int g;

        public int G
        {
            get { return g; }
            set
            {
                g = value;
                F = g + h;
            }
        }

        private int h;

        public int H
        {
            get { return h; }
            set
            {
                h = value;
                F = g + h;
            }
        }

        public int F { get; private set; }

        public MaskNode parent;

        public MaskNode(int indexRow, int indexColumn, bool isWalkable) : base(indexRow, indexColumn)
        {
            IsWalkable = isWalkable;
        }

        public void SetWalkable(bool isWalkable)
        {
            IsWalkable = isWalkable;
        }

        public void Reset()
        {
            parent = null;
            g = 0;
            h = 0;
            F = 0;
        }
    }

    public class MaskData : GridData
    {
        public int[][] nodes;
    }

    public class MaskGrid : Grid<MaskNode, MaskData>
    {
        protected override MaskNode CreateNode(int indexRow, int indexColumn, MaskData data)
        {
            bool isWalkable = null == data || data.nodes[indexRow][indexColumn] == 1;
            return new MaskNode(indexRow, indexColumn, isWalkable);
        }

        public bool IsWalkableAt(int indexRow, int indexColumn)
        {
            MaskNode node = GetNode(indexRow, indexColumn);
            return null != node && node.IsWalkable;
        }

        private bool TryAddNeighborsNode(int indexRow, int indexColumn, ref List<MaskNode> neighbors)
        {
            if (IsWalkableAt(indexRow, indexColumn))
            {
                neighbors.Add(nodes[indexRow][indexColumn]);
                return true;
            }

            return false;
        }

        public void GetNeighbors(MaskNode maskNode, DiagonalMovement movement, ref List<MaskNode> neighbors)
        {
            neighbors.Clear();
            bool s0 = false, s1 = false, s2 = false, s3 = false;
            bool d0 = false, d1 = false, d2 = false, d3 = false;
            // ↑
            if (maskNode.IndexRow - 1 >= 0)
            {
                s0 = TryAddNeighborsNode(maskNode.IndexRow - 1, maskNode.IndexColumn, ref neighbors);
            }

            // →
            s1 = TryAddNeighborsNode(maskNode.IndexRow, maskNode.IndexColumn + 1, ref neighbors);
            // ↓
            s2 = TryAddNeighborsNode(maskNode.IndexRow + 1, maskNode.IndexColumn, ref neighbors);
            // ←
            if (maskNode.IndexColumn - 1 >= 0)
            {
                s3 = TryAddNeighborsNode(maskNode.IndexRow, maskNode.IndexColumn - 1, ref neighbors);
            }

            if (movement == DiagonalMovement.Never)
            {
                return;
            }

            if (movement == DiagonalMovement.OnlyWhenNoObstacles)
            {
                d0 = s3 && s0;
                d1 = s0 && s1;
                d2 = s1 && s2;
                d3 = s2 && s3;
            }
            else if (movement == DiagonalMovement.IfAtMostOneObstacle)
            {
                d0 = s3 || s0;
                d1 = s0 || s1;
                d2 = s1 || s2;
                d3 = s2 || s3;
            }
            else if (movement == DiagonalMovement.Always)
            {
                d0 = true;
                d1 = true;
                d2 = true;
                d3 = true;
            }

            // ↖
            if (d0)
            {
                TryAddNeighborsNode(maskNode.IndexRow - 1, maskNode.IndexColumn - 1, ref neighbors);
            }

            // ↗
            if (d1)
            {
                TryAddNeighborsNode(maskNode.IndexRow - 1, maskNode.IndexColumn + 1, ref neighbors);
            }

            // ↘
            if (d2)
            {
                TryAddNeighborsNode(maskNode.IndexRow + 1, maskNode.IndexColumn + 1, ref neighbors);
            }

            // ↙
            if (d3)
            {
                TryAddNeighborsNode(maskNode.IndexRow + 1, maskNode.IndexColumn - 1, ref neighbors);
            }
        }

        public void ResetNodesCost()
        {
            if (null != nodes)
            {
                for (int i = 0; i < Row; i++)
                {
                    for (int j = 0; j < Column; j++)
                    {
                        nodes[i][j].Reset();
                    }
                }
            }
        }
    }
}
//#endif