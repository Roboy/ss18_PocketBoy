using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Logging;
using System;


namespace Pocketboy.Logging
{
    [Serializable]
    public class RunaroundStat
    {
        public int Index;
        public bool Correct;
        public int Score;

        public RunaroundStat(int index, bool correct, int score)
        {
            Index = index;
            Correct = correct;
            Score = score;
        }
    }

    [Serializable]
    public class RunaroundStats
    {
        public List<RunaroundStat> StatsList = new List<RunaroundStat>();
    }

    public class RunaroundLogger : SceneLogger
    {
        [SerializeField, HideInInspector]
        private RunaroundStats m_Stats;

        public void AddStats(int index, bool correct, int score)
        {
            m_Stats.StatsList.Add(new RunaroundStat(index, correct, score));
        }

        public override string GetStats()
        {
            return JsonUtility.ToJson(m_Stats);
        }
    }

}
