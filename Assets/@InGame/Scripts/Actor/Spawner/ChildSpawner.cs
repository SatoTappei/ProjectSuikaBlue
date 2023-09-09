using UnityEngine;
using UniRx;

namespace PSB.InGame
{
    /// <summary>
    /// �����ɕK�v�ȃp�����[�^�̃��b�Z�[�W����M���Ďq�𐶐�����
    /// </summary>
    public class ChildSpawner : ActorSpawner
    {
        enum Result
        {
            Normal,
            Mutation,
        }

        [Header("�ʏ펞�̐U�ꕝ")]
        [SerializeField] byte _randomRange = 2;
        [Header("�ˑR�ψق����ۂ̐U�ꕝ�̔{��")]
        [SerializeField] byte _mutationMag = 3;
        [Header("�ˑR�ψٗ�")]
        [Range(0, 1)]
        [SerializeField] float _mutationProb = 0.05f;
        [Header("�X�R�A���v�Z����ۂ̐H����臒l")]
        [Range(0, 1)]
        [SerializeField] float _foodThresholdHigh = 0.5f;
        [Range(0, 1)]
        [SerializeField] float _foodThresholdLow = 0;
        [Header("�X�R�A���v�Z����ۂ̐�����臒l")]
        [Range(0, 1)]
        [SerializeField] float _waterThresholdHigh = 0.5f;
        [Range(0, 1)]
        [SerializeField] float _waterThresholdLow = 0;
        [Header("�X�R�A���v�Z����ۂ̗̑͂�臒l")]
        [Range(0, 1)]
        [SerializeField] float _hpThresholdHigh = 0.9f;
        [Range(0, 1)]
        [SerializeField] float _hpThresholdLow = 0.6f;
        [Header("�X�R�A���v�Z����ۂ̎�����臒l")]
        [Range(0, 1)]
        [SerializeField] float _lifeSpanThresholdHigh = 0.66f;
        [Range(0, 1)]
        [SerializeField] float _lifeSpanThresholdLow = 0.33f;

        // �ˑR�ψَ��� �U�ꕝ�̍ő�l * �萔�{ �����ω�������
        byte MutationValue => (byte)(_randomRange * _mutationMag);
        // �ʏ펞�͐U�ꕝ�̂��������_���Ȓl�����ω�������
        byte RandomValue => (byte)Random.Range(0, _randomRange + 1);

        void Awake()
        {
            ReceiveMessage();
        }

        void ReceiveMessage()
        {
            MessageBroker.Default.Receive<SpawnChildMessage>().Subscribe(Execute).AddTo(this);
        }

        /// <summary>
        /// �q�̐������s��
        /// ���e�̈�`�q + ���̃X�e�[�^�X ����Ɏq�̈�`�q�����肷��
        /// �T�C�Y�͑傫�����A�F�͔Z�����Ɉ�`�q���ǂ�
        /// </summary>
        void Execute(SpawnChildMessage msg)
        {
            Result result = CalcChildGene(msg, out uint childGene);

            if (TryInstantiate(ActorType.Kinpatsu, msg.Pos, out Actor actor, childGene))
            {
                SendEventLogMessage(actor, result);
                PlaySE(result);
            }
        }

