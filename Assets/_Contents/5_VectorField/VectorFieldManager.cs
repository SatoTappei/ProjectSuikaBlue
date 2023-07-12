using System.Collections.Generic;
using UnityEngine;
using VectorField;

/// <summary>
/// �x�N�g���̗���̗񋓌^
/// �w�肵���n�_�Ɍ�����/�w�肵���n�_���痣�������肷��
/// </summary>
public enum FlowMode
{
    Toward, // ������
    Away,   // �����
}

/// <summary>
/// �x�N�^�[�t�B�[���h�p�̃O���b�h�Ɋւ���ǂݎ���p�̃f�[�^
/// �O���b�h�̐���/�x�N�g���̌v�Z���ɁA�����̃N���X�ŃO���b�h�̏������L�o����
/// </summary>
[System.Serializable]
public class GridData
{
    [Header("�O���b�h�̐ݒ�")]
    [SerializeField] int _width = 20;
    [SerializeField] int _height = 20;
    [SerializeField] float _cellSize = 5.0f;
    [Header("��Q���̃��C���[")]
    [SerializeField] LayerMask _obstacleLayer;
    [Header("��Q���̍���")]
    [Tooltip("���̍�������Ray���΂��ď�Q���Ƀq�b�g�������ǂ����Ō��o����")]
    [SerializeField] float _obstacleHeight = 10.0f;

    public int Width => _width;
    public int Height => _height;
    public float CellSize => _cellSize;
    public LayerMask ObstacleLayer => _obstacleLayer;
    public float ObstacleHeight => _obstacleHeight;

    public Vector3 GridOrigin { get; set; }
}

public class VectorFieldManager : MonoBehaviour
{
    [SerializeField] GridData _gridData;

    Cell[,] _grid;
    VectorCalculator _calculator;
    DebugVectorVisualizer _vectorVisualizer;
    DebugGridVisualizer _gridVisualizer;

    void Awake()
    {
        // �O���b�h����
        GridBuilder gridBuilder = new();
        _grid = gridBuilder.CreateGrid(_gridData);

        _calculator = new(_grid, _gridData);
        TryGetComponent(out _vectorVisualizer);
        TryGetComponent(out _gridVisualizer);
    }

    /// <summary>
    /// �O������Ăяo�����ƂŁA�w�肵���ʒu����Ƀx�N�g���t�B�[���h���쐬����
    /// Y���W�̓O���b�h�̍�������ɂ���̂Ŗ��������
    /// </summary>
    public void CreateVectorField(Vector3 pos, FlowMode mode)
    {
        _calculator.CreateVectorField(pos);

#if UNITY_EDITOR
        // �x�N�g���̗���̕`��
        if (_vectorVisualizer != null)
        {
            foreach (Cell cell in _grid)
            {
                _vectorVisualizer.VisualizeCellVector(cell);
            }
        }
#endif
    }

    /// <summary>
    /// �O������Ăяo�����ƂŁA�w�肵���ʒu����̐��K�����ꂽ�x�N�g���̗�����擾����
    /// Y���W�̓O���b�h�̍�������ɂ���̂Ŗ��������
    /// </summary>
    public List<Vector3> GetFlow(Vector3 pos)
    {
        throw new System.Exception("������");
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying && _gridVisualizer != null)
        {
            // �O���b�h�Ɗe�Z���̃R�X�g�̕`��
            _gridVisualizer.DrawGridOnGizmos(_grid, _gridData);
        }
    }
}
