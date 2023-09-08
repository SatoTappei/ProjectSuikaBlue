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

        // セルのScaleが 1 の場合に、隣接するセルをレイキャストで取得できる半径
        public const float NeighbourCellRadius = 1.45f;

        readonly MoveModule _move;
        readonly FieldModule _field;
        Stage _stage;
        float _matingTimer = 0;
        // 周囲八近傍のセルの分だけ判定するので 8 で固定
        Collider[] _detected = new Collider[8];
        // 各種行動のフラグ、上から順に処理される
        bool _firstStep; // 経路のスタート地点から次のセルに移動中
        bool _isMating;  // 交尾中
        bool _completed; // 交尾完了

        public MaleBreedState(DataContext context) : base(context, StateType.MaleBreed) 
        {
            _move = new(context);
            _field = new(context);
        }

        protected override void Enter()
        {
            TryStepNextCell();
            _field.SetActorOnCell();
            _stage = Stage.Move;
            _matingTimer = 0;
            _firstStep = true;
            _isMating = false;
            _completed = false;
        }

        protected override void Exit()
        {
            // 使い終わった経路を消す
            Context.Path.Clear();
        }

        protected override void Stay()
        {
            // 移動
            if (_stage == Stage.Move)
            {
                if (_move.OnNextCell)
                {
                    // 経路のスタート地点は予約されているので、次のセルに移動した際に消す
                    // 全てのセルに対して行うと、別のキャラクターで予約したセルまで消してしまう。
                    if (_firstStep)
                    {
                        _firstStep = false;
                        _field.DeleteActorOnCell(_move.CurrentCellPos);
                    }

                    // 別のステートが選択されていた場合は遷移する
                    if (Context.ShouldChangeState(this)) { ToEvaluateState(); return; }

                    if (TryStepNextCell())
                    {
                        // 経路の途中のセルの場合の処理
                    }
                    else
                    {
                        _stage = Stage.Mating;
                    }
                }
                else
                {
                    _move.Move();
                }
            }
            // 交尾
            else if (_stage == Stage.Mating)
            {
                // 繁殖相手の隣に移動完了、周囲八近傍の雌に対して交尾
                if (!_isMating)
                {
                    _isMating = true;
                    // 交尾する雌がいない場合は評価ステートに戻る
                    if (!CallNeighbourFemale()) { ToEvaluateState(); return; }
                }

                // 交尾中に交尾にかかる時間以上経過した場合は、交尾失敗とみなし評価ステートに戻る
                if (_isMating) _matingTimer += Time.deltaTime;
                if (_matingTimer > Context.Base.MatingTime)
                {
                    // 連続で繁殖ステートに遷移してこないように繁殖率を50％にする
                    Context.BreedingRate.Value = StatusBase.Max / 2;
                    ToEvaluateState();
                    return;
                }

                // 交尾中は死んでも集合でも他のステートに遷移しない。

                // 交尾完了フラグが立った場合は評価ステートに戻る
                if (_completed)
                {
                    Context.BreedingRate.Value = 0;
                    ToEvaluateState();
                    return;
                }
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

        /// <summary>
        /// 周囲八近傍の繁殖ステートの雌に対して交尾を呼びかける。
        /// </summary>
        /// <returns>交尾成功:true 交尾失敗:false</returns>
        bool CallNeighbourFemale()
        {
            Vector3 pos = Context.Transform.position;
            LayerMask layer = Context.Base.SightTargetLayer;
            int count = Physics.OverlapSphereNonAlloc(pos, NeighbourCellRadius, _detected, layer);
            if (count == 0) return false;

            foreach (Collider collider in _detected)
            {
                if (collider == null) break;
                if (collider.transform == Context.Transform) continue; // 自分を弾く
                // 雌以外を弾く
                if (!(collider.TryGetComponent(out Actor female) && female.Sex == Sex.Female)) continue;
                // セルの情報が必要ないので、雌と隣り合っているかどうかの判定を距離の2乗で行う。
                Vector3 vec = Context.Transform.position - female.transform.position;
                float sq = NeighbourCellRadius * NeighbourCellRadius;
                if (Vector3.SqrMagnitude(vec) > sq) continue;
                // 雌が繁殖ステートの場合のみ
                if (female.State != StateType.FemaleBreed) continue;

                // 子供を産んだ場合は繁殖完了フラグを立てるコールバックを呼び出してもらう。
                female.SpawnChild(Context.Gene, () => _completed = true);
                return true;
            }

            return false;
        }
    }
}