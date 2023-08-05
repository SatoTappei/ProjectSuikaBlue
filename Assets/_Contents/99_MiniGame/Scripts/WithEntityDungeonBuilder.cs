using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using MiniGameECS;

namespace MiniGame
{
    /// <summary>
    /// DungeonBuilder�̂قڃR�s�y
    /// ���������^�C���Ɖӏ���ECS���ɓ`���鏈�����ǉ����ꂽ����
    /// </summary>
    [RequireComponent(typeof(TileGenerator))]
    public class WithEntityDungeonBuilder : MonoBehaviour
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

        /// <summary>
        /// �����ɑΉ������^�C���̃��X�g���擾���邽�߂̎���
        /// </summary>
        Dictionary<char, List<GameObject>> _tileDataDict = new();
        /// <summary>
        /// ���Ƀ_���W�����������ς݂̏ꍇ�ɌĂяo���Ă��Ȃ����`�F�b�N���邽�߂̃t���O
        /// </summary>
        bool _created;

        public IReadOnlyDictionary<char, List<GameObject>> TileDataDict => _tileDataDict;

        /// <summary>
        /// �O������Ăяo�����ƂŃ_���W�����𐶐�����
        /// </summary>
        public void Build()
        {
            if (_created)
            {
                Debug.LogWarning("���ɐ����ς݂Ȃ̂Ƀ_���W�����������\�b�h���Ăяo���Ă���̂Ŗ���");
                return;
            }
            _created = true;

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
            for (int i = 0; i < lines.Length; i++)
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
            if (TryGetComponent(out IDungeonBaseStringGenerator stringGenerator))
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

                    // ��:��������DungeonBuilder�ƈႤ
                    EntityType type = CharToEntityType(lines[i][k]);
                    MonoToEcsTransfer.Instance.AddData(tile.transform.position, Vector3.zero, type);

                    AddTileDataToDict(lines[i][k], tile);
                }
            }
        }

        EntityType CharToEntityType(char letter)
        {
            if (letter == '_' || letter == '@') return EntityType.Grass;
            else if (letter == '#') return EntityType.Wall;
            else if (letter == '<') return EntityType.SpawnPoint;
            else
            {
                throw new System.ArgumentException("�^�C���̕�����Entity�ɑΉ����Ă��Ȃ�: " + letter);
            }
        }

        /// <summary>
        /// �O�����琶�������_���W�����̃^�C���̃f�[�^���Q�Ƃł���悤�Ɏ����ɒǉ�����
        /// </summary>
        void AddTileDataToDict(char key, GameObject value)
        {
            if (!_tileDataDict.ContainsKey(key))
            {
                _tileDataDict.Add(key, new List<GameObject>());
            }

            _tileDataDict[key].Add(value);
        }
    }
}
