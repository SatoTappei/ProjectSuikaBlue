using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform _child;
    [SerializeField] float _moveSpeed = 3.0f;
    [SerializeField] int _zoomStep = 5;

    Transform _transform;

    void Awake()
    {
        _transform = transform;
    }

    void Update()
    {
        (Vector2 move, float fov) input = Input();
        Move(input.move);
        Zoom(input.fov);
    }

    (Vector2 move, float fov) Input()
    {
        // à⁄ìÆ
        Vector2 move = Vector2.zero;
        if (UnityEngine.Input.GetKey(KeyCode.A)) move += Vector2.left;
        if (UnityEngine.Input.GetKey(KeyCode.S)) move += Vector2.down;
        if (UnityEngine.Input.GetKey(KeyCode.D)) move += Vector2.right;
        if (UnityEngine.Input.GetKey(KeyCode.W)) move += Vector2.up;
        
        // ÉYÅ[ÉÄ
        float fov;
        fov = UnityEngine.Input.GetAxis("Mouse ScrollWheel");

        return (move, fov);
    }

    void Move(Vector2 input)
    {
        input = input.normalized;
        _transform.position += new Vector3(input.x, 0, input.y) * Time.deltaTime * _moveSpeed;
    }

    void Zoom(float input)
    {
        Vector3 pos = _child.transform.position;
        pos.y += input * -_zoomStep;

        _child.transform.position = pos;
    }
}
