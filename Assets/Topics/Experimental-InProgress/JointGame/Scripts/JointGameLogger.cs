using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;
using System;

namespace Pocketboy.Logging
{
    [Serializable]
    public class JointGameStat
    {
        public float Score;

        public List<int> TriesPerRound = new List<int>();

        public JointGameStat(float score, List<int> triesPerRound)
        {
            Score = score;
            TriesPerRound = new List<int>(triesPerRound);
        }
    }

    [Serializable]
    public class JointGameStats
    {
        public List<JointGameStat> Stats = new List<JointGameStat>();
    }

    public class JointGameLogger : SceneLogger
    {
        [SerializeField, HideInInspector]
        private JointGameStats m_Stats;

        private List<int> m_Tries = new List<int>();

        public override string GetStats()
        {
            return JsonUtility.ToJson(m_Stats);
        }

        public void SaveScore(float score)
        {
            m_Stats.Stats.Add(new JointGameStat(score, m_Tries));
            m_Tries.Clear();
        }

        public void SaveTries(int tries)
        {
            m_Tries.Add(tries);
        }
    }
}


