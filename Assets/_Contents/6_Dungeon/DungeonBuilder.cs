using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����񂩂�_���W�����𐶐�����N���X
/// �e�L�X�g�t�@�C���������̓A���S���Y���Ő���������������g�p����
/// </summary>
[RequireComponent(typeof(TileGenerator))]
public class DungeonBuilder : MonoBehaviour
{
    enum BuildMode
    {
        TextAsset,
        Algorithm,
    }

    [Header("�_���W�����̐e�I�u�W�F�N�g")]
    [SerializeField] Transform _parent;
    [Header("�e�L�X�g�t�@�C��/�A���S���Y��")]
    [SerializeField] BuildMode _buildMode;
    [Header("������e�L�X�g")]
    [SerializeField] TextAsset _blueprint;

    void Start()
    {
        if (_buildMode == BuildMode.TextAsset) BuildFromTextAsset();
        if (_buildMode == BuildMode.Algorithm) BuildFromAlgorithm();
    }

    /// <summary>
    /// �C���X�y�N�^�[�Ɋ��蓖�Ă�ꂽ�e�L�X�g�t�@�C������_���W�����𐶐�����
    /// </summary>
    void BuildFromTextAsset()
    {
        string[] lines = _blueprint.text.Split("\n");
        // �e�L�X�g�̏ꍇ�A������̑O��̋󔒂��폜���Ȃ��ƃY����
        for(int i = 0; i < lines.Length; i++)
        {
            lines[i] = lines[i].Trim();
        }
        Generate(lines);
    }

    /// <summary>
    /// �A���S���Y����p���Đ������������񂩂�_���W�����𐶐�����
    /// </summary>
    void BuildFromAlgorithm()
    {
        if(TryGetComponent(out IDungeonBaseStringGenerator stringGenerator))
        {
            string[] lines = stringGenerator.Generate();
            Generate(lines);
        }
        else
        {
            throw new System.NullReferenceException("�_���W������������A���S���Y��������");
        }
    }

    /// <summary>
    /// ���������͂��Ċe�����ɑΉ������^�C���𐶐��A�K�؂Ȉʒu�ɔz�u����
    /// </summary>
    void Generate(string[] lines)
    {
        TileGenerator tileGenerator = GetComponent<TileGenerator>();

        for (int i = 0; i < lines.Length; i++)
        {
            // ���S���
            float basePosX = _parent.position.x - lines[i].Length / 2;
            float basePosZ = _parent.position.z - lines.Length / 2;

            for (int k = 0; k < lines[i].Length; k++)
            {
                if (!tileGenerator.IsSupported(lines[i][k])) continue;

                // �������Ĉʒu�Ɛe�̐ݒ�
                GameObject tile = tileGenerator.Generate(lines[i][k]);
                tile.transform.position = new Vector3(basePosX + k, 0, basePosZ + i);
                tile.transform.SetParent(_parent);
            }
        }
    }
}
