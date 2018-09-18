using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStandingOnMe : MonoBehaviour {

    private RunaroundAnswer m_ans;

    private void Start()
    {
        if (GetComponent<RunaroundAnswer>() != null)
        {
            m_ans = GetComponent<RunaroundAnswer>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameMaster.Instance.CheckPosition(m_ans);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            GameMaster.Instance.CheckPosition(m_ans);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            GameMaster.Instance.ResetPlane(m_ans);
        }
    }
}
