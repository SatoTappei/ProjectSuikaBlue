using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class UtilityAIController : MonoBehaviour
{
    /// <summary>
    /// �œK�ȕ]���l��I������Ԋu
    /// </summary>
    const float UtilityUpdateInterval = 0.1f;

    [SerializeField] UtilityParams _paramControlModule;

    UtilityStateBase _currentState;
    //UtilityStateAndParamLinker _linker;

    void Awake()
    {
        UtilitySateSleep stateSleep = new();
        UtilityStateEat stateEat = new();
        UtilityStateWork stateWork = new();

        _currentState = stateSleep;

        // �œK�ȕ]���l�̎擾
        Observable.Interval(System.TimeSpan.FromSeconds(UtilityUpdateInterval)).Subscribe(_ => 
        {
            // TODO:�I���������Ă�����ǂ����鏈���������Ă��Ȃ�
            _paramControlModule.SelectNext();
        });

        // �]���l�̎��R���������݂̏�Ԃ̍X�V
        this.UpdateAsObservable().Subscribe(_ => 
        {
            _paramControlModule.Update();
            _currentState.Update();
        });
    }

    void Start()
    {
        
    }
}

// ��ԋ߂��ӏ��܂ōs���č�Ƃ�����
// �x�e����(�H��)
// �Ƃɖ߂�

// �]���l�̍X�V
// ���Ԋu�ŕ]���l����ɃX�e�[�g�̑J�ڂ�������
// �X�e�[�g���̂����t���[���X�V

// ���Ԍo�߂Ō�������(�󕠓x�Ȃ�)
// ��������̃A�N�V�����Ō�������(��J�Ȃ