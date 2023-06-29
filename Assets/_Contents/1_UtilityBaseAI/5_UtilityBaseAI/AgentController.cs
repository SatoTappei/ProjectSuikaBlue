using System.Collections;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace UtilityBaseAI
{
    public enum ActionType
    {
        Rest, // �x�e
        Eat,  // �H��
        Work, // �d��
        Play, // �V��
        Move, // �ړ���(���C�ɓ���̓���ɐݒ�s�\)
    }

    [RequireComponent(typeof(UtilityParamView))]
    [RequireComponent(typeof(AgentPlacementHolder))]
    public class AgentController : MonoBehaviour
    {
        [SerializeField] AgentDesire _foodDesire;
        [SerializeField] AgentDesire _funDesire;
        [SerializeField] AgentDesire _fatigueDesire;
        [Header("�S�Ă̗~������������Ă���ꍇ�Ɏ��s�������铮��")]
        [SerializeField] ActionType _favoriteActionType;

        UtilityParamView _uIController;
        AgentPlacementHolder _placementHolder;
        AgentTaskHolder _taskHolder = new();
        ActionType _currentBehavior = ActionType.Move;

        void Awake()
        {
            if (_favoriteActionType == ActionType.Move)
            {
                Debug.LogWarning("���C�ɓ���̓��삪 �ړ��� �������̂� �x�e �ɋ����ύX");
                _favoriteActionType = ActionType.Rest;
            }

            _uIController = GetComponent<UtilityParamView>();
            _placementHolder = GetComponent<AgentPlacementHolder>();

            // �~�����X�V
            this.UpdateAsObservable().Subscribe(_ => 
            {
                _foodDesire.Decrease();
                _funDesire.Decrease();
                _fatigueDesire.Decrease();

                if (_foodDesire.BelowThreshold && _taskHolder.IsContain(ActionType.Eat))
                {
                    _taskHolder.Add(ActionType.Eat);
                }
                if (_funDesire.BelowThreshold && _taskHolder.IsContain(ActionType.Play))
                {
                    _taskHolder.Add(ActionType.Play);
                }
                if (_fatigueDesire.BelowThreshold && _taskHolder.IsContain(ActionType.Rest))
                {
                    _taskHolder.Add(ActionType.Rest);
                }

                //_uIController.SetFoodValue(_foodDesire.Current, AgentDesire.MaxValue);
                //_uIController.SetFunValue(_funDesire.Current, AgentDesire.MaxValue);
                //_uIController.SetEnergyValue(_fatigueDesire.Current, AgentDesire.MaxValue);
            });

            // ���Ԋu�Ń^�X�N��]������
            Observable.Interval(System.TimeSpan.FromSeconds(1.0f)).Subscribe(_ => 
            {

            }).AddTo(this);
        }

        //IEnumerator MoveAndAction()
        //{

        //}
    }

    // hungry��������H�ׂ�
    // fatigue��������x�e����
    // fun��������d��������

    // �e����ɂ͕K�v�Ȏ��Ԃ�����
    // �Œ�������鎞�Ԍ�ɒ���I�ɑ����邩�ǂ������肷��

    // �l�����ȉ��ɂȂ�����^�X�N�𔭍s����
    // �^�X�N���L���[�ɒǉ����Ă���
    // ���s�^�C�~���O�Ń^�X�N��]�����Ď��s���ׂ��Ȃ���s�A�����łȂ��Ȃ�j������

    // ���s�^�C�~���O�ŕK�v�Ȓl��臒l�ȉ������ׂ�
    // �e����
}