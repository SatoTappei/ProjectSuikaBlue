using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] LayerMask _mask;
        VectorFieldManager _vectorFieldManager;

        /// <summary>
        /// ���������ۂɁA���������K���ĂԕK�v������B�R���X�g���N�^�̑���
        /// </summary>
        public void Init(VectorFieldManager vectorFieldManager)
        {
            _vectorFieldManager = vectorFieldManager;
        }

        void Start()
        {
            // ���ڃV�[���ɔz�u�����ꍇ�͒T���Ă���
            _vectorFieldManager ??= FindFirstObjectByType<VectorFieldManager>();
        }

        void Update()
        {
            Vector3 vector = _vectorFieldManager.GetCellVector(transform.position);
            Debug.Log(vector);

            //Vector3 rayOrigin = transform.position + Vector3.up * 0.25f;
            //Physics.Raycast(rayOrigin, new Vector3(-1, 0, 0), out RaycastHit hit, 1.0f, _mask);
            //if (hit.collider != null)
            //{
            //    float dist = (rayOrigin - hit.point).sqrMagnitude;
            //    float percent = dist / 1.0f;
            //    Debug.Log("��Q���܂ł̋���: " + percent);

            //    //vector += (new Vector3(0, 0, 1.0f) * (2.0f -percent));
            //}
            
            //Debug.DrawRay(transform.position + Vector3.up * 0.25f, new Vector3(-1, 0, 0));
            //Vector3 rayHitPos = 

            //if (_currentDir != vector)
            //{
            //    _prevDir = _currentDir;
            //    _currentDir = vector;
            //}
            //_currentDir = Vector3.Lerp(_prevDir, _currentDir, Time.deltaTime * 3.0f);

            transform.Translate(vector.normalized * Time.deltaTime * 1.5f);
        }
    }
}

// �x�N�g���t�B�[���h�g�����ǂ����悤���
// ���̃Z���ɉf�����r�[�؂�ւ���Ă��܂��̂ŃZ���̕ԏ���ړ�����݂����ȋ����ɂȂ�
// Lerp���g���ĕ⊮����H������Z���̒����ɋ��邱�ƑO��
