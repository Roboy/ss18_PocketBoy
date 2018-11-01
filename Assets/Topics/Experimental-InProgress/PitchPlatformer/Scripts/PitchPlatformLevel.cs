using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;

namespace Pocketboy.PitchPlatformer
{
    public class PitchPlatformLevel : MonoBehaviour
    {
        [SerializeField]
        private Collider GoalCollider;

        public int CurrentPlatformIndex { get { return m_CurrentPlatformIndex; } }

        public int GoalPlatformIndex { get { return m_Platforms.Length - 1; } }

        private int m_CurrentPlatformIndex = -1;

        private PitchPlatform[] m_Platforms;

        private void FixedUpdate()
        {
            PitchPlatformerManager.Instance.PitchRecognizer.ProcessBuffer(MicrophoneManager.Instance.GetSamples());
        }

        public void Show()
        {    
            gameObject.SetActive(true);
            foreach (var platform in m_Platforms)
            {
                platform.StopListen();
                platform.DisablePlatform();
            }

            PitchPlatformerEvents.PlatformFinishedEvent += GoToNextPlatform;
            MicrophoneManager.Instance.StartRecording();

            m_CurrentPlatformIndex = -1;
            GoalCollider.enabled = false;
            GoToNextPlatform();
            PitchPlatformerEvents.OnShowLevel();
        }

        public void Hide()
        {
            PitchPlatformerEvents.PlatformFinishedEvent -= GoToNextPlatform;
            MicrophoneManager.Instance.StopRecording();
                        
            gameObject.SetActive(false);
        }

        public void Setup(float heightRange, int accuracyThreshold, float platformLengthPerSecond)
        {
            m_Platforms = GetComponentsInChildren<PitchPlatform>(true);
            foreach (var platform in m_Platforms)
            {
                // get height value between 0 and 1
                float normalizedHeightValue = MathUtility.NormalizeValue(-heightRange, heightRange, platform.transform.localPosition.y);
                // get the corresponding note value depending on height value
                int platformNote = (int)Mathf.Lerp(CalibrationManager.Instance.CalibratedLowNote, CalibrationManager.Instance.CalibratedHighNote, normalizedHeightValue);
                // DEBUG
                //int platformNote = (int)Mathf.Lerp(36, 69, normalizedHeightValue);
                platform.Setup(platformNote, accuracyThreshold, platformLengthPerSecond, heightRange);
            }
        }

        private void GoToNextPlatform()
        {
            if (m_CurrentPlatformIndex < m_Platforms.Length - 1)
            {
                m_CurrentPlatformIndex++;
                m_Platforms[m_CurrentPlatformIndex].StartListen();
            }
            else
            {
                GoalCollider.enabled = true;
            }
        }
    }
}
