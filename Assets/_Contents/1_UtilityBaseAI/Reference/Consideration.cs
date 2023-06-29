using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAIReference
{
    public abstract class Consideration : ScriptableObject
    {
        public string Name;
        float _score;

        public float Score
        {
            get => _score;
            set => _score = Mathf.Clamp01(value);
        }

        public virtual void Awake()
        {
            _score = 0;
        }

        public abstract float ScoreConsideration(NPCController npc);
    }
}
