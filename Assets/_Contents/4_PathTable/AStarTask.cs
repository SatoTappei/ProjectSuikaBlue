using System.Collections.Generic;
using UnityEngine;

namespace PathTableGraph
{
    /// <summary>
    /// 経路テーブルで用いるA*を実行するクラス
    /// </summary>
    public class AStarTask : IPathfinding
    {
        Vertex[] _graph;

        public AStarTask(Vertex[] graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// 指定した頂点番号から頂点番号までを経路探索
        /// 外部からコンメソッドを呼ぶことで機能する
        /// </summary>
        public Stack<Vector3> Pathfinding(int startNumber, int goalNumber)
        {
            ResetCost();

            Vertex current = _graph[startNumber];
            current.HCost = CalculateHeuristicCost(startNumber, goalNumber);

            BinaryHeap<Vertex> openHeap = new(_graph.Length);
            openHeap.Add(current);
            HashSet<Vertex> closeSet = new();

            while (true)
            {
                if (openHeap.Count == 0)
                {
                    Debug.LogWarning("経路が見つからないので途中までの経路を作成");
                    return CreatePath(current);
                }

                // 最小コストの頂点
                current = openHeap.Pop();
                // 同じ番号の場合は経路を生成して返す
                if (current.Number == goalNumber)
                {
                    return CreatePath(current);
                }
                // 開いたノードのリストから閉じたノードのリストに移す
                closeSet.Add(current);
                // 隣接した頂点のコストを計算
                foreach (Neighbour neighbour in current.NeighbourList)
                {
                    // 閉じたノードのリストに含まれていたら弾く
                    if (closeSet.Contains(neighbour.Vertex)) continue;

                    float gCost = current.GCost + neighbour.Distance;
                    float hCost = CalculateHeuristicCost(neighbour.Vertex.Number, goalNumber);
                    float fCost = gCost + hCost;
                    bool unContainedInOpenList = !openHeap.Contains(neighbour.Vertex);

                    // 開いたノードのリストに含まれていない
                    // もしくはよりコストが低い場合は、コストと親を上書き
                    if (fCost < neighbour.Vertex.FCost || unContainedInOpenList)
                    {
                        neighbour.Vertex.GCost = gCost;
                        neighbour.Vertex.HCost = hCost;
                        neighbour.Vertex.Parent = current;
                    }
                    // ノードを開いた場合は開いたノードのリストに追加
                    if (unContainedInOpenList) openHeap.Add(neighbour.Vertex);
                }
            }
        }

        /// <summary>
        /// 経路探索の前に各頂点のコストをリセットする
        /// </summary>
        void ResetCost()
        {
            for (int i = 1; i < _graph.Length; i++)
            {
                _graph[i].GCost = float.MaxValue;
                _graph[i].HCost = float.MaxValue;
                _graph[i].Parent = null;
            }
        }

        /// <summary>
        /// ヒューリスティックコストの計算
        /// 各枝のコストが距離と同じくゴールまでの距離の2乗
        /// </summary>
        float CalculateHeuristicCost(int currentNumber, int goalNumber)
        {
            Vector3 currentPos = _graph[currentNumber].transform.position;
            Vector3 goalPos = _graph[goalNumber].transform.position;
            return (goalPos - currentPos).sqrMagnitude;
        }

        /// <summary>
        /// パスの生成
        /// 頂点の親の位置をStackに挿入していく
        /// </summary>
        Stack<Vector3> CreatePath(Vertex current)
        {
            Stack<Vector3> path = new();
            while (current.Parent != null)
            {
                path.Push(current.transform.position);
                current = current.Parent;
            }
            path.Push(current.transform.position);

            return path;
        }
    }
}
