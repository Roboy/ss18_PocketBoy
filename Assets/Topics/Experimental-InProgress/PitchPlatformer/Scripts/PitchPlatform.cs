using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.PitchPlatformer
{
    public class PitchPlatform : MonoBehaviour
    {
        private int m_Note;

        private int m_MinimumNote;

        private float m_MaximumNote;

        private float m_StepSize;

        private Material m_Material;

        private bool m_IsListening;

        private int Ticks;

        private void Start()
        {
            var renderer = GetComponent<MeshRenderer>();
            m_Material = new Material(renderer.material);
            renderer.material = m_Material;
        }

        private void OnDestroy()
        {
            Destroy(m_Material);
        }

        public void Setup(int note, int accuracy, float lengthPerSecond)
        {
            m_Note = note;
            m_MinimumNote = note - accuracy;
            m_MaximumNote = note + accuracy;

            var duration = transform.localScale.y / lengthPerSecond;
            // when duration is f.e. 5 seconds, we assume 60fps, the stepSize is 1/60 * 5;
            m_StepSize = (1f / 60f) / duration;
            Debug.Log(m_StepSize);

            Debug.Log(m_MinimumNote + " : " + note + " : " + m_MaximumNote);
        }

        public void StartListen()
        {
            if (m_IsListening)
                StopListen();

            PitchPlatformerManager.Instance.PitchRecognizer.PitchDetected += OnPitchDetected;
            StartCoroutine(BuildPlatform());
        }

        public void StopListen()
        {
            PitchPlatformerManager.Instance.PitchRecognizer.PitchDetected -= OnPitchDetected;
        }

        private void OnPitchDetected(PitchTracker sender, PitchTracker.PitchRecord pitchRecord)
        {
            //var currentValue = m_Material.GetFloat("_DissolveValue");
            //m_Material.SetFloat("_DissolveValue", Mathf.Clamp01(currentValue + m_StepSize));
            //Ticks++;
            //return;

            // recognized pitch is within bounds of min and max note
            if (pitchRecord.MidiNote >= m_MinimumNote && pitchRecord.MidiNote <= m_MaximumNote)
            {
                var currentValue = m_Material.GetFloat("_DissolveValue");
                m_Material.SetFloat("_DissolveValue", Mathf.Clamp01(currentValue + m_StepSize));
                Ticks++;
            }
            else
            {
            }
        }

        private IEnumerator BuildPlatform()
        {
            m_Material.SetFloat("_DissolveValue", 0f);
            var currentValue = 0f;
            var time = Time.time;
            var stepDecrement = m_StepSize / 10f;
            while (currentValue < 1f)
            {
                if (currentValue == 0f)
                    time = Time.time;
                currentValue = m_Material.GetFloat("_DissolveValue");
                m_Material.SetFloat("_DissolveValue", Mathf.Clamp01(currentValue - stepDecrement));
                yield return null;
            }
            StopListen();
            Debug.Log(Time.time - time);
            m_Material.SetFloat("_DissolveValue", 1f);

        }
    }
}