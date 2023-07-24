using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ライフゲームを実行するクラス。以下の2つのモードがある。<br></br>
/// 1.インスペクターで設定した行数＆列数で生成し、セルの生死がランダムで設定される<br></br>
/// 2.テキストファイルから読み込んだ文字列を元に行数＆列数＆セルの生死が設定される
/// </summary>
public class LifeGame : MonoBehaviour
{
    enum CreateType
    {
        Inspector,
        TextFile,
    }

    class Cell
    {
        public Image Img { get; set; }
        public byte Value { get; set; }
    }

    /// <summary>
    /// 8方向の配列、12時から時計回り
    /// </summary>
    static readonly Vector2Int[] EightDirections =
    {
        new (0, -1), new (1, -1), new (1, 0), new (1, 1),
        new (0, 1), new (-1, 1), new (-1, 0), new (-1, -1),
    };

    [SerializeField] InputField _generationInputField;
    [SerializeField] Text _generationView;
    [SerializeField] Button _nextGenerationButton;
    [SerializeField] Button _regenerateButton;
    [SerializeField] GridLayoutGroup _gridLayoutGroup;
    [SerializeField] Transform _parent;
    [Header("生成方法")]
    [SerializeField] CreateType _createType;
    [Header("インスペクターの値から生成する場合")]
    [Range(1, 30)]
    [SerializeField] int _row;
    [Range(1, 30)]
    [SerializeField] int _column;
    [Range(1, 900)]
    [SerializeField] int _initAliveCell;
    [Header("テキストファイルから生成する場合")]
    [SerializeField] TextAsset _blueprint;
    [Header("セルの色")]
    [SerializeField] Color _aliveCellColor;
    [SerializeField] Color _deadCellColor;
    [Range(0.05f, 3.0f)]
    [Header("世代更新間隔")]
    [SerializeField] float _updateInterval;

    Cell[,] _grid;
    /// <summary>
    /// グリッドの更新のタイミングで一時的に次の世代の生死を保持しておく配列
    /// 全てのセルの生死が決まった後にてグリッドに反映する
    /// </summary>
    byte[,] _tempNextGeneration;
    /// <summary>
    /// 現在の世代数
    /// </summary>
    int _generation; 

    int Generation
    {
        get => _generation;
        set
        {
            _generation = value;
            _generationView.text = "世代: " + _generation.ToString();
        }
    }
    /// <summary>
    /// 行数と列数をはみ出ないように修正した初期生成される生存のセルの数
    /// </summary>
    int FixedInitAliveCell => Mathf.Min(_initAliveCell, _row * _column);

    void Awake()
    {
        AddOnButtonClick();
        AddOnInputFieldInput();
        ResetGrid();
        InvokeRepeating(nameof(UpdateGrid), 0, _updateInterval);
    }

    /// <summary>
    /// ボタンをクリックした際の処理を登録する
    /// </summary>
    void AddOnButtonClick()
    {
        _nextGenerationButton.onClick.AddListener(UpdateGrid);
        _regenerateButton.onClick.AddListener(ResetGrid);
    }

    /// <summary>
    /// InputFieldに入力をした際の処理を登録する
    /// </summary>
    void AddOnInputFieldInput()
    {
        // 値が入力されたら自動的に世代が進むのを止める
        _generationInputField.onValueChanged.AddListener(value => 
        {
            if (value == string.Empty) return;
            CancelInvoke(nameof(UpdateGrid));
        });
        // 入力した世代に飛ぶ
        _generationInputField.onSubmit.AddListener(value =>
        {
            if (!int.TryParse(value, out int generation)) return;
            if (generation - Generation <= 0) return;

            while (generation != Generation)
            {
                UpdateGrid();
            }
        });
        // 入力をやめた際は再び自動的に世代を進める
        _generationInputField.onEndEdit.AddListener(value =>
        {
            if (_generationInputField.text == string.Empty) return;
            _generationInputField.text = string.Empty;
            InvokeRepeating(nameof(UpdateGrid), _updateInterval, _updateInterval);
        });
    }

