using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace PSB.InGame
{
    public interface IReadOnlyBreedingParam
    {
        uint Gene { get; }
    }

    public enum Sex
    {
        None,
        Male,
        Female,
    }

    /// <summary>
    /// �ɐB�͈ȉ��̗���ōs��
    /// 1.�X�e�[�g�ɑJ�ڎ��A�ɐB�ҋ@�̃��b�Z�[�W�𑗐M����
    /// 2.�ɐB�}�l�[�W�������ɐB�ҋ@�̌̓��m�Ɍo�H�����邩���ׁA�}�b�`���O������B
    /// 3.�}�b�`���O�����ꍇ�͂��̃X�e�[�g�����ʂƃp�[�g�i�[�̃��b�Z�[�W����M����B
    /// 4.�o�H�T�� <- �K�v�H
    /// </summary>
    public class BreedState : BaseState
    {
        // ���̎��Ԃ��I�[�o�[�����ꍇ�͋����I�ɕ]���X�e�[�g�ɑJ�ڂ���
        const float TimeOut = 30.0f;

        Transform _actor;
        Stack<Vector3> _path = new();
        Vector3 _currentCellPos;
        Vector3 _nextCellPos;
        float _lerpProgress;
        float _speedModify = 1;
        float _timer;
        // �}�b�`���O�p
        Transform _partner;
        Sex _sex;

        bool HasPath => _path != null;
        bool OnNextCell => _actor.position == _nextCellPos;
        // �}�b�`���O�����ۂɃ��b�Z�[�W����M����true�ɂȂ�
        bool IsMatching => _partner != null && _sex != Sex.None;
        bool IsDeath => Context.NextState.Type == StateType.Killed ||
                        Context.NextState.Type == StateType.Senility;

        public BreedState(DataContext context) : base(context, StateType.Breed)
        {
            _actor = context.Transform;
        }

        protected override void Enter()
        {
            _timer = 0;
            SubscribeReceiveMessage();
            SendMessage();
        }

        protected override void Exit()
        {
            SendCancelMessage();
            _partner = null;
            _sex = Sex.None;
        }

        protected override void Stay()
        {
            //// �^�C�}�[��i�߂�B���Ԑ؂�̏ꍇ�͕]���X�e�[�g�ɑJ��
            //if (!StepTimer()) { ToEvaluateState(); return; }
            //// �}�b�`���O���Ă��Ȃ���ԂŎ��񂾏ꍇ�̓Z����ɂ���̂Ŏ��S�X�e�[�g�ɑJ�ڂ��đ��v
            //if (!IsMatching && IsDeath) { ToEvaluateState(); return; }
            //// �}�b�`���O���Ă��邩�`�F�b�N
            //if (!IsMatching) return;
            //// �o�H�����邩�`�F�b�N
            //if (!HasPath) return;

            //if (_sex == Sex.Male)
            //{
            //    if (OnNextCell)
            //    {
            //        if (IsDeath) { ToEvaluateState(); return; }

            //        if (!TryStepNextCell())
            //        {
            //            // �Ԃ̎��ɃT�C���𑗐M���A��M���������o�Y�̏��������s����
            //            OnArrivalNeighbourPertner();
            //            ToEvaluateState();
            //        }
            //    }
            //    else
            //    {
            //        Move();
            //    }
            //}
            //else if (_sex == Sex.Female)
            //{
            //    // ���͂��̏�őҋ@�A���b�Z�[�W����M������o�Y
            //}
        }

        /// <summary>
        /// �^�C�}�[��i�߂�
        /// </summary>
        /// <returns>���ԓ�:true ���Ԑ؂�:false</returns>
        bool StepTimer()
        {
            _timer += Time.deltaTime;
            return _timer < TimeOut;
        }

        void SubscribeReceiveMessage()
        {
            // �}�b�`���O����
            MessageBroker.Default.Receive<MatchingMessage>()
                .Where(msg => msg.ID == _actor.GetInstanceID())
                .Subscribe(MatchingComplete).AddTo(_actor);
            // �p�[�g�i�[����̃T�C��
            MessageBroker.Default.Receive<BreedingPartnerMessage>()
                .Where(msg => msg.ID == _actor.GetInstanceID())
                .Subscribe(PartnerSign).AddTo(_actor);
            // �p�[�g�i�[���}�b�`���O���L�����Z������
            MessageBroker.Default.Receive<CancelBreedingMessage>()
                .Where(_ => _partner != null)
                .Where(msg => msg.Actor == _partner)
                .Subscribe(MatchingCancel).AddTo(_actor);
        }

        void SendMessage()
        {
            MessageBroker.Default.Publish(new BreedingMessage() { Actor = _actor });
        }

        void SendCancelMessage()
        {
            MessageBroker.Default.Publish(new CancelBreedingMessage() { Actor = _actor });
        }

        void OnArrivalNeighbourPertner()
        {
            //// ���ɎY�܂���
            //MessageBroker.Default.Publish(new BreedingPartnerMessage() { ID = _partner.GetInstanceID() });
            //// �o�^���ꂽ�ɐB����0�ɂ��鏈�������s
            //_blackBoard.OnMaleBreedingInvoke();
        }

        void MatchingComplete(MatchingMessage msg)
        {
            // �}�b�`���O���̃Z�b�g
            _sex = msg.Sex;
            _partner = msg.Partner;
            // �o�H�T��
            //bool hasPath = TryPathfinding();
            //if (!hasPath) throw new System.NullReferenceException("�ɐB�X�e�[�g�̃p�[�g�i�[�ւ̌o�H��null");
            
            TryStepNextCell();
        }

        void MatchingCancel(CancelBreedingMessage _)
        {
            //ToEvaluateState();
        }

        void PartnerSign(BreedingPartnerMessage msg)
        {
            // �Y�ޏ����̎��s
            uint gene = _partner.GetComponent<IReadOnlyBreedingParam>().Gene;
            //_blackBoard.OnFemaleBreedingInvoke(gene);
            
            // �]���X�e�[�g�ɑJ��
            //ToEvaluateState();
        }

        //void ToEvaluateState() => TryChangeState(_blackBoard.EvaluateState);

        //bool TryPathfinding()
        //{
        //    return FieldManager.Instance.TryGetPath(_actor.position, _partner.position, out _path);
        //}

        /// <summary>
        /// ���݂̃Z���̈ʒu�����g�̈ʒu�ōX�V����B
        /// ���̃Z���̈ʒu������Ύ��̃Z���̈ʒu�A�Ȃ���Ύ��g�̈ʒu�ōX�V����B
        /// </summary>
        /// <returns>���̃Z��������:true ���̃Z��������(�ړI�n�ɓ���):false</returns>
        bool TryStepNextCell()
        {
            _currentCellPos = _actor.position;

            if (_path.TryPop(out _nextCellPos))
            {
                // �o�H�̃Z���ƃL�����N�^�[�̍������Ⴄ�̂Ő����Ɉړ������邽�߂ɍ��������킹��
                _nextCellPos.y = _actor.position.y;
                Modify();
                Look();
                _lerpProgress = 0;

                return true;
            }

            _nextCellPos = _actor.position;

            return false;
        }

        void Look()
        {
            Vector3 dir = _nextCellPos - _currentCellPos;
            //_blackBoard.Model.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }

        /// <summary>
        /// �΂߈ړ��̑��x��␳����
        /// </summary>
        void Modify()
        {
            bool dx = Mathf.Approximately(_currentCellPos.x, _nextCellPos.x);
            bool dz = Mathf.Approximately(_currentCellPos.z, _nextCellPos.z);

            _speedModify = (dx || dz) ? 1 : 0.7f;
        }

        void Move()
        {
            //_lerpProgress += Time.deltaTime * _blackBoard.Speed * _speedModify;
            _actor.position = Vector3.Lerp(_currentCellPos, _nextCellPos, _lerpProgress);
        }

        // �f�o�b�O�p
        void MatchingLog()
        {
            Debug.Log($"�}�b�`���O ���g:{_actor.name} �� ����:{_partner.name} ���g�̐���:{_sex}");
        }
    }
}