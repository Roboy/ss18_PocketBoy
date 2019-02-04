using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RadarSensor : MonoBehaviour {

    public bool SensorActive = false;
    public Light SpotLight;

    private float counter = 0.0f;

    [SerializeField]
    private ParticleSystem ps;
    private bool m_ParticleActive;

    private void Awake()
    {
        ps.Stop();
        m_ParticleActive = ps.isPlaying;
       
    }



    // Update is called once per frame
    void Update () {

        //Enable emission of rays
        if (SensorActive)
        {
            if (!m_ParticleActive)
            {
                ps.Play();
                m_ParticleActive = true;

            }

            
        }

        //Disable emission of rays
        if (!SensorActive)
        {
            if (m_ParticleActive)
            {
                ps.Stop();
                m_ParticleActive = false;
                
            }
        }

        
        
    }


    public void ToggleLight(string state)
    {

        if (state == "ON")
        {
            SpotLight.enabled = true;
        }

        if (state == "OFF")
        {
            SpotLight.enabled = false;
        }

    }
}
