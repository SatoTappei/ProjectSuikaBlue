using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// �X�e�[�g��FieldManager�֊e��o�^���s���N���X
    /// </summary>
    public class FieldModule
    {
        DataContext _context;

        public FieldModule(DataContext context)
        {
            _context = context;
        }

        public void SetActorOnCell()
        {
            FieldManager.Instance.SetActorOnCell(_context.Transform.position, _context.Type);
        }

        public void SetActorOnCell(Vector3 pos)
        {
            FieldManager.Instance.SetActorOnCell(pos, _context.Type);
        }

        public void DeleteActorOnCell()
        {
            FieldManager.Instance.SetActorOnCell(_context.Transform.position, ActorType.None);
        }

        public void DeleteActorOnCell(Vector3 pos)
        {
            FieldManager.Instance.SetActorOnCell(pos, ActorType.None);
        }
    }
}
