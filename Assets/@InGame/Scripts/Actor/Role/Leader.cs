using UnityEngine;

public class Leader : MonoBehaviour
{
    [SerializeField] GameObject _symbolPrefab;

    GameObject _symbol;

    void Awake()
    {
        _symbol = Instantiate(_symbolPrefab);
        _symbol.transform.SetParent(transform);
        _symbol.transform.localPosition = Vector3.up;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // ����p���ĕ]���l�����߂�
    // ���[�_�[�̍��߂ŏW�����������B
        // ���[�_�[�𒆐S�ɂ��邮��for���ŋ󂢂Ă���Z����T���Ƃ�����H
}
