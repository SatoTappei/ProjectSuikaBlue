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
        /// <summary>
        /// �e�������ꍇ�̃f�t�H���g�̈�`�q
        /// �J���[�����A�T�C�Y��1�ɂȂ�l
        /// </summary>
        public const int DefaultGene = 1;

        public static event UnityAction<Actor> OnSpawned;

        static StatusBaseHolder _holder;

        // �C���X�y�N�^�Ŋ��蓖�ĂȂ�
        // ��ƂȂ�l + �e�����`�����l
        // �������̂̓X�|�i�[���s���B���̍ۂɈ�`�����l��n�������B
        // ��`�����l�̓����_���Ȃ̂�SO��C���X�y�N�^����n���Ȃ��B

        // StatusBase�̎擾��Controller���ł̐���ɕK�v�Ȃ̂Ō̖��Ƀf�[�^������
        [SerializeField] ActorType _type;

        ChildSpawner _spawner;

        Status _status;
        //[SerializeField] Param _food;
        //[SerializeField] Param _water;
        //[SerializeField] Param _hp;
        //[SerializeField] Param _lifeSpan;
        //[SerializeField] Param _breedingRate;

        public ActorType Type => _type;
        // UI�����ǂݎ��p
        float IReadOnlyParams.Food => /*_food.Percentage*/1;
        float IReadOnlyParams.Water => /*_water.Percentage*/1;
        float IReadOnlyParams.HP => /*_hp.Percentage*/1;

        /// <summary>
        /// �X�e�[�^�X�̐ݒ�AController���Ő��䂷�邽�߂̃R�[���o�b�N�̌Ăяo��
        /// �O������f�[�^���擾����̂�Start�̃^�C�~���O�ŌĂԕK�v������
        /// </summary>
        public void InitOnStart(int gene) 
        {
            // SO���擾�A�e��f�[�^��ǂݍ���
            //_holder ??= FindFirstObjectByType<StatusBaseHolder>();

            OnSpawned?.Invoke(this); // �o�^�`�ɂ͎��g�̃^�C�v���K�v
            Debug.Log("Init");
        }

        void Awake()
        {
            GetComponent<ChildSpawner>();
            Debug.Log("Awake");
        }

        void Start()
        {
            //// SO���擾�A�e��f�[�^��ǂݍ���
            //_holder ??= FindFirstObjectByType<StatusBaseHolder>();
        }

        void OnDestroy()
        {
            // �����ꂩ1�ł�Destory�����^�C�~���O�ŁA�Q�Ƃ�null�ɂȂ��Ă��܂��̂ŁA
            // ���̃C���X�^���X�łʂ�肪�o��̂Œ��ӁB
            _holder = null;
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