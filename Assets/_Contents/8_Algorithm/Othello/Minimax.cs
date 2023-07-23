using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �~�j�}�b�N�X�@
/// �Q�l:https://uguisu.skr.jp/othello/minimax.html
/// </summary>
public class Minimax : MonoBehaviour
{
    class Node
    {
        public Node(string name, params Node[] children)
        {
            Name = name;
            Value = -1; // <- ���̒l���Ֆʂ̕]���l�ɂȂ�
            Children = new(3);
            Children.AddRange(children);

            // �q�̐e�Ɏ��g��o�^����
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
        // ���[�ɐ��l�����蓖�Ă�
        Node[] leaf = new Node[12];
        int[] leafValue = { 2, 4, 5, 7, 3, 5, 6, 4, 1, 3, 4, 2 };
        for (int i = 0; i < leaf.Length; i++)
        {
            leaf[i] = new Node("�t_" + i) { Value = leafValue[i] };
        }

        Node nodeC = new("�m�[�hC", leaf[0], leaf[1], leaf[2]);
        Node nodeD = new("�m�[�hD", leaf[3], leaf[4], leaf[5]);
        Node nodeE = new("�m�[�hE", leaf[6], leaf[7], leaf[8]);
        Node nodeF = new("�m�[�hF", leaf[9], leaf[10], leaf[11]);

        Node nodeA = new("�m�[�hA", nodeC, nodeD);
        Node nodeB = new("�m�[�hB", nodeE, nodeF);

        Node root = new("��", nodeA, nodeB);

        // �����ɂƂ��čő�̎�����
        DFS(root, SearchType.Max);
        // ����ɂƂ��čŏ��̎�����
        DFS(root,SearchType.Min);
        // �����ɂƂ��čő�̎�����
        DFS(root, SearchType.Max);

        Print(root, nodeA, nodeB, nodeC, nodeD, nodeE, nodeF);
    }

    /// <summary>
    /// �w�肵���m�[�h���疖�[�܂ŒT��
    /// �l�������Ă���(-1�ȊO)�Ȃ�1��̃m�[�h�ɕԂ�
    /// </summary>
    int DFS(Node node, SearchType searchType)
    {
        // �l�������Ă���m�[�h�̏ꍇ�͂��̂܂ܒl��Ԃ�����
        int temp = node.Value;
        if (temp != -1) return temp;

        foreach (Node child in node.Children)
        {
            // �q���l�������Ă���/���Ȃ�
            int result = DFS(child, searchType);
            if (result == -1) continue;
            // ����̍X�V�Ȃ炻�̂܂܍X�V
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
