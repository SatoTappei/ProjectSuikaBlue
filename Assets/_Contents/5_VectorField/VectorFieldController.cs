using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grid = VectorField.Grid;

/// <summary>
/// �x�N�^�[�t�B�[���h�𐧌䂷��N���X
/// </summary>
public class VectorFieldController : MonoBehaviour
{
    [Header("�O���b�h�̊e��ݒ�")]
    [SerializeField] int _width = 10;
    [SerializeField] int _height = 10;
    [SerializeField] float _cellSize = 1;
    [Header("�O���b�h���M�Y���ɕ\������")]
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
