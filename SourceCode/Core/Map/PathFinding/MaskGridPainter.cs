#if MAP
using UnityEngine;

namespace Core.Map
{
    public class MaskGridPainter : GridPainter
    {
        public MaskGrid MaskGrid { get; private set; }

        protected override void OnBuildGrids()
        {
            if (null == MaskGrid)
            {
                MaskGrid = new MaskGrid();
            }

            if (MaskGrid.Row != RowCount || MaskGrid.Column != ColumnCount)
            {
                MaskNode[][] rawNodes = new MaskNode[MaskGrid.Row][];
                for (int i = 0; i < MaskGrid.Row; i++)
                {
                    rawNodes[i] = new MaskNode[MaskGrid.Column];
                    for (int j = 0; j < MaskGrid.Column; j++)
                    {
                        MaskNode rawNode = MaskGrid.GetNode(i, j);
                        bool isWalkable = null == rawNode || rawNode.IsWalkable;
                        rawNodes[i][j] = new MaskNode(i, j, isWalkable);
                    }
                }

                MaskGrid.Initial(RowCount, ColumnCount);
                for (int i = 0; i < MaskGrid.Row; i++)
                {
                    for (int j = 0; j < MaskGrid.Column; j++)
                    {
                        MaskNode rawNode = null;
                        if (rawNodes.Length > i && rawNodes[i].Length > j)
                        {
                            rawNode = rawNodes[i][j];
                        }

                        MaskNode node = MaskGrid.GetNode(i, j);
                        bool isWalkable = null == rawNode || rawNode.IsWalkable;
                        node.SetWalkable(isWalkable);
                    }
                }
            }
        }

        public override void UpdatePosition()
        {
            if (null != MaskGrid)
            {
                MaskGrid.SetGridsPosition(transform.position, gridSize);
            }
        }

        public override void SetGridsConfig(string jsonText)
        {
            MaskData data = JsonUtils.ToObject<MaskData>(jsonText);
            if (null != data)
            {
                transform.position = data.pos;
                mapWidth = (int) (data.size * data.column);
                mapLength = (int) (data.size * data.row);
                gridSize = data.size;
                MaskGrid = new MaskGrid();
                MaskGrid.Initial(data);
                UpdateBoxSize();
            }
        }

        public override GridData GenerateGridsData()
        {
            MaskData data = new MaskData();
            data.size = MaskGrid.GridSize;
            data.pos = transform.position;
            data.row = MaskGrid.Row;
            data.column = MaskGrid.Column;
            data.nodes = new int[MaskGrid.Row][];
            for (int i = 0; i < MaskGrid.Row; i++)
            {
                data.nodes[i] = new int[MaskGrid.Column];
                for (int j = 0; j < MaskGrid.Column; j++)
                {
                    data.nodes[i][j] = MaskGrid.GetNode(i, j).IsWalkable ? 1 : 0;
                }
            }

            return data;
        }

        protected override void OnDrawGrids(Vector3 size)
        {
            if (null != MaskGrid)
            {
                for (int i = 0; i < MaskGrid.Row; i++)
                {
                    for (int j = 0; j < MaskGrid.Column; j++)
                    {
                        MaskNode maskNode = MaskGrid.GetNode(i, j);
                        if (null != maskNode)
                        {
                            Gizmos.color = maskNode.IsWalkable ? Color.yellow : Color.red;
                            Gizmos.DrawCube(maskNode.Pos, size);
                        }
                    }
                }
            }
        }
    }
}
#endif