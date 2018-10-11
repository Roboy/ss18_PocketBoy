using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveRay : MonoBehaviour {


    public float speed = 1.0f;
    public float lifetime = 3.0f;
    private Vector3 startingPos;

    

	// Use this for initialization
	void Start () {
        startingPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	}

    private void FixedUpdate()
    {
        
        if (Vector3.Distance(transform.position, startingPos) < lifetime)
        {
            transform.position += transform.forward * Time.deltaTime * speed;
        }

        if (Vector3.Distance(transform.position, startingPos) > lifetime)
        {
            Destroy(gameObject);
        }
    }
}
