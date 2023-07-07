using System;

/// <summary>
/// BinaryHeap<T>�N���X�Œl���Ǘ����邽�߂ɕK�v�ȃC���^�[�t�F�[�X
/// </summary>
public interface IBinaryHeapCollectable<T> : IComparable<T>
{
    int BinaryHeapIndex { get; set; }
}

/// <summary>
/// �񕪃q�[�v����������N���X
/// A*�Ōo�H�T��������ۂɍŏ��R�X�g�̃m�[�h��Ԃ����߂Ɏg�p
/// </summary>
public class BinaryHeap<T> where T : IBinaryHeapCollectable<T>
{
    T[] _values;
    public int Count { get; private set; }

    public BinaryHeap(int maxHeapSize)
    {
        _values = new T[maxHeapSize];
    }

    /// <summary>
    /// �ǉ����ɓK�؂ȏꏊ�Ɉړ�������̂ŁA�l�����̏ꏊ�ɑ��݂���΃q�[�v���ɑ��݂���
    /// </summary>
    public bool Contains(T value) => Equals(_values[value.BinaryHeapIndex], value);

    /// <summary>
    /// ��Ԍ��ɒǉ����ēK�؂ȏꏊ�܂ňړ�������
    /// </summary>
    public void Add(T value)
    {
        value.BinaryHeapIndex = Count;
        _values[Count++] = value;
        SortUp(value);
    }

    /// <summary>
    /// �擪��Ԃ��āA��Ԍ��̒l��擪�Ɏ����Ă��ēK�؂ȏꏊ�܂ňړ�������
    /// </summary>
    public T Pop()
    {
        // �v�f���������Ɏ擾���悤�Ƃ����ꍇ�͋K��l��Ԃ�
        if (--Count < 0) return default;

        T value = _values[0];
        _values[0] = _values[Count];
        _values[0].BinaryHeapIndex = 0;
        SortDown(_values[0]);
        return value;
    }

    /// <summary>
    /// �����̒l��Add()�����ۂɖ؂̉������Ɍ����ă\�[�g���Ă���
    /// </summary>
    void SortUp(T value)
    {
        int parentIndex = (value.BinaryHeapIndex - 1) / 2;

        // ��r���Đe�ƌ�������
        T parent = _values[parentIndex];
        if (value.CompareTo(parent) < 0)
        {
            Swap(value, parent);
            SortUp(value);
        }
    }

    /// <summary>
    /// �����̒l��Pop()�����ۂɖ؂̏ォ�牺�Ɍ����ă\�[�g���Ă���
    /// </summary>
    void SortDown(T value)
    {
        int leftChildIndex = value.BinaryHeapIndex * 2 + 1;
        int rightChildIndex = value.BinaryHeapIndex * 2 + 2;

        // ���̎q�m�[�h���w���Y�������؂̗v�f���ȏ�Ȃ珈�������Ȃ�
        if (leftChildIndex >= Count) return;

        int swapIndex = leftChildIndex;
        // �E�̎q�����̎q���傫���ꍇ�͂�����ƌ�������
        if (rightChildIndex < Count && _values[leftChildIndex].CompareTo(_values[rightChildIndex]) > 0)
        {
            swapIndex = rightChildIndex;
        }

        // �q���傫����Ό�������
        if (value.CompareTo(_values[swapIndex]) > 0)
        {
            Swap(value, _values[swapIndex]);
            SortDown(value);
        }
    }

    /// <summary>
    /// ���݂̓Y�����̌��̒l������������A�Y��������������
    /// </summary>
    void Swap(T value1, T value2)
    {
        _values[value1.BinaryHeapIndex] = value2;
        _values[value2.BinaryHeapIndex] = value1;
        int itemIndex = value1.BinaryHeapIndex;
        value1.BinaryHeapIndex = value2.BinaryHeapIndex;
        value2.BinaryHeapIndex = itemIndex;
    }
}