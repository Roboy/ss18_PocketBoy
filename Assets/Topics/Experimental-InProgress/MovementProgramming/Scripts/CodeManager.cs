using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Pocketboy.Common;
using System;
using TMPro;

namespace Pocketboy.MovementProgramming
{

    public class CodeManager : Singleton<CodeManager>
    {
        public RectTransform LineOfCodePrefab;
        public RectTransform DisplayPanel;
        public RectTransform MoveLinesPanel;

        public Color Col_Default;
        public Color Col_Selected;
        public Color Col_Highlighted;
        [HideInInspector]
        public int m_NumberOfTries = 0;

        [SerializeField]
        private GameObject m_RoboyInMaze;
        [SerializeField]
        private TextMeshProUGUI m_AttemptCounter;
        [SerializeField]
        private Button m_ForwardButton;
        [SerializeField]
        private Button m_RotateRightButton;
        [SerializeField]
        private Button m_RotateLefttButton;
        [SerializeField]
        private Button m_DeleteButton;
        [SerializeField]
        private Button m_StartButton;
        [SerializeField]
        private Button m_MoveUpButton;
        [SerializeField]
        private Button m_MoveDownButton;
        [SerializeField]
        private List<RectTransform> m_LinesOfCode = new List<RectTransform>();
        private List<RectTransform> m_SelectedLines = new List<RectTransform>();

        private List<Button> m_Buttons = new List<Button>();
        private int m_CountSelectedLines = 0;
        private bool m_LineExecuting = false;
        private MazeRunner m_Player;
        private Coroutine m_currentCoroutine;
        private RectTransform m_currentVisualLineOfCode;
        

        // Use this for initialization
        void Start()
        {
            m_ForwardButton.onClick.AddListener(InstructionForward);
            m_RotateRightButton.onClick.AddListener(InstructionRight);
            m_RotateLefttButton.onClick.AddListener(InstructionLeft);
            m_DeleteButton.onClick.AddListener(DeleteInsctruction);
            m_StartButton.onClick.AddListener(delegate { m_currentCoroutine = StartCoroutine(ExecuteInstructionCode()); });
            m_MoveUpButton.onClick.AddListener(MoveInsctructionUp);
            m_MoveDownButton.onClick.AddListener(MoveInstructionDown);

            m_Buttons.Add(m_ForwardButton);
            m_Buttons.Add(m_RotateRightButton);
            m_Buttons.Add(m_RotateLefttButton);
            m_Buttons.Add(m_DeleteButton);
            m_Buttons.Add(m_StartButton);
            m_Buttons.Add(m_MoveUpButton);
            m_Buttons.Add(m_MoveDownButton);

            m_Player = m_RoboyInMaze.GetComponent<MazeRunner>();
        }


        private IEnumerator ExecuteInstructionCode()
        {
            if (m_LinesOfCode.Count == 0)
                yield return null;

            ToggleButtons("OFF");
            UpdateAttemptCounter(1);

            LineOfCode currentLine;
            TextMeshProUGUI currentInstruction;
            m_Player.m_GoalHit = false;

            //Start executing the instructions one by one
            foreach (RectTransform rt in m_LinesOfCode)
            {
                m_currentVisualLineOfCode = rt;
                currentLine = rt.GetComponent<LineOfCode>();
                currentInstruction = rt.GetComponent<TextMeshProUGUI>();
                currentInstruction.color = Col_Highlighted;

                if (currentLine.operation == "Forward")
                {
                    yield return StartCoroutine(m_Player.GoForward());
                }
                else
                {
                    yield return StartCoroutine(m_Player.TurnAround(currentLine.operation));
                }

                if (m_Player.m_GoalHit)
                {
                    m_AttemptCounter.text = ("You escaped with " + m_NumberOfTries + " attempts.");
                    currentInstruction.color = Col_Default;
                    break;
                }


                currentInstruction.color = Col_Default;
                
            }

            //Check for winning/losing state
            if (!m_Player.m_GoalHit)
            {
                m_AttemptCounter.text = ("You are still stuck with " + m_NumberOfTries + " attempts.");
                m_Player.ResetPlayerPose();
            }

            m_currentVisualLineOfCode = null;
            ToggleButtons("ON");
            yield return null;
        }



        private void InstructionForward()
        {
            CreateInstruction("Forward");
        }

        private void InstructionRight()
        {
            CreateInstruction("Turn Right");
        }

        private void InstructionLeft()
        {
            CreateInstruction("Turn Left");
        }


