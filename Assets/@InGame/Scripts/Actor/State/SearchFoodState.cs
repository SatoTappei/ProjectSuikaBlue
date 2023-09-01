using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// ���̃Z���܂ňړ����A�ݒ肳�ꂽ���ʒl�����X�e�[�^�X�̐��̒l�����X�ɉ񕜂���B
    /// �X�e�[�^�X�̃p�����[�^�����ʒl�������Ă��Ă��A���ʒl���̉񕜏��������s�����
    /// </summary>
    public class SearchFoodState : BaseState
    {
        const int EffectValue = 100; // ���̃X�e�[�g�ŉ񕜂���H���̒l
        const int EffectDelta = 100; // ���ۂɂ�DeltaTime�Ƃ̏�Z�ŉ񕜂���

        enum Stage
        {
            Move,
            Eat,
        }

        MoveModule _move;
        FieldModule _field;

        Stage _stage;
        List<Vector3> _path = new();
        float _effectProgress;
        bool _hasPath;

        public SearchFoodState(DataContext context) : base(context, StateType.SearchFood)
        {
            _move = new(context);
            _field = new(context);
        }

        protected override void Enter()
        {
            _move.Reset();
            _stage = Stage.Move;
            _effectProgress = 0;

            _field.SetActorOnCell();

            _hasPath = TryPathfinding();
            TryStepNextCell();
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            if (!_hasPath) { ToEvaluateState(); return; }

            // �ړ�
            if (_stage == Stage.Move)
            {
                if (_move.OnNextCell)
                {
                    // ���̃Z���ɓ��������^�C�~���O�ňړ��O�̃Z���̏�������
                    _field.DeleteActorOnCell(_move.CurrentCellPos);

                    // �ʂ̃X�e�[�g���I������Ă����ꍇ�͑J�ڂ���
                    if (Context.ShouldChangeState(this)) { ToEvaluateState(); return; }

                    if (TryStepNextCell())
                    {
                        // �o�H�̓r���̃Z���̏ꍇ�̏���
                    }
                    else
                    {
                        _stage = Stage.Eat; // �H�ׂ��Ԃ�
                    }
                }
                else
                {
                    _move.Move();
                }
            }
            // �H�ׂ�
            else
            {
                Debug.Log("Ӹ�Ӹ�");
                // �H�ׂ鎞�̓G�t�F�N�g���~�����B
                //if (!StepEatProgress()) { ToEvaluateState(); return; }
                Context.Food.Value += 100;
                ToEvaluateState();
            }
        }

        bool TryPathfinding()
        {
            _path.Clear();

            // �H���̃Z�������邩���ׂ�
            if (FieldManager.Instance.TryGetResourceCells(ResourceType.Tree, out List<Cell> cellList))
            {
                // �H���̃Z�����߂����Ɍo�H�T��
                Vector3 pos = Context.Transform.position;
                foreach (Cell food in cellList.OrderBy(c => Vector3.SqrMagnitude(c.Pos - pos)))
                {
                    // TODO:�S�Ă̐H���ɑ΂��Čo�H�T��������Əd���̂ł�����x�̏��őł��؂鏈��

                    Vector2Int currentIndex = FieldManager.Instance.WorldPosToGridIndex(pos);
                    Vector2Int foodIndex = FieldManager.Instance.WorldPosToGridIndex(food.Pos);

                    int dx = Mathf.Abs(currentIndex.x - foodIndex.x);
                    int dy = Mathf.Abs(currentIndex.y - foodIndex.y);
                    if (dx <= 1 && dy <= 1)
                    {
                        // �ׂ̃Z���ɐH��������ꍇ�͈ړ����Ȃ��̂ŁA���ݒn���o�H�Ƃ��Ēǉ�����
                        _path.Add(Context.Transform.position);
                        _field.SetActorOnCell();
                        return true;
                    }
                    else
                    {
                        // �Ώۂ̃Z�� + ���͔��ߖT�ɑ΂��Čo�H�T��
                        foreach (Vector2Int dir in Utility.SelfAndEightDirections)
                        {
                            Vector2Int targetIndex = foodIndex + dir;
                            if (FieldManager.Instance.TryGetPath(currentIndex, targetIndex, out _path))
                            {
                                // �o�H�̖��[(�����̃Z���̗�)�ɃL�����N�^�[������ꍇ�͒e��
                                if (FieldManager.Instance.IsActorOnCell(_path[^1], out ActorType _)) continue;
                                
                                _field.SetActorOnCell(_path[^1]);
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// �e�l������l�ɖ߂����ƂŁA���݂̃Z���̈ʒu�����g�̈ʒu�ōX�V����B
        /// ���̃Z���̈ʒu������Ύ��̃Z���̈ʒu�A�Ȃ���Ύ��g�̈ʒu�ōX�V����B
        /// </summary>
        /// <returns>���̃Z��������:true ���̃Z��������(�ړI�n�ɓ���):false</returns>
        bool TryStepNextCell()
        {
            _move.Reset();

            if (_path.Count > 0)
            {
                // �o�H�̖�������1���o��
                _move.NextCellPos = _path[0];
                _path.RemoveAt(0);
                // �o�H�̃Z���ƃL�����N�^�[�̍������Ⴄ�̂Ő����Ɉړ������邽�߂ɍ��������킹��
                _move.NextCellPos.y = Context.Transform.position.y;
                
                _move.Modify();
                _move.Look();

                return true;
            }

            _move.NextCellPos = Context.Transform.position;

            return false;
        }

        /// <summary>
        /// �񕜂̐i���x��i�߂�
        /// </summary>
        /// <returns>�񕜂̐i����:true �񕜂̐i�����񕜒l�ɒB����:false</returns>
        bool StepEatProgress()
        {
            float value = Time.deltaTime * EffectDelta;
            _effectProgress += value;
            //Context.OnEatFoodInvoke(value); // �l�̍X�V

            return _effectProgress <= EffectValue;
        }
    }
}