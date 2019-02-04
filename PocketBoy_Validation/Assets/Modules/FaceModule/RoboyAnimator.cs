using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class RoboyAnimator : MonoBehaviour
{
    private Animator m_Animator;
    private bool m_Talking = false;
    private bool m_GlassesOn = false;

    // Use this for initialization
    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {

        if (Random.value < 0.001f)
            m_Animator.SetTrigger("blink");

    }


   
}