using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.ModelCategorization
{
    /// <summary>
    /// A CategorizationPlatform represents an AR platform where the user can put in a <see cref="CategorizationModel"/> and gets
    /// correct feedback when 
    /// </summary>
    public class CategorizationPlatform : MonoBehaviour
    {
        [SerializeField]
        private Transform Flag;

        [SerializeField]
        private float FlagStartHeight;

        [SerializeField]
        private float FlagEndHeight;

        [SerializeField]
        private ContentRelated ContentRelatedState;

        private int m_ContentCount;

        private int m_CurrentContentCount;

        private Vector3 m_HeightStep;

        public void SetContentCount(int count)
        {
            m_ContentCount = count;
            SetupFlag();
        }

        public bool CheckContent(ContentRelated state)
        {
            if (m_CurrentContentCount > m_ContentCount)
                return false;

            if (state == ContentRelatedState)
            {
                StartCoroutine(RiseFlag());
                return true;
            }
            return false;
        }

        IEnumerator RiseFlag()
        {
            Vector3 startPosition = Flag.transform.localPosition;
            Vector3 endPosition = Flag.transform.localPosition + m_HeightStep;
            float currentDuration = 0f;
            float duration = 0.5f;
            while (currentDuration < duration)
            {
                currentDuration += Time.deltaTime;
                Flag.transform.localPosition = Vector3.Lerp(startPosition, endPosition, currentDuration / duration);
                yield return null;
            }
            Flag.transform.localPosition = endPosition;
        }


        private void SetupFlag()
        {
            m_HeightStep = new Vector3( 0f, (FlagEndHeight - FlagStartHeight) / m_ContentCount, 0f);
            Flag.transform.localPosition = new Vector3(Flag.transform.localPosition.x, FlagStartHeight, Flag.transform.localPosition.z);
        }
    }
}
