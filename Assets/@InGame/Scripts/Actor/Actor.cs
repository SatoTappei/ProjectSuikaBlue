using UnityEngine;
using UnityEngine.Events;
using UniRx;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

namespace PSB.InGame
{
    public enum ActorType
    {
        None,
        Kinpatsu,
        KinpatsuLeader,
        Kurokami,
    }

    public enum Sex
    {
        Male,
        Female,
    }

    /// <summary>
    /// スポナーから生成され、Controllerによって操作される。
    /// 単体で動作することを考慮していないのでシーン上に直に配置しても機能しない。
    /// </summary>
    [RequireComponent(typeof(DataContext))]
    public class Actor : MonoBehaviour, IReadOnlyActorStatus
    {
        public static event UnityAction<Actor> OnSpawned;

        [SerializeField] DataContext _context;
        [SerializeField] SkinnedMeshRenderer _renderer;
        [SerializeField] Material _defaultMaterial;

        ActionEvaluator _evaluator;
        BaseState _currentState;
        Collider[] _detected = new Collider[8];
        Coroutine _spawnChild; // 交尾をキャンセル可能にするため
        SpawnChildMessage _spawnChildMessage = new();
        bool _initialized;
        bool _isDead;
        bool _isMating; // 交尾中フラグ

        // キャラクターの各種パラメータ。初期化前に読み取った場合は仮の値を返す。
        public float Food         => _initialized ? _context.Food.Percentage : default;
        public float Water        => _initialized ? _context.Water.Percentage : default;
        public float HP           => _initialized ? _context.HP.Percentage : default;
        public float LifeSpan     => _initialized ? _context.LifeSpan.Percentage : default;
        public float BreedingRate => _initialized ? _context.BreedingRate.Percentage : default;
        public StateType State    => _initialized ? _currentState.Type : StateType.Base;
        public ActorType Type     => _initialized ? _context.Type : ActorType.None;
        public Sex Sex            => _initialized ? _context.Sex : Sex.Male;
        // 死んだ場合(プールに返却した)のフラグ
        public bool IsDead => _initialized ? _isDead : false;

        /// <summary>
        /// スポナーから生成された際にスポナー側が呼び出して初期化する必要がある。
        /// </summary>
        public void Init(uint? gene = null) 
        {
            _isDead = false;

            _context.Init(gene);
            ApplyGene();
            _currentState = _context.EvaluateState;
            _evaluator ??= new(_context);
            // 初期位置のセル上に居るという情報を書き込む
            FieldManager.Instance.SetActorOnCell(transform.position, _context.Type);
            // わかりやすいように名前に性別を反映しておく
            name += _context.Sex == Sex.Male ? "♂" : "♀";

            OnSpawned?.Invoke(this);
            _initialized = true;
            MessageBroker.Default.Publish(new ActorSpawnMessage() { Type = _context.Type });
        }

        // 死亡する際に非表示になる
        void OnDisable()
        {
            _isDead = true;
        }

        /// <summary>
        /// 遺伝子を反映してサイズと色を変える
        /// </summary>
        void ApplyGene()
        {
            _context.Model.localScale *= _context.Size;

            // 現在のマテリアルを削除
            Destroy(_renderer.material);
            // デフォルトのコピーからマテリアルを作成
            Material next = new(_defaultMaterial);
            next.SetColor("_BaseColor", _context.Color);
            _renderer.material = next;
        }

        /// <summary>
        /// パラメータを1フレーム分だけ変化させる
        /// </summary>
        public void StepParams()
        {
            _context.StepFood();
            _context.StepWater();
            _context.StepLifeSpan();
           
            // 食料と水分が0以下なら体力を減らす
            if (_context.Food.IsBelowZero && _context.Water.IsBelowZero)
            {
                _context.StepHp();
            }
            // 体力が一定以上なら繁殖率が増加する
            if (_context.IsBreedingRateIncrease)
            {
                _context.StepBreedingRate();
            }
        }

        /// <summary>
        /// 現在のステートを1フレーム分だけ更新する
        /// </summary>
        public void StepAction()
        {
            _currentState = _currentState.Update();
        }

