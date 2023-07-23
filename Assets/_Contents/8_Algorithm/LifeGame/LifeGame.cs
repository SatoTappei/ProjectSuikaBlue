using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeGame : MonoBehaviour
{
    [SerializeField] TextAsset _map;
    [SerializeField] Transform _parent;

    GridLayoutGroup _gridLayoutGroup;

    void Awake()
    {
        _gridLayoutGroup = _parent.GetComponent<GridLayoutGroup>();

        string[] map = _map.text.Split("\n");
        _gridLayoutGroup.constraintCount = map.Length;
        
        for(int i = 0; i < map.Length; i++)
        {
            map[i] = map[i].Trim();
            for(int k = 0; k < map[i].Length; k++)
            {
                GameObject cell = new GameObject();
                cell.AddComponent<Image>();
                cell.transform.SetParent(_parent);
            }
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
