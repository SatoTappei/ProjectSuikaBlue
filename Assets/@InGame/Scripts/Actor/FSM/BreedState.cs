using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace PSB.InGame
{
    public enum Sex
    {
        None,
        Male,
        Female,
    }

    public class BreedState : BaseState
    {
        IBlackBoardForState _blackBoard;
        Transform _actor;
        Transform _partner;
        Sex _sex;
        Stack<Vector3> _path = new();
        // �}�b�`���O�����ۂɃ��b�Z�[�W����M����true�ɂȂ�
        bool IsMatching => _partner != null && _sex != Sex.None;

        public BreedState(IBlackBoardForState blackBoard) : base(StateType.Breed)
        {
            _blackBoard = blackBoard;
            _actor = blackBoard.Transform;
        }

        protected override void Enter()
        {
            SubscribeMatchingMessage();
            SendMessage();
        }

        protected override void Exit()
        {
            SendCancelMessage();
        }

        protected override void Stay()
        {
            // �Ԃ�2�̂̔ɐB�����Ȃ��Ɛ������Ȃ��B
            // Enter�̃^�C�~���O�Ń��b�Z�[�W���O���邪�A���̃^�C�~���O�ŔԂ����Ȃ��ꍇ�A�p�[�g�i�[�Ɛ��ʂ̃f�[�^�����Ȃ��B
            // �̂ŁAStay�̃^�C�~���O�Ńp�[�g�i�[�Ɛ��ʂ̃f�[�^�����Ă��邩���ׂ�K�v����B

            if (!IsMatching) return;

            MatchingLog();

            if (_blackBoard.Sex == Sex.Male)
            {
                //// ���X�̏��܂ňړ�����
                //Vector3 femalePos = _blackBoard.Partner.GetComponent<IBlackBoardForState>().Transform.position;
                //// ���X�܂ł̌o�H
                //FieldManager.Instance.TryGetPath(_actor.position, femalePos, out _path);

            }
            else if (_blackBoard.Sex == Sex.Female)
            {
                // ���̏�őҋ@
                // 
            }

            //var partner = _blackBoard.Partner;
            //var sex = _blackBoard.Sex;

            // ���:��蒼���A��͂莩�����g(Actor�N���X)�𑗐M���ă}�b�`���O����N���X���������ǂ��C������B
            // ���:�I�X�̓��X�̉ӏ��Ɉړ����Ȃ��Ƃ����Ȃ��̂����A�o�H���擾�ł��Ȃ������ꍇ�͂ǂ�����H
            // ���:�I�X�������̓��X���ɐB���Ɏ��񂾏ꍇ�̃L�����Z������
            //      ���ɐB���͎��ȂȂ��悤�ɂ���Ηǂ��H
            //      �����S�̔���͕]���X�e�[�g�Ɉˑ����Ă���̂ŔɐB���I�������]���X�e�[�g�ɑJ�ڂ���΂悢

            // �ɐB�������̂����b�Z�[�W���O
            // �ɐB�������̂����b�Z�[�W���O
            // ������2�̂���z������
            // �ɐB�������͔̂ɐB�X�e�[�g�ɓ����Ă��̏�őҋ@���Ă���
            // Matching�N���X�̓L���[�������Ă���B2�̓��閈�ɐ擪����2�̔����o����Matching������B
            // �Е����I�X�A�����Е������X�Ƃ��Đݒ肷��
            // �I�X���p�[�g�i�[�ׂ̗܂ňړ�  ���X�̓p�[�g�i�[������܂őҋ@
            // �o�H�������ꍇ�͂ǂ��Ȃ�H
            // ���Ԍo��                      ���Ԍo��
            // �]���X�e�[�g�ɑJ��            �q���𐶐� -> �]���X�e�[�g�ɑJ��

            // �Ƃ肠���������]���X�e�[�g�ɑJ�ڂ�����B
            //ToEvaluateState();

            // �ߏ�̑��̌̂�T��
            //  �ǂ������̂̃��X�g��ێ����Ă���K�v������
            // �ׂ̃Z���܂ňړ�����
            //  �ɐB������ɐB�X�e�[�g�ɑJ�ڂ���K�v������B
            //  �ǂ�����đ���ɓ`���邩
            //  ���b�Z�[�W���O
            //   �U�߂����b�Z�[�W�𑗐M���� �X�e�[�g���ő��M����
            //   �󂯂����b�Z�[�W����M����
            //   �󂯂����b�Z�[�W�𑗐M����
            //   �U�߂����b�Z�[�W����M����
            //  ���b�Z�[�W�̎�M -> ���[�_�[�̕]�� -> �̂̕]�� �̏�
            //  �ɐB���(��)�ƔɐB���(�U��)������H
            //   ��:�҂�
            //   �U��:�󂯂Ɍ����Ĉړ�
            //   ��:�ɐB��0��
            //   �U��:�ɐB��0��
            //   ��:�X�|�i�[���琶��

            //  ��:�^���I�ȗY����t����?

            // �ɐB
        }

        void SubscribeMatchingMessage()
        {
            MessageBroker.Default.Receive<MatchingMessage>().Subscribe(ReceiveMessage).AddTo(_actor);
        }

        void SendMessage()
        {
            MessageBroker.Default.Publish(new BreedingMessage() { Actor = _actor });
        }

        void SendCancelMessage()
        {
            MessageBroker.Default.Publish(new CancelBreedingMessage() { Actor = _actor });
        }

        void ReceiveMessage(MatchingMessage msg)
        {
            if (msg.ID == _actor.GetInstanceID())
            {
                _sex = msg.Sex;
                _partner = msg.Partner;
            }
        }

        void ToEvaluateState() => TryChangeState(_blackBoard.EvaluateState);

        void MatchingLog()
        {
            Debug.Log($"�}�b�`���O {_actor.name} �� {_partner.name} ���g�̐���:{_sex}");
        }
    }
}
