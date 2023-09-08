using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace PSB.InGame
{
    public class BreedingProvider : MonoBehaviour
    {
        public static BreedingProvider Instance;

#if UNITY_EDITOR
        [Header("�f�o�b�O�p")]
        [SerializeField] Text _debugText;
        [SerializeField] bool _isDebug;
#endif

        List<DataContext> _maleList = new();
        List<DataContext> _femaleList = new();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        void Update()
        {
            if (_maleList.Count >= 2)
            {
                //Matching();
                Shuffle();
            }

#if UNITY_EDITOR
            //if(_isDebug) DebugPrint();
#endif
        }

        public void Add(DataContext context)
        {
            if (context.Sex == Sex.Male) _maleList.Add(context);
            else if (context.Sex == Sex.Female) _femaleList.Add(context);
        }

        public void Remove(DataContext context)
        {
            if (context.Sex == Sex.Male) _maleList.Remove(context);
            else if (context.Sex == Sex.Female) _femaleList.Remove(context);
        }

        //public bool TryMatching(DataContext context, out DataContext partner)
        //{
        //    if (context.Sex == Sex.Male)
        //    {
        //        // ���̃��X�g�̐擪�𔲂��o��
        //        partner = _femaleList[0];
        //        _femaleList.RemoveAt(0);
        //        // �o�H�����邩���ׂ�
        //        Vector3 actorPos = context.Transform.position;
        //        Vector3 partnerPos = partner.Transform.position;
        //        if (FieldManager.Instance.TryGetPath(actorPos, partnerPos, out List<Vector3> _))
        //        {
        //            // ���g�ɂ̓p�[�g�i�[���A�p�[�g�i�[�ɂ͎��g���Z�b�g����
        //            context.Partner = partner;
        //            partner.Partner = context;
        //            // �}�b�`�����̂Ń��X�g����폜
        //            _maleList.Remove(context);
        //            _femaleList.Remove(partner);

        //            return true;
        //        }
        //        else
        //        {
        //            _femaleList.Add(partner);
        //        }
        //    }
        //    else if (context.Sex == Sex.Female)
        //    {

        //    }
        //}

        public void Matching(MaleBreedState actor)
        {
            // �擪���擾���Čo�H�T���A�o�H�������ꍇ�͖߂��H
            //Actor male = _actorList[0];
            //_actorList.RemoveAt(0);
            //Actor female = _actorList[0];
            //_actorList.RemoveAt(0);

            // 2�l�̊ԂɌo�H�����邩���ׂ�
            //Vector3 m = male.transform.position;
            //Vector3 f = female.transform.position;
            //if (FieldManager.Instance.TryGetPath(m, f, out Stack<Vector3> _))
            //{
            //    // �o�H������΃}�b�`���O
            //    MessageBroker.Default.Publish(new MatchingMessage()
            //    {
            //        ID = male.transform.GetInstanceID(),
            //        Partner = female.transform,
            //        Sex = Sex.Male,
            //    });
            //    MessageBroker.Default.Publish(new MatchingMessage()
            //    {
            //        ID = female.transform.GetInstanceID(),
            //        Partner = male.transform,
            //        Sex = Sex.Female,
            //    });
            //}
            //else
            //{
            //    // �ēx�ǉ�
            //    _actorList.Add(male);
            //    _actorList.Add(female);
            //}
        }

        void Shuffle()
        {
            _maleList = _maleList.OrderBy(_ => System.Guid.NewGuid()).ToList();
        }

        void Remove(CancelBreedingMessage msg)
        {
            Actor actor = msg.Actor.GetComponent<Actor>();
            //_maleList.Remove(actor);
        }

        //void DebugPrint()
        //{
        //    string str = "";
        //    foreach (Actor a in _maleList)
        //    {
        //        str += a.name;
        //        str += "\n";
        //    }
        //    _debugText.text = str;
        //}
    }
}