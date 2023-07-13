using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �e�X�g�p�Ƀx�N�^�[�t�B�[���h���Ăяo��
/// </summary>
public class TestVFController : MonoBehaviour
{
    VectorFieldManager _vfManager;
    Queue<Vector3> _queue = new();

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
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _vfManager.InsertVectorFlowToQueue(transform.position, _queue);
        }
    }
}
