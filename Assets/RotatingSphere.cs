﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingSphere : MonoBehaviour {

    public float Speed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up, Speed * Time.deltaTime);
	}
}
