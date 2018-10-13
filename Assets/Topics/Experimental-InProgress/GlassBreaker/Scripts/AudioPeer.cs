using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class AudioPeer : MonoBehaviour {


    AudioSource audioSource;
    public float updateStep = 0.1f;
    public int sampleDataLength = 1024;

    private float currentUpdateTime = 0f;

    private float clipLoudness;
    private float[] clipSampleData;
    private Vector3 m_originScale;

    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        clipSampleData = new float[sampleDataLength];
        m_originScale = transform.localScale;
    }
    

    // Update is called once per frame
    void Update()
    {

        currentUpdateTime += Time.deltaTime;
        if (currentUpdateTime >= updateStep)
        {
            currentUpdateTime = 0f;
            audioSource.clip.GetData(clipSampleData, audioSource.timeSamples); //ad 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.I re
            clipLoudness = 0f;
            foreach (var sample in clipSampleData)
            {
                clipLoudness += Mathf.Abs(sample);
            }
            clipLoudness /= sampleDataLength; //clipLoudness is what you are looking for
            transform.localScale = m_originScale + clipLoudness * m_originScale;
        }

        

    }

}
