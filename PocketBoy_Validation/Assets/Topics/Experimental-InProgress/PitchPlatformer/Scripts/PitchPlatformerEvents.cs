using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.PitchPlatformer
{
    public class PitchPlatformerEvents
    {
        public delegate void PitchPlatformerDelegate();

        public static event PitchPlatformerDelegate ReachedGoalEvent, PlatformFinishedEvent, ShowLevelEvent;

        public static void OnReachedGoal()
        {
            if (ReachedGoalEvent != null)
            {
                ReachedGoalEvent();
            }
        }

        public static void OnPlatformFinished()
        {
            if (PlatformFinishedEvent != null)
            {
                PlatformFinishedEvent();
            }
        }

        public static void OnShowLevel()
        {
            if (ShowLevelEvent != null)
            {
                ShowLevelEvent();
            }
        }
    }
}
