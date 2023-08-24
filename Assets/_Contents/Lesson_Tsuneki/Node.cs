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
        /// �����Ƃ��̑O��m�[�h�������Ƀ\�[�g����
        /// </summary>
        public void Sort()
        {
            Node current = this;
            Node first = null;

            // �o�u���\�[�g?

            // �擪�܂ňړ�
            while (current.Prev != null) current = Prev;
            // ��r���Č���
            if (current.Value > current.Next.Value)
            {
                //Swap(current, current.Next);
            }
        }

        public void Swap(Node target)
        {
            // �w�肵���m�[�h�ƈʒu����������
            // �q���w�肷�邱�Ƃ��\�A���̏ꍇ�͎��g�̎q�͒����ė��Ȃ��B
            Node tempPrev = Prev;
            Node tempNext = Next;
            Node tempChild = Child;
            Node tempParent = Parent;

            if (Prev != null) Prev.Next = target;
            if (Next != null) Next.Prev = target;
            if (Child != null) Child.Parent = target;
            //if (Parent != null) Parent.Child = target;
            if (Parent != null)
            {
                if (Parent.Child == this) Parent.Child = target;
            }

            if (target.Prev != null) target.Prev.Next = this;
            if (target.Next != null) target.Next.Prev = this;
            if (target.Child != null) target.Child.Parent = this;
            //if (target.Parent != null) target.Parent.Child = this;
            if (target.Parent != null)
            {
                if (target.Parent.Child == target) target.Parent.Child = this;
            }

            Prev = target.Prev;
            Next = target.Next;
            Child = target.Child;
            Parent = target.Parent;

            target.Prev = tempPrev;
            target.Next = tempNext;
            target.Child = tempChild;
            target.Parent = tempParent;
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
