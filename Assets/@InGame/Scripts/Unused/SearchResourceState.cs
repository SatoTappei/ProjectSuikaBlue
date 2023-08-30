//using System.Linq;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Events;

//namespace PSB.InGame
//{
//    // TODO:食料/水の探すステートが未使用
//    // 食料と水で現状ほぼ同じなので共通化させたクラス
//    // アニメーションや資源タイルの変化などで違うところがあるかもしれないので、作りきった後に継承するか決める

//    // ↓継承した場合のコンストラクタ例
//    // public SearchWarterState(IBlackBoardForState blackBoard, StateType type) : base(
//    // blackBoard: blackBoard,
//    // stateType: type,
//    // effectValue: 100, // 状態の効果量
//    // effectDelta: 100, // 一度の処理で効果する量
//    // resourceType: ResourceType.Stone, // 対象の資源
//    // onEatResource: blackBoard.OnDrinkWaterInvoke // 黒板のステータスに反映するメソッド
//    // )
//    // { }

//    /// <summary>
//    /// 資源のセルまで移動し、設定された効果値だけステータスの食料の値を徐々に回復する
//    /// ステータスのパラメータが効果値を上回っていても、効果値分の回復処理が実行される
//    /// </summary>
//    public class SearchResourceState : BaseState
//    {
//        enum Stage
//        {
//            Move,
//            Eat,
//        }

//        // 資源毎
//        readonly IBlackBoardForState BlackBoard;
//        readonly Transform Actor;
//        readonly int EffectValue;
//        readonly int EffectDelta;
//        // 共通
//        readonly ResourceType ResourceType;
//        readonly UnityAction<float> OnEatResource;
        
//        Stage _stage;
//        Stack<Vector3> _path = new();
//        Vector3 _currentCellPos;
//        Vector3 _nextCellPos;
//        float _lerpProgress;
//        float _effectProgress;
//        // 食料のセルがあり、食料までの経路が存在するかどうかのフラグ
//        bool _hasPath;

//        bool OnNextCell => Actor.position == _nextCellPos;

//        public SearchResourceState(IBlackBoardForState blackBoard, StateType stateType, int effectValue, int effectDelta,
//            ResourceType resourceType, UnityAction<float> onEatResource) : base(stateType)
//        {
//            BlackBoard = blackBoard;
//            Actor = blackBoard.Transform;
//            EffectValue = effectValue;
//            EffectDelta = effectDelta;
//            ResourceType = resourceType;
//            OnEatResource = onEatResource;
//        }

//        protected override void Enter()
//        {
//            _stage = Stage.Move;

//            _effectProgress = 0;

//            _hasPath = TryPathfinding();
//            TryStepNextCell();
//        }

//        protected override void Exit()
//        {
//        }

//        protected override void Stay()
//        {
//            // 経路が無いので評価ステートに遷移
//            if (!_hasPath) { ToEvaluateState(); return; }

//            switch (_stage)
//            {
//                case Stage.Move: MoveStage(); break;
//                case Stage.Eat: EatStage(); break;
//            }
//        }

//        bool TryPathfinding()
//        {
//            _path.Clear();

//            // 食料のセルがあるか調べる
//            if (FieldManager.Instance.TryGetResourceCells(ResourceType, out List<Cell> cellList))
//            {
//                // 食料のセルを近い順に経路探索
//                Vector3 pos = Actor.position;
//                foreach (Cell food in cellList.OrderBy(c => Vector3.SqrMagnitude(c.Pos - pos)))
//                {
//                    if (FieldManager.Instance.TryGetPath(pos, food.Pos, out _path)) // <- ｱﾔｼｲ
//                    {
//                        return true;
//                    }
//                }

//                return false;
//            }

//            return false;
//        }

//        void ToEvaluateState() => TryChangeState(BlackBoard.EvaluateState);

//        /// <summary>
//        /// 食料のセルに移動
//        /// </summary>
//        void MoveStage()
//        {
//            // 次のセルの上に来た場合はチェックする
//            if (OnNextCell)
//            {
//                // 違うステートに遷移する場合は一度評価ステートを経由する
//                if (BlackBoard.NextState != this) { ToEvaluateState(); return; }

//                if (TryStepNextCell())
//                {
//                    // 経路の途中のセルの場合の処理
//                }
//                else
//                {
//                    _stage = Stage.Eat; // 食べる状態へ
//                }
//            }
//            else
//            {
//                Move();
//            }
//        }

//        /// <summary>
//        /// 食料を食べる
//        /// </summary>
//        void EatStage()
//        {
//            if (!StepEatProgress()) { ToEvaluateState(); return; }
//        }

//        /// <summary>
//        /// 現在のセルの位置を自身の位置で更新する。
//        /// 次のセルの位置をあれば次のセルの位置、なければ自身の位置で更新する。
//        /// </summary>
//        /// <returns>次のセルがある:true 次のセルが無い(目的地に到着):false</returns>
//        bool TryStepNextCell()
//        {
//            _currentCellPos = Actor.position;

//            if (_path.TryPop(out _nextCellPos))
//            {
//                // 経路のセルとキャラクターの高さが違うので水平に移動させるために高さを合わせる
//                _nextCellPos.y = Actor.position.y;
//                _lerpProgress = 0;
//                return true;
//            }

//            _nextCellPos = Actor.position;

//            return false;
//        }

//        void Move()
//        {
//            _lerpProgress += Time.deltaTime * BlackBoard.Speed;
//            Actor.position = Vector3.Lerp(_currentCellPos, _nextCellPos, _lerpProgress);
//        }

//        /// <summary>
//        /// 回復の進捗度を進める
//        /// </summary>
//        /// <returns>回復の進捗中:true 回復の進捗が回復値に達した:false</returns>
//        bool StepEatProgress()
//        {
//            float value = Time.deltaTime * EffectDelta;
//            _effectProgress += value;
//            OnEatResource(value); // 値の更新

//            return _effectProgress <= EffectValue;
//        }
//    }
//}