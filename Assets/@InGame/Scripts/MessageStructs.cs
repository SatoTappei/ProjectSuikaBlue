using UnityEngine;
using Field;

/// <summary>
/// セルの資源が生成された際にCellクラスから送信される
/// </summary>
public struct CellResourceCreateMessage
{
    public ResourceType Type { get; set; }
    public Vector3 Pos { get; set; }
}