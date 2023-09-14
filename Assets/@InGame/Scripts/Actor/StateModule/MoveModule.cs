using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// �X�e�[�g�̃Z���Ԃ��ړ����鏈���y�ѕK�v�ȃ��\�b�h�𔲂��o�����N���X
    /// </summary>
    public class MoveModule
    {
        readonly DataContext _context;

        public Vector3 CurrentCellPos;
        public Vector3 NextCellPos;

        float _lerpProgress;
        float _speedModify = 1;

        public MoveModule(DataContext context)
        {
            _context = context;
        }

        public bool OnNextCell => _context.Transform.position == NextCellPos;

        List<Vector3> Path => _context.Path;
        float MoveSpeed => _context.Base.MoveSpeed;
        Vector3 Position
        {
            get => _context.Transform.position;
            set => _context.Transform.position = value;
        }
        Quaternion Rotation
        {
            get => _context.Model.rotation;
            set => _context.Model.rotation = value;
        }

        /// <summary>
        /// �e�l������l�ɖ߂�
        /// </summary>
        public void Reset()
        {
            CurrentCellPos = Position;
            NextCellPos = default;
            _lerpProgress = 0;
            _speedModify = 1;
        }

        /// <summary>
        /// �e�l������l�ɖ߂����ƂŁA���݂̃Z���̈ʒu�����g�̈ʒu�ōX�V����B
        /// ���̃Z���̈ʒu������Ύ��̃Z���̈ʒu�A�Ȃ���Ύ��g�̈ʒu�ōX�V����B
        /// </summary>
        /// <returns>���̃Z��������:true ���̃Z��������(�ړI�n�ɓ���):false</returns>
        public bool TryStepNextCell()
        {
            Reset();

            if (Path.Count > 0)
            {
                // �o�H�̐擪(���̃Z��)����1���o��
                NextCellPos = Path[0];
                Path.RemoveAt(0);
                // �o�H�̃Z���ƃL�����N�^�[�̍������Ⴄ�̂Ő����Ɉړ������邽�߂ɍ��������킹��
                NextCellPos.y = Position.y;

                Modify();
                Look();
                return true;
            }
            else
            {
                NextCellPos = Position;
                return false;
            }
        }

        public void Move()
        {
            _lerpProgress += Time.deltaTime * MoveSpeed * _speedModify;
            Position = Vector3.Lerp(CurrentCellPos, NextCellPos, _lerpProgress);
        }

        public void Look()
        {
            Vector3 dir = NextCellPos - CurrentCellPos;

            if (dir != Vector3.zero)
            {
                Rotation = Quaternion.LookRotation(dir, Vector3.up);
            }
        }

        /// <summary>
        /// �΂߈ړ��̑��x��␳����
        /// </summary>
        public void Modify()
        {
            bool dx = Mathf.Approximately(CurrentCellPos.x, NextCellPos.x);
            bool dz = Mathf.Approximately(CurrentCellPos.z, NextCellPos.z);

            _speedModify = (dx || dz) ? 1 : 0.7f;
        }
    }
}