    /// <summary>
    /// 再生成
    /// </summary>
    void ResetGrid()
    {
        if (_grid != null && _tempNextGeneration != null)
        {
            // グリッドと世代の一時保存用の配列をクリア
            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int k = 0; k < _grid.GetLength(1); k++)
                {
                    _grid[i, k] = null;
                    _tempNextGeneration[i, k] = default;
                }
            }
            // セルを全削除
            foreach (Transform child in _parent)
            {
                Destroy(child.gameObject);
            }
        }
        // グリッドを生成
        if (_createType == CreateType.Inspector) CreateGridFromInspector();
        else if (_createType == CreateType.TextFile) CreateGridFromTextFile();

        Generation = 1;
    }

    /// <summary>
    /// インスペクターで設定した値を元にグリッドを生成する
    /// </summary>
    void CreateGridFromInspector()
    {
        InitRowAndColumn(_column, _row);
        // 初期生存セルの数だけフラグを立てる
        bool[] flags = new bool[_row * _column];
        for (int i = 0; i < FixedInitAliveCell; i++)
        {
            flags[i] = true;
        }
        // シャッフル
        int count = flags.Length;
        for(int i = 0; i < flags.Length; i++)
        {
            int r = UnityEngine.Random.Range(0, count);
            bool temp = flags[count - 1];
            flags[count - 1] = flags[r];
            flags[r] = temp;
            count--;
        }
        // フラグが立っていれば生存、そうでなければ死亡したセルとして生成
        for (int i = 0; i < _column; i++)
        {
            for (int k = 0; k < _row; k++)
            {
                CreateCell(i, k, flags[i * _row + k] ? '1' : '0');
            }
        }
    }

    /// <summary>
    /// テキストファイルからグリッドを生成する
    /// </summary>
    void CreateGridFromTextFile()
    {
        // グリッドの1行を調整
        string[] lines = _blueprint.text.Split("\n");
        InitRowAndColumn(lines.Length, lines[0].Trim().Length);
        // セルの生成
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = lines[i].Trim();
            for (int k = 0; k < lines[i].Length; k++)
            {
                CreateCell(i, k, lines[i][k]);
            }
        }
    }

    /// <summary>
    /// グリッドの行数と列数を指定して初期化する
    /// </summary>
    void InitRowAndColumn(int column, int row)
    {
        _gridLayoutGroup.constraintCount = column;
        _grid = new Cell[column, row];
        _tempNextGeneration = new byte[column, row];
    }

    /// <summary>
    /// グリッドの各セルを生成する
    /// </summary>
    void CreateCell(int c, int r, char letter)
    {
        GameObject cell = new ("Cell");
        cell.transform.SetParent(_parent);
        Image img = cell.AddComponent<Image>();
        // 0:死亡 1:生存
        img.color = letter == '0' ? _deadCellColor : _aliveCellColor;
        _grid[c, r] = new ()
        {
            Img = img,
            Value = byte.Parse(letter.ToString()),
        };
    }

    /// <summary>
    /// グリッドを更新する
    /// </summary>
    void UpdateGrid()
    {
        // 全てのセルで周囲八近傍を調べて次の世代での状態を一時保存
        for (int i = 0; i < _grid.GetLength(0); i++)
        {
            for(int k = 0; k < _grid.GetLength(1); k++)
            {
                int count = CountNeighbourAliveCells(i, k);
                if      (_grid[i, k].Value == 0 && count == 3) _tempNextGeneration[i, k] = 1;
                else if (_grid[i, k].Value == 1 && count == 2) _tempNextGeneration[i, k] = 1;
                else if (_grid[i, k].Value == 1 && count == 3) _tempNextGeneration[i, k] = 1;
                else _tempNextGeneration[i, k] = 0;
            }
        }
        // 次の世代の状態を反映
        for(int i = 0; i < _grid.GetLength(0); i++)
        {
            for(int k = 0; k < _grid.GetLength(1); k++)
            {
                _grid[i, k].Value = _tempNextGeneration[i, k];
                _grid[i, k].Img.color = _grid[i, k].Value == 0 ? _deadCellColor : _aliveCellColor;
            }
        }

        Generation++;
    }

    /// <summary>
    /// 指定したセルの周囲八近傍の生存しているセルを数える
    /// </summary>
    int CountNeighbourAliveCells(int c, int r)
    {
        int count = 0;
        foreach (Vector2Int dir in EightDirections)
        {
            Vector2Int index = new(r + dir.x, c + dir.y);
            if (!IsWithinRange(index.y, index.x)) continue;
            count += _grid[index.y, index.x].Value == 1 ? 1 : 0;
        }

        return count;
    }

    /// <summary>
    /// 指定した添え字がグリッドの範囲内に存在するかを返す
    /// </summary>
    bool IsWithinRange(int c, int r)
    {
        return 0 <= c && c < _grid.GetLength(0) && 0 <= r && r < _grid.GetLength(1);
    }
}
