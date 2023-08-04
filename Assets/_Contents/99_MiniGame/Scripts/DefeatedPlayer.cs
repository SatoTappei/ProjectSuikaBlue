using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefeatedPlayer : MonoBehaviour
{
    [SerializeField] GameObject _turret;

    void Start()
    {
        _turret.TryGetComponent(out Rigidbody rb);
        rb.AddForce(Vector3.up * 15.0f, ForceMode.Impulse);
    }
}
