using UnityEngine;
using UniRx;
using System;

/// <summary>
/// �S�Ă̓G�ɋ��ʂ��Ďg�p����A���E�̋@�\
/// ���ɏ������ރN���X
/// </summary>
[RequireComponent(typeof(CommonLayerBlackBoard))]
public class SightSensor : MonoBehaviour
{
    /// <summary>
    /// ���E���X�V����Ԋu
    /// </summary>
    const float UpdateInterval = 0.1f;
    /// <summary>
    /// ���E�ň�x�ɔ����ł��鐔
    /// </summary>
    const int Capacity = 9;

    [Header("�v���C���[�Ɋւ���l")]
    [SerializeField] string _playerTag = "Player";
    [SerializeField] LayerMask _playerLayer;
    [Header("��Q���̃��C���[")]
    [SerializeField] LayerMask _obstacleLayer;
    [Header("���E�̔��a")]
    [SerializeField] float _radius = 10.0f;
    [Header("�M�Y����ɕ\�����邩")]
    [SerializeField] bool _isDrawGizmos = true;

    CommonLayerBlackBoard _commonLayerBlackBoard;
    Transform _transform;
    Collider[] _results = new Collider[Capacity];

    void Awake()
    {
        _transform = transform;
        _commonLayerBlackBoard = GetComponent<CommonLayerBlackBoard>();

        // ���Ԋu�ŋ��`�̔���Ńv���C���[�����m����
        Observable.Interval(System.TimeSpan.FromSeconds(UpdateInterval)).Subscribe(_ => DetectPlayer()).AddTo(this);
    }

    /// <summary>
    /// ���̃��\�b�h�����Ԋu�ŌĂԂ��ƂŃv���C���[�����m����
    /// ���`�̔���Ńv���C���[�����m�����獕�̎��E�ɑ��������̃��X�g���X�V����
    /// </summary>
    void DetectPlayer()
    {
        // �X�V���邽�߂Ɍ��ʂƍ��̕ێ����郊�X�g���폜
        Array.Clear(_results, 0, _results.Length);
        _commonLayerBlackBoard.VisibleTargetDict[VisibleTargetType.Player].Clear();

        // ���m
        int count = Physics.OverlapSphereNonAlloc(_transform.position, _radius, _results, _playerLayer);
        if (count == 0) return;

        foreach (Collider result in _results)
        {
            if (result == null) continue;

            // ���̎����ɐV�����f�[�^���X�V���Ēǉ�
            _commonLayerBlackBoard.VisibleTargetDict[VisibleTargetType.Player].Clear();
            VisibleTargetData data = new VisibleTargetData(VisibleTargetType.Player, result.transform.position, Time.time);
            _commonLayerBlackBoard.VisibleTargetDict[VisibleTargetType.Player].Add(data);
        }
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying && _isDrawGizmos)
        {
            // ���E�͈̔�
            Gizmos.DrawWireSphere(_transform.position, _radius);
        }
    }
}
