using UnityEngine;

// ���g�p

/// <summary>
/// �Z��������/�L�����N�^�[�𐶐��\���ǂ�����ǂݎ�邽�߂̃C���^�[�t�F�[�X
/// </summary>
public interface ICellSpawnableReader
{
    bool IsWalkable { get; }
    Vector3 Pos { get; }
}
