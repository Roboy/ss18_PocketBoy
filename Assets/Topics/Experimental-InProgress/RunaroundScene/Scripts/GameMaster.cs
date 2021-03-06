﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Pocketboy.Common;
using UnityEngine.UI;

namespace Pocketboy.Runaround
{


    public class GameMaster : Singleton<GameMaster>
    {

        /// <summary>
        /// The user can stand on planes representing answers to a question, one will be the correct one.
        /// </summary>
        public List<RunaroundAnswer> AnswerPlanes;
        /// <summary>
        /// How short will be the highlighting of one plane. The lower the speed the quicker the highlight.
        /// </summary>
        public float speed;
        /// <summary>
        /// How many iterations of different highlighting are necessary.
        /// </summary>
        public float duration;
        /// <summary>
        /// Standard appearance of the plane when not highlighted.
        /// </summary>
        public Material mat_default;
        /// <summary>
        /// If the plane is highlighted its appearance will change to a more gaze attracting style.
        /// </summary>
        public Material mat_highlighted;
        /// <summary>
        /// Additional feedback where the player is currently Standing.
        /// </summary>
        public Material mat_position;
        public Material mat_currentAnswer;
       
        public Color m_colWinning;
        public Color m_colLoosing;


        [SerializeField]
        private TextMeshProUGUI m_TimerText;
        [SerializeField]
        private TextMeshProUGUI m_Outcome;
        [SerializeField]
        private TextMeshProUGUI m_Score;
        [SerializeField]
        private GameObject m_HUD;
        [SerializeField]
        private Button m_NextButton;
        [SerializeField]
        private Button m_PreviousButton;



        /// <summary>
        /// Reference to floors original materials with id.
        /// </summary>
        public Dictionary<int, Material> dic_mat_floors = new Dictionary<int, Material>();
        /// <summary>
        /// Reference to signposts orginal materials with id.
        /// </summary>
        public Dictionary<int, Material> dic_mat_posts = new Dictionary<int, Material>();
        /// <summary>
        /// Reference to answer planes original positions.
        /// </summary>
        public Dictionary<int, Vector3> dic_plane_positions = new Dictionary<int, Vector3>();
        /// <summary>
        /// Referenc to answer floors original scales.
        /// </summary>
        public Dictionary<int, Vector3> dic_plane_scale = new Dictionary<int, Vector3>();
        /// <summary>
        /// Which plane is currently highlighted.
        /// </summary>
        private GameObject m_highlightedPlane;
        private int m_correctAnswer = -1;
        private RunaroundAnswer m_playerAnswer;
        private bool m_playerWon = false;
       

        //Scoring
        private int m_CurrentMultiplier = 0;
        private int m_CurrentScore = 0;
        private Vector3 m_CurrentPlayerPosition;
        private Vector3 m_PreviousPlayerPosition;
        private float m_CurrentDistance;
        private bool m_GameHasEnded = false;
        private Color m_ButtonDefaultColor;

        public void SetInitProperties()
        {
            
            for (int i = 0; i < AnswerPlanes.Count; i++)
            {
                //Save the material of every floor
                dic_mat_floors.Add(i, AnswerPlanes[i].Floor.GetComponent<Renderer>().material);
                //Save the material of every post
                dic_mat_posts.Add(i, AnswerPlanes[i].SignPost.GetComponent<Renderer>().material);
                //Save the position of every Answer
                Vector3 position = new Vector3(AnswerPlanes[i].transform.position.x, AnswerPlanes[i].transform.position.y, AnswerPlanes[i].transform.position.z);
                dic_plane_positions.Add(i, position);
                //Save the scale of every floor
                Vector3 scale = new Vector3(AnswerPlanes[i].Floor.transform.localScale.x, AnswerPlanes[i].Floor.transform.localScale.y, AnswerPlanes[i].Floor.transform.localScale.z);
                dic_plane_scale.Add(i, scale);
         
            }

            m_ButtonDefaultColor = m_NextButton.GetComponent<Image>().color;

        }

        public void StartRunaround(int correctAns)
        {
            QuestionManager.Instance.ToggleAnswersVisibility("ON");
            ToggleButtons("OFF");
            ToggleHUDvisibility("ON");
            Coroutine game = StartCoroutine(PlayRunaround(correctAns));
            if (!m_TimerText.IsActive())
            {
                m_TimerText.gameObject.SetActive(true);
            }

            
            if (!m_Score.gameObject.activeInHierarchy)
            {
                m_Score.gameObject.SetActive(true);
            }

            StartCoroutine(this.GetComponent<GameTimer>().Countdown(duration));
            StartCoroutine(CalculateScore(duration - 1));
        }

