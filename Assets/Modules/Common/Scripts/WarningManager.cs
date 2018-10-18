using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Pocketboy.Common
{
    public class WarningManager : Singleton<WarningManager>
    {
        [SerializeField]
        private GameObject WarningParent;

        [SerializeField]
        private TextMeshProUGUI WarningText;

        private bool m_ShowingMessage;

        public void ShowWarning(string message)
        {
            if (m_ShowingMessage) // avoid changing warning when already showing one
                return;

            m_ShowingMessage = true;
            WarningParent.SetActive(true);
            WarningText.text = message;         
        }

        public void HideWarning()
        {
            m_ShowingMessage = false;
            WarningParent.SetActive(false);
            WarningText.text = "";
        }
    }
}


