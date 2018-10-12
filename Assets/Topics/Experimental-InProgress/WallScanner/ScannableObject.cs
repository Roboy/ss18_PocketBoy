using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannableObject : MonoBehaviour
{

    private Vector3 m_EntryPosition;
    private Vector3 m_Direction;
    private float m_EntryDistance;
    private GameObject m_TriggerObject;
    private Renderer m_MyRenderer;

    // Use this for initialization
    void Start()
    {

        m_TriggerObject = null;
        m_EntryPosition = Vector3.zero;
        m_Direction = Vector3.zero;
        m_EntryDistance = 0.0f;
        m_MyRenderer = gameObject.GetComponent<Renderer>();


    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject != null && other.gameObject.tag == "Hand")
        {
            m_Direction = other.gameObject.transform.position - transform.position;
            m_Direction /= 2.0f;
            m_TriggerObject = other.gameObject;
            m_EntryPosition = m_TriggerObject.transform.position;
            m_EntryDistance = Vector3.Distance(gameObject.transform.position + m_Direction, other.gameObject.transform.position);

        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject != null && other.gameObject.tag == "Hand")
        {

            float currentDistance = Vector3.Distance(gameObject.transform.position + m_Direction, other.gameObject.transform.position);
            float alpha = Mathf.Lerp(0.0f, 255.0f, currentDistance / m_EntryDistance);
            m_MyRenderer.material.color = new Color(m_MyRenderer.material.color.r, m_MyRenderer.material.color.g, m_MyRenderer.material.color.b, alpha / 255.0f);
            
        }

    }

    private void OnTriggerExit(Collider other)
    {
        m_TriggerObject = null;
        m_EntryPosition = Vector3.zero;
        m_Direction = Vector3.zero;
        m_EntryDistance = 0.0f;


    }
}
