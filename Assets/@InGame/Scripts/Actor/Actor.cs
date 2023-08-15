using UnityEngine;
using UnityEngine.Events;

namespace PSB.InGame
{
    public enum ActorType
    {
        None,
        Kinpatsu,
        KinpatsuLeader,
        Kurokami,
    }

    [RequireComponent(typeof(ChildSpawner))]
    public class Actor : MonoBehaviour, IReadOnlyParams
    {
        public static event UnityAction<Actor> OnSpawned;

        // �C���X�y�N�^�Ŋ��蓖�ĂȂ�
        // ��ƂȂ�l + �e�����`�����l
        // �������̂̓X�|�i�[���s���B���̍ۂɈ�`�����l��n�������B
        // ��`�����l�̓����_���Ȃ̂�SO��C���X�y�N�^����n���Ȃ��B

        [SerializeField] ChildSpawner _spawner;
        // StatusBase�̎擾��Controller���ł̐���ɕK�v�Ȃ̂Ō̖��Ƀf�[�^������
        [SerializeField] ActorType _type;
        [Header("3D���f���̃��b�V���̃I�u�W�F�N�g")]
        [SerializeField] Transform _model;

        Status _status;
        bool _initialized;
        
        public ActorType Type => _type;
        // UI�����ǂݎ��p
        float IReadOnlyParams.Food => /*_food.Percentage*/1;
        float IReadOnlyParams.Water => /*_water.Percentage*/1;
        float IReadOnlyParams.HP => /*_hp.Percentage*/1;

        /// <summary>
        /// �X�e�[�^�X�̐ݒ�AController���Ő��䂷�邽�߂̃R�[���o�b�N�̌Ăяo��
        /// ��`�q��null�̏ꍇ�͐e�����Ȃ̂Ńf�t�H���g�̈�`�q���g�p����
        /// </summary>
        public void Init(uint? gene = null) 
        {
            // ��ނƈ�`�q����X�e�[�^�X��������
            StatusBase statusBase = StatusBaseHolder.Get(_type);
            gene ??= statusBase.DefaultGene;
            _status = new(statusBase, (uint)gene);
            
            // ��`�q���̂ɔ��f
            ApplyInheritedSize(_status.Size);
            ApplyInheritedColor(_status.Color);

            OnSpawned?.Invoke(this);

            // ������x���̃��\�b�h���Ă�ŏ��������Ȃ��悤�Ƀt���O�𗧂Ă�
            _initialized = true;
        }

        void Start()
        {
            // ���ڃV�[���ɔz�u�����ꍇ�ȂǃX�|�i�[���o�R���Ȃ��ꍇ�p
            // �O������f�[�^���擾����̂�Start�̃^�C�~���O�ŌĂԕK�v������
            if (!_initialized) Init();
        }

        void ApplyInheritedSize(float size)
        {
            Debug.Log(size);
            _model.transform.localScale *= size;
        }

        void ApplyInheritedColor(Color32 color)
        {
            Debug.Log(color);
        }

        public void Move()
        {
            transform.Translate(Vector3.forward * Time.deltaTime);
        }
    }

    // ��:���݂̖��_
    //    �L�����N�^�[�̓X�|�i�[���琶�������B���������^�C�~���O�ŃL�����N�^�[��Init���\�b�h���ĂԁB
    //    �������ꂽ�L�����N�^�[��StateBase��GameManager����擾���Ă���B
    
    //    �ȉ������݂̖��_
    //    �E���g��StatusBase���擾����^�C�~���O��Start()�Ȃ̂ŁAInit�Ƃ͕ʃ^�C�~���O�ɂȂ��Ă��܂��B
    //    �E�L�����N�^�[��static��GameManager�̎Q�Ƃ������Ă���̂ł����ꂩ�̃L������Destroy�����null���������A�S�̂łʂ�肪�o��B
    //    �E��`�q�̈��p���ɂ��āB�X�|�i�[���琶�����Ă���̂ň�`�q���X�|�i�[�ɓn��->���̈�`�q��Init�ŃL�����N�^�[�ɓn�������Ȃ��B

    // ����: ���Ԍo�߂Ō���A����ŉ�
    // �H��: ���Ԍo�߂Ō���A�H�ׂĉ�
    // �̗�: 0�ɂȂ�Ǝ���
        // �����ƐH����0�̎��͌����Ă���
        // �U�������Ƒ̗͂�����
        // �����Ƒ̗͂���������Ă���ꍇ�A�񕜂��Ă���
    // �ɐB��: �̗͂�������Ԃ��Ɖ��Z����Ă����B�ɐB�����烊�Z�b�g�����
        // ��x�ɐB�����͔̂ɐB���Ȃ��悤�ɂ���Ǝq�������񂾂�Q�[�����l��
    // ����: 0�ɂȂ����玀�ʁB���̃p�����[�^�ɉe�����ꂸ��Ɉ��ʌ������Ă����B

    // ���[�_�[�����ʂƃ����_���Ŏ��̃��[�_�[�����܂�B
        // �Q��̍Ō��1�C�����ʂƂ��߂��ׂ�

    // ���Ԋu�Ő����ƐH���̂������Ȃ��ق��𖞂������Ƃ���
        // ��̓I�ɂ͉��񂩃Z�����ړ�������`�F�b�N
        // ��������Ă��邩�`�F�b�N�B��������Ă���ꍇ�͉������Ȃ��B
        // �߂��̐����������͐H���̃}�X���擾
        // �n���Ōo�H�T���B�o�H������Ό������B�����ꍇ�͂Ȃɂ����Ȃ�

    // �s���̒P�ʂ̓A�j���[�V����
        // �A�j���[�V�������I������玟�̍s����I������
        // �܂�A�s��A -> ���f -> �s��B �ƍs�����ɒ����̏�Ԃɖ߂��Ď��̍s�����`�F�b�N����K�v������B
            // �ʏ�̃X�e�[�g�x�[�X�ł͖����B
            // �O������̍U���Ȃǂōs�����Ɏ��ʏꍇ������H

    // �ɐB����ۂ͐e�̓������󂯌p��(��`)
        // ��`�I�A���S���Y��
        // ��`�q��4�ARGB+�T�C�Y�A

    // ����
        // ���[�_�[�����Ȃ��B�e�X������ɍs������B
        // ������������ƍU�����Ă���B
        // �U���Ŏ��ʁB
        // �ɐB�͂��Ȃ��B
        // �����_���ŕ����B
        // ���ʂ��тɈ�`�I�ȕψق�����H�����Ȃ�����キ�Ȃ�����
}