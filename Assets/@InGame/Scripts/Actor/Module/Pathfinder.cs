using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public class Pathfinder
    {
        DataContext _context;
        Collider[] _detected = new Collider[8];

        public Pathfinder(DataContext context)
        {
            _context = context;
        }

        Vector3 GoalPos => _context.Path[_context.Path.Count - 1];
        Vector3 Position => _context.Transform.position;

        /// <summary>
        /// ���E���̓G��T��
        /// �������m�����ꍇ�͈�Ԏ�߂��G��ΏۂƂ���
        /// </summary>
        public bool SearchEnemy()
        {
            if (Detect.OverlapSphere(_context, _detected) == 0) return false;

            // �߂����ɔz��ɓ����Ă���̂ŁA��ԋ߂��G��Ώۂ̓G�Ƃ��ď������ށB
            foreach (Collider collider in _detected)
            {
                if (collider != null && collider.CompareTag(_context.EnemyTag))
                {
                    // �G���^�O�Ŕ���A�R���|�[�l���g�̎擾
                    return collider.TryGetComponent(out _context.Enemy);
                }
            }

            return false;
        }

        /// <summary>
        /// �G�܂ł̌o�H��T������
        /// </summary>
        /// <returns>�o�H����:true �Ȃ�:false</returns>
        public bool TryPathfindingToEnemy()
        {
            DeletePathGoalOnCell();

            // �O���b�h��ŋ�����r
            Vector3 enemyPos = _context.Enemy.transform.position;
            Vector2Int currentIndex = World2Grid(Position);
            Vector2Int enemyIndex = World2Grid(enemyPos);
            if (CreatePathIfNeighbourOnGrid(currentIndex, enemyIndex)) return true;

            // �Ώۂ̃Z�� + ���͔��ߖT�ɑ΂��Čo�H�T��
            foreach (Vector2Int dir in Utility.SelfAndEightDirections)
            {
                Vector2Int targetIndex = enemyIndex + dir;
                // �o�H�̖��[(�G�̃Z���̗�)�ɃL�����N�^�[������ꍇ�͒e��
                if (IsOnCell(targetIndex)) continue;
                // �o�H��������Ȃ������ꍇ�͒e��
                if (!TryGetPath(currentIndex, targetIndex)) continue;

                SetOnCell(targetIndex);
                return true;
            }

            return false;
        }

        /// <summary>
        /// ������o�H��T������
        /// �o�H�̖��[�����͔��ߖT�Ɉ��̋������ꂽ�ʒu���珙�X�ɋ߂Â��Ă���
        /// </summary>
        /// <returns>�o�H����:true �Ȃ�:false</returns>
        public bool TryPathfindingToEscapePoint()
        {
            DeletePathGoalOnCell();

            // �O���b�h��ŋ�����r
            Vector3 enemyPos = _context.Enemy.transform.position;
            Vector3 dir = (Position - enemyPos).normalized;
            Vector2Int currentIndex = World2Grid(Position);
            for (int i = 10; i >= 1; i--) // TODO:�K���Ȓl
            {
                Vector3 escapePos = dir * i;
                Vector2Int escapeIndex = World2Grid(escapePos);
                if (CreatePathIfNeighbourOnGrid(currentIndex, escapeIndex)) return true;
                // �o�H�̖��[(�G�̃Z���̗�)�ɃL�����N�^�[������ꍇ�͒e��
                if (IsOnCell(escapeIndex)) continue;
                // �o�H��������Ȃ������ꍇ�͒e��
                if (!TryGetPath(currentIndex, escapeIndex)) continue;

                SetOnCell(escapeIndex);
                return true;
            }

            return false;
        }

        /// <summary>
        /// �������m���A�o�H�T�����s���B�o�H�����������ꍇ�̓S�[���̃Z����\�񂷂�B
        /// ���̃Z�� + ���͔��ߖT �̃Z���ւ̌o�H�����݂��邩���ׂ�
        /// </summary>
        /// <returns>���ւ̌o�H������:true �������Ȃ�of���ւ̌o�H������:false</returns>
        public bool TryDetectPartner()
        {
            DeletePathGoalOnCell();

            if (Detect.OverlapSphere(_context, _detected) == 0) return false;
            
            // null�Ǝ��g��e��
            foreach (Collider collider in _detected.Where(c => c != null && c != _context.Transform))
            {
                if (collider.TryGetComponent(out DataContext target) && target.Sex == Sex.Female)
                {
                    // �O���b�h��ŋ�����r
                    Vector2Int currentIndex = World2Grid(Position);
                    Vector2Int targetIndex = World2Grid(target.Transform.position);
                    if (CreatePathIfNeighbourOnGrid(currentIndex, targetIndex)) return true;

                    // �Ώۂ̃Z�� + ���͔��ߖT�ɑ΂��Čo�H�T��
                    foreach (Vector2Int dir in Utility.SelfAndEightDirections)
                    {
                        Vector2Int dirIndex = targetIndex + dir;
                        // �o�H��������Ȃ������ꍇ�͒e��
                        if (!TryGetPath(currentIndex, dirIndex)) continue;
                        // �o�H�̖��[(�����̃Z���̗�)�ɃL�����N�^�[������ꍇ�͒e��
                        if (IsOnCell(GoalPos)) continue;

                        SetOnCell(GoalPos);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// �����܂ł̌o�H�T��
        /// �o�H�����������ꍇ�̓S�[���̃Z����\�񂷂�
        /// </summary>
        /// <returns>�o�H����:true �o�H����:false</returns>
        public bool TryPathfindingToResource(ResourceType type)
        {
            DeletePathGoalOnCell();

            // �����̃Z�������邩���ׂ�
            if (!TryGetResourceCells(type, out List<Cell> cellList)) return false;

            // �����̃Z�����߂����Ɍo�H�T��
            foreach (Cell resource in cellList.OrderBy(c => Vector3.SqrMagnitude(c.Pos - Position)))
            {
                Vector2Int currentIndex = World2Grid(Position);
                Vector2Int resourceIndex = World2Grid(resource.Pos);

                if (CreatePathIfNeighbourOnGrid(currentIndex, resourceIndex)) return true;

                // �Ώۂ̃Z�� + ���͔��ߖT�ɑ΂��Čo�H�T��
                foreach (Vector2Int dir in Utility.SelfAndEightDirections)
                {
                    Vector2Int targetIndex = resourceIndex + dir;
                    // ���ɃL�����N�^�[������A�o�H��������Ȃ������ꍇ�͒e��
                    if (IsOnCell(targetIndex)) continue;
                    if (!TryGetPath(currentIndex, targetIndex)) continue;
                    // �o�H�̖��[(�����̃Z���̗�)�Ɏ����L�����N�^�[������ꍇ�͒e��
                    SetOnCell(targetIndex);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// �W���n�_�ւ̌o�H��T������
        /// �W���n�_����X�p�C�����ɒT�����Ă����B
        /// </summary>
        /// <returns>�W���n�_�ւ̌o�H������:true �W���n�_�ւ̌o�H������:false</returns>
        public bool TryPathfindingToGatherPoint()
        {
            //DeletePath();

            Vector2Int currentIndex = World2Grid(Position);
            Vector2Int gatherIndex = World2Grid(PublicBlackBoard.GatherPos);
            if (CreatePathIfNeighbourOnGrid(currentIndex, gatherIndex)) return true;

            int count = 0;
            int sideLength = 1;
            int sideCount = 0;
            // �L�����N�^�[�̍ő吔���J��Ԃ�
            while (count++ < InvalidActorHolder.PoolSize)
            {
                int index = sideCount % 4;
                for (int k = 0; k < sideLength; k++)
                {
                    gatherIndex.y += Utility.Counterclockwise[index].y;
                    gatherIndex.x += Utility.Counterclockwise[index].x;

                    // �o�H�����݂��邩�H
                    if (!TryGetPath(currentIndex, gatherIndex)) continue;
                    // TODO:�o�H�̒�����0�̏ꍇ������
                    if (_context.Path.Count == 0) continue;
                    // �o�H�̖��[(�����̃Z���̗�)�Ɏ����L�����N�^�[������ꍇ�͒e��
                    if (IsOnCell(GoalPos)) continue;

                    SetOnCell(GoalPos);
                    return true;
                }

                // 2�ӈړ��������ӓ�����̒�����1������
                if (index == 1 || index == 3) sideLength++;

                sideCount++;
            }

            return false;
        }

        /// <summary>
        /// ���݂̌o�H�̖��[�̗\����폜����B
        /// �o�H��T������ۂɌĂ΂Ȃ��ƈȑO�̌o�H�̖��[�̗\�񂪎c�����܂܂ɂȂ��Ă��܂��B
        /// </summary>
        public void DeletePathGoalOnCell()
        {
            if (_context.Path.Count > 0)
            {
                DeleteOnCell(GoalPos);
            }
        }

        // �ȉ����b�p�[
        Vector2Int World2Grid(in Vector3 pos) => FieldManager.Instance.WorldPosToGridIndex(pos);
        bool TryGetPath(Vector2Int from, Vector2Int to) => FieldManager.Instance.TryGetPath(from, to, _context.Path);
        void SetOnCell(in Vector3 pos) => FieldManager.Instance.SetActorOnCell(pos, _context.Type);
        void SetOnCell(in Vector2Int index) => FieldManager.Instance.SetActorOnCell(index, _context.Type);
        bool IsOnCell(in Vector3 pos) => FieldManager.Instance.IsActorOnCell(pos);
        bool IsOnCell(in Vector2Int index) => FieldManager.Instance.IsActorOnCell(index);
        void DeleteOnCell(in Vector3 pos) => FieldManager.Instance.SetActorOnCell(pos, ActorType.None);
        bool TryGetResourceCells(ResourceType type, out List<Cell> list) => FieldManager.Instance.TryGetResourceCells(type, out list);
        bool CreatePathIfNeighbourOnGrid(Vector2Int from, Vector2Int to) => ActorHelper.CreatePathIfNeighbourOnGrid(from, to, _context);
    }
}
