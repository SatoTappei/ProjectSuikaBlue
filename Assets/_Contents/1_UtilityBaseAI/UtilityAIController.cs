using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

/// <summary>
/// �e�@�\��p���ă��[�e�B���e�B�x�[�XAI�𐧌䂷��N���X
/// </summary>
[RequireComponent(typeof(UtilityBlackBoard))]
[RequireComponent(typeof(UtilityParamEvaluator))]
[RequireComponent(typeof(UtilityStateHolder))]
public class UtilityAIController : MonoBehaviour
{
    /// <summary>
    /// �œK�ȕ]���l��I������Ԋu
    /// </summary>
    const float UtilityUpdateInterval = 1.0f;

    void Awake()
    {
        UtilityParamEvaluator evaluator = GetComponent<UtilityParamEvaluator>();
        UtilityBodyLayer bodyLayer = GetComponent<UtilityBodyLayer>();
        UtilityBlackBoard blackBoard = GetComponent<UtilityBlackBoard>();
        UtilityParamToStateConverter converter = new();

        // ������Ԃ̏�Ԃ�ݒ�
        UtilityStateBase currentState = blackBoard[UtilityStateType.Sleep];

        IObservable<Unit> update = this.UpdateAsObservable();
        // �]���l���擾 -> �g�̂̑w�Œ��� -> ���ɏ�������
        update.ThrottleFirst(TimeSpan.FromSeconds(UtilityUpdateInterval)).Subscribe(_ =>
        {
            UtilityParamType highestParam = evaluator.SelectHighestParamType();
            UtilityStateType nextState = converter.ConvertToState(highestParam);
            nextState = bodyLayer.Adjust(nextState);
            blackBoard.SelectedStateType = nextState;
        });
        // �]���l�̎��R���������݂̏�Ԃ̍X�V
        update.Subscribe(_ => 
        {
            currentState = currentState.Update();
        });
    }
}

// ************************************************************************
// TOOD: ���ݎ��s���̏�Ԃƍ��ɏ������܂�Ă��Ԃ�������ꍇ�͑J�ڂ�����
// ************************************************************************

// ��ԋ߂��ӏ��܂ōs���č�Ƃ�����
// �x�e����(�H��)
// �Ƃɖ߂�

// �]���l�̍X�V
// ���Ԋu�ŕ]���l����ɃX�e�[�g�̑J�ڂ�������
// �X�e�[�g���̂����t���[���X�V

// ���Ԍo�߂Ō�������(�󕠓x�Ȃ�)
// ��������̃A�N�V�����Ō�������(��J�Ȃ�

// �e�p�����[�^�̎��R�J�ڂ͂ǂ��ōs����
// 
// �m�\�̃��C���[:�J�ڂ̃��b�Z�[�W���󂯎��
// �g�̂̃��C���[:���b�Z�[�W�����s�\�����ׂ�

// ��Ԃƕ]���l�͕R�Â��Ă��Ȃ��Ă����v�H
// �Q�Ă����ԂŃX�^�[�g�A�H�~��������� �Ȃ炷���N����

// �ǂ�����đJ�ڂ�����H
// ���b�Z�[�W�̎�M �������� ���̎Q�ƁH

// ���Ԋu�Œm�\����ԍ����]���l��Ԃ�
// �]���l��Ή����铮��ɕύX <- �����܂ł��m�\�̑w
// ���̓��삪�g�̂��o���邩�ǂ����`�F�b�N <- ��������g�̂̑w
// 
// �e��Ԃ͔��˂Ȃǂł��J�ڂ��Ă����v�Ȃ悤�ɍ���Ă�������
// �J�ڐ悪�����ł��낤�Ƒ��v�A�܂�e��Ԃ͓Ɨ����Ă���

// �e��Ԃɂ͍������Ȃ�
// ���ɂ͕ϐ��AProperty�����Ȃ�
// ��Ԃ̃��X�g�����ɏ������ނ����Ȃ�

// �d��: ��J�������� �����肪������ �y����������
// ����: ��J������ �����肪������
// �H��: �����肪���� �y������������