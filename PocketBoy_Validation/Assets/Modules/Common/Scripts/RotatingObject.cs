using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : MonoBehaviour {

    [SerializeField]
    private float Speed;
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up, Speed * Time.deltaTime);
	}
}
