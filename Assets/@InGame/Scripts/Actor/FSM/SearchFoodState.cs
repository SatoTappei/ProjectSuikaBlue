using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PSB.InGame
{
    public class SearchFoodState : BaseState
    {
        //const float DistanceThreshold = 0.005f;

        IBlackBoardForState _blackBoard;
        Stack<Vector3> _path = new();
        Vector3 _nextCellPos;
        // �H���̃Z��������A�H���܂ł̌o�H�����݂��邩�ǂ����̃t���O
        bool _hasPath;

        public SearchFoodState(IBlackBoardForState blackBoard) : base(StateType.Evaluate)
        {
            _blackBoard = blackBoard;
        }

        protected override void Enter()
        {
            _hasPath = TryPathfinding();
            _path.TryPop(out _nextCellPos);
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            if (!_hasPath)
            {
                TryChangeState(_blackBoard.EvaluateState);
                return;
            }

            if (!StepNextCell())
            {
                TryChangeState(_blackBoard.EvaluateState);
                return;
            }

            Move();

            // TODO: �o�H�T�� -> �o�H���̃Z���̍X�V -> �ړ� �̗��ꂪ�o�����̂Ńe�X�g���Ė{���ɓ���������

            // ������
            // �H�ׂ�
            // �]���X�e�[�g�ɑJ��

            Log();
        }

        bool TryPathfinding()
        {
            _path.Clear();

            // �H���̃Z�������邩���ׂ�
            if (FieldManager.Instance.TryGetResourceCells(ResourceType.Tree, out List<Cell> cellList))
            {
                // �H���̃Z�����߂����Ɍo�H�T��
                Vector3 pos = _blackBoard.Transform.position;
                foreach(Cell food in cellList.OrderBy(c => Vector3.SqrMagnitude(c.Pos - pos)))
                {
                    if(FieldManager.Instance.TryGetPath(pos, food.Pos, out _path))
                    {
                        return true;
                    }
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// ���Ɍ������Z���̍X�V
        /// </summary>
        /// <returns>�Z���Ɉړ���/���̃Z��������:true ���̃Z��������(�ړI�n�ɓ�������):false</returns>
        bool StepNextCell()
        {
            if (_blackBoard.Transform.position != _nextCellPos) return true;
            return _path.TryPop(out _nextCellPos);
        }

        void Move()
        {
            Vector3 from = _blackBoard.Transform.position;
            Vector3 to = _nextCellPos;
            _blackBoard.Transform.position = Vector3.Slerp(from, to, Time.deltaTime * _blackBoard.Speed);
        }
    }
}