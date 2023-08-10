using UnityEngine;

// 未使用

/// <summary>
/// セルが資源/キャラクターを生成可能かどうかを読み取るためのインターフェース
/// </summary>
public interface ICellSpawnableReader
{
    bool IsWalkable { get; }
    Vector3 Pos { get; }
}
