//#if MAP

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Map
{
    public class Heuristic
    {
        public enum Mode
        {
            Manhattan,
            Euclidean,
            Octile,
            Chebyshev
        }

        public static int GetHeuristicValue(Mode mode, int dRow, int dColumn)
        {
            int result = 0;
            switch (mode)
            {
                case Mode.Manhattan:
                    result = Manhattan(dRow, dColumn);
                    break;
                case Mode.Euclidean:
                    result = Euclidean(dRow, dColumn);
                    break;
                case Mode.Octile:
                    result = Octile(dRow, dColumn);
                    break;
                case Mode.Chebyshev:
                    result = Chebyshev(dRow, dColumn);
                    break;
            }

            return result;
        }

        private static int Manhattan(int dRow, int dColumn)
        {
            return (dRow + dColumn) * 10;
        }

        private static int Euclidean(int dRow, int dColumn)
        {
            return Mathf.RoundToInt(Mathf.Sqrt(dRow * dRow + dColumn * dColumn) * 10);
        }

        private static int Octile(int dRow, int dColumn)
        {
            float f = 0.4f;
            return Mathf.RoundToInt((dColumn < dRow ? f * dColumn + dRow : f * dRow + dColumn) * 10);
        }

        private static int Chebyshev(int dRow, int dColumn)
        {
            return Mathf.Max(dRow, dColumn) * 10;
        }
    }

    public class AStarFinder
    {
        public Heuristic.Mode heuristicMode = Heuristic.Mode.Manhattan;
        private DiagonalMovement diagonalMovement = DiagonalMovement.None;
        private bool allowDiagonal = true;
        private bool dontCrossCorners = true;
        public bool smoothWayPoints = true;
        public int weight = 1;

        private readonly MaskGrid maskGrid;
        private readonly HashSet<MaskNode> openSet;
        private readonly HashSet<MaskNode> closeSet;
        private readonly List<MaskNode> wayNodes;
        private List<MaskNode> neighbors;
        private List<MaskNode> smoothLine;
        private List<MaskNode> resultNodes;
        private List<Vector3> resultPoints;

        public AStarFinder(MaskGrid maskGrid)
        {
            this.maskGrid = maskGrid;
            SetDiagonalMovement();

            openSet = new HashSet<MaskNode>();
            closeSet = new HashSet<MaskNode>();
            int capacity = (int)(maskGrid.Row * maskGrid.Column * 0.2f);
            wayNodes = new List<MaskNode>(capacity);
            neighbors = new List<MaskNode>(capacity);
            smoothLine = new List<MaskNode>(capacity);
        }

        public bool AllowDiagonal
        {
            get { return allowDiagonal; }
            set
            {
                if (value != allowDiagonal)
                {
                    allowDiagonal = value;
                    SetDiagonalMovement();
                }
            }
        }

        public bool DontCrossCorners
        {
            get { return dontCrossCorners; }
            set
            {
                if (value != dontCrossCorners)
                {
                    dontCrossCorners = value;
                    SetDiagonalMovement();
                }
            }
        }

        private void SetDiagonalMovement()
        {
            if (!allowDiagonal)
            {
                diagonalMovement = DiagonalMovement.Never;
            }
            else
            {
                diagonalMovement = dontCrossCorners
                    ? DiagonalMovement.OnlyWhenNoObstacles
                    : DiagonalMovement.IfAtMostOneObstacle;
            }
        }

        public void FindPath(MaskNode startNode, MaskNode endNode, ref List<MaskNode> points)
        {
            if (null == points)
            {
                points = new List<MaskNode>();
            }
            else
            {
                points.Clear();
            }

            if (null == startNode || null == endNode || !endNode.IsWalkable)
            {
                return;
            }

            maskGrid.ResetNodesCost();
            openSet.Clear();
            closeSet.Clear();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                MaskNode miniNode = FindMiniFNode();
                openSet.Remove(miniNode);
                closeSet.Add(miniNode);
                if (miniNode == endNode)
                {
                    GenerateWayNodes(endNode, ref points);
                    return;
                }

                maskGrid.GetNeighbors(miniNode, diagonalMovement, ref neighbors);
                for (int i = 0, count = neighbors.Count; i < count; i++)
                {
                    MaskNode item = neighbors[i];
                    if (closeSet.Contains(item))
                    {
                        continue;
                    }

                    int ng = miniNode.G +
                             (item.IndexColumn == miniNode.IndexColumn || item.IndexRow == miniNode.IndexRow
                                 ? 10
                                 : 14);
                    bool isOpened = openSet.Contains(item);
                    if (!isOpened || ng < item.G)
                    {
                        item.G = ng;
                        item.H = item.H != 0
                            ? item.H
                            : weight * Heuristic.GetHeuristicValue(heuristicMode,
                                Math.Abs(item.IndexRow - endNode.IndexRow),
                                Math.Abs(item.IndexColumn - endNode.IndexColumn));
                        item.parent = miniNode;
                        if (!isOpened)
                        {
                            openSet.Add(item);
                        }
                    }
                }
            }
        }
        public List<MaskNode> FindPath(MaskNode startNode, MaskNode endNode)
        {
            FindPath(startNode, endNode, ref resultNodes);
            return resultNodes;
        }

        
        public void FindPath(Vector3 startPos, Vector3 endPos, ref List<Vector3> points)
        {
            MaskNode startNode = maskGrid.GetNode(startPos);
            MaskNode endNode = maskGrid.GetNode(endPos);
            FindPath(startNode, endNode, ref resultNodes);

            if (null == points)
            {
                points = new List<Vector3>();
            }
            else
            {
                points.Clear();
            }

            foreach (var item in resultNodes)
            {
                points.Add(item.Pos);
            }
        }
        public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos)
        {
            FindPath(startPos, endPos, ref resultPoints);
            return resultPoints;
        }

        private MaskNode FindMiniFNode()
        {
            MaskNode miniNode = null;
            //find minimum f node
            foreach (var item in openSet)
            {
                if (null == miniNode)
                {
                    miniNode = item;
                }
                else
                {
                    if (item.F < miniNode.F)
                    {
                        miniNode = item;
                    }
                }
            }

            return miniNode;
        }

        private void GenerateWayNodes(MaskNode endMaskNode, ref List<MaskNode> points)
        {
            wayNodes.Clear();
            MaskNode temp = endMaskNode;
            wayNodes.Add(temp);
            while (null != temp.parent)
            {
                temp = temp.parent;
                wayNodes.Add(temp);
            }

            wayNodes.Reverse();

            if (smoothWayPoints)
            {
                SmoothWayPoints(wayNodes, ref points);
            }
            else
            {
                GenerateWayPoints(wayNodes, ref points);
            }
        }
        private void GenerateWayPoints(List<MaskNode> wayNodes, ref List<MaskNode> points)
        {
            foreach (var item in wayNodes)
            {
                points.Add(item);
            }
        }

        
        private void GenerateWayNodes(MaskNode endMaskNode, ref List<Vector3> points)
        {
            wayNodes.Clear();
            MaskNode temp = endMaskNode;
            wayNodes.Add(temp);
            while (null != temp.parent)
            {
                temp = temp.parent;
                wayNodes.Add(temp);
            }

            wayNodes.Reverse();

            if (smoothWayPoints)
            {
                SmoothWayPoints(wayNodes, ref points);
            }
            else
            {
                GenerateWayPoints(wayNodes, ref points);
            }
        }
        private void GenerateWayPoints(List<MaskNode> wayNodes, ref List<Vector3> points)
        {
            foreach (var item in wayNodes)
            {
                points.Add(item.Pos);
            }
        }


        private void SmoothWayPoints(List<MaskNode> wayNodes, ref List<MaskNode> points)
        {
            int length = wayNodes.Count;
            MaskNode start = wayNodes[0]; //path start
            MaskNode end = wayNodes[length - 1]; //path end

            points.Add(start);

            MaskNode startCoor = start; //current start coordinate
            for (int i = 2; i < length; i++)
            {
                InterpolateNode(startCoor, wayNodes[i], ref smoothLine);
                bool blocked = false;
                for (int j = 1, count = smoothLine.Count; j < count; j++)
                {
                    if (!maskGrid.IsWalkableAt(smoothLine[j].IndexRow, smoothLine[j].IndexColumn))
                    {
                        blocked = true;
                        break;
                    }
                }

                if (blocked)
                {
                    points.Add(wayNodes[i - 1]);
                    startCoor = wayNodes[i - 1];
                }
            }

            points.Add(end);
        }
        private void SmoothWayPoints(List<MaskNode> wayNodes, ref List<Vector3> points)
        {
            int length = wayNodes.Count;
            MaskNode start = wayNodes[0]; //path start
            MaskNode end = wayNodes[length - 1]; //path end

            points.Add(start.Pos);

            MaskNode startCoor = start; //current start coordinate
            for (int i = 2; i < length; i++)
            {
                InterpolateNode(startCoor, wayNodes[i], ref smoothLine);
                bool blocked = false;
                for (int j = 1, count = smoothLine.Count; j < count; j++)
                {
                    if (!maskGrid.IsWalkableAt(smoothLine[j].IndexRow, smoothLine[j].IndexColumn))
                    {
                        blocked = true;
                        break;
                    }
                }

                if (blocked)
                {
                    points.Add(wayNodes[i - 1].Pos);
                    startCoor = wayNodes[i - 1];
                }
            }

            points.Add(end.Pos);
        }


        private void InterpolateNode(MaskNode start, MaskNode end, ref List<MaskNode> line)
        {
            line.Clear();
            int cr = start.IndexRow;
            int cc = start.IndexColumn;
            int dr = Mathf.Abs(end.IndexRow - start.IndexRow);
            int dc = Mathf.Abs(end.IndexColumn - start.IndexColumn);

            int sx = start.IndexColumn < end.IndexColumn ? 1 : -1;
            int sy = start.IndexRow < end.IndexRow ? 1 : -1;
            int ecr = dc - dr;
            while (true)
            {
                line.Add(maskGrid.GetNode(cr, cc));
                if (cc == end.IndexColumn && cr == end.IndexRow)
                {
                    break;
                }

                int e2 = 2 * ecr;
                if (e2 > -dr)
                {
                    ecr = ecr - dr;
                    cc = cc + sx;
                }

                if (e2 < dc)
                {
                    ecr = ecr + dc;
                    cr = cr + sy;
                }
            }
        }
    }
}
//#endif