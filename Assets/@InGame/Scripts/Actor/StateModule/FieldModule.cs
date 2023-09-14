using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// �X�e�[�g��FieldManager�փZ���̗\��/�\��������s�������̃��b�p�[
    /// </summary>
    public class FieldModule
    {
        readonly DataContext _context;

        public FieldModule(DataContext context)
        {
            _context = context;
        }

        public void SetOnCell()
        {
            FieldManager.Instance.SetActorOnCell(_context.Transform.position, _context.Type);
        }

        public void SetOnCell(Vector3 pos)
        {
            FieldManager.Instance.SetActorOnCell(pos, _context.Type);
        }

        public void SetOnCell(Vector2Int index)
        {
            FieldManager.Instance.SetActorOnCell(index, _context.Type);
        }

        public void DeleteOnCell()
        {
            FieldManager.Instance.SetActorOnCell(_context.Transform.position, ActorType.None);
        }

        public void DeleteOnCell(Vector3 pos)
        {
            FieldManager.Instance.SetActorOnCell(pos, ActorType.None);
        }

        public void DeleteOnCell(Vector2Int index)
        {
            FieldManager.Instance.SetActorOnCell(index, ActorType.None);
        }

        /// <summary>
        /// �o�H������ꍇ�͌o�H�̖��[�̃Z���̗\����폜����
        /// </summary>
        public void DeletePathGoalOnCell()
        {
            if (_context.Path.Count > 0)
            {
                Vector3 pos = _context.Path[_context.Path.Count - 1];
                FieldManager.Instance.SetActorOnCell(pos, ActorType.None);
            }
        }
    }
}
