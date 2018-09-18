using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Pocketboy.Common;

public class GameMaster : Singleton<GameMaster> {

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
    /// <summary>
    /// Which plane is currently highlighted.
    /// </summary>
    private GameObject m_highlightedPlane;
    private int m_correctAnswer = -1;
    private RunaroundAnswer m_playerAnswer;
    private bool m_playerWon = false;



    public void DebugMessage(string s)
    {
        Debug.Log(s);
    }

    public void StartRunaround(int correctAnswer)
    {
        Debug.Log("Game has started.");
        Coroutine game = StartCoroutine(PlayRunaround(correctAnswer));
        if (!m_TimerText.IsActive())
        {
            m_TimerText.gameObject.SetActive(true);
        }
        StartCoroutine(m_TimerText.GetComponent<GameTimer>().Countdown(duration));
    }


    private IEnumerator PlayRunaround(int correctAnswer)
    {
        //Deactivate the previous result
        m_Outcome.gameObject.SetActive(false);
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
        StartCoroutine(DisplayResult());
        m_TimerText.gameObject.SetActive(false);

        
        
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
            if (m_playerWon && m_playerAnswer!=null)
            {
                m_Outcome.text = "You won!";
            }
            else
            {
                m_Outcome.text = "You lost!";
            }
            m_Outcome.gameObject.SetActive(true);
        }

        //Fancy flashy animation, highlighting correct answer in green and if player is on the wrong spot, in red.
        while (counter < 2.0f)
        {
            if (!m_playerWon && m_playerAnswer !=null)
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

    public void CheckPosition(RunaroundAnswer ans)
    {
        
        int tmp = AnswerPlanes.IndexOf(ans);
        m_playerAnswer = ans;

        if (m_correctAnswer == tmp)
        {
            m_playerWon = true;
        }
        else
        {
            m_playerWon = false;
        }

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
