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

        public RunaroundStat(int i, bool c, int s)
        {
            Index = i;
            Correct = c;
            Score = s;
        }
    }

    [Serializable]
    public class RunaroundStats
    {
        public List<RunaroundStat> RunaroundStatsList = new List<RunaroundStat>();
    }

    public class RunaroundLogger : SceneLogger
    {
        private RunaroundStats m_Stats;

        public void AddStats(int i, bool c, int s)
        {
            m_Stats.RunaroundStatsList.Add(new RunaroundStat(i, c, s));
        }

        public override string GetStats()
        {
            return JsonUtility.ToJson(m_Stats);
        }
    }

}
