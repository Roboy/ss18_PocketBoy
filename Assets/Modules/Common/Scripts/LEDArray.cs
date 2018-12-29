using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    public class LEDArray : MonoBehaviour
    {
        [SerializeField]
        private List<MeshRenderer> LEDs = new List<MeshRenderer>();

        [SerializeField]
        private float HighlightTimePerLED = 0.2f;

        [SerializeField]
        private Color HighlightColor = Color.white;

        [SerializeField]
        private Material LEDMaterial;

        Gradient gradient = new Gradient();
        float currentDuration = 0f;
        float duration = 5f;

        private void Start()
        {
            var colorKey = new GradientColorKey[3];
            colorKey[0].color = Color.red;
            colorKey[0].time = 0f;
            colorKey[1].color = Color.green;
            colorKey[1].time = 0.5f;
            colorKey[2].color = Color.blue;
            colorKey[2].time = 1f;

            var alphaKey = new GradientAlphaKey[3];
            alphaKey[0].alpha = alphaKey[1].alpha = alphaKey[2].alpha = 1f;
            alphaKey[0].time = 0f;
            alphaKey[1].time = 0.5f;
            alphaKey[2].time = 1f;

            gradient.SetKeys(colorKey, alphaKey);
        }

        private void Update()
        {
            if (currentDuration > duration)
            {
                currentDuration = 0f;
            }
            LEDMaterial.color = gradient.Evaluate(currentDuration / duration);
            currentDuration += Time.deltaTime;


        }

        IEnumerator DefaultAnimation()
        {
            Gradient gradient = new Gradient();
            var colorKey = new GradientColorKey[3];
            colorKey[0].color = Color.red;
            colorKey[0].time = 0f;
            colorKey[1].color = Color.green;
            colorKey[1].time = 0.5f;
            colorKey[2].color = Color.blue;
            colorKey[2].time = 1f;

            var alphaKey = new GradientAlphaKey[3];
            alphaKey[0].alpha = alphaKey[1].alpha = alphaKey[2].alpha = 1f;
            alphaKey[0].time = 0f;
            alphaKey[1].time = 0.5f;
            alphaKey[2].time = 1f;

            gradient.SetKeys(colorKey, alphaKey);
            int currentLEDIndex = 0;
            float duration = 5f;
            float currentDuration = 0f;
            while (true)
            {
                if (currentDuration > duration)
                {
                    currentDuration = 0f;
                }
                LEDMaterial.color = gradient.Evaluate(currentDuration / duration);
                currentDuration += Time.deltaTime;

                
            }
        }
    }
}


