namespace PSB.InGame
{
    public class EscapeState : BaseState
    {
        readonly MoveModule _move;
        readonly FieldModule _field;
        bool _firstStep; // 経路のスタート地点から次のセルに移動中

        public EscapeState(DataContext context) : base(context, StateType.Escape)
        {
            _move = new(context);
            _field = new(context);
        }

        protected override void Enter()
        {
            TryStepNextCell();
            _field.SetOnCell();
            _firstStep = true;

            // びっくりマーク再生
            Context.PlayDiscoverEffect();
        }

        protected override void Exit()
        {
            Context.Enemy = null;
            // 使い終わった経路を消す
            Context.Path.Clear();
        }

        protected override void Stay()
        {
            if (_move.OnNextCell)
            {
                // 経路のスタート地点は予約されているので、次のセルに移動した際に消す
                // 全てのセルに対して行うと、別のキャラクターで予約したセルまで消してしまう。
                if (_firstStep)
                {
                    _firstStep = false;
                    _field.DeleteOnCell(_move.CurrentCellPos);
                }

                if (!TryStepNextCell()) { ToEvaluateState(); return; }
                // 別のステートが選択されていた場合は遷移する
                if (Context.ShouldChangeState(this)) { ToEvaluateState(); return; }
            }
            else
            {
                _move.Move();
            }
        }

        /// <summary>
        /// 各値を既定値に戻すことで、現在のセルの位置を自身の位置で更新する。
        /// 次のセルの位置をあれば次のセルの位置、なければ自身の位置で更新する。
        /// </summary>
        /// <returns>次のセルがある:true 次のセルが無い(目的地に到着):false</returns>
        bool TryStepNextCell()
        {
            _move.Reset();

            if (Context.Path.Count > 0)
            {
                // 経路の先頭(次のセル)から1つ取り出す
                _move.NextCellPos = Context.Path[0];
                Context.Path.RemoveAt(0);
                // 経路のセルとキャラクターの高さが違うので水平に移動させるために高さを合わせる
                _move.NextCellPos.y = Context.Transform.position.y;

                _move.Modify();
                _move.Look();
                return true;
            }
            else
            {
                _move.NextCellPos = Context.Transform.position;
                return false;
            }
        }
    }
}
