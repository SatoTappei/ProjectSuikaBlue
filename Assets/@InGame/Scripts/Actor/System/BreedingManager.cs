using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace PSB.InGame
{
    public class BreedingManager : MonoBehaviour
    {
        List<Actor> _actorList = new();

        void Awake()
        {
            SubscribeBreedMessage();
        }

        void Update()
        {
            if (_actorList.Count >= 2)
            {
                Matching();
                Shuffle();
            }
        }

        void SubscribeBreedMessage()
        {
            MessageBroker.Default.Receive<BreedingMessage>().Subscribe(Add).AddTo(this);
            MessageBroker.Default.Receive<CancelBreedingMessage>().Subscribe(Remove).AddTo(this);
        }

        void Add(BreedingMessage msg)
        {
            Actor actor = msg.Actor.GetComponent<Actor>();
            _actorList.Add(actor);
        }

        void Matching()
        {
            Actor male = _actorList[0];
            _actorList.RemoveAt(0);
            Actor female = _actorList[0];
            _actorList.RemoveAt(0);

            // 2人の間に経路があるか調べる
            Vector3 m = male.transform.position;
            Vector3 f = female.transform.position;
            if (FieldManager.Instance.TryGetPath(m, f, out Stack<Vector3> _))
            {
                // 経路があればマッチング
                MessageBroker.Default.Publish(new MatchingMessage()
                {
                    ID = male.transform.GetInstanceID(),
                    Partner = female.transform,
                    Sex = Sex.Male,
                });
                MessageBroker.Default.Publish(new MatchingMessage()
                {
                    ID = female.transform.GetInstanceID(),
                    Partner = male.transform,
                    Sex = Sex.Female,
                });
            }
            else
            {
                // 再度追加
                _actorList.Add(male);
                _actorList.Add(female);
            }
        }

        void Shuffle()
        {
            _actorList = _actorList.OrderBy(_ => System.Guid.NewGuid()).ToList();
        }

        void Remove(CancelBreedingMessage msg)
        {
            Actor actor = msg.Actor.GetComponent<Actor>();
            _actorList.Remove(actor);
        }
    }
}