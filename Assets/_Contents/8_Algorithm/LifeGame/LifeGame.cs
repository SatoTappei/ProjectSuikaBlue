using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���C�t�Q�[�������s����N���X�B�ȉ���2�̃��[�h������B<br></br>
/// 1.�C���X�y�N�^�[�Őݒ肵���s�����񐔂Ő������A�Z���̐����������_���Őݒ肳���<br></br>
/// 2.�e�L�X�g�t�@�C������ǂݍ��񂾕���������ɍs�����񐔁��Z���̐������ݒ肳���
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
    /// 8�����̔z��A12�����玞�v���
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
    [Header("�������@")]
    [SerializeField] CreateType _createType;
    [Header("�C���X�y�N�^�[�̒l���琶������ꍇ")]
    [Range(1, 30)]
    [SerializeField] int _row;
    [Range(1, 30)]
    [SerializeField] int _column;
    [Range(1, 900)]
    [SerializeField] int _initAliveCell;
    [Header("�e�L�X�g�t�@�C�����琶������ꍇ")]
    [SerializeField] TextAsset _blueprint;
    [Header("�Z���̐F")]
    [SerializeField] Color _aliveCellColor;
    [SerializeField] Color _deadCellColor;
    [Range(0.05f, 3.0f)]
    [Header("����X�V�Ԋu")]
    [SerializeField] float _updateInterval;

    Cell[,] _grid;
    /// <summary>
    /// �O���b�h�̍X�V�̃^�C�~���O�ňꎞ�I�Ɏ��̐���̐�����ێ����Ă����z��
    /// �S�ẴZ���̐��������܂�����ɂăO���b�h�ɔ��f����
    /// </summary>
    byte[,] _tempNextGeneration;
    /// <summary>
    /// ���݂̐��㐔
    /// </summary>
    int _generation; 

    int Generation
    {
        get => _generation;
        set
        {
            _generation = value;
            _generationView.text = "����: " + _generation.ToString();
        }
    }
    /// <summary>
    /// �s���Ɨ񐔂��͂ݏo�Ȃ��悤�ɏC������������������鐶���̃Z���̐�
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
    /// �{�^�����N���b�N�����ۂ̏�����o�^����
    /// </summary>
    void AddOnButtonClick()
    {
        _nextGenerationButton.onClick.AddListener(UpdateGrid);
        _regenerateButton.onClick.AddListener(ResetGrid);
    }

    /// <summary>
    /// InputField�ɓ��͂������ۂ̏�����o�^����
    /// </summary>
    void AddOnInputFieldInput()
    {
        // �l�����͂��ꂽ�玩���I�ɐ��オ�i�ނ̂��~�߂�
        _generationInputField.onValueChanged.AddListener(value => 
        {
            if (value == string.Empty) return;
            CancelInvoke(nameof(UpdateGrid));
        });
        // ���͂�������ɔ��
        _generationInputField.onSubmit.AddListener(value =>
        {
            if (!int.TryParse(value, out int generation)) return;
            if (generation - Generation <= 0) return;

            while (generation != Generation)
            {
                UpdateGrid();
            }
        });
        // ���͂���߂��ۂ͍Ăю����I�ɐ����i�߂�
        _generationInputField.onEndEdit.AddListener(value =>
        {
            if (_generationInputField.text == string.Empty) return;
            _generationInputField.text = string.Empty;
            InvokeRepeating(nameof(UpdateGrid), _updateInterval, _updateInterval);
        });
    }

    /// <summary>
    /// �Đ���
    /// </summary>
    void ResetGrid()
    {
        if (_grid != null && _tempNextGeneration != null)
        {
            // �O���b�h�Ɛ���̈ꎞ�ۑ��p�̔z����N���A
            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int k = 0; k < _grid.GetLength(1); k++)
                {
                    _grid[i, k] = null;
                    _tempNextGeneration[i, k] = default;
                }
            }
            // �Z����S�폜
            foreach (Transform child in _parent)
            {
                Destroy(child.gameObject);
            }
        }
        // �O���b�h�𐶐�
        if (_createType == CreateType.Inspector) CreateGridFromInspector();
        else if (_createType == CreateType.TextFile) CreateGridFromTextFile();

        Generation = 1;
    }

    /// <summary>
    /// �C���X�y�N�^�[�Őݒ肵���l�����ɃO���b�h�𐶐�����
    /// </summary>
    void CreateGridFromInspector()
    {
        InitRowAndColumn(_column, _row);
        // ���������Z���̐������t���O�𗧂Ă�
        bool[] flags = new bool[_row * _column];
        for (int i = 0; i < FixedInitAliveCell; i++)
        {
            flags[i] = true;
        }
        // �V���b�t��
        int count = flags.Length;
        for(int i = 0; i < flags.Length; i++)
        {
            int r = UnityEngine.Random.Range(0, count);
            bool temp = flags[count - 1];
            flags[count - 1] = flags[r];
            flags[r] = temp;
            count--;
        }
        // �t���O�������Ă���ΐ����A�����łȂ���Ύ��S�����Z���Ƃ��Đ���
        for (int i = 0; i < _column; i++)
        {
            for (int k = 0; k < _row; k++)
            {
                CreateCell(i, k, flags[i * _row + k] ? '1' : '0');
            }
        }
    }

    /// <summary>
    /// �e�L�X�g�t�@�C������O���b�h�𐶐�����
    /// </summary>
    void CreateGridFromTextFile()
    {
        // �O���b�h��1�s�𒲐�
        string[] lines = _blueprint.text.Split("\n");
        InitRowAndColumn(lines.Length, lines[0].Trim().Length);
        // �Z���̐���
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
    /// �O���b�h�̍s���Ɨ񐔂��w�肵�ď���������
    /// </summary>
    void InitRowAndColumn(int column, int row)
    {
        _gridLayoutGroup.constraintCount = column;
        _grid = new Cell[column, row];
        _tempNextGeneration = new byte[column, row];
    }

    /// <summary>
    /// �O���b�h�̊e�Z���𐶐�����
    /// </summary>
    void CreateCell(int c, int r, char letter)
    {
        GameObject cell = new ("Cell");
        cell.transform.SetParent(_parent);
        Image img = cell.AddComponent<Image>();
        // 0:���S 1:����
        img.color = letter == '0' ? _deadCellColor : _aliveCellColor;
        _grid[c, r] = new ()
        {
            Img = img,
            Value = byte.Parse(letter.ToString()),
        };
    }

    /// <summary>
    /// �O���b�h���X�V����
    /// </summary>
    void UpdateGrid()
    {
        // �S�ẴZ���Ŏ��͔��ߖT�𒲂ׂĎ��̐���ł̏�Ԃ��ꎞ�ۑ�
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
        // ���̐���̏�Ԃ𔽉f
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
    /// �w�肵���Z���̎��͔��ߖT�̐������Ă���Z���𐔂���
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
    /// �w�肵���Y�������O���b�h�͈͓̔��ɑ��݂��邩��Ԃ�
    /// </summary>
    bool IsWithinRange(int c, int r)
    {
        return 0 <= c && c < _grid.GetLength(0) && 0 <= r && r < _grid.GetLength(1);
    }
}