        /// <summary>
        /// 評価ステートだった場合にリセットする処理
        /// </summary>
        public void ResetOnEvaluateState()
        {
            // 子供を産むコルーチンを停止
            if (_spawnChild != null) StopCoroutine(_spawnChild);
            _isMating = false;
        }

        /// <summary>
        /// 評価ステート以外では評価しないことで、毎フレーム評価処理を行うのを防ぐ
        /// 自身の情報とリーダーの評価値を元に次の行動を決める。
        /// </summary>
        public void Evaluate(float[] leaderEvaluate)
        {
            // 周囲の敵を検知
            //_sightSensor.TrySearchTarget(_context.EnemyTag, out _context.Enemy);

            // 視界で捉えるもの
            // 敵･･･経路があれば攻撃/逃げる
            // 繁殖相手
            // 資源･･･経路があれば向かう

            // 評価値を用いて評価を行う
            // 繁殖が選択された場合
            //  雄は繁殖可能な雌を探す。
            //  雌は繁殖相手がいる場合は･･･
            //  繁殖可能なら繁殖相手を検知
            //  検知した相手までの経路がある

            // 敵に狙われている場合は、攻撃もしくは逃げることが最優先なので、評価値より先に判定する
            
            // 敵を探す
            SearchEnemy();

            // 評価値を用いて次の行動を選択
            ActionType action = _evaluator.SelectAction(leaderEvaluate);
            // 死亡はそのまま死ぬ
            if      (action == ActionType.Killed) _context.NextAction = ActionType.Killed;
            else if (action == ActionType.Senility) _context.NextAction = ActionType.Senility;
            // 攻撃/逃げる場合は経路が必要
            else if (action == ActionType.Attack)
            {
                if (TryPathfindingToEnemy()) _context.NextAction = ActionType.Attack;
                else _context.Enemy = null; // 他のステートに遷移するので敵への参照を削除
            }
            else if (action == ActionType.Escape)
            {
                if (TryPathfindingToEscapePoint()) _context.NextAction = ActionType.Escape;
                else _context.Enemy = null; // 他のステートに遷移するので敵への参照を削除
            }
            // 繁殖する場合は雄と雌で取る行動が違う
            else if (action == ActionType.Breed)
            {
                if (_context.Sex == Sex.Male && TryDetectPartner()) _context.NextAction = ActionType.Breed;
                else if (_context.Sex == Sex.Female) _context.NextAction = ActionType.Breed;
            }
            // 水もしくは食料を探す場合、対象の資源までの経路が必要
            else if (action == ActionType.SearchWater && TryDetectResource(ResourceType.Water))
            {
                _context.NextAction = ActionType.SearchWater;
            }
            else if (action == ActionType.SearchFood && TryDetectResource(ResourceType.Tree))
            {
                _context.NextAction = ActionType.SearchFood;
            }
            else
            {
                // ランダムに隣のセルに移動する
                _context.NextAction = ActionType.Wander;
            }
        }

        /// <summary>
        /// 視界内の敵を探す
        /// 複数検知した場合は一番手近い敵を対象とする
        /// </summary>
        void SearchEnemy()
        {
            Array.Clear(_detected, 0, _detected.Length);

            Vector3 pos = transform.position;
            float radius = _context.Base.SightRadius;
            LayerMask layer = _context.Base.SightTargetLayer;
            if (Physics.OverlapSphereNonAlloc(pos, radius, _detected, layer) == 0) return;

            // 近い順に配列に入っているので、一番近い敵を対象の敵として書き込む。
            foreach (Collider collider in _detected)
            {
                if (collider == null) break;
                if (collider.CompareTag(_context.EnemyTag))
                {
                    collider.TryGetComponent(out _context.Enemy);
                    break;
                }
            }
        }

