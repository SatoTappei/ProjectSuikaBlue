using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyOthello
{
    public class Cursor : MonoBehaviour
    {
        void Update()
        {
            // ��邾��
            transform.Rotate(Vector3.up);
        }
    }
}