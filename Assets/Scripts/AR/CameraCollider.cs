using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraCollider : MonoBehaviour {

    public  Image Logo;
    private Coroutine m_cor;
    private float m_treshold = 7.0f;
    private Vector3 m_logo_scale;
    private Quaternion m_logo_rotation;


    private void Start()
    {
        m_logo_scale = Logo.transform.localScale;
        m_logo_rotation = Logo.transform.localRotation;


    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collision happened.");
        Debug.Log(other.gameObject.name.ToString());
        m_cor = StartCoroutine(BeginAnimation());
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_cor != null)
        {
            StopCoroutine(m_cor);
            Logo.transform.localRotation = m_logo_rotation;
            Logo.transform.localScale = m_logo_scale;
            Logo.gameObject.SetActive(false);
        }
    }

    private IEnumerator BeginAnimation()
    {
        float time = 0.0f;
        Debug.Log("animation has begun");
        Logo.gameObject.SetActive(true);
        //Counting time and animate
        while (time < m_treshold)
        {
            Logo.transform.Rotate(Vector3.forward, 10.0f, Space.Self);
            //Logo.transform.Rotate(Vector3.forward, 10.0f);
            //Logo.transform.position = new Vector3(Logo.transform.position.x, Logo.transform.position.y, Logo.transform.position.z - 3 * Time.deltaTime);
            Logo.transform.localScale = new Vector3(Logo.transform.localScale.x + 1 * Time.deltaTime, Logo.transform.localScale.y + 1 * Time.deltaTime, Logo.transform.localScale.z);
            time += Time.deltaTime;
            yield return null;
        }

        //Reset and disable the logo
        Logo.transform.localRotation = m_logo_rotation;
        Logo.transform.localScale = m_logo_scale;
        Logo.gameObject.SetActive(false);

    }
    
}
