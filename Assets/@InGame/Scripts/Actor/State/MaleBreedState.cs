using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public class MaleBreedState : BaseState
    {
        enum Stage
        {
            Move,
            Mating,
        }

        readonly MoveModule _move;
        readonly FieldModule _field;
        readonly DetectModule _detect;
        readonly RuleModule _rule;

        Stage _stage;
        bool _firstStep; // �o�H�̃X�^�[�g�n�_���玟�̃Z���Ɉړ���
        float _matingTimer;
        bool _isMating;  // �����
        bool _completed; // �������

        public MaleBreedState(DataContext context) : base(context, StateType.MaleBreed) 
        {
            _move = new(context);
            _field = new(context);
            _detect = new(context);
            _rule = new(context);
        }

        Collider[] Detected => Context.Detected;
        List<Vector3> Path => Context.Path;
        Vector3 Position => Context.Transform.position;
        float MatingTime => Context.Base.MatingTime;
        uint Gene => Context.Gene;
        float BreedingRate { set => Context.BreedingRate.Value = value; }

        protected override void Enter()
        {
            _move.Reset();
            _move.TryStepNextCell();
            _field.SetOnCell(_move.CurrentCellPos);
            _stage = Stage.Move;
            _firstStep = true;
            _matingTimer = 0;
            _isMating = false;
            _completed = false;
        }

        protected override void Exit()
        {
            _field.DeletePathGoalOnCell();
            Path.Clear();
        }

        protected override void Stay()
        {
            // �ړ�
            if (_stage == Stage.Move)
            {
                if (_move.OnNextCell)
                {
                    if (_firstStep)
                    {
                        _firstStep = false;
                        _field.DeleteOnCell(_move.CurrentCellPos);
                    }

                    // ���S�����ꍇ�͑J�ڂ���
                    if (_rule.IsDead()) { ToEvaluateState(); return; }

                    // �o�H�̖��[(���Ɠ����������ׂ͗̃Z��)�܂ŒH�蒅�����ꍇ�͌��
                    if (!_move.TryStepNextCell()) _stage = Stage.Mating;
                }
                else
                {
                    _move.Move();
                }
            }
            // ���
            else if (_stage == Stage.Mating)
            {
                // �ɐB����ׂ̗Ɉړ������A���͔��ߖT�̎��ɑ΂��Č��
                if (!_isMating)
                {
                    _isMating = true;
                    // ������鎓�����Ȃ��ꍇ�͕]���X�e�[�g�ɖ߂�
                    if (!CallNeighbourFemale()) { ToEvaluateState(); return; }
                }

                // ������Ɍ���ɂ����鎞�Ԉȏ�o�߂����ꍇ�́A������s�Ƃ݂Ȃ��]���X�e�[�g�ɖ߂�
                if (_isMating) _matingTimer += Time.deltaTime;
                if (_matingTimer > MatingTime)
                {
                    // �A���ŔɐB�X�e�[�g�ɑJ�ڂ��Ă��Ȃ��悤�ɔɐB����50���ɂ���
                    BreedingRate = StatusBase.Max / 2;
                    ToEvaluateState();
                    return;
                }

                // ������͎���ł��W���ł����̃X�e�[�g�ɑJ�ڂ��Ȃ��H

                // ��������t���O���������ꍇ�͕]���X�e�[�g�ɖ߂�
                if (_completed)
                {
                    BreedingRate = 0;
                    ToEvaluateState();
                    return;
                }
            }
        }

        /// <summary>
        /// ���͔��ߖT�̔ɐB�X�e�[�g�̎������m����
        /// </summary>
        /// <returns>�Ԃ̎�������:true ���Ȃ�:false</returns>
        bool DetectPartnerOnNeighbourCell(out Actor actor)
        {
            _detect.OverlapSphere(Utility.NeighbourCellRadius);

            foreach (Collider collider in Detected)
            {
                if (collider == null) break;
                // ���ȊO��e��
                if (!(collider.TryGetComponent(out Actor female) && female.Sex == Sex.Female)) continue;
                // �����ɐB�X�e�[�g�̏ꍇ�̂�
                if (female.State != StateType.FemaleBreed) continue;

                actor = female;
                return true;
            }

            actor = null;
            return false;
        }

        bool CallNeighbourFemale()
        {
            if (DetectPartnerOnNeighbourCell(out Actor actor))
            {
                actor.SpawnChild(Gene, () => _completed = true);
                return true;
            }

            return false;
        }
    }
}