        /// <summary>
        /// 敵までの経路を探索する
        /// </summary>
        /// <returns>経路あり:true なし:false</returns>
        bool TryPathfindingToEnemy()
        {
            DeletePath();

            // グリッド上で距離比較
            Vector3 pos = transform.position;
            Vector3 enemyPos = _context.Enemy.transform.position;
            Vector2Int currentIndex = FieldManager.Instance.WorldPosToGridIndex(pos);
            Vector2Int enemyIndex = FieldManager.Instance.WorldPosToGridIndex(enemyPos);
            int dx = Mathf.Abs(currentIndex.x - enemyIndex.x);
            int dy = Mathf.Abs(currentIndex.y - enemyIndex.y);
            if (dx <= 1 && dy <= 1)
            {
                // 隣のセルにある場合は移動しないので、現在地を経路として追加する
                _context.Path.Add(pos);
                FieldManager.Instance.SetActorOnCell(pos, _context.Type);
                return true;
            }
            else
            {
                // 対象のセル + 周囲八近傍に対して経路探索
                foreach (Vector2Int dir in Utility.SelfAndEightDirections)
                {
                    Vector2Int dirIndex = enemyIndex + dir;
                    // 経路が見つからなかった場合は弾く
                    if (!FieldManager.Instance.TryGetPath(currentIndex, dirIndex, out _context.Path)) continue;
                    // 経路の末端(敵のセルの隣)にキャラクターがいる場合は弾く
                    int goal = _context.Path.Count - 1;
                    if (FieldManager.Instance.IsActorOnCell(_context.Path[goal], out ActorType _)) continue;

                    FieldManager.Instance.SetActorOnCell(_context.Path[goal], _context.Type);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 逃げる経路を探索する
        /// </summary>
        /// <returns>経路あり:true なし:false</returns>
        bool TryPathfindingToEscapePoint()
        {
            DeletePath();

            // グリッド上で距離比較
            Vector3 pos = transform.position;
            Vector3 enemyPos = _context.Enemy.transform.position;
            Vector3 dir = (pos - enemyPos).normalized;
            Vector2Int currentIndex = FieldManager.Instance.WorldPosToGridIndex(pos);
            for (int i = 10; i >= 1; i--) // 適当な値
            {
                Vector3 escapePos = dir * i;
                Vector2Int escapeIndex = FieldManager.Instance.WorldPosToGridIndex(escapePos);
                int dx = Mathf.Abs(currentIndex.x - escapeIndex.x);
                int dy = Mathf.Abs(currentIndex.y - escapeIndex.y);
                if (dx <= 1 && dy <= 1)
                {
                    // 隣のセルにある場合は移動しないので、現在地を経路として追加する
                    _context.Path.Add(pos);
                    FieldManager.Instance.SetActorOnCell(pos, _context.Type);
                    return true;
                }
                else
                {
                    // 経路が見つからなかった場合は弾く
                    if (!FieldManager.Instance.TryGetPath(currentIndex, escapeIndex, out _context.Path)) continue;
                    // 経路の末端(敵のセルの隣)にキャラクターがいる場合は弾く
                    int goal = _context.Path.Count - 1;
                    if (FieldManager.Instance.IsActorOnCell(_context.Path[goal], out ActorType _)) continue;

                    FieldManager.Instance.SetActorOnCell(_context.Path[goal], _context.Type);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 現在の経路を削除(空にする)し、末端の予約を削除する。
        /// 経路を探索する際に呼ばないと以前の経路の末端の予約が残ったままになってしまう。
        /// </summary>
        void DeletePath()
        {
            if (_context.Path.Count > 0)
            {
                int goal = _context.Path.Count - 1;
                FieldManager.Instance.SetActorOnCell(_context.Path[goal], ActorType.None);
                _context.Path.Clear();
            }
        }

        /// <summary>
        /// 雌を検知し、経路探索を行う。経路が見つかった場合はゴールのセルを予約する。
        /// 雌のセル + 周囲八近傍 のセルへの経路が存在するか調べる
        /// </summary>
        /// <returns>雌への経路がある:true 雌がいないof雌への経路が無い:false</returns>
        public bool TryDetectPartner()
        {
            Array.Clear(_detected, 0, _detected.Length);
            DeletePath();

            Vector3 pos = transform.position;
            float radius = _context.Base.SightRadius;
            LayerMask layer = _context.Base.SightTargetLayer;
            if (Physics.OverlapSphereNonAlloc(pos, radius, _detected, layer) == 0) return false;

            foreach (Collider collider in _detected)
            {
                if (collider == null) break;
                if (collider.transform == transform) continue; // 自分を弾く
                // 雌以外を弾く
                if (collider.TryGetComponent(out DataContext target) && target.Sex == Sex.Female)
                {
                    // グリッド上で距離比較
                    Vector2Int currentIndex = FieldManager.Instance.WorldPosToGridIndex(pos);
                    Vector2Int targetIndex = FieldManager.Instance.WorldPosToGridIndex(target.Transform.position);
                    int dx = Mathf.Abs(currentIndex.x - targetIndex.x);
                    int dy = Mathf.Abs(currentIndex.y - targetIndex.y);
                    if (dx <= 1 && dy <= 1)
                    {
                        // 隣のセルにある場合は移動しないので、現在地を経路として追加する
                        _context.Path.Add(pos);
                        FieldManager.Instance.SetActorOnCell(pos, _context.Type);
                        return true;
                    }
                    else
                    {
                        // 対象のセル + 周囲八近傍に対して経路探索
                        foreach (Vector2Int dir in Utility.SelfAndEightDirections)
                        {
                            Vector2Int dirIndex = targetIndex + dir;
                            // 経路が見つからなかった場合は弾く
                            if (!FieldManager.Instance.TryGetPath(currentIndex, dirIndex, out _context.Path)) continue;
                            // 経路の末端(資源のセルの隣)にキャラクターがいる場合は弾く
                            int goal = _context.Path.Count - 1;
                            if (FieldManager.Instance.IsActorOnCell(_context.Path[goal], out ActorType _)) continue;

                            FieldManager.Instance.SetActorOnCell(_context.Path[goal], _context.Type);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 資源までの経路探索
        /// 経路が見つかった場合はゴールのセルを予約する
        /// </summary>
        /// <returns>経路あり:true 経路無し:false</returns>
        bool TryDetectResource(ResourceType resource)
        {
            DeletePath();

            // 食料のセルがあるか調べる
            if (FieldManager.Instance.TryGetResourceCells(resource, out List<Cell> cellList))
            {
                // 食料のセルを近い順に経路探索
                Vector3 pos = transform.position;
                foreach (Cell food in cellList.OrderBy(c => Vector3.SqrMagnitude(c.Pos - pos)))
                {
                    // TODO:全ての食料に対して経路探索をすると重いのである程度の所で打ち切る処理

                    Vector2Int currentIndex = FieldManager.Instance.WorldPosToGridIndex(pos);
                    Vector2Int foodIndex = FieldManager.Instance.WorldPosToGridIndex(food.Pos);

                    int dx = Mathf.Abs(currentIndex.x - foodIndex.x);
                    int dy = Mathf.Abs(currentIndex.y - foodIndex.y);
                    if (dx <= 1 && dy <= 1)
                    {
                        // 隣のセルに食料がある場合は移動しないので、現在地を経路として追加する
                        _context.Path.Add(pos);
                        FieldManager.Instance.SetActorOnCell(pos, _context.Type);
                        return true;
                    }
                    else
                    {
                        // 対象のセル + 周囲八近傍に対して経路探索
                        foreach (Vector2Int dir in Utility.SelfAndEightDirections)
                        {
                            Vector2Int targetIndex = foodIndex + dir;
                            // 経路が見つからなかった場合は弾く
                            if (!FieldManager.Instance.TryGetPath(currentIndex, targetIndex, out _context.Path)) continue;
                            // 経路の末端(資源のセルの隣)に資源キャラクターがいる場合は弾く
                            int goal = _context.Path.Count - 1;
                            FieldManager.Instance.TryGetCell(_context.Path[goal], out Cell cell);
                            if (!cell.IsEmpty) continue;

                            FieldManager.Instance.SetActorOnCell(_context.Path[goal], _context.Type);
                            return true;
                        }
                    }
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// 雄が雌を呼び出す。
        /// 交尾中でない場合、一定時間経過後、子供を産む処理を呼び出す。
        /// キャンセルすることが出来る。
        /// </summary>
        public void SpawnChild(uint maleGene, UnityAction callback = null)
        {           
            if (_currentState.Type != StateType.FemaleBreed)
            {
                Debug.LogWarning($"{name} 雌の繁殖ステート以外で子供を産む処理が呼ばれている。");
            }

            if (!_isMating)
            {
                _isMating = true;
                _spawnChild = StartCoroutine(SpawnChildCoroutine(maleGene, callback));
            }
        }

        IEnumerator SpawnChildCoroutine(uint maleGene, UnityAction callback = null)
        {
            // 演出としてパーティクルを何回か出す
            int c = 3; // 繰り返す回数は適当
            for (int i = 0; i < c; i++) 
            {
                MessageBroker.Default.Publish(new PlayParticleMessage()
                {
                    Type = ParticleType.Mating,
                    Pos = transform.position,
                });
                yield return new WaitForSeconds(_context.Base.MatingTime / c);
            }
            // 周囲八近傍のセルに子供を産む
            if (TryGetNeighbourPos(out Vector3 pos))
            {
                _spawnChildMessage.Gene1 = maleGene;
                _spawnChildMessage.Gene2 = _context.Gene;
                _spawnChildMessage.Food = _context.Food.Value;
                _spawnChildMessage.Water = _context.Water.Value;
                _spawnChildMessage.HP = _context.HP.Value;
                _spawnChildMessage.LifeSpan = _context.LifeSpan.Value;
                _spawnChildMessage.Pos = pos;
                MessageBroker.Default.Publish(_spawnChildMessage);
            }

            yield return null;
            _isMating = false;
            callback?.Invoke();
        }

        /// <summary>
        /// 周囲八近傍のセルを調べ、子を生成する位置を取得する
        /// </summary>
        /// <returns>取得出来た:true 生成するセルが無い:false</returns>
        bool TryGetNeighbourPos(out Vector3 pos)
        {
            Vector2Int index = FieldManager.Instance.WorldPosToGridIndex(transform.position);
            foreach (Vector2Int dir in Utility.EightDirections)
            {
                Vector2Int neighbourIndex = index + dir;
                // 陸地かつ資源が無く、キャラクターがいないセル
                if (!FieldManager.Instance.IsWithInGrid(neighbourIndex)) continue;
                if (!FieldManager.Instance.TryGetCell(neighbourIndex, out Cell cell)) continue;
                if (!cell.IsWalkable) continue;
                if (!cell.IsEmpty) continue;

                // 生成する高さを自身の高さに合わせる
                pos = cell.Pos;
                pos.y = transform.position.y;

                return true;
            }

            pos = Vector3.zero;
            return false;
        }

        void OnDrawGizmos()
        {
            // 周囲八近傍のセルの距離を描画
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, Utility.NeighbourCellRadius);
        }
    }

    // 出来れば: 他のステートも繁殖ステートと同じく途中で餓死＆殺害されるよう修正
    // 出来れば: 生成数による生成の判定がバグっている
    // 変更案: 個体の強さを数値化する。サイズと色で求め、各種評価にはその値を使う。
    // 次タスク: リーダーが死んだ際の処理、群れの長がいないといけない
    // 次タスク: リーダーが死ぬとランダムで次のリーダーが決まる。群れの最後の1匹が死ぬとがめおべら

    // 評価値で次何をしたいか選択。
    // Actor側での敵の検知。
    // 案:1つではなく、優先度でソートして次の行動を保持。
    // Actor側で諸々の検知を行い、行動を選択できればステート側で検知や分岐をしなくて済む？

    // リーダーが死ぬとランダムで次のリーダーが決まる。
    // 群れの最後の1匹が死ぬとがめおべら

    // 一定間隔で水分と食料のうち少ないほうを満たそうとする
    // 具体的には何回かセルを移動したらチェック
    // 満たされているかチェック。満たされている場合は何もしない。
    // 近くの水源もしくは食料のマスを取得
    // ハムで経路探索。経路があれば向かう。無い場合はなにもしない

    // 行動の単位はアニメーション
    // アニメーションが終わったら次の行動を選択する
    // つまり、行動A -> 判断 -> 行動B と行動毎に中央の状態に戻って次の行動をチェックする必要がある。
    // 通常のステートベースでは無い。
    // 外部からの攻撃などで行動中に死ぬ場合がある？

    // 繁殖する際は親の特徴を受け継ぐ(遺伝)
    // 遺伝的アルゴリズム
    // 遺伝子は4つ、RGB+サイズ、

    // キャラクター:黒髪
    // リーダーがいない。各々が勝手に行動する。
    // 金髪を見つけると攻撃してくる。
    // 攻撃で死ぬ。
    // 繁殖はしない。
    // ランダムで沸く。
    // 死ぬたびに遺伝的な変異をする？強くなったり弱くなったり
}