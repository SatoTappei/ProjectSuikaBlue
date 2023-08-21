using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAIReference
{
    public class NPCController : MonoBehaviour
    {
        public MoveController Mover { get; set; }
        public AIBrain AIBrain { get; set; }
        public Action[] _actionsAvailable;

        void Start()
        {
            Mover = GetComponent<MoveController>();
            AIBrain = GetComponent<AIBrain>();
        }

        void Update()
        {
            if (AIBrain.FinishedDeciding)
            {
                AIBrain.FinishedDeciding = false;
                AIBrain.BestAction.Execute(this);
            }
        }

        public void OnFinishedAction()
        {
            AIBrain.DecideBestAction(_actionsAvailable);
        }

        public void DoWork(int time)
        {
            StartCoroutine(WorkCoroutine(time));
        }

        public void DoSleep(int time)
        {
            StartCoroutine(SleepCoroutine(time));
        }

        IEnumerator WorkCoroutine(int time)
        {
            // ��: ���� * 1�b�҂����A�]��ɂ����ʂ�����̂ŎQ�l�ɂ��Ȃ�
            int counter = time;
            while (counter > 0)
            {
                yield return new WaitForSeconds(1.0f);
                counter--;
            }

            Debug.Log("�����擾");
            // �C���x���g���Ɏ擾�����������i�[���鏈��

            OnFinishedAction();
        }

        IEnumerator SleepCoroutine(int time)
        {
            // ��: ���� * 1�b�҂����A�]��ɂ����ʂ�����̂ŎQ�l�ɂ��Ȃ�
            int counter = time;
            while (counter > 0)
            {
                yield return new WaitForSeconds(1.0f);
                counter--;
            }

            Debug.Log("������");
            // : �H���������̂ŃG�l���M�[���񕜂��鏈��

            OnFinishedAction();
        }
    }

}