        private void CreateInstruction(string operation)
        {
            Rect DP = DisplayPanel.rect;
            //Resize parent panel to have enough space for the additional rect
            DisplayPanel.sizeDelta = new Vector2(DP.width, DP.height + LineOfCodePrefab.rect.height);
            //Instantiate the additional rect
            var LOC = GameObject.Instantiate(LineOfCodePrefab, DisplayPanel);
            //Set the position to the last element in the panel
            Vector2 pos;
            if (m_LinesOfCode.Count == 0)
            {
                pos.x = LOC.rect.width / 2.0f;
                pos.y = LOC.rect.height / -2.0f;


            }
            else
            {
                pos = m_LinesOfCode[m_LinesOfCode.Count - 1].anchoredPosition;
                //Shift y pos below last element in the panel
                pos.y -= LOC.rect.height;

            }
            LOC.anchoredPosition = pos;

            //Rename for better readability in the editor
            LOC.name = "Line#" + (m_LinesOfCode.Count);
            //Add the lineofcode component to the graphic panel and set the operation, e.g. forward, right, etc
            LineOfCode line = LOC.gameObject.GetComponent<LineOfCode>();
            line.operation = operation;
            LOC.GetComponent<TextMeshProUGUI>().text = line.operation;
            //Add the whole construct to the list that can be managed
            m_LinesOfCode.Add(LOC);

        }

        private void DeleteInsctruction()
        {
            List<RectTransform> ToBeRemoved = new List<RectTransform>();

            foreach (RectTransform rt in m_LinesOfCode)
            {
                if (rt.GetComponent<LineOfCode>().IsSelected())
                {
                    ToBeRemoved.Add(rt);

                }
            }

            foreach (RectTransform rt in ToBeRemoved)
            {
                m_LinesOfCode.Remove(rt);
                Destroy(rt.gameObject);
            }
            ToBeRemoved.Clear();

            //Update remaining visual instructions
            updateNames();
            updatePositions();
            updatePanelSize();

            m_SelectedLines.Clear();
            ToggleLineMovement();
        }

        public void DeleteAllInstructions()
        {
            List<RectTransform> ToBeRemoved = m_LinesOfCode;
            foreach (RectTransform rt in ToBeRemoved)
            {
                //m_LinesOfCode.Remove(rt);
                Destroy(rt.gameObject);
            }
            m_LinesOfCode.Clear();

            ToBeRemoved.Clear();

            //Update remaining visual instructions
            updateNames();
            updatePositions();
            updatePanelSize();

            m_SelectedLines.Clear();
            ToggleLineMovement();
        }


        private void MoveInstructionDown()
        {
            //Only one element can be switched at a time
            if (m_SelectedLines.Count != 1)
                return;
            //Element is already at the lowest position
            if (m_LinesOfCode.IndexOf(m_SelectedLines[0]) == m_LinesOfCode.Count - 1)
                return;
            //Switching is possible
            int index_selected = m_LinesOfCode.IndexOf(m_SelectedLines[0]);
            int index_switchingTo = m_LinesOfCode.IndexOf(m_SelectedLines[0]) + 1;

            RectTransform selectedLine = m_LinesOfCode[index_selected];
            RectTransform LineToSwitchWith = m_LinesOfCode[index_switchingTo];

            Vector2 tmp_pos = selectedLine.anchoredPosition;
            selectedLine.anchoredPosition = LineToSwitchWith.anchoredPosition;
            LineToSwitchWith.anchoredPosition = tmp_pos;

            m_LinesOfCode[index_selected] = LineToSwitchWith;
            m_LinesOfCode[index_switchingTo] = selectedLine;
            ToggleLineMovement();
            updateNames();
        }

        private void MoveInsctructionUp()
        {
            //Only one element can be switched at a time
            if (m_SelectedLines.Count != 1)
                return;
            //Element is already at the highest position
            if (m_LinesOfCode.IndexOf(m_SelectedLines[0]) == 0)
                return;
            //Switching is possible
            int index_selected = m_LinesOfCode.IndexOf(m_SelectedLines[0]);
            int index_switchingTo = m_LinesOfCode.IndexOf(m_SelectedLines[0]) - 1;

            RectTransform selectedLine = m_LinesOfCode[index_selected];
            RectTransform LineToSwitchWith = m_LinesOfCode[index_switchingTo];

            Vector2 tmp_pos = selectedLine.anchoredPosition;
            selectedLine.anchoredPosition = LineToSwitchWith.anchoredPosition;
            LineToSwitchWith.anchoredPosition = tmp_pos;

            m_LinesOfCode[index_selected] = LineToSwitchWith;
            m_LinesOfCode[index_switchingTo] = selectedLine;
            ToggleLineMovement();
            updateNames();
        }


