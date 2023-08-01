using UniRx.Toolkit;
using UnityEngine;

namespace MiniGame
{
    public class PlayerBulletPool : ObjectPool<PlayerBullet>
    {
        readonly PlayerBullet _prefab;
        readonly Transform _parent;

        public PlayerBulletPool(PlayerBullet prefab, string poolName)
        {
            _prefab = prefab;
            _parent = new GameObject(poolName).transform;
        }

        protected override PlayerBullet CreateInstance()
        {
            PlayerBullet bullet = Object.Instantiate(_prefab, _parent);
            bullet.Init(this);
            return bullet;
        }
    }
}