        /// <summary>
        /// Goal of the game is to stand to answer a question right. There are three answer spots on the ground. Standing in the right one lets you win. Score bonus points by moving.
        /// </summary>
        /// <param name="correctAnswer"></param>
        /// <returns></returns>

        private IEnumerator PlayRunaround(int correctAnswer)
        {
            yield return StartCoroutine(ResetGame());
            //Initialize the game, highlight the first plane.
            float counter = 0.0f;
            float current_speed = speed;
            m_correctAnswer = correctAnswer;
            m_highlightedPlane = AnswerPlanes[0].Floor;
            m_highlightedPlane.GetComponent<Renderer>().material = mat_highlighted;

            yield return new WaitForSeconds(speed);

            while (counter < duration)
            {
                for (int i = 0; i < AnswerPlanes.Count; i++)
                {

                    //make the old plane default again
                    int tmp = AnswerPlanes.IndexOf(m_highlightedPlane.gameObject.transform.parent.GetComponent<RunaroundAnswer>());
                    m_highlightedPlane.GetComponent<Renderer>().material = dic_mat_floors[tmp];
                    if (i == AnswerPlanes.Count - 1)
                    {
                        m_highlightedPlane = AnswerPlanes[0].Floor;
                    }
                    else
                    {
                        m_highlightedPlane = AnswerPlanes[i + 1].Floor;
                    }
                    //highlight the new plane
                    m_highlightedPlane.GetComponent<Renderer>().material = mat_highlighted;
                    //slow down the highlighting with math power of function
                    //current_speed += current_speed * current_speed;
                    counter += current_speed;
                    yield return new WaitForSeconds(current_speed);
                }


            }
            int n = AnswerPlanes.IndexOf(m_highlightedPlane.gameObject.transform.parent.GetComponent<RunaroundAnswer>());
            m_highlightedPlane.GetComponent<Renderer>().material = dic_mat_floors[n];
            //Display the outcome including win or loose state
            StartCoroutine(DisplayResult());
            //Make the timer disappear again
            m_TimerText.gameObject.SetActive(false);
            m_GameHasEnded = true;

        }

