using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;
using UnityEngine.UI;
using System;
using TMPro;

namespace Pocketboy.PuzzleGame {

    public class PuzzleController : Singleton<PuzzleController> {

        [SerializeField]
        private PuzzleMaster PM;
        [SerializeField]
        private ImpactMaster IM;
        [SerializeField]
        private Button m_StartButton;
        [SerializeField]
        private TextMeshProUGUI m_ProgressCounter;

        private List<Button> m_Buttons = new List<Button>();

        private void Awake()
        {
            LevelManager.Instance.RegisterGameObjectWithRoboy(PM.gameObject, new Vector3(-1.5f, 0.05f, 0.0f));
        }
        private void Start()
        {
            m_StartButton.onClick.AddListener(StartPuzzleGame);
            m_Buttons.Add(m_StartButton);
            PM.GetProgressStatus();
        }

        private void StartPuzzleGame()
        {
            if (m_StartButton.gameObject.activeInHierarchy)
            {
                m_StartButton.gameObject.SetActive(false);
                m_ProgressCounter.gameObject.SetActive(true);
            }
            StartCoroutine(IM.Explode());
        }

        public void UpdateProgressCounter(string progress)
        {
            m_ProgressCounter.text = progress;
        }

    }
}
