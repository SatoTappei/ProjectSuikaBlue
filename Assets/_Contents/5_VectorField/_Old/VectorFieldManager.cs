using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Old;

namespace Old
{
    /// <summary>
    /// ベクターフィールドを制御するクラス
    /// </summary>
    [RequireComponent(typeof(GridController))]
    public class VectorFieldManager : MonoBehaviour
    {
        [SerializeField] GameObject _testPrefab; // <- テスト用
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
            // テスト用
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