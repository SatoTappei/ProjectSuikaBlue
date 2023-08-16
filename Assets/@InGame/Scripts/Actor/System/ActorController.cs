using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public class ActorController : MonoBehaviour
    {
        Actor _kinpatsuLeader;
        LinkedList<Actor> _kinpatsuList = new();
        LinkedList<Actor> _kurokamiList = new();

        void Awake()
        {
            Actor.OnSpawned += AddActor;
        }

        void OnDestroy()
        {
            Actor.OnSpawned -= AddActor;
        }

        void Update()
        {
            if (_kinpatsuLeader != null)
            {
                _kinpatsuLeader.StepParams();
            }
            if (_kinpatsuList.Count > 0)
            {
                foreach (Actor kinpatsu in _kinpatsuList)
                {
                    kinpatsu.StepParams();
                }
            }
            if (_kurokamiList.Count > 0)
            {
                foreach (Actor enemy in _kurokamiList)
                {
                    enemy.StepParams();
                }
            }
        }

        void AddActor(Actor actor)
        {
            if      (actor.Type == ActorType.KinpatsuLeader) _kinpatsuLeader = actor;
            else if (actor.Type == ActorType.Kinpatsu)       _kinpatsuList.AddLast(actor);
            else if (actor.Type == ActorType.Kurokami)       _kurokamiList.AddLast(actor);
            else
            {
                string msg = "キャラクターの種類がNoneなのでControllerで制御不可能: " + actor.name;
                throw new System.ArgumentException(msg);
            }
        }
    }
}