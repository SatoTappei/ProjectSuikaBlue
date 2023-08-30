using UnityEngine;
using UniRx;

namespace PSB.InGame
{
    public class ChildSpawner : ActorSpawner
    {
        [Header("�ʏ펞�̐U�ꕝ")]
        [SerializeField] byte _randomRange = 2;
        [Header("�ˑR�ψٗ�")]
        [SerializeField] float _mutationProb = 0.05f;
        [Header("�ˑR�ψق����ۂ̐U�ꕝ�̔{��")]
        [SerializeField] byte _mutationMag = 3;

        // �ˑR�ψَ��� �U�ꕝ�̍ő�l * �萔�{ �����ω�������
        byte MutationValue => (byte)(_randomRange * _mutationMag);
        // �ʏ펞�͐U�ꕝ�̂��������_���Ȓl�����ω�������
        byte RandomValue => (byte)Random.Range(0, _randomRange + 1);

        void Awake()
        {
            SubscribeReceiveMessage();
        }

        void SubscribeReceiveMessage()
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
            // �ő吔�ɒB���Ă����琶�����Ȃ�
            if (!Check()) return;

            uint childGene = CalcChildGene(msg);
            Actor actor = InstantiateActor(ActorType.Kinpatsu, msg.Pos, childGene);

            // �L�����N�^�[�𐶐��������b�Z�[�W�𑗐M����
            SendSpawnMessage(actor);
        }

        uint CalcChildGene(SpawnChildMessage msg)
        {
            uint gene1 = msg.Gene1;
            uint gene2 = msg.Gene2;
            IReadOnlyParams paramsRO = msg.Params;

            byte gene1Size = (byte)(gene1 & 0xFF);
            byte gene2Size = (byte)(gene2 & 0xFF);
            byte gene1R = (byte)(gene1 >> 24 & 0xFF);
            byte gene2R = (byte)(gene2 >> 24 & 0xFF);
            byte gene1G = (byte)(gene1 >> 16 & 0xFF);
            byte gene2G = (byte)(gene2 >> 16 & 0xFF);
            byte gene1B = (byte)(gene1 >> 8 & 0xFF);
            byte gene2B = (byte)(gene2 >> 8 & 0xFF);

            // �ˑR�ψق�����
            bool isMutation = Random.value < _mutationProb;

            // �T�C�Y�̐ݒ�B���ς���� + ���m���œˑR�ψ�
            int tempSize = (gene1Size + gene2Size) / 2;
            tempSize += (isMutation ? MutationValue : RandomValue) * Sign(paramsRO);
            // byte�̃T�C�Y�ɃN�����v
            byte size = Clamp(tempSize);

            // �J���[�̐ݒ�BRGB�̊e�l���ǂ��炩�̐e������
            int tempR = Random.Range(0, 2) == 0 ? gene1R : gene2R;
            int tempG = Random.Range(0, 2) == 0 ? gene1G : gene2G;
            int tempB = Random.Range(0, 2) == 0 ? gene1B : gene2B;
            tempR -= (isMutation ? MutationValue : RandomValue) * Sign(paramsRO);
            tempG -= (isMutation ? MutationValue : RandomValue) * Sign(paramsRO);
            tempB -= (isMutation ? MutationValue : RandomValue) * Sign(paramsRO);
            byte r = Clamp(tempR);
            byte g = Clamp(tempG);
            byte b = Clamp(tempB);

            // ���̍Đ�
            AudioManager.PlayAudio(isMutation ? AudioKey.BreedingMutationSE : AudioKey.BreedingSE);

            return (uint)(r << 24 | g << 16 | b << 8 | size);
        }

        /// <summary>
        /// �X�R�A�Ɋ�Â����m���ŁA�ǂ��ω��������ω��̂ǂ���ɕω����邩�����߂�
        /// </summary>
        /// <returns>�ǂ��ω�:1 �����ω�:-1</returns>
        int Sign(IReadOnlyParams paramsRO) => Random.value <= CalcScore(paramsRO) ? 1 : -1;

        // TODO:�ɐB�̃X�R�A�v�Z�����x�^����
        float CalcScore(IReadOnlyParams paramsRO)
        {
            float score = 5; // ���ꂼ��ő�90��

            if (paramsRO.HP >= 0.5f) score++;        // �H����0.5�ȏ� ++
            if (paramsRO.HP <= 0) score--;           // �H����0 --
            if (paramsRO.Water >= 0.5f) score++;     // ������0.5�ȏ� ++
            if (paramsRO.Water <= 0) score--;        // ������0 --
            if (paramsRO.HP >= 0.95f) score++;       // HP��0.95�ȏ� ++
            if (paramsRO.HP <= 0.75f) score--;       // HP��0.75�ȉ� --
            if (paramsRO.LifeSpan >= 0.66f) score++; // ������0.66�ȏ� ++
            if (paramsRO.LifeSpan <= 0.33f) score--; // ������0.33�ȉ� --

            // 0����1�̒l�ɐ��`����
            return score /= 10;
        }

        byte Clamp(int value) => (byte)Mathf.Clamp(value, byte.MinValue, byte.MaxValue);

        void SendSpawnMessage(Actor actor)
        {
            // ���܂ꂽ
            MessageBroker.Default.Publish(new ActorSpawnMessage() { Pos = actor.transform.position });
            // ���O
            string color = Utility.ColorCodeGreen;
            string log = $"<color={color}>{actor.name}</color>�����̕��s�������E�ɎY�ݗ��Ƃ��ꂽ�ł��B";
            MessageBroker.Default.Publish(new EventLogMessage() { Message = log });
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
