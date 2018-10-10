using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Pocketboy.Wordcloud;


public class Word : MonoBehaviour, IPointerClickHandler {

    public string Text;
    public SimpleHelvetica Obj;
    private GameObject m_WordCloud;

    private void Start()
    {
        m_WordCloud = GameObject.FindGameObjectWithTag("WordCloud");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        m_WordCloud.GetComponent<WordCloud>().OnMouseDown(this.gameObject);
    }
}
