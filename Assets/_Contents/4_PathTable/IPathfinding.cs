using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 経路探索をする機能を実装させるインターフェース
/// </summary>
public interface IPathfinding
{
    public Stack<Vector3> Pathfinding(int startNumber, int goalNumber);
}
