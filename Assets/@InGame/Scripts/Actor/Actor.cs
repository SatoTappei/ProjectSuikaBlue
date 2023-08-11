using UnityEngine;
using UnityEngine.Events;

namespace PSB.InGame
{
    public enum ActorType
    {
        None,
        Kinpatsu,
        KinpatsuLeader,
        Enemy,
    }

    public class Actor : MonoBehaviour
    {
        public static event UnityAction<Actor> OnSpawned;

        ActorType _type;

        public ActorType Type => _type;

        public void Init(ActorType type)
        {
            _type = type;
            OnSpawned?.Invoke(this);
        }

        public void Move()
        {
            transform.Translate(Vector3.forward * Time.deltaTime);
        }
    }
}