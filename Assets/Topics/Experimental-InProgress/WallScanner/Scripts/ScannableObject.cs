using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannableObject : MonoBehaviour
{
    public GameObject RoboyHand;
    public float m_MinDistanceTreshold;
    public float m_MaxDistanceTreshold;

    [SerializeField]
    private float m_CurrentDistance = 0.0f;
    private Renderer m_MyRenderer;
    private float m_TotalDistance;


    // Use this for initialization
    void Start()
    {
        m_MyRenderer = gameObject.GetComponent<Renderer>();
        m_TotalDistance = m_MaxDistanceTreshold - m_MinDistanceTreshold;
    }

    private void Update()
    {
        AdjustVisibilityToDistance();
    }


    private void AdjustVisibilityToDistance()
    {
        float currentDistance = Vector3.Distance(gameObject.transform.position, RoboyHand.transform.position);
        float alpha = -1.0f;


        if (currentDistance < m_MinDistanceTreshold)
        {
            
            alpha = 0.0f;
        }
        if (currentDistance > m_MaxDistanceTreshold)
        {
            
            alpha = 255.0f;
        }
        if (currentDistance > m_MinDistanceTreshold && currentDistance < m_MaxDistanceTreshold)
        {
           
            float tmp = (currentDistance - m_MinDistanceTreshold) / m_TotalDistance;
            tmp *= tmp;
            alpha = Mathf.Lerp(0.0f, 255.0f, tmp);
        }

        m_CurrentDistance = currentDistance;
        Debug.Log("alpha: " + alpha);
        m_MyRenderer.material.color = new Color(m_MyRenderer.material.color.r, m_MyRenderer.material.color.g, m_MyRenderer.material.color.b, alpha / 255.0f);

    }

}
