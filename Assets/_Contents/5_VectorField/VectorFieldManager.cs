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

/// <summary>
/// �x�N�g���t�B�[���h�̐������Ǘ�����N���X
/// �O������͂��̃N���X�̃��\�b�h��p���ăx�N�g���t�B�[���h�𑀍삷��
/// </summary>
public class VectorFieldManager : MonoBehaviour
{
    [SerializeField] GridData _gridData;

    Cell[,] _grid;
    VectorCalculator _vectorCalculator;
    FlowCalculator _flowCalculator;
    DebugVectorVisualizer _vectorVisualizer;
    DebugGridVisualizer _gridVisualizer;

    /// <summary>
    /// ��xCreateVectorField���\�b�h���ĂсA�x�N�g���t�B�[���h�𐶐������t���O
    /// �x�N�g���t�B�[���h���쐬���Ȃ��ƃx�N�g���̗�����擾�ł��Ȃ�
    /// </summary>
    bool _vectorFieldCreated;

    /// <summary>
    /// �O������Ăяo�����ƂŃx�N�g���t�B�[���h�̍쐬�ɕK�v�ȃO���b�h���쐬����
    /// �O���b�h���̂̓X�e�[�W�ɍ��킹�č쐬����K�v������̂ŁA�����̐����������I�������ɌĂ�
    /// </summary>
    public void CreateGrid()
    {
        // �O���b�h����
        GridBuilder gridBuilder = new();
        _grid = gridBuilder.CreateGrid(_gridData);

        _vectorCalculator = new(_grid, _gridData);
        _flowCalculator = new(_grid, _gridData);
        TryGetComponent(out _vectorVisualizer);
        TryGetComponent(out _gridVisualizer);
    }

    /// <summary>
    /// �O������Ăяo�����ƂŁA�w�肵���ʒu����Ƀx�N�g���t�B�[���h���쐬����
    /// Y���W�̓O���b�h�̍�������ɂ���̂Ŗ��������
    /// </summary>
    public void CreateVectorField(Vector3 pos, FlowMode mode)
    {
        if (_grid == null)
        {
            throw new System.NullReferenceException("�O���b�h���쐬");
        }

        _vectorCalculator.CreateVectorField(pos);
        _vectorFieldCreated = true;

#if UNITY_EDITOR
        // �x�N�g���̗���̕`��
        if (_vectorVisualizer != null)
        {
            _vectorVisualizer.VisualizeVectorFlow(_grid);
        }
#endif
    }

    /// <summary>
    /// �ʒu�ɑΉ������Z���̃x�N�g�����擾����
    /// </summary>
    public Vector3 GetCellVector(in Vector3 pos)
    {
        Vector2Int index = GridUtility.WorldPosToGridIndex(in pos, _grid, _gridData);
        return _grid[index.y, index.x].Vector;
    }

    /// <summary>
    /// �O������Ăяo�����ƂŁA�w�肵���ʒu����̐��K�����ꂽ�x�N�g���̗�����擾����
    /// Y���W�̓O���b�h�̍�������ɂ���̂Ŗ��������
    /// </summary>
    public void InsertVectorFlowToQueue(Vector3 pos, Queue<Vector3> queue)
    {
        if (!_vectorFieldCreated)
        {
            throw new System.InvalidOperationException("�x�N�g���t�B�[���h���쐬");
        }

        _flowCalculator.InsertVectorFlowToQueue(pos, queue);
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
