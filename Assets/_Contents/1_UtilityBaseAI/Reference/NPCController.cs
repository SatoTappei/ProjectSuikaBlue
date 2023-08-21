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
            // ★: 引数 * 1秒待つ処理、余りにも無駄すぎるので参考にしない
            int counter = time;
            while (counter > 0)
            {
                yield return new WaitForSeconds(1.0f);
                counter--;
            }

            Debug.Log("資源取得");
            // インベントリに取得した資源を格納する処理

            OnFinishedAction();
        }

        IEnumerator SleepCoroutine(int time)
        {
            // ★: 引数 * 1秒待つ処理、余りにも無駄すぎるので参考にしない
            int counter = time;
            while (counter > 0)
            {
                yield return new WaitForSeconds(1.0f);
                counter--;
            }

            Debug.Log("睡眠回復");
            // : 食事をしたのでエネルギーが回復する処理

            OnFinishedAction();
        }
    }

}