        Result CalcChildGene(SpawnChildMessage msg, out uint gene)
        {
            uint gene1 = msg.Gene1;
            uint gene2 = msg.Gene2;

            byte gene1Size = (byte)(gene1 & 0xFF);
            byte gene2Size = (byte)(gene2 & 0xFF);
            byte gene1R = (byte)(gene1 >> 24 & 0xFF);
            byte gene2R = (byte)(gene2 >> 24 & 0xFF);
            byte gene1G = (byte)(gene1 >> 16 & 0xFF);
            byte gene2G = (byte)(gene2 >> 16 & 0xFF);
            byte gene1B = (byte)(gene1 >> 8 & 0xFF);
            byte gene2B = (byte)(gene2 >> 8 & 0xFF);

            // �X�R�A�̌v�Z
            float score = CalcScore(msg);

            // �ˑR�ψق�����
            bool isMutation = Random.value < _mutationProb;

            // �T�C�Y�̐ݒ�B���ς���� + ���m���œˑR�ψ�
            int tempSize = (gene1Size + gene2Size) / 2;
            tempSize += SubmitValue(isMutation) * Sign(score);
            // byte�̃T�C�Y�ɃN�����v
            byte size = Clamp(tempSize);

            // �J���[�̐ݒ�BRGB�̊e�l���ǂ��炩�̐e������
            int tempR = NextBool() ? gene1R : gene2R;
            int tempG = NextBool() ? gene1G : gene2G;
            int tempB = NextBool() ? gene1B : gene2B;
            tempR -= SubmitValue(isMutation) * Sign(score);
            tempG -= SubmitValue(isMutation) * Sign(score);
            tempB -= SubmitValue(isMutation) * Sign(score);
            byte r = Clamp(tempR);
            byte g = Clamp(tempG);
            byte b = Clamp(tempB);

            gene = (uint)(r << 24 | g << 16 | b << 8 | size);
            return isMutation ? Result.Mutation : Result.Normal;
        }

        float CalcScore(SpawnChildMessage msg)
        {
            // �Ō��10�Ŋ���A0~1�̊Ԃɐ��^����
            // ���ꂼ��ő�90��
            float score = 5;

            if (msg.Food >= _foodThresholdHigh) score++;         // �H����n�ȏ� ++
            if (msg.Food <= _foodThresholdLow) score--;          // �H����n�ȉ� --
            if (msg.Water >= _waterThresholdHigh) score++;       // ������n�ȏ� ++
            if (msg.Water <= _waterThresholdLow) score--;        // ������n�ȉ� --
            if (msg.HP >= _hpThresholdHigh) score++;             // HP��n�ȏ� ++
            if (msg.HP <= _hpThresholdLow) score--;              // HP��n�ȉ� --
            if (msg.LifeSpan >= _lifeSpanThresholdHigh) score++; // ������n�ȏ� ++
            if (msg.LifeSpan <= _lifeSpanThresholdLow) score--;  // ������n�ȉ� --

            // 0����1�̒l�ɐ��`����
            return score /= 10;
        }

        byte SubmitValue(bool isMutation) => isMutation ? MutationValue : RandomValue;
        int Sign(float score) => Random.value <= score ? 1 : -1;
        bool NextBool() => Random.Range(0, 2) == 0;
        byte Clamp(int value) => (byte)Mathf.Clamp(value, byte.MinValue, byte.MaxValue);

        void SendEventLogMessage(Actor actor, Result result)
        {
            string color = Utility.ColorCodeGreen;
            string log = string.Empty;
            if (result == Result.Normal)
            {
                log = $"<color={color}>{actor.name}</color>�����̕��s�������E�ɎY�ݗ��Ƃ��ꂽ�ł��B";
            }
            else if (result == Result.Mutation)
            {
                log = $"<color={color}>{actor.name}</color>�����̕��s�������E�ɎY�ݗ��Ƃ��ꂽ�ł��B�ˑR�ψفI";
            }
            MessageBroker.Default.Publish(new EventLogMessage() { Message = log });
        }

        void PlaySE(Result result)
        {
            // ���̍Đ�
            AudioManager.PlayAudio(result == Result.Mutation ? AudioKey.BreedingMutationSE : AudioKey.BreedingSE);
        }

        // �f�o�b�O�p
        void Log(uint gene, float sizeMax = 1.5f, float sizeMin = 0.5f)
        {
            byte colorR = (byte)(gene >> 24 & 0xFF);
            byte colorG = (byte)(gene >> 16 & 0xFF);
            byte colorB = (byte)(gene >> 8 & 0xFF);

            // 0~255
            float f = gene & 0xFF;
            // f���ŏ�/�ő�T�C�Y�͈̔͂Ƀ��}�b�v
            float size = (f - 0) * (sizeMax - sizeMin) / (byte.MaxValue - byte.MinValue) + sizeMin;

            Debug.Log($"R:{colorR} G:{colorG} B:{colorB} �T�C�Y:{size}");
        }
    }
}