        private void updateNames()
        {
            if (m_LinesOfCode.Count == 0)
                return;


            foreach (RectTransform rt in m_LinesOfCode)
            {
                rt.name = "Line#" + m_LinesOfCode.IndexOf(rt);
            }
        }
        private void updatePositions()
        {
            if (m_LinesOfCode.Count == 0)
                return;

            foreach (RectTransform rt in m_LinesOfCode)
            {
                int index = m_LinesOfCode.IndexOf(rt);
                if (index == 0)
                {
                    rt.anchoredPosition = new Vector2(LineOfCodePrefab.rect.width / 2.0f, LineOfCodePrefab.rect.height / -2.0f);
                    continue;
                }

                Vector2 pos = m_LinesOfCode[index - 1].anchoredPosition;
                pos.y -= LineOfCodePrefab.rect.height;
                rt.anchoredPosition = pos;
            }
        }

        private void updatePanelSize()
        {
            Rect DP = DisplayPanel.rect;
            DisplayPanel.sizeDelta = new Vector2(DP.width, LineOfCodePrefab.rect.height * m_LinesOfCode.Count);
        }

        public void ChangeInstructionColour(RectTransform rt, bool selected)
        {
            if (selected)
            {
                rt.GetComponent<TextMeshProUGUI>().color = Col_Selected;
            }
            if (!selected)
            {
                rt.GetComponent<TextMeshProUGUI>().color = Col_Default;
            }

        }

        public void CalculateSelectedLines(RectTransform rt, bool selected)
        {

            if (selected)
            {
                m_SelectedLines.Add(rt);
            }

            if (!selected)
            {
                m_SelectedLines.Remove(rt);
            }

            
             ToggleLineMovement();
        }


        /// <summary>
        /// Disable/enable vertical switching of a LOC
        /// </summary>
        private void ToggleLineMovement()
        {
            if (m_SelectedLines.Count != 1)
            {
                m_MoveUpButton.gameObject.SetActive(false);
                m_MoveDownButton.gameObject.SetActive(false);
                return;
            }

            //Beginning of the Code, only allow down movement
            if (m_LinesOfCode.IndexOf(m_SelectedLines[0]) == 0)
            {
                m_MoveUpButton.gameObject.SetActive(true);
                m_MoveUpButton.enabled = false;
                m_MoveUpButton.GetComponent<Image>().color = Color.grey;

                m_MoveDownButton.gameObject.SetActive(true);
                m_MoveDownButton.enabled = true;
                m_MoveDownButton.GetComponent<Image>().color = Col_Selected;
                return;
            }

            //End of the Code, only allow up movement
            if (m_LinesOfCode.IndexOf(m_SelectedLines[0]) == m_LinesOfCode.Count - 1)
            {
                m_MoveUpButton.gameObject.SetActive(true);
                m_MoveUpButton.enabled = true;
                m_MoveUpButton.GetComponent<Image>().color = Col_Selected;

                m_MoveDownButton.gameObject.SetActive(true);
                m_MoveDownButton.enabled = false;
                m_MoveDownButton.GetComponent<Image>().color = Color.grey;
                return;
            }

            m_MoveUpButton.gameObject.SetActive(true);
            m_MoveUpButton.enabled = true;
            m_MoveUpButton.GetComponent<Image>().color = Col_Selected;

            m_MoveDownButton.gameObject.SetActive(true);
            m_MoveDownButton.enabled = true;
            m_MoveDownButton.GetComponent<Image>().color = Col_Selected;

        }

        public void StopExecution()
        {
            //Stop executing the code
            StopCoroutine(m_currentCoroutine);
            //Remove visual highlight of the line
            m_currentVisualLineOfCode.GetComponent<TextMeshProUGUI>().color = Col_Default;
            m_currentVisualLineOfCode = null;
            //Reset player to init pose
            m_Player.ResetPlayerPose();
            ToggleButtons("ON");

        }

        public void ToggleButtons(string operation)
        {
            foreach (Button b in m_Buttons)
            {

                if (operation == "ON")
                {
                    b.GetComponent<Image>().color = Col_Selected;
                    b.enabled = true;
                    
                }

                if (operation == "OFF")
                {
                    b.GetComponent<Image>().color = Color.grey;
                    b.enabled = false;
                }
            }
        }

        public void ExchangeRoboyInMaze(GameObject Roboy)
        {
            m_RoboyInMaze = Roboy;
        }

        public void UpdateAttemptCounter(int amount)
        {
            m_NumberOfTries += amount;
            string count;

            if(m_NumberOfTries == 0)
            {
                m_AttemptCounter.text = "";
                return;
            }

            switch (m_NumberOfTries)
            {
                case 1:
                    count = "one";
                    break;
                case 2:
                    count = "two";
                    break;
                case 3:
                    count = "three";
                    break;
                default:
                    count = m_NumberOfTries.ToString();
                    break;
            }

            m_AttemptCounter.text = "You tried " + count + " time(s).";
        }

        
    }
}
