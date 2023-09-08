using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// このステート自体が繁殖の処理を持つのではなく
    /// 交尾->出産の処理はActorクラスに書かれており、このステートは繁殖の処理を受け付けるだけ。
    /// </summary>
    public class FemaleBreedState : BaseState
    {
        const float TimeOut = 10.0f;

        FieldModule _field;
        float _timer;

        public FemaleBreedState(DataContext context) : base(context, StateType.FemaleBreed)
        {
            _field = new(context);
        }

        protected override void Enter()
        {
            _field.SetActorOnCell();
            _timer = 0;
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            // 時間切れで評価ステートに遷移
            _timer += Time.deltaTime;
            if (_timer > TimeOut)
            {
                // 繁殖率を50％にして連続でこのステートに遷移しないようにする
                Context.BreedingRate.Value = StatusBase.Max / 2;
                ToEvaluateState();
                return;
            }
        }
    }
}
