//#if MAP

using UnityEngine;

namespace Core.Map
{
    public class GridData
    {
        public float size;
        public Vector3 pos;
        public int row;
        public int column;
    }

    public abstract class Grid<N, D> where N : Node where D : GridData
    {
        protected float gridSize;
        protected Vector3 originPos;
        protected N[][] nodes;

        public int Row
        {
            get { return null == nodes ? 0 : nodes.Length; }
        }

        public int Column
        {
            get { return null == nodes ? 0 : nodes[0].Length; }
        }

        public float GridSize
        {
            get { return gridSize; }
        }

        public Vector3 OriginPos
        {
            get { return originPos; }
        }

        public void Initial(int row, int column)
        {
            nodes = new N[row][];
            for (int i = 0; i < row; i++)
            {
                nodes[i] = new N[column];
                for (int j = 0; j < column; j++)
                {
                    nodes[i][j] = CreateNode(i, j, null);
                }
            }
        }

        protected abstract N CreateNode(int indexRow, int indexColumn, D data);

        public void Initial(D data)
        {
            nodes = new N[data.row][];
            for (int i = 0; i < data.row; i++)
            {
                nodes[i] = new N[data.column];
                for (int j = 0; j < data.column; j++)
                {
                    nodes[i][j] = CreateNode(i, j, data);
                }
            }

            Vector3 pos = new Vector3(data.pos[0], data.pos[1], data.pos[2]);
            SetGridsPosition(pos, data.size);
        }

        protected bool IsInside(int indexRow, int indexColumn)
        {
            return indexRow < Row && indexColumn < Column;
        }

        public N GetNode(int indexRow, int indexColumn)
        {
            if (IsInside(indexRow, indexColumn))
            {
                return nodes[indexRow][indexColumn];
            }

            return null;
        }

        public N GetNode(Vector3 pos)
        {
            if (null != nodes)
            {
                N originPathNode = nodes[0][0];
                float disRow = (originPathNode.Pos.z + gridSize * 0.5f) - pos.z;
                float disColumn = pos.x - (originPathNode.Pos.x - gridSize * 0.5f);
                int indexRow = Mathf.FloorToInt(disRow / gridSize);
                int indexColumn = Mathf.FloorToInt(disColumn / gridSize);
                return GetNode(indexRow, indexColumn);
            }

            return null;
        }

        public void SetGridsPosition(Vector3 originPos, float gridSize, bool isHex = false)
        {
            this.originPos = originPos;
            if (null != nodes)
            {
                float offsetDist = isHex ? gridSize * 0.5f : 0;
                this.gridSize = gridSize;
                for (int i = 0; i < Row; i++)
                {
                    for (int j = 0; j < Column; j++)
                    {
                        Vector3 pos = originPos + new Vector3((j + 0.5f) * gridSize, 0, -(i + 0.5f) * gridSize);
                        if (isHex)
                        {
                            pos.x += offsetDist;
                        }

                        nodes[i][j].SetPosition(pos);
                    }
                }
            }
        }
    }
}
//#endif