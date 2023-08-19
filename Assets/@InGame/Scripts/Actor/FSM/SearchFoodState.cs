using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PSB.InGame
{
    public class SearchFoodState : BaseState
    {
        //const float DistanceThreshold = 0.005f;
        enum Stage
        {
            Move,
            Eat,
        }

        IBlackBoardForState _blackBoard;
        Transform _actor;
        Stack<Vector3> _path = new();
        Vector3 _currentCellPos;
        Vector3 _nextCellPos;
        float _lerpProgress = 0;
        // �H���̃Z��������A�H���܂ł̌o�H�����݂��邩�ǂ����̃t���O
        bool _hasPath;
        Stage _stage;

        bool OnNextCell => _actor.position == _nextCellPos;

        public SearchFoodState(IBlackBoardForState blackBoard) : base(StateType.SearchFood)
        {
            _blackBoard = blackBoard;
            _actor = _blackBoard.Transform;
        }

        protected override void Enter()
        {
            _stage = Stage.Move;

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
                case Stage.Eat:  EatStage();  break;
            }
        }

        bool TryPathfinding()
        {
            _path.Clear();

            // �H���̃Z�������邩���ׂ�
            if (FieldManager.Instance.TryGetResourceCells(ResourceType.Tree, out List<Cell> cellList))
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
                    Debug.Log("�� " + _blackBoard.Transform.name);

                }
                else
                {
                    Debug.Log("�ړI�n " + _blackBoard.Transform.name);
                    _stage = Stage.Eat; // �H�ׂ��Ԃ�
                }
            }
            else
            {
                Move();
            }
        }

        /// <summary>
        /// �H����H�ׂ�
        /// </summary>
        void EatStage()
        {
            _blackBoard.OnEatFoodInvoke(1);
            EatFood();

            // �X�e�[�^�X��ύX����
            //  ���ɃX�e�[�^�X�ւ̎Q�Ƃ���������H�������A�X�e�[�^�X�͂��̂т��p�����Ă��Ȃ��B
            //  �f���Q�[�g�̗��p�H
            //  ���b�Z�[�W���O�̏ꍇ�́A�N���X�S�̂ɑ��M����Ă��܂����A���̌̂ƘA�g������H
            // �̗͂��ǂꂭ�炢�񕜂���̂��H
            // �H���␅�ɂ͉񕜗ʂ�ݒ肵�A���̒l�����񕜂���
        }

        /// <summary>
        /// ���݂̃Z���̈ʒu�����g�̈ʒu�ōX�V����B
        /// ���̃Z���̈ʒu������Ύ��̃Z���̈ʒu�A�Ȃ���Ύ��g�̈ʒu�ōX�V����B
        /// </summary>
        /// <returns>���̃Z��������:true ���̃Z��������(�ړI�n�ɓ���):false</returns>
        bool TryStepNextCell()
        {
            _currentCellPos = _actor.position;

            if(_path.TryPop(out _nextCellPos))
            {
                // �o�H�̃Z���ƃL�����N�^�[�̍������Ⴄ�̂Ő����Ɉړ������邽�߂ɍ��������킹��
                _nextCellPos.y = _actor.position.y;
                _lerpProgress = 0;
                return true;
            }

            _nextCellPos = _actor.position;

            return false;
        }

        /// <summary>
        /// ���̃Z���𒲂ׂ�B
        /// �Z���ɂ��ǂ蒅���āA���̃Z��������ꍇ�͍X�V����B
        /// </summary>
        /// <returns>�Z���Ɉړ���/���̃Z��������:true ���̃Z��������(�ړI�n�ɓ�������):false</returns>
        bool CheckNextCell()
        {
            if (_actor.position != _nextCellPos) return true;
            return TryStepNextCell();
        }

        void Move()
        {
            _lerpProgress += Time.deltaTime * _blackBoard.Speed;
            _actor.position = Vector3.Lerp(_currentCellPos, _nextCellPos, _lerpProgress);
        }

        void EatFood()
        {
            Debug.Log("��������");
        }

        void DebugVisualize()
        {
            Stack<Vector3> temp = new(_path);
            foreach(Vector3 p in temp)
            {
                var g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                g.transform.position = p + Vector3.up;
            }
        }
    }
}