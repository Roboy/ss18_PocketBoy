using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarSensor : MonoBehaviour {

    public GameObject RayPrefab;
    public bool SensorActive = true;
    public float EmissionRate = 1.0f;
    public float Lifetime = 3.0f;

    private float counter = 0.0f;
	
	// Update is called once per frame
	void Update () {


		
	}

    private void FixedUpdate()
    {

        if (counter < EmissionRate)
        {
            counter += Time.deltaTime;
            return;
        }
        if (SensorActive)
        {
            SpawnRay();
        }
        
        //reset counter
        counter = 0.0f;
        
    }

    private void SpawnRay()
    {
        RayPrefab.transform.forward = transform.up;
        var wave = GameObject.Instantiate(RayPrefab);
        wave.transform.position = transform.position;
        wave.gameObject.GetComponent<WaveRay>().lifetime = Lifetime;
        

    }
}
