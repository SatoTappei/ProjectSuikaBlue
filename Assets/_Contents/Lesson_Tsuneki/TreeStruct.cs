using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TreeStruct
{
    public class TreeStruct : MonoBehaviour
    {
        [SerializeField] Transform _root;
        [SerializeField] Text _nodeText;

        void Start()
        {
            Node root = new Node { Value = 0 };
            Node node1 = new Node { Value = 1 };
            Node node2 = new Node { Value = 2 };
            Node node3 = new Node { Value = 3 };
            Node node4 = new Node { Value = 4 };
            Node node5 = new Node { Value = 5 };
            Node node6 = new Node { Value = 6 };
            Node node7 = new Node { Value = 7 };
            Node node8 = new Node { Value = 8 };
            Node node9 = new Node { Value = 9 };
            Node node10 = new Node { Value = 10 };

            root.Add(node4);
            root.Add(node1);
            root.Add(node2);
            root.Add(node5);
            root.Add(node3);

            node1.AddChild(node6);
            node6.AddChild(node7);
            node6.AddChild(node8);
            node6.AddChild(node9);

            node8.Remove();

            // SwapÇ∆SortÇçÏÇÈ

            Visualize(root, Vector2.zero);
        }

        Vector2 Visualize(Node node, Vector2 pos)
        {
            Create(node, pos);
            Vector2 cPos = pos;
            if (node.Child != null)
            {
                cPos = Visualize(node.Child, pos + Vector2.down * 30 + Vector2.right * 30);
                cPos.x = pos.x;
            }
            if (node.Next != null)
            {
                cPos = Visualize(node.Next, cPos + Vector2.down * 30);
            }

            return cPos;
        }

        void Create(Node node, Vector2 pos)
        {
            Text text = Instantiate(_nodeText, _root);
            text.text = node.Value.ToString();
            text.transform.localPosition = pos;
        }
    }
}