using UnityEngine;

/// <summary>
/// �e�X�g�p�Ƀx�N�^�[�t�B�[���h���Ăяo��
/// </summary>
public class TestVFController : MonoBehaviour
{
    VectorFieldManager _vfManager;

    void Awake()
    {
        _vfManager = FindObjectsByType<VectorFieldManager>(FindObjectsSortMode.InstanceID)[0];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _vfManager.CreateVectorField(transform.position, FlowMode.Toward);
        }
    }
}
