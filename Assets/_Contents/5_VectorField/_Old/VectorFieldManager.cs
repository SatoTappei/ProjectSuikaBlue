using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Old;

namespace Old
{
    /// <summary>
    /// �x�N�^�[�t�B�[���h�𐧌䂷��N���X
    /// </summary>
    [RequireComponent(typeof(GridController))]
    public class VectorFieldManager : MonoBehaviour
    {
        [SerializeField] GameObject _testPrefab; // <- �e�X�g�p
        GridController _grid;

        void Awake()
        {
            _grid = GetComponent<GridController>();
        }

        void Start()
        {
            _grid.SetVectorFlowCenterCell(transform.position, FlowMode.Toward);
        }

        void Update()
        {
            // �e�X�g�p
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _grid.GetFlow(_testPrefab.transform.position);
            }
        }

        void OnDrawGizmos()
        {
            //if (_drawGizmos && Application.isPlaying)
            //{
            //    _grid.DrawOnGizmos();
            //}
        }
    }
}