using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Dragon : MonoBehaviour
{
    [SerializeField] GameObject _breathPrefab;
    [SerializeField] Transform _breathMuzzle;

    int _maxHp = 100;
    int _currentHp;

    void Awake()
    {
        _currentHp = _maxHp;
    }

    void Start()
    {
        InvokeRepeating(nameof(Fire), 0, 0.5f);
    }

    void Fire()
    {
        GameObject breath = Instantiate(_breathPrefab, _breathMuzzle.transform.position, Quaternion.identity);
        Vector3 dir = _breathMuzzle.forward + Vector3.down * 0.5f;
        dir += Vector3.right * Random.Range(-0.5f, 0.5f);
        breath.transform.DOMove(dir * 10, 1.5f)
            .SetRelative()
            .OnComplete(() => Destroy(breath))
            .SetLink(gameObject);
    }
}
