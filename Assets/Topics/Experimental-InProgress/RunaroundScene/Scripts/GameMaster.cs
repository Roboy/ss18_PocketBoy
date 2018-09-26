using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Pocketboy.Common;

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
        /// The colour the plane will flash in when it is the correct answer.
        /// </summary>
        public Material mat_correct;
        /// <summary>
        /// The colour the plane will flash in when it is the incorrect answer.
        /// </summary>
        public Material mat_incorrect;
        /// <summary>
        /// Additional feedback where the player is currently Standing.
        /// </summary>
        public Material mat_position;


        [SerializeField]
        private TextMeshPro m_TimerText;
        [SerializeField]
        private TextMeshPro m_Outcome;
        [SerializeField]
        private TextMeshPro m_Score;
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



        public void DebugMessage(string s)
        {
            Debug.Log(s);
        }

        public void StartRunaround(int correctAns)
        {
            Coroutine game = StartCoroutine(PlayRunaround(correctAns));
            if (!m_TimerText.IsActive())
            {
                m_TimerText.gameObject.SetActive(true);
            }
            GameObject score = m_Score.gameObject.transform.parent.gameObject;
            if (!score.activeInHierarchy)
            {
                score.SetActive(true);
            }

            StartCoroutine(m_TimerText.GetComponent<GameTimer>().Countdown(duration));
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
                    m_highlightedPlane.GetComponent<Renderer>().material = mat_default;
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
                    current_speed += current_speed * current_speed;
                    counter += current_speed;
                    yield return new WaitForSeconds(current_speed);
                }


            }
            m_highlightedPlane.GetComponent<Renderer>().material = mat_default;
            //Display the outcome including win or loose state
            StartCoroutine(DisplayResult());
            //Make the timer disappear again
            m_TimerText.gameObject.SetActive(false);

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
                if (m_CurrentDistance < 0.5)
                {
                    m_CurrentMultiplier = -7;
                }

                //Moving not quite enough
                if (m_CurrentDistance > 0.5 && m_CurrentDistance < 1)
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
                    m_Outcome.text = "You win!";
                }
                else
                {
                    m_playerWon = false;
                    m_Outcome.text = "You loose!";
                }
                m_Outcome.gameObject.SetActive(true);
            }

            //Fancy flashy animation, highlighting correct answer in green and if player is on the wrong spot, in red.
            while (counter < 2.0f)
            {
                if (!m_playerWon && m_playerAnswer != null)
                {
                    m_playerAnswer.Floor.GetComponent<Renderer>().material = mat_incorrect;
                }
                AnswerPlanes[m_correctAnswer].Floor.GetComponent<Renderer>().material = mat_correct;
                yield return new WaitForSeconds(0.1f);
                if (!m_playerWon && m_playerAnswer != null)
                {
                    m_playerAnswer.Floor.GetComponent<Renderer>().material = mat_default;
                }
                AnswerPlanes[m_correctAnswer].Floor.GetComponent<Renderer>().material = mat_default;
                yield return new WaitForSeconds(0.1f);

                counter += 0.2f;

            }




        }

        private IEnumerator ResetGame()
        {
            //Deactivate the previous result
            m_Outcome.gameObject.SetActive(false);
            //Reset game outcome
            m_playerWon = false;
            m_playerAnswer = null;
            //Reset score
            m_Score.text = "0";
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
                m_CurrentDistance = 3000 * distance;
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
            //Store the players answer
            m_playerAnswer = ans;
            //Indicate the position of the player on the field
            ans.SignPost.GetComponent<Renderer>().material = mat_position;
        }

        /// <summary>
        /// This is called when the player steps out of one answer.
        /// </summary>
        /// <param name="ans"></param>
        public void ResetPlane(RunaroundAnswer ans)
        {

            //Reset the floor
            ans.Floor.GetComponent<Renderer>().material = mat_default;
            //Reset the signpost
            ans.SignPost.GetComponent<Renderer>().material = mat_default;
            //Reset the player's answer
            m_playerAnswer = null;
        }


    }
}
