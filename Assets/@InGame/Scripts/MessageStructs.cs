using UnityEngine;
using Field;

/// <summary>
/// �Z���̎������������ꂽ�ۂ�Cell�N���X���瑗�M�����
/// </summary>
public struct CellResourceCreateMessage
{
    public ResourceType Type { get; set; }
    public Vector3 Pos { get; set; }
}