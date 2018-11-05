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

        private void Start()
        {
            StartCoroutine(DefaultAnimation());
        }

        IEnumerator DefaultAnimation()
        {
            int currentLEDIndex = 0;
            while (true)
            {
                LEDs[currentLEDIndex].sharedMaterial.SetColor("_EmissionColor", Color.black);
                currentLEDIndex = MathUtility.WrapArrayIndex(currentLEDIndex + 1, LEDs.Count);
                LEDs[currentLEDIndex].sharedMaterial.SetColor("_EmissionColor", HighlightColor);
                yield return new WaitForSeconds(HighlightTimePerLED);
            }
        }
    }
}


