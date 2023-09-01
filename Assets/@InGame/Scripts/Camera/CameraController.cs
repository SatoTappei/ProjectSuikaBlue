using UnityEngine;

namespace PSB.InGame
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] Transform _parent;
        [SerializeField] Transform _child;
        [SerializeField] float _moveSpeed = 3.0f;
        [SerializeField] float _rotSpeed = 5.0f;
        [SerializeField] int _zoomStep = 5;

        Transform _transform;

        void Awake()
        {
            _transform = transform;
        }

        void Update()
        {
            (bool shift, Vector2 move, float fov) input = Input();
            if (input.shift)
            {
                Rotate(input.move);
            }
            else
            {
                Move(input.move);
            }
            
            Zoom(input.fov);
        }

        (bool shift, Vector2 move, float fov) Input()
        {
            // シフト
            bool shift = UnityEngine.Input.GetKey(KeyCode.LeftShift);

            // 移動
            Vector2 move = Vector2.zero;
            if (UnityEngine.Input.GetKey(KeyCode.A)) move += Vector2.left;
            if (UnityEngine.Input.GetKey(KeyCode.S)) move += Vector2.down;
            if (UnityEngine.Input.GetKey(KeyCode.D)) move += Vector2.right;
            if (UnityEngine.Input.GetKey(KeyCode.W)) move += Vector2.up;

            // ズーム
            float fov;
            fov = UnityEngine.Input.GetAxis("Mouse ScrollWheel");

            return (shift, move, fov);
        }

        void Move(Vector2 input)
        {
            input = input.normalized;
            Vector3 hori = _parent.right * input.x;
            Vector3 vert = _parent.forward * input.y;
            _transform.position += (hori + vert) * Time.deltaTime * _moveSpeed;
        }

        void Rotate(Vector2 input)
        {
            _parent.eulerAngles += -input.x * Vector3.up * Time.deltaTime * _rotSpeed;

        }

        void Zoom(float input)
        {
            Vector3 pos = _parent.transform.position;
            pos.y += input * -_zoomStep;

            _parent.transform.position = pos;
        }
    }
}
