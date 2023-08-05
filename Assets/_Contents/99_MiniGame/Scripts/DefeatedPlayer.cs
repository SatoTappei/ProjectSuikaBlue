using UniRx;
using UnityEngine;

namespace MiniGame
{
    public class DefeatedPlayer : MonoBehaviour
    {
        [SerializeField] GameObject _turret;

        void Awake()
        {
            MessageBroker.Default.Receive<InGameStartMessage>()
                .Subscribe(_ => Destroy(gameObject)).AddTo(this);
        }

        void Start()
        {
            _turret.TryGetComponent(out Rigidbody rb);
            rb.AddForce(Vector3.up * 15.0f, ForceMode.Impulse);
        }
    }
}
