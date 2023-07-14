using UnityEngine;
using UniRx;
using System;

/// <summary>
/// ���E�̋@�\�A���ɏ������ރN���X
/// </summary>
[RequireComponent(typeof(CommonLayerBlackBoard))]
public class SightSensor : MonoBehaviour
{
    /// <summary>
    /// ���E���X�V����Ԋu
    /// </summary>
    const float UpdateInterval = 0.05f;
    /// <summary>
    /// ���E�ň�x�ɔ����ł��鐔
    /// </summary>
    const int Capacity = 9;

    [Header("�v���C���[�Ɋւ���l")]
    [SerializeField] string _playerTag;
    [SerializeField] LayerMask _playerLayer;
    [Header("��Q���̃��C���[")]
    [SerializeField] LayerMask _obstacleLayer;
    [Header("���E�̔��a")]
    [SerializeField] float _radius = 10.0f;
    [Header("�M�Y����ɕ\�����邩")]
    [SerializeField] bool _isDrawGizmos = true;

    Transform _transform;

    void Awake()
    {
        _transform = transform;
        CommonLayerBlackBoard commonLayerBlackBoard = GetComponent<CommonLayerBlackBoard>();

        // ���Ԋu�ŋ��`�̔���Ńv���C���[�����m����
        Collider[] results = new Collider[Capacity];
        Observable.Interval(System.TimeSpan.FromSeconds(UpdateInterval)).Subscribe(_ => 
        {
            Array.Clear(results, 0, results.Length);
            Physics.OverlapSphereNonAlloc(_transform.position, _radius, results, _playerLayer);
        });
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying && _isDrawGizmos)
        {
            Gizmos.DrawWireSphere(_transform.position, _radius);
        }
    }
}

// ���E�ɂƂ炦�Ă�����̂�GameObject�Ƃ��ĕۑ�����
// �v���C���[�͕ʓr�ێ�����B�����^�⃊�X�g���g�p���Ĉ����o����悤�ɂ���H
