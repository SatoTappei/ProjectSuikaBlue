using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public class BreedState : BaseState
    {
        IBlackBoardForState _blackBoard;
        Transform _actor;

        public BreedState(IBlackBoardForState blackBoard) : base(StateType.Breed)
        {
            _blackBoard = blackBoard;
            _actor = blackBoard.Transform;
        }

        protected override void Enter()
        {
            
        }

        protected override void Exit()
        {
            
        }

        protected override void Stay()
        {
            // �Ƃ肠���������]���X�e�[�g�ɑJ�ڂ�����B
            ToEvaluateState();
        }

        void ToEvaluateState() => TryChangeState(_blackBoard.EvaluateState);
    }
}
