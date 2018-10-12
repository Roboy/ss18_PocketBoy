using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarSensor : MonoBehaviour {

    public GameObject RayPrefab;
    public bool SensorActive = false;
    public float EmissionRate = 1.0f;
    public float Lifetime = 3.0f;

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

    //private void FixedUpdate()
    //{
       

    //    if (counter < EmissionRate)
    //    {
    //        counter += Time.deltaTime;
    //        return;
    //    }
        
        
    //    //reset counter
    //    counter = 0.0f;
        
    //}

    private void SpawnRay()
    {
        RayPrefab.transform.forward = transform.up;
        var wave = GameObject.Instantiate(RayPrefab);
        wave.transform.position = transform.position;
        wave.gameObject.GetComponent<WaveRay>().lifetime = Lifetime;
        

    }
}
