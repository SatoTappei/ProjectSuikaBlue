using UnityEngine;

/// <summary>
/// テスト用にベクターフィールドを呼び出す
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
