using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Pocketboy.Wordcloud;


public class Word : MonoBehaviour, IPointerClickHandler
{

    public string Text;
    public SimpleHelvetica Obj;
    public float HoveringDampening;
    private GameObject m_WordCloud;
    private bool m_ClickedOn = false;
    private bool m_Spawned = false;

    private void Start()
    {
        m_WordCloud = GameObject.FindGameObjectWithTag("WordCloud");
        int decider = Random.Range(0, 2);
        if (decider == 0)
        {
            HoveringDampening = Random.Range(1000.0f, 2000.0f);
        }
        if (decider == 1)
        {
            HoveringDampening = Random.Range(-2000.0f, -1000.0f);
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        m_WordCloud.GetComponent<WordCloud>().OnMouseDown(this.gameObject);
        m_ClickedOn = !m_ClickedOn;
    }

    public bool isClickedOn()
    {
        return m_ClickedOn;
    }

    public void setSpawnState(bool status)
    {
        m_Spawned = status;
    }

    private void FixedUpdate()
    {
        if (m_Spawned)
        {

            transform.localPosition = new Vector3(transform.localPosition.x + (Mathf.Cos(Time.timeSinceLevelLoad) / HoveringDampening), transform.localPosition.y + (Mathf.Sin(Time.timeSinceLevelLoad) / HoveringDampening), transform.localPosition.z);
        }

    }
}
