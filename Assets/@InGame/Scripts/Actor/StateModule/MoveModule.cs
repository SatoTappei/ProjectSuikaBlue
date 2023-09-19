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
        int cellIndex = 0;

        public MoveModule(DataContext context)
        {
            _context = context;
        }

        public bool OnNextCell => Position == NextCellPos;

        List<Vector3> Path => _context.Path;
        float MoveSpeed => _context.Base.MoveSpeed;
        Vector3 Position
        {
            get => _context.Transform.position;
            set => _context.Transform.position = value;
        }
        Quaternion Rotation
        {
            set => _context.Model.rotation = value;
        }

        public void Reset()
        {
            cellIndex = 0;
            OnCell();
        }

        void OnCell()
        {
            CurrentCellPos = Position;
            NextCellPos = Position;
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
            OnCell();

            if (cellIndex < Path.Count)
            {
                // �o�H�̐擪(���̃Z��)����1���o��
                NextCellPos = Path[cellIndex++];
                // �o�H�̃Z���ƃL�����N�^�[�̍������Ⴄ�̂Ő����Ɉړ������邽�߂ɍ��������킹��
                NextCellPos.y = Position.y;

                // TODO:���̃Z���Ƃ̋�����1�Z���ȏ゠�邩�̔�����s���Ă���
                Vector2Int currentIndex = FieldManager.Instance.WorldPosToGridIndex(CurrentCellPos);
                Vector2Int nextIndex = FieldManager.Instance.WorldPosToGridIndex(NextCellPos);
                if (!ActorHelper.IsNeighbourOnGrid(currentIndex, nextIndex))
                {
                    NextCellPos = Position;
                    return false;
                }
                #region �f�o�b�O�p: ���̃Z���̋�����2�ȏ�ɂȂ��Ă�H
                //var i1 = FieldManager.Instance.WorldPosToGridIndex(CurrentCellPos);
                //var i2 = FieldManager.Instance.WorldPosToGridIndex(NextCellPos);
                //if (!ActorHelper.IsNeighbourOnGrid(i1, i2))
                //{
                //    int dx = Mathf.Abs(i1.x - i2.x);
                //    int dy = Mathf.Abs(i1.y - i2.y);
                //    FieldManager.Instance.TryGetCell(i1, out Cell c1);
                //    FieldManager.Instance.TryGetCell(i2, out Cell c2);
                //    Debug.Log("�o�H���� " + _context.Type + " " + _context.name + ": " + _context.GetComponent<Actor>().State + " ����x:" + dx.ToString() + "����y:" + dy.ToString());
                //    var v1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //    v1.transform.position = c1.Pos + Vector3.up * 0.5f;
                //    var v2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //    v2.transform.position = c2.Pos + Vector3.up * 0.5f;

                //}
                #endregion

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