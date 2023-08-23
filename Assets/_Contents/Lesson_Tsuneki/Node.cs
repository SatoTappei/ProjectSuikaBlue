using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TreeStruct
{
    class Node
    {
        public int Value { get; set; }
        public Node Next { get; set; }
        public Node Prev { get; set; }
        public Node Child { get; set; }
        public Node Parent { get; set; }

        public void Add(Node node)
        {
            // �擪���璲�ׂĎ��������m�[�h�܂ŒH��
            Node current = this;
            while (current.Next != null)
            {
                current = current.Next;
            }
            // ���ɒǉ�
            current.Next = node;
            current.Next.Prev = current;
        }

        public void AddChild(Node node)
        {
            // ���g�̎q�ɒǉ�����
            if (Child == null)
            {
                Child = node;
                node.Parent = this;
            }
            // 2�Ԗڈȍ~��1�Ԗڂ̎q�̌��ɒǉ�����
            else
            {
                Child.Add(node);
            }
        }

        public void Remove()
        {
            if (Child != null) Delete(Child);

            if (Parent != null)
            {
                if (Next != null)
                {
                    Parent.Child = Next;
                    Next.Parent = Parent;
                }
                else
                {
                    Parent.Child = null;
                }

                Parent = null;
            }
            
            if (Prev != null) Prev.Next = Next;
            if (Next != null) Next.Prev = Prev;
        }

        static void Delete(Node node)
        {
            if (node == null) return;  // �ǉ�: null�`�F�b�N

            // �q�m�[�h�Ǝ��̃m�[�h���ċA�I�ɍ폜
            if (node.Child != null) Delete(node.Child);
            if (node.Next != null) Delete(node.Next);

            // ���݂̃m�[�h�̎Q�Ƃ��N���A����
            node.Prev = null;
            node.Next = null;
            node.Parent = null;
            node.Child = null;
        }

        /// <summary>
        /// �����Ƃ��̌��̃m�[�h�������Ƀ\�[�g����
        /// </summary>
        public void Sort()
        {
            Node current = this;
            Node first = null;
            while (true)
            {
                Node min = this;
                while (current.Next != null)
                {
                    if (current.Value < min.Value) min = current;
                    current = current.Next;
                }

                if (first == null) first = min;
                else if (first.Value == min.Value) break;
                
                min.Remove();
                Add(min);
            }
        }

        public static void Swap(Node nodeA, Node nodeB)
        {
            // �w�肵���m�[�h�ƈʒu����������
            // �q���w�肷�邱�Ƃ��\�A���̏ꍇ�͎��g�̎q�͒����ė��Ȃ��B
            Node tempPrev = nodeA.Prev;
            Node tempNext = nodeA.Next;
            Node tempChild = nodeA.Child;
            Node tempParent = nodeA.Parent;

            nodeA.Prev = nodeB.Prev;
            nodeA.Next = nodeB.Next;
            nodeA.Child = nodeB.Child;
            nodeA.Parent = nodeB.Parent;

            nodeB.Prev = tempPrev;
            nodeB.Next = tempNext;
            nodeB.Child = tempChild;
            nodeB.Parent = tempParent;
        }

        public void Log()
        {
            Debug.Log("�ԍ�: " + Value);
            Debug.Log("�q: " + Child?.Value);
            Debug.Log("�e: " + Parent?.Value);
            Debug.Log("��: " + Next?.Value);
            Debug.Log("�Z: " + Prev?.Value);
        }
    }
}
