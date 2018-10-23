using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingFLoor : MonoBehaviour {

    public bool iHaveBall = true;
    public GameObject ResponseEmitter;
    private ParticleSystemRenderer PE;
    [SerializeField]
    private Material mat_correct;
    [SerializeField]
    private Material mat_incorrect;



    // Use this for initialization
    void Start () {
        if (ResponseEmitter.GetComponent<ParticleSystem>() != null)
            PE = ResponseEmitter.GetComponent<ParticleSystemRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CheckResult()
    {

        if (iHaveBall)
        {
            PE.material = mat_correct;
        }

        if (!iHaveBall)
        {
            PE.material = mat_incorrect;
        }
        
    }


  
}
