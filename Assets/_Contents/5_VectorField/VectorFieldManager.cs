using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VectorField;

/// <summary>
/// �x�N�^�[�t�B�[���h�𐧌䂷��N���X
/// </summary>
[RequireComponent(typeof(GridController))]
public class VectorFieldManager : MonoBehaviour
{
    GridController _grid;

    void Awake()
    {
        _grid = GetComponent<GridController>();
    }

    void Update()
    {
        
    }

    void OnDrawGizmos()
    {
        //if (_drawGizmos && Application.isPlaying)
        //{
        //    _grid.DrawOnGizmos();
        //}
    }
}
