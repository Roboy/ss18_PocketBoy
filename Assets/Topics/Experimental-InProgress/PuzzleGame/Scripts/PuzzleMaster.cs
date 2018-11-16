using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;

namespace Pocketboy.PuzzleGame
{

    public class PuzzleMaster : Singleton<PuzzleMaster>
    {
        public Color Correct;
        public Color Incorrect;
        public Color Default;

        [SerializeField]
        private GameObject m_RoboyTarget;
        [SerializeField]
        private GameObject m_RoboyInPieces;
        [SerializeField]
        private GameObject m_PlayingArea;

        private int m_NumberOfParts;
        private int m_NumberOfCorrectParts;

        

        // Use this for initialization
        void Awake()
        {
            Initialize();
        }



        public void Initialize()
        {
            m_NumberOfParts = m_RoboyTarget.GetComponentsInChildren<RoboyPartTarget>().Length;
            Debug.Log(m_NumberOfParts);
            m_NumberOfCorrectParts = 0;

        }

        public void IncrementNumberOfCorrectParts()
        {
            m_NumberOfCorrectParts++;
        }

        public void GetProgressStatus()
        {
            PuzzleController.Instance.UpdateProgressCounter(m_NumberOfCorrectParts + " / " + m_NumberOfParts + " parts");
        }

        public void CheckForCompletion()
        {
            PuzzleController.Instance.UpdateProgressCounter(m_NumberOfCorrectParts + " / " + m_NumberOfParts + " parts");
            if (m_NumberOfParts == m_NumberOfCorrectParts)
            {
                Debug.Log("You've done it!");
            }
            else
            {
                Debug.Log("Not yet there, " + (m_NumberOfParts - m_NumberOfCorrectParts) + "piece(s) are still missing.");
            }
        }

        public void EnableRoboyTarget()
        {
            m_RoboyTarget.SetActive(true);
        }

        public void ColorTargetPart(GameObject part, string StateOfPart)
        {
            Color c = new Color();

            if (StateOfPart == "Correct")
            {
                c = Correct;
            }
            if (StateOfPart == "Incorrect")
            {
                c = Incorrect;
            }

            if (StateOfPart == "Default")
            {
                c = Default;
            }

            part.GetComponent<Renderer>().material.color = c;
        }
    }

}
