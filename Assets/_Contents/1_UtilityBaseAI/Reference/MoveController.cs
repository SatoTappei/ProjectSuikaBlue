using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UtilityAIReference
{
    public class MoveController : MonoBehaviour
    {
        NavMeshAgent _agent;

        void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {

        }

        public void MoveTo(Vector3 pos)
        {
            _agent.destination = pos;
        }
    }
}
