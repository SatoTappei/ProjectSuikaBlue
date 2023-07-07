using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grid = VectorField.Grid;

/// <summary>
/// ベクターフィールドを制御するクラス
/// </summary>
public class VectorFieldController : MonoBehaviour
{
    [Header("グリッドの各種設定")]
    [SerializeField] int _width = 10;
    [SerializeField] int _height = 10;
    [SerializeField] float _cellSize = 1;
    [Header("グリッドをギズモに表示する")]
    [SerializeField] bool _drawGizmos = true;

    Grid _grid;

    void Awake()
    {
        _grid = new Grid(_width, _height, _cellSize, transform.position);
    }

    void Update()
    {
        
    }

    void OnDrawGizmos()
    {
        if (_drawGizmos && Application.isPlaying)
        {
            _grid.DrawOnGizmos();
        }
    }
}
