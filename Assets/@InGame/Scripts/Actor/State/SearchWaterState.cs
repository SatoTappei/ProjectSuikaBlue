using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PSB.InGame
{
    // �H���̂قڃR�s�y

    /// <summary>
    /// ���̃Z���܂ňړ����A�ݒ肳�ꂽ���ʒl�����X�e�[�^�X�̐��̒l�����X�ɉ񕜂���B
    /// �X�e�[�^�X�̃p�����[�^�����ʒl�������Ă��Ă��A���ʒl���̉񕜏��������s�����
    /// </summary>
    public class SearchWaterState : BaseState
    {
        const int EffectValue = 100; // ���̃X�e�[�g�ŉ񕜂��鐅�̒l
        const int EffectDelta = 100; // ���ۂɂ�DeltaTime�Ƃ̏�Z�ŉ񕜂���

        enum Stage
        {
            Move,
            Drink,
        }

        Stage _stage;
        Transform _actor;
        Stack<Vector3> _path = new();
        Vector3 _currentCellPos;
        Vector3 _nextCellPos;
        float _lerpProgress;
        float _effectProgress;
        float _speedModify = 1;
        // �H���̃Z��������A�H���܂ł̌o�H�����݂��邩�ǂ����̃t���O
        bool _hasPath;

        bool OnNextCell => _actor.position == _nextCellPos;

        public SearchWaterState(DataContext context) : base(context, StateType.SearchWarter)
        {
            _actor = _blackBoard.Transform;
        }

        protected override void Enter()
        {
            _stage = Stage.Move;

            _effectProgress = 0;

            _hasPath = TryPathfinding();
            TryStepNextCell();
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            // �o�H�������̂ŕ]���X�e�[�g�ɑJ��
            if (!_hasPath) { ToEvaluateState(); return; }
            
            switch (_stage)
            {
                case Stage.Move: MoveStage(); break;
                case Stage.Drink: DrinkStage(); break;
            }
        }

        bool TryPathfinding()
        {
            _path.Clear();

            // �H���̃Z�������邩���ׂ�
            if (FieldManager.Instance.TryGetResourceCells(ResourceType.Water, out List<Cell> cellList))
            {
                // �H���̃Z�����߂����Ɍo�H�T��
                Vector3 pos = _actor.position;
                foreach (Cell food in cellList.OrderBy(c => Vector3.SqrMagnitude(c.Pos - pos)))
                {
                    if (FieldManager.Instance.TryGetPath(pos, food.Pos, out _path)) // <- �Լ�
                    {
                        return true;
                    }
                }

                return false;
            }

            return false;
        }

        void ToEvaluateState() => TryChangeState(_blackBoard.EvaluateState);

        /// <summary>
        /// �H���̃Z���Ɉړ�
        /// </summary>
        void MoveStage()
        {
            // ���̃Z���̏�ɗ����ꍇ�̓`�F�b�N����
            if (OnNextCell)
            {
                // �Ⴄ�X�e�[�g�ɑJ�ڂ���ꍇ�͈�x�]���X�e�[�g���o�R����
                if (_blackBoard.NextState != this) { ToEvaluateState(); return; }

                if (TryStepNextCell())
                {
                    // �o�H�̓r���̃Z���̏ꍇ�̏���
                }
                else
                {
                    _stage = Stage.Drink; // ���ޏ�Ԃ�
                }
            }
            else
            {
                Move();
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        void DrinkStage()
        {
            if (!StepEatProgress()) { ToEvaluateState(); return; }
        }

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
            _blackBoard.Model.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }

        void Move()
        {
            _lerpProgress += Time.deltaTime * _blackBoard.Speed * _speedModify;
            _actor.position = Vector3.Lerp(_currentCellPos, _nextCellPos, _lerpProgress);
        }

        /// <summary>
        /// �񕜂̐i���x��i�߂�
        /// </summary>
        /// <returns>�񕜂̐i����:true �񕜂̐i�����񕜒l�ɒB����:false</returns>
        bool StepEatProgress()
        {
            float value = Time.deltaTime * EffectDelta;
            _effectProgress += value;
            _blackBoard.OnDrinkWaterInvoke(value); // �l�̍X�V

            return _effectProgress <= EffectValue;
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
    }
}