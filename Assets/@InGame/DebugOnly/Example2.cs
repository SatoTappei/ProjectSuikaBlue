using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.DebugOnly
{
    public class Example2 : MonoBehaviour
    {
        void Start()
        {
            int i = 2;
            switch (i)
            {
                case 1: Debug.Log("1"); return;
                case 2 when N(): Debug.Log("2"); return;
                case 3: Debug.Log("3"); return;
            }
        }

        void Update()
        {

        }

        bool N()
        {
            Debug.Log("true");
            return true;
        }

        void M()
        {
            var vv = new Collider[8];
            var v = Physics.OverlapSphereNonAlloc(transform.position, 0.5f, vv);

            Debug.Log(vv[0].name);
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}