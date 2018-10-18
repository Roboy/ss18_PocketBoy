using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    public static class MathUtility
    {

        /// <summary>
        /// Wraps the given index from 0 to arrayLength.
        /// In Unity "%" is the mathematical rem not mod, they behave differentely for different signs of a,b so we need to transform rem to mod.
        /// Source: http://answers.unity.com/answers/1120641/view.html
        /// </summary>
        /// <param name="index"></param>
        /// <param name="arrayLength"></param>
        /// <returns></returns>
        public static int WrapArrayIndex(int index, int arrayLength)
        {
            return (index % arrayLength + arrayLength) % arrayLength;
        }

        public static Vector2 Shake2D(float amplitude, float frequency, int octaves, float persistance, float lacunarity, float burstFrequency, int burstContrast, float time)
        {
            float valX = 0;
            float valY = 0;

            float iAmplitude = 1;
            float iFrequency = frequency;
            float maxAmplitude = 0;

            // Burst frequency
            float burstCoord = time / (1 - burstFrequency);

            // Sample diagonally trough perlin noise
            float burstMultiplier = Mathf.PerlinNoise(burstCoord, burstCoord);

            //Apply contrast to the burst multiplier using power, it will make values stay close to zero and less often peak closer to 1
            burstMultiplier = Mathf.Pow(burstMultiplier, burstContrast);

            for (int i = 0; i < octaves; i++) // Iterate trough octaves
            {
                float noiseFrequency = time / (1 - iFrequency) / 10;

                float perlinValueX = Mathf.PerlinNoise(noiseFrequency, 0.5f);
                float perlinValueY = Mathf.PerlinNoise(0.5f, noiseFrequency);

                // Adding small value To keep the average at 0 and   *2 - 1 to keep values between -1 and 1.
                perlinValueX = (perlinValueX + 0.0352f) * 2 - 1;
                perlinValueY = (perlinValueY + 0.0345f) * 2 - 1;

                valX += perlinValueX * iAmplitude;
                valY += perlinValueY * iAmplitude;

                // Keeping track of maximum amplitude for normalizing later
                maxAmplitude += iAmplitude;

                iAmplitude *= persistance;
                iFrequency *= lacunarity;
            }

            valX *= burstMultiplier;
            valY *= burstMultiplier;

            // normalize
            valX /= maxAmplitude;
            valY /= maxAmplitude;

            valX *= amplitude;
            valY *= amplitude;

            return new Vector2(valX, valY);
        }

        public static float ConvertRange(float oldMin, float oldMax, float newMin, float newMax, float value)
        {
            if (oldMin > oldMax || newMin > newMax) // dont accept incorrect ranges
            {
                return -1f;
            }
            float scale = (newMax - newMin) / (oldMax - oldMin);
            return newMin + ((value - oldMin) * scale);
        }
    }
}
