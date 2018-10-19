using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CollidingObjects : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    private bool m_IsTouched = false;
    private Vector3 m_initPos;
    private GameObject m_CupToSwapWith = null;
    private bool m_toBePlacedOnFloor = false;
    private float m_OffsetLiftHeight = -1.0f;


    private void Start()
    {
        m_initPos = transform.position;
        m_OffsetLiftHeight = transform.gameObject.GetComponent<Renderer>().bounds.size.y;
        m_OffsetLiftHeight += 0.1f * m_OffsetLiftHeight;
        Debug.Log(m_initPos);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsTouched)
        {
            UpdateDrag();
        }
    }


    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!m_IsTouched)
    //        return;

    //    //if (other.tag == "Cup")
    //    //{
    //    //    m_toBePlacedOnFloor = false;
    //    //    transform.position = new Vector3(transform.position.x, m_OffsetLiftHeight, transform.position.z);
    //    //    Debug.Log(other.name);
    //    //    m_CupToSwapWith = other.gameObject;
    //    //}
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (!m_IsTouched)
    //        return;

    //    //if (other.tag == "Cup")
    //    //{
    //    //    Debug.Log(other.name);
    //    //    m_toBePlacedOnFloor = true;
    //    //}

    //    //if (other.tag == "GameBoard")
    //    //{
    //    //    Debug.Log(other.name);
    //    //    transform.position = m_initPos;
    //    //}
    //}

    public void OnPointerDown(PointerEventData eventData)
    {
        m_IsTouched = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_IsTouched = false;
    }


    void UpdateDrag()
    {

//#if UNITY_EDITOR
//        Vector3 m_TouchPositionScreenSpace = Input.mousePosition;
//#elif UNITY_ANDROID
//            m_TouchPositionScreenSpace = Input.touches[0].position;
//#endif
        


//        Vector3 m_TouchPositionWorldSpace = Camera.main.ScreenToWorldPoint(m_TouchPositionScreenSpace);
//        m_TouchPositionWorldSpace.z = transform.position.z;
//        transform.position = new Vector3(m_TouchPositionWorldSpace.x, transform.position.y, transform.position.z );

        //if (m_toBePlacedOnFloor) { transform.position = new Vector3(transform.position.x, m_initPos.y, m_initPos.z); }
           


    }
}