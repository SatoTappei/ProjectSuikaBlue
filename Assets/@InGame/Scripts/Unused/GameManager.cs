using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace PSB.InGame
{
    public class GameManager : MonoBehaviour
    {
        //[SerializeField] 

        //void Start()
        //{
        //    ExecuteAsync(this.GetCancellationTokenOnDestroy()).Forget();
        //}

        //async UniTaskVoid ExecuteAsync(CancellationToken token)
        //{
        //    // �L�����N�^�[�̃X�e�[�^�X�ǂݍ���
        //    await StatusBaseHolder.LoadAsync(token);
        //    // �t�B�[���h�̐���
        //    FieldManager.Instance.Create();
        //    // ����������z�u
        //    InitKinpatsuSpawner kinpatsu = GetComponent<InitKinpatsuSpawner>();
        //    //kinpatsu.Spawn(field);
        //}

        //void OnDestroy()
        //{
        //    StatusBaseHolder.Release();
        //}

        //void M()
        //{
        //    uint gene = 0b_0000_0000_0000_0000_0000_1000_0000_0000;
        //    // 0~255
        //    float f = gene & 0xFF;
        //    // f��0.75����1.5�͈̔͂Ƀ��}�b�v
        //    float mappedSize = (f - 0) * (1.5f - 0.75f) / (255 - 0) + 0.75f;
        //    Debug.Log(mappedSize);
        //}

        ///// <summary>
        ///// �r�b�g�������͈̔͂�؂�o���Ĉ�`�q�Ƃ��Ĉ���
        ///// �_���r�b�g�V�t�g(�V�t�g����������0)���g�p����̂�uint�^
        ///// </summary>
        //void Gene()
        //{
        //    // 1.�C�ӂ̐�(�����8)�̔{���ŉE�Ƀr�b�g�V�t�g����B32�r�b�g�Ȃ̂�4�ɕ�������B
        //    // 2.�r�b�g�}�X�N���s���A�����8�r�b�g�Ȃ̂�256�ł���0xFF�Ƃ̃r�b�g�ς��Ƃ�B

        //    //             �J���[R   �J���[G   �J���[B   �T�C�Y
        //    uint gene = 0b_0000_0000_0000_0000_0000_1000_1000_1010;
        //    Debug.Log("�J���[R: " + ((gene >> 24) & 0xFF));
        //    Debug.Log("�J���[G: " + ((gene >> 16) & 0xFF));
        //    Debug.Log("�J���[B: " + ((gene >> 8)  & 0xFF));
        //    Debug.Log("�T�C�Y: "  + (gene         & 0xFF));
        //}
    }
}
