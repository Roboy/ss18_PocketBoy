using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Pocketboy.Common
{

    public class EmotionList : MonoBehaviour
    {
        private FaceController m_FaceController;
        private TMP_Dropdown m_DropDown;

        private void Start()
        {
            m_DropDown = gameObject.GetComponent<TMP_Dropdown>();
        }

        public void TriggerEmotion()
        {
            int currentIndex = m_DropDown.value;
            string emotion = m_DropDown.options[currentIndex].text;

            if (m_FaceController == null)
            {
                SetFaceController();
            }

            if (m_FaceController != null)
            {
                if(emotion != "cool")
                {
                    m_FaceController.DisplayEmotion("remove");
                }
                m_FaceController.DisplayEmotion(emotion);
            }


        }

        private void SetFaceController()
        {
            m_FaceController = FaceController.Instance;
        }

    }


}