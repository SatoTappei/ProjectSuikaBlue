using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] Player _prefab;

        public Player Spawn(Vector3 pos)
        {
            return Instantiate(_prefab, pos, Quaternion.identity);
        }
    }
}
