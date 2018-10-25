using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;

namespace Pocketboy.PitchPlatformer
{
    public class PitchPlatformLevel : MonoBehaviour
    {
        [SerializeField]
        private List<PitchPlatform> Platforms = new List<PitchPlatform>();

        [SerializeField]
        private Collider GoalCollider;

        private int m_CurrentPlatformIndex = -1;

        private PlatformPlayer m_Player;

        private void FixedUpdate()
        {
            PitchPlatformerManager.Instance.PitchRecognizer.ProcessBuffer(MicrophoneManager.Instance.GetSamples());
        }

        public void Show()
        {
            gameObject.SetActive(true);

            foreach (var platform in Platforms)
            {
                platform.StopListen();
                platform.DisablePlatform();
            }

            PitchPlatformerEvents.PlatformFinishedEvent += GoToNextPlatform;
            MicrophoneManager.Instance.StartRecording();

            m_CurrentPlatformIndex = -1;
            m_Player.ResetPosition();
            m_Player.Stop();
            GoalCollider.enabled = false;
            GoToNextPlatform();                   
        }

        public void Hide()
        {
            PitchPlatformerEvents.PlatformFinishedEvent -= GoToNextPlatform;
            MicrophoneManager.Instance.StopRecording();
                        
            gameObject.SetActive(false);
        }

        public void Setup(float heightRange, int accuracyThreshold, float platformLengthPerSecond, PlatformPlayer player)
        {
            foreach (var platform in Platforms)
            {
                // get height value between 0 and 1
                float normalizedHeightValue = MathUtility.NormalizeValue(-heightRange, heightRange, platform.transform.localPosition.y);
                // get the corresponding note value depending on height value
                //int platformNote = (int)Mathf.Lerp(CalibrationManager.Instance.CalibratedLowNote, CalibrationManager.Instance.CalibratedHighNote, normalizedHeightValue);

                // DEBUG
                int platformNote = (int)Mathf.Lerp(36, 69, normalizedHeightValue);
                platform.Setup(platformNote, accuracyThreshold, platformLengthPerSecond, heightRange);
            }
            m_Player = player;
        }

        private void GoToNextPlatform()
        {
            if (m_CurrentPlatformIndex < Platforms.Count - 1)
            {
                m_CurrentPlatformIndex++;
                Platforms[m_CurrentPlatformIndex].StartListen();
            }
            else
            {
                GoalCollider.enabled = true;
            }
        }
    }
}
