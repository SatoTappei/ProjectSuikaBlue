using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �o�H�T��������@�\������������C���^�[�t�F�[�X
/// </summary>
public interface IPathfinding
{
    public Stack<Vector3> Pathfinding(int startNumber, int goalNumber);
}
