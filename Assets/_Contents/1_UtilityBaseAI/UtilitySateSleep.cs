using UnityEngine;

/// <summary>
/// ���[�e�B���e�B�x�[�X�Ŏ��s���鐇���������Ԃ̃N���X
/// </summary>
public class UtilitySateSleep : UtilityStateBase
{
    /// <summary>
    /// ��J����������Ԋu
    /// </summary>
    const float Interval = 1.0f;
    /// <summary>
    /// ���̔�J�̌�����
    /// </summary>
    const float HealingValue = 0.05f;

    float _timer;

    public UtilitySateSleep(UtilityBlackBoard blackBoard) 
        : base(UtilityStateType.Sleep, blackBoard) { }

    protected override void Enter()
    {
        _timer = 0;
    }

    protected override void Exit()
    {
    }

    protected override void Stay()
    {
        Vector3 toBed = BlackBoard[EnvironmentType.Bed].transform.position - BlackBoard.Transform.position;
        float distance = Vector3.SqrMagnitude(toBed);
        if(distance < .1f)
        {
            // ���X�ɔ�J����������
            _timer += Time.deltaTime;
            if(_timer > Interval)
            {
                _timer = 0;
                BlackBoard.TiredParam.Value -= HealingValue;

                // �J��
                TransitionIfStateChanged();
            }
        }
        else
        {
            // �x�b�h�Ɍ����Ĉړ�
            Vector3 velo = toBed.normalized * Time.deltaTime * BlackBoard.MoveSpeed;
            BlackBoard.Transform.position += velo;
        }

        // �H�ׂ����~��������
        BlackBoard.FoodParam.Increase();
    }
}