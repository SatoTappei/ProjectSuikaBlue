using UnityEngine;

public class ActorViewCameraController : MonoBehaviour
{
    [SerializeField] float _rotSpeed = 1.0f;
    Transform _child;

    void Awake()
    {
        _child = transform.GetChild(0);
    }

    void Update()
    {
        _child.Rotate(Vector3.up * Time.deltaTime * _rotSpeed);
    }
}