        /// <summary>
        /// Score is based on player movement. Moving a large amount in the player area before answering gets you highpoints.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private IEnumerator CalculateScore(float time)
        {
            int Score = 0;
            while (time > 0.0f)
            {
                //Standing still is penalized
                if (m_CurrentDistance < 0.005)
                {
                    m_CurrentMultiplier = -7;
                }

                //Moving not quite enough
                if (m_CurrentDistance > 0.005 && m_CurrentDistance < 0.1)
                {
                    m_CurrentMultiplier = 0;
                }
                //Moving quickly is rewarded.
                if (m_CurrentDistance > 2)
                {
                    m_CurrentMultiplier = (int)(2 * m_CurrentDistance);
                }

                //Actual calculation
                Score += m_CurrentMultiplier;
                //Prevent negative score
                if (Score <= 0)
                {
                    Score = 0;
                }
                //Display the current score as feedback for the player
                m_Score.text = Score.ToString();
                time -= Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }

        /// <summary>
        /// Make the correct answer visible by flashing the responding plane.
        /// </summary>
        /// <returns></returns>
        private IEnumerator DisplayResult()
        {
            float counter = 0.0f;

            //Display the winning or loosing state
            if (!m_Outcome.IsActive())
            {
                if (m_playerAnswer != null && m_correctAnswer == AnswerPlanes.IndexOf(m_playerAnswer))
                {
                    m_playerWon = true;
                    m_Outcome.text = "Richtig!";
                    AudioSourcesManager.Instance.PlaySound("Winning");
                    m_Outcome.GetComponent<TextMeshProUGUI>().color = m_colWinning;
                }
                else
                {
                    m_playerWon = false;
                    m_Outcome.text = "Falsch!";
                    AudioSourcesManager.Instance.PlaySound("Losing");
                    m_Outcome.GetComponent<TextMeshProUGUI>().color = m_colLoosing;
                }
                ToggleResultText("ON");
            }

            foreach(RunaroundAnswer ans in AnswerPlanes)
            {
                if (m_correctAnswer == AnswerPlanes.IndexOf(ans))
                {
                    ans.transform.position = ans.transform.parent.position;
                    ans.Floor.transform.localScale = new Vector3(4, ans.Floor.transform.localScale.y, ans.Floor.transform.localScale.z);
                }
                else
                {
                    ans.gameObject.SetActive(false);
                }
            }

            //Fancy flashy animation, highlighting correct answer in green and if player is on the wrong spot, in red.
            while (counter < 2.0f)
            {
                int t = AnswerPlanes.IndexOf(m_playerAnswer);
                //if (!m_playerWon && m_playerAnswer != null)
                //{
                //    //Standing on the wrong plate
                //    m_playerAnswer.Floor.GetComponent<Renderer>().material = mat_incorrect;
                //}
                AnswerPlanes[m_correctAnswer].Floor.GetComponent<Renderer>().material = mat_highlighted;
                yield return new WaitForSeconds(0.1f);
                //if (!m_playerWon && m_playerAnswer != null)
                //{
                //    m_playerAnswer.Floor.GetComponent<Renderer>().material = dic_mat_answers[t];
                //}
                int n = AnswerPlanes.IndexOf(AnswerPlanes[m_correctAnswer]);
                AnswerPlanes[m_correctAnswer].Floor.GetComponent<Renderer>().material = dic_mat_floors[n];
                yield return new WaitForSeconds(0.1f);

                counter += 0.2f;

            }

            ToggleButtons("ON");


        }

        public IEnumerator ResetGame()
        {
            //Deactivate the previous result
            m_Outcome.gameObject.SetActive(false);
            //Reset game outcome
            m_playerWon = false;
            //Reset player answer
            m_playerAnswer = null;
            //Reset game state
            m_GameHasEnded = false;
            //Reset score
            m_Score.text = "0";
            //Reset the planes to original colour
            foreach (RunaroundAnswer ans in AnswerPlanes)
            {
                ans.gameObject.SetActive(true);
                int tmp = AnswerPlanes.IndexOf(ans);
                //Reset position of answer object
                ans.transform.position = dic_plane_positions[tmp];
                //Reset scale of floor
                ans.Floor.transform.localScale = dic_plane_scale[tmp];
                //Asign the original materials to the floor
                ans.Floor.GetComponent<Renderer>().material = dic_mat_floors[tmp];
                //Asign the orginal materials to the posts
                ans.SignPost.GetComponent<Renderer>().material = dic_mat_posts[tmp];
            }
            yield return null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                m_CurrentPlayerPosition = other.transform.position;
                m_PreviousPlayerPosition = m_CurrentPlayerPosition;
            }

        }


        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "Player")
            {
                m_CurrentPlayerPosition = other.transform.position;
                float distance = Vector3.Distance(m_CurrentPlayerPosition, m_PreviousPlayerPosition);
                m_CurrentDistance = 10000 * distance;
                m_PreviousPlayerPosition = m_CurrentPlayerPosition;
            }

        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                m_CurrentMultiplier = 0;
            }
        }


        public void CheckPosition(RunaroundAnswer ans)
        {
            if (m_GameHasEnded)
                return;
            //Store the players answer
            m_playerAnswer = ans;
            //Indicate the position of the player on the field
            ans.SignPost.GetComponent<Renderer>().material = mat_position;
            QuestionManager.Instance.answer_Images[AnswerPlanes.IndexOf(ans)].material = mat_currentAnswer;
        }

        /// <summary>
        /// This is called when the player steps out of one answer.
        /// </summary>
        /// <param name="ans"></param>
        public void ResetPlane(RunaroundAnswer ans)
        {
            int t = AnswerPlanes.IndexOf(ans);
            //Reset the floor
            ans.Floor.GetComponent<Renderer>().material = dic_mat_floors[t];
            //Reset the signpost
            ans.SignPost.GetComponent<Renderer>().material = dic_mat_posts[t];
            //Reset the player's answer
            m_playerAnswer = null;
            QuestionManager.Instance.answer_Images[AnswerPlanes.IndexOf(ans)].material = dic_mat_floors[AnswerPlanes.IndexOf(ans)];
        }

        public void ToggleHUDvisibility(string operation)
        {
            if (operation == "ON")
            {
                m_HUD.SetActive(true);
            }
            if (operation == "OFF")
            {
                m_HUD.SetActive(false);
            }
        }

        public void ToggleButtons(string operation)
        {
            if (operation == "ON")
            {
                m_NextButton.enabled = true;
                m_PreviousButton.enabled = true;
                m_NextButton.GetComponent<Image>().color = m_ButtonDefaultColor;
                m_PreviousButton.GetComponent<Image>().color = m_ButtonDefaultColor;
            }
            if (operation == "OFF")
            {
                m_NextButton.enabled = false;
                m_PreviousButton.enabled = false;
                m_NextButton.GetComponent<Image>().color = Color.grey;
                m_PreviousButton.GetComponent<Image>().color = Color.grey;
            }
        }

        public void ToggleResultText(string operation)
        {
            if (operation == "ON")
            {
                m_Outcome.gameObject.SetActive(true);
            }
            if (operation == "OFF")
            {
                m_Outcome.gameObject.SetActive(false);
            }
        }
    }
}
