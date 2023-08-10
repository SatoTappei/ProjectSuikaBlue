using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Field;
using Actor;

public class GameManager : MonoBehaviour
{
    [SerializeField] FieldBuilder _fieldBuilder;
    [SerializeField] InitKinpatsuSpawner _initKinpatsuSpawner;

    void Start()
    {
        Cell[,] field = _fieldBuilder.Build();
        _initKinpatsuSpawner.Spawn(field);
    }

    void Update()
    {
        
    }
}
