namespace FSM
{
    /// <summary>
    /// キャラクターが生成された際の初期状態
    /// ボスと敵で共通の状態
    /// </summary>
    public class InitState : EnemyStateBase
    {
        CommonLayerBlackBoard _blackBoard;

        public InitState(EnemyStateType type, CommonLayerBlackBoard blackBoard) : base(type)
        {
            _blackBoard = blackBoard;
        }

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            // 生成されてからこの状態で諸々の初期設定を終えた後にプレイヤー未発見状態に遷移する
            TryChangeState(_blackBoard[EnemyStateType.PlayerUndetected]);
        }
    }
}
