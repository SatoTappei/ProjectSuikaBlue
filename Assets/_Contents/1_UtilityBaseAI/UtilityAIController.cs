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

    [SerializeField] UtilityParamControlModule _paramControlModule;

    UtilityStateBase _currentState;

    void Awake()
    {
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
// ��������̃A�N�V�����Ō�������(��J�Ȃ�)