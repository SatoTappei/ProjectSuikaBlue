//using System.Linq;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Events;

//namespace PSB.InGame
//{
//    // TODO:�H��/���̒T���X�e�[�g�����g�p
//    // �H���Ɛ��Ō���قړ����Ȃ̂ŋ��ʉ��������N���X
//    // �A�j���[�V�����⎑���^�C���̕ω��ȂǂňႤ�Ƃ��낪���邩������Ȃ��̂ŁA��肫������Ɍp�����邩���߂�

//    // ���p�������ꍇ�̃R���X�g���N�^��
//    // public SearchWarterState(IBlackBoardForState blackBoard, StateType type) : base(
//    // blackBoard: blackBoard,
//    // stateType: type,
//    // effectValue: 100, // ��Ԃ̌��ʗ�
//    // effectDelta: 100, // ��x�̏����Ō��ʂ����
//    // resourceType: ResourceType.Stone, // �Ώۂ̎���
//    // onEatResource: blackBoard.OnDrinkWaterInvoke // ���̃X�e�[�^�X�ɔ��f���郁�\�b�h
//    // )
//    // { }

//    /// <summary>
//    /// �����̃Z���܂ňړ����A�ݒ肳�ꂽ���ʒl�����X�e�[�^�X�̐H���̒l�����X�ɉ񕜂���
//    /// �X�e�[�^�X�̃p�����[�^�����ʒl�������Ă��Ă��A���ʒl���̉񕜏��������s�����
//    /// </summary>
//    public class SearchResourceState : BaseState
//    {
//        enum Stage
//        {
//            Move,
//            Eat,
//        }

//        // ������
//        readonly IBlackBoardForState BlackBoard;
//        readonly Transform Actor;
//        readonly int EffectValue;
//        readonly int EffectDelta;
//        // ����
//        readonly ResourceType ResourceType;
//        readonly UnityAction<float> OnEatResource;
        
//        Stage _stage;
//        Stack<Vector3> _path = new();
//        Vector3 _currentCellPos;
//        Vector3 _nextCellPos;
//        float _lerpProgress;
//        float _effectProgress;
//        // �H���̃Z��������A�H���܂ł̌o�H�����݂��邩�ǂ����̃t���O
//        bool _hasPath;

//        bool OnNextCell => Actor.position == _nextCellPos;

//        public SearchResourceState(IBlackBoardForState blackBoard, StateType stateType, int effectValue, int effectDelta,
//            ResourceType resourceType, UnityAction<float> onEatResource) : base(stateType)
//        {
//            BlackBoard = blackBoard;
//            Actor = blackBoard.Transform;
//            EffectValue = effectValue;
//            EffectDelta = effectDelta;
//            ResourceType = resourceType;
//            OnEatResource = onEatResource;
//        }

//        protected override void Enter()
//        {
//            _stage = Stage.Move;

//            _effectProgress = 0;

//            _hasPath = TryPathfinding();
//            TryStepNextCell();
//        }

//        protected override void Exit()
//        {
//        }

//        protected override void Stay()
//        {
//            // �o�H�������̂ŕ]���X�e�[�g�ɑJ��
//            if (!_hasPath) { ToEvaluateState(); return; }

//            switch (_stage)
//            {
//                case Stage.Move: MoveStage(); break;
//                case Stage.Eat: EatStage(); break;
//            }
//        }

//        bool TryPathfinding()
//        {
//            _path.Clear();

//            // �H���̃Z�������邩���ׂ�
//            if (FieldManager.Instance.TryGetResourceCells(ResourceType, out List<Cell> cellList))
//            {
//                // �H���̃Z�����߂����Ɍo�H�T��
//                Vector3 pos = Actor.position;
//                foreach (Cell food in cellList.OrderBy(c => Vector3.SqrMagnitude(c.Pos - pos)))
//                {
//                    if (FieldManager.Instance.TryGetPath(pos, food.Pos, out _path)) // <- �Լ�
//                    {
//                        return true;
//                    }
//                }

//                return false;
//            }

//            return false;
//        }

//        void ToEvaluateState() => TryChangeState(BlackBoard.EvaluateState);

//        /// <summary>
//        /// �H���̃Z���Ɉړ�
//        /// </summary>
//        void MoveStage()
//        {
//            // ���̃Z���̏�ɗ����ꍇ�̓`�F�b�N����
//            if (OnNextCell)
//            {
//                // �Ⴄ�X�e�[�g�ɑJ�ڂ���ꍇ�͈�x�]���X�e�[�g���o�R����
//                if (BlackBoard.NextState != this) { ToEvaluateState(); return; }

//                if (TryStepNextCell())
//                {
//                    // �o�H�̓r���̃Z���̏ꍇ�̏���
//                }
//                else
//                {
//                    _stage = Stage.Eat; // �H�ׂ��Ԃ�
//                }
//            }
//            else
//            {
//                Move();
//            }
//        }

//        /// <summary>
//        /// �H����H�ׂ�
//        /// </summary>
//        void EatStage()
//        {
//            if (!StepEatProgress()) { ToEvaluateState(); return; }
//        }

//        /// <summary>
//        /// ���݂̃Z���̈ʒu�����g�̈ʒu�ōX�V����B
//        /// ���̃Z���̈ʒu������Ύ��̃Z���̈ʒu�A�Ȃ���Ύ��g�̈ʒu�ōX�V����B
//        /// </summary>
//        /// <returns>���̃Z��������:true ���̃Z��������(�ړI�n�ɓ���):false</returns>
//        bool TryStepNextCell()
//        {
//            _currentCellPos = Actor.position;

//            if (_path.TryPop(out _nextCellPos))
//            {
//                // �o�H�̃Z���ƃL�����N�^�[�̍������Ⴄ�̂Ő����Ɉړ������邽�߂ɍ��������킹��
//                _nextCellPos.y = Actor.position.y;
//                _lerpProgress = 0;
//                return true;
//            }

//            _nextCellPos = Actor.position;

//            return false;
//        }

//        void Move()
//        {
//            _lerpProgress += Time.deltaTime * BlackBoard.Speed;
//            Actor.position = Vector3.Lerp(_currentCellPos, _nextCellPos, _lerpProgress);
//        }

//        /// <summary>
//        /// �񕜂̐i���x��i�߂�
//        /// </summary>
//        /// <returns>�񕜂̐i����:true �񕜂̐i�����񕜒l�ɒB����:false</returns>
//        bool StepEatProgress()
//        {
//            float value = Time.deltaTime * EffectDelta;
//            _effectProgress += value;
//            OnEatResource(value); // �l�̍X�V

//            return _effectProgress <= EffectValue;
//        }
//    }
//}