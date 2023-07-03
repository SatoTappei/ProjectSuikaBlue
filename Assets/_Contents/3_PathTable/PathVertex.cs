using UnityEngine;

/// <summary>
/// �o�H�e�[�u����p�����o�H�T���p�̒��_
/// PathTable�N���X���ێ����Ă��钸�_�̃I�u�W�F�N�g�ɑ΂��Ď��s����AddComponent�����
/// </summary>
public class PathVertex : MonoBehaviour
{
    public int Number { get; set; }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            // ���_�ԍ��̕\��
            Vector3 pos = transform.position + Vector3.up * 1.5f;
            string str = $"���_: {Number}";
            UnityEditor.Handles.color = Color.black;
            UnityEditor.Handles.Label(pos, str);
        }
#endif
    }
}
