using UnityEngine;

public class Leader : MonoBehaviour
{
    [SerializeField] GameObject _symbolPrefab;

    GameObject _symbol;

    void Awake()
    {
        _symbol = Instantiate(_symbolPrefab);
        _symbol.transform.SetParent(transform);
        _symbol.transform.localPosition = Vector3.up;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // 黒板を用いて評価値を決める
    // リーダーの号令で集合させたい。
        // リーダーを中心にぐるぐるfor文で空いているセルを探索とかする？
}
