using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyAroundPlanet : MonoBehaviour {

    public Transform Target;

    private Vector3 m_InitDistance;

	// Use this for initialization
	void Start () {
        m_InitDistance = transform.position - Target.position;
	}
	
	// Update is called once per frame
	void Update () {

        //var newOffset = RotatePointAroundPivot(transform.position, Target.position, new Vector3(0f, 0f, 1f));
        //transform.position = Target.position + newOffset;
        transform.RotateAround(Target.position, -transform.up, 1f);
        //var test = transform.position - Target.position;
        //transform.position = Target.position + test.normalized * m_InitDistance.magnitude;
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) /*+ pivot*/;
    }
}
