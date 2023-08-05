using MiniGameECS;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace MiniGame
{
    [RequireComponent(typeof(Damageable))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] Transform _model;
        [SerializeField] Damageable _damageable;
        [Header("撃破時スコア")]
        [SerializeField] int _score = 100;
        [Header("移動速度に関する値")]
        [Tooltip(" 移動方向が完全に変わるまでの時間は 移動速度 と セルの大きさ に応じて調節する")]
        [SerializeField] float _dirChangeDuration = 3.0f;
        [SerializeField] float _moveSpeed = 1.5f;

        AudioModule _audio;
        VectorFieldManager _vectorFieldManager;
        Vector3 _currentDir;

        /// <summary>
        /// 生成した際に、生成側が必ず呼ぶ必要がある。コンストラクタの代わり
        /// </summary>
        public void Init(VectorFieldManager vectorFieldManager)
        {
            _vectorFieldManager = vectorFieldManager;
        }

        void Awake()
        {
            TryGetComponent(out _audio);

            // がめおべらになったら消える、スコアは入らない。
            MessageBroker.Default.Receive<GameOverMessage>()
                .Subscribe(_ => { Invalid(); PlayDefeatedEffect(gameObject); }).AddTo(this);

            // 撃破された際のコールバックに登録/削除
            _damageable.OnDefeated += Defeated;
            this.OnDestroyAsObservable().Subscribe(_ => _damageable.OnDefeated -= Defeated);
        }

        void Start()
        {
            // 本来はスポナーから生成されるが、直接シーンに配置した場合は探してくる
            _vectorFieldManager ??= FindFirstObjectByType<VectorFieldManager>();
        }

        void Update()
        {
            FollowVector();
            LookAtDirection();
        }

        /// <summary>
        /// ベクトルフィールド上の現在自身がいるセルのベクトルに沿って移動する
        /// 以下の条件を満たしている必要がある。
        /// 1.ベクトルフィールドは上下左右の4方向
        /// 2.DirChangeDurationとMoveSpeedを調整して両方ともセルの中心を通るような値になっている。
        /// </summary>
        void FollowVector()
        {
            Vector3 vector = _vectorFieldManager.GetCellVector(transform.position);
            // 線形補完することで、セルを跨いだ時にベクトルが変わったせいで、セルの辺上を移動するのを防ぐ。
            _currentDir = Vector3.Lerp(_currentDir, vector, Time.deltaTime * _dirChangeDuration);
            transform.Translate(_currentDir.normalized * Time.deltaTime * _moveSpeed);
        }

        void LookAtDirection() => _model.forward = _currentDir;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagUtility.PlayerTag))
            {
                // プレイヤーと衝突した場合は自身も撃破される
                other.GetComponent<IDamageable>().Damage(1, gameObject);
                Defeated(gameObject);
            }
        }

        void Defeated(GameObject attacker)
        {
            Invalid();
            PlayDefeatedEffect(attacker);
            MessageBroker.Default.Publish(new AddScoreMessage() { Score = _score });
        }

        void Invalid()
        {
            // スケールを0に変更＆コライダーの無効化で画面から消し、1秒後に削除する
            transform.localScale = Vector3.zero;
            GetComponent<Collider>().enabled = false;
            Destroy(gameObject, 1.0f);
        }

        void PlayDefeatedEffect(GameObject attacker)
        {
            // 音
            if (_audio != null) _audio.Play(AudioKey.SeBlood);
            
            Vector3 effectDir;
            // プレイヤーの弾に倒された場合は、弾の向いている方向
            if (attacker.TryGetComponent(out PlayerBullet bullet))
            {
                effectDir = bullet.Forward;
            }
            // プレイヤーへの激突などで死亡した場合は自身の後ろ方向へエフェクトを出す
            else
            {
                effectDir = -_model.forward;
            }
            MonoToEcsTransfer.Instance.AddData(transform.position, effectDir, EntityType.Debris);
        }
    }
}
