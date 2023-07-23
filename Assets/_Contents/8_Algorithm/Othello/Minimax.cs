using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ミニマックス法
/// 参考:https://uguisu.skr.jp/othello/minimax.html
/// </summary>
public class Minimax : MonoBehaviour
{
    class Node
    {
        public Node(string name, params Node[] children)
        {
            Name = name;
            Value = -1; // <- この値が盤面の評価値になる
            Children = new(3);
            Children.AddRange(children);

            // 子の親に自身を登録する
            foreach (Node child in children)
            {
                child.Parent = this;
            }
        }

        public string Name { get; set; }
        public int Value { get; set; }
        public Node Parent { get; set; }
        public List<Node> Children { get; set; }
    }

    enum SearchType
    {
        Min,
        Max,
    }

    void Start()
    {
        // 末端に数値を割り当てる
        Node[] leaf = new Node[12];
        int[] leafValue = { 2, 4, 5, 7, 3, 5, 6, 4, 1, 3, 4, 2 };
        for (int i = 0; i < leaf.Length; i++)
        {
            leaf[i] = new Node("葉_" + i) { Value = leafValue[i] };
        }

        Node nodeC = new("ノードC", leaf[0], leaf[1], leaf[2]);
        Node nodeD = new("ノードD", leaf[3], leaf[4], leaf[5]);
        Node nodeE = new("ノードE", leaf[6], leaf[7], leaf[8]);
        Node nodeF = new("ノードF", leaf[9], leaf[10], leaf[11]);

        Node nodeA = new("ノードA", nodeC, nodeD);
        Node nodeB = new("ノードB", nodeE, nodeF);

        Node root = new("根", nodeA, nodeB);

        // 自分にとって最大の手を取る
        DFS(root, SearchType.Max);
        // 相手にとって最小の手を取る
        DFS(root,SearchType.Min);
        // 自分にとって最大の手を取る
        DFS(root, SearchType.Max);

        Print(root, nodeA, nodeB, nodeC, nodeD, nodeE, nodeF);
    }

    /// <summary>
    /// 指定したノードから末端まで探す
    /// 値を持っていた(-1以外)なら1つ上のノードに返す
    /// </summary>
    int DFS(Node node, SearchType searchType)
    {
        // 値を持っているノードの場合はそのまま値を返すだけ
        int temp = node.Value;
        if (temp != -1) return temp;

        foreach (Node child in node.Children)
        {
            // 子が値を持っている/いない
            int result = DFS(child, searchType);
            if (result == -1) continue;
            // 初回の更新ならそのまま更新
            if (node.Value == -1) node.Value = result;

            if(searchType == SearchType.Min)
            {
                node.Value = Mathf.Min(node.Value, result);
            }
            else if(searchType == SearchType.Max)
            {
                node.Value = Mathf.Max(node.Value, result);
            }
        }

        return temp;
    }

    void Print(params Node[] nodes)
    {
        foreach(Node node in nodes)
        {
            Debug.Log(node.Name + " " + (node.Value == -1 ? "None" : node.Value));
        }
    }
}
