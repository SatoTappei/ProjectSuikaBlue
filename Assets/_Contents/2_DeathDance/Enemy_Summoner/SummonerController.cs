using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// �������Ă���G�𐧌䂷��N���X
/// �ŏ�����X�e�[�W�ɔz�u�����̂ł͂Ȃ��A�Q�[�����ɐ�������邱�Ƃ��l���������ɂȂ��Ă���
/// </summary>
[RequireComponent(typeof(CommonLayerBlackBoard))]
public class SummonerController : MonoBehaviour
{
    [SerializeField] GameObject _spawnPrefab;

    void Awake()
    {
        CommonLayerBlackBoard commonLayerBlackBoard = GetComponent<CommonLayerBlackBoard>();
        // ������Ԃ����蓖�Ă�
        EnemyStateBase currentState = commonLayerBlackBoard[EnemyStateType.Init];
        this.UpdateAsObservable().Subscribe(_ =>
        {
            currentState = currentState.Update();
        });
    }

    void Start()
    {

    }

    void Update()
    {
        
    }
}

// �ҋ@:������
// �퓬:�����A�G����������
// ���:�v���C���[����苗���ȉ����U���̃��b�Z�[�W����M�����^�C�~���O

// �G�̔����Ȃǂ̓X�e�[�g�ōs��