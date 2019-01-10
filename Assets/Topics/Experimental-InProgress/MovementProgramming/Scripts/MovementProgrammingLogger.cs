using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pocketboy.Logging
{
    [Serializable]
    public class MovementProgrammingStat
    {
        public int TryIndex;

        public int Difficulty;

        public bool GoalReached;

        public MovementProgrammingStat(int tryIndex, int difficulty, bool goalReached)
        {
            TryIndex = tryIndex;
            Difficulty = difficulty;
            GoalReached = goalReached;
        }
    }

    [Serializable]
    public class MovementProgrammingStats
    {
        public List<MovementProgrammingStat> StatsList = new List<MovementProgrammingStat>();
    }

    public class MovementProgrammingLogger : SceneLogger
    {
        [HideInInspector, SerializeField]
        private MovementProgrammingStats m_Stats;

        public override string GetStats()
        {
            return JsonUtility.ToJson(m_Stats);
        }

        public void SaveTry(int tryIndex, int difficulty, bool goalReached)
        {
            m_Stats.StatsList.Add(new MovementProgrammingStat(tryIndex, difficulty, goalReached));
        }
        
    }
}
