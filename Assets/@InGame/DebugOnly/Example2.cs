using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.DebugOnly
{
    public class Example2 : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var vv = new Collider[8];
            var v = Physics.OverlapSphereNonAlloc(transform.position, 0.5f, vv);

            Debug.Log(vv[0].name);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}