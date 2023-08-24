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
            // 先頭から調べて次が無いノードまで辿る
            Node current = this;
            while (current.Next != null)
            {
                current = current.Next;
            }
            // 次に追加
            current.Next = node;
            current.Next.Prev = current;
        }

        public void AddChild(Node node)
        {
            // 自身の子に追加する
            if (Child == null)
            {
                Child = node;
                node.Parent = this;
            }
            // 2番目以降は1番目の子の後ろに追加する
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
            if (node == null) return;  // 追加: nullチェック

            // 子ノードと次のノードを再帰的に削除
            if (node.Child != null) Delete(node.Child);
            if (node.Next != null) Delete(node.Next);

            // 現在のノードの参照をクリアする
            node.Prev = null;
            node.Next = null;
            node.Parent = null;
            node.Child = null;
        }

        /// <summary>
        /// 自分とその前後ノードを昇順にソートする
        /// </summary>
        public void Sort()
        {
            Node current = this;
            Node first = null;

            // バブルソート?

            // 先頭まで移動
            while (current.Prev != null) current = Prev;
            // 比較して交換
            if (current.Value > current.Next.Value)
            {
                //Swap(current, current.Next);
            }
        }

        public void Swap(Node target)
        {
            // 指定したノードと位置を交換する
            // 子を指定することも可能、その場合は自身の子は着いて来ない。
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
            Debug.Log("番号: " + Value);
            Debug.Log("子: " + Child?.Value);
            Debug.Log("親: " + Parent?.Value);
            Debug.Log("弟: " + Next?.Value);
            Debug.Log("兄: " + Prev?.Value);
        }
    }
}
