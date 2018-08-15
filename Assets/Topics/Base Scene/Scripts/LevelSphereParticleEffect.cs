using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSphereParticleEffect : MonoBehaviour {

    [SerializeField]
    private ParticleSystem Warp;

    private float m_InitDistance;

    private float m_InitEmmissionRate;
	// Use this for initialization
	void Start () {
		m_InitDistance = (transform.position - Camera.main.transform.position).magnitude;
        m_InitEmmissionRate = Warp.emission.rateOverTime.constant;
        var emission = Warp.emission;
        var minMaxCurve = new ParticleSystem.MinMaxCurve();
        minMaxCurve.constant = 0f;
        emission.rateOverTime = minMaxCurve;
    }
	
	// Update is called once per frame
	void Update () {
        var emission = Warp.emission;
        var minMaxCurve = new ParticleSystem.MinMaxCurve();

        var currDistance = (transform.position - Camera.main.transform.position).magnitude;
        if (currDistance > m_InitDistance)
        {
            minMaxCurve.constant = 0f;
        }
        else
        {
            minMaxCurve.constant = m_InitEmmissionRate * (1f - (currDistance / m_InitDistance));
        }   
        emission.rateOverTime = minMaxCurve;
    }
}
