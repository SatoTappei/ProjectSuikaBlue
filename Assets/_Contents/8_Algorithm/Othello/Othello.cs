using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DiscType
{
    None = 0,
    Black = -1,
    White = 1,
}

public class Disc
{
    GameObject _blackDisc;
    GameObject _whiteDisc;
    DiscType _type;

    public Disc(GameObject blackDisc, GameObject whiteDisc)
    {
        _blackDisc = blackDisc;
        _whiteDisc = whiteDisc;
        Type = DiscType.None;
    }

    public DiscType Type
    {
        get => _type;
        set
        {
            _type = value;
            if (_type == DiscType.Black)
            {
                _blackDisc.SetActive(true);
                _whiteDisc.SetActive(false);
            }
            else if (_type == DiscType.White)
            {
                _blackDisc.SetActive(false);
                _whiteDisc.SetActive(true);
            }
            else
            {
                _blackDisc.SetActive(false);
                _whiteDisc.SetActive(false);
            }
        }
    }
    
}

public class Othello : MonoBehaviour
{
    [SerializeField] GameObject _blackDisc;
    [SerializeField] GameObject _whiteDisc;
    [SerializeField] Transform _discParent;
    [SerializeField] Transform _cursor;

    Disc[,] _board = new Disc[8, 8];
    int _currentY;
    int _currentX;

    void Awake()
    {
        CreateBoard();
        InitPlace();
        SelectCell(3, 3);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))    SelectCell(_currentX, ++_currentY);
        if (Input.GetKeyDown(KeyCode.DownArrow))  SelectCell(_currentX, --_currentY);
        if (Input.GetKeyDown(KeyCode.LeftArrow))  SelectCell(--_currentX, _currentY);
        if (Input.GetKeyDown(KeyCode.RightArrow)) SelectCell(++_currentX, _currentY);
        if (Input.GetKeyDown(KeyCode.Space)) TryPut(_currentY, _currentX, DiscType.White);
    }

    void CreateBoard()
    {
        for(int i = 0; i < _board.GetLength(0); i++)
        {
            for(int k = 0; k < _board.GetLength(1); k++)
            {
                float x = transform.position.x - _board.GetLength(0) / 2 + i + 0.5f;
                float z = transform.position.z - _board.GetLength(1) / 2 + k + 0.5f;
                GameObject blackDisc = Instantiate(_blackDisc, new Vector3(z, 0.1f, x), Quaternion.identity, _discParent);
                GameObject whiteDisc = Instantiate(_whiteDisc, new Vector3(z, 0.1f, x), Quaternion.identity, _discParent);
                _board[i, k] = new Disc(blackDisc, whiteDisc);
            }
        }
    }

    void InitPlace()
    {
        _board[3, 3].Type = DiscType.White;
        _board[3, 4].Type = DiscType.Black;
        _board[4, 3].Type = DiscType.Black;
        _board[4, 4].Type = DiscType.White;
    }

    bool TryPut(int y, int x, DiscType type)
    {
        if (_board[y, x].Type != DiscType.None) return false;
        
        _board[y, x].Type = type;
        return true;
    }

    void SelectCell(int x, int y)
    {
        _currentY = y;
        _currentX = x;

        // カーソルの移動はY座標はそのまま
        float cursorX = -_board.GetLength(0) / 2 + _currentX + 0.5f;
        float cursorY = _cursor.transform.position.y;
        float cursorZ = -_board.GetLength(1) / 2 + _currentY + 0.5f;
        _cursor.transform.position = new Vector3(cursorX, cursorY, cursorZ);
    }
}
