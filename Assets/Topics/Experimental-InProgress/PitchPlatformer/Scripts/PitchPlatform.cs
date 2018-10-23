using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.PitchPlatformer
{
    [RequireComponent(typeof(BoxCollider))]
    public class PitchPlatform : MonoBehaviour
    {
        private int m_Note;

        private int m_MinimumNote;

        private float m_MaximumNote;

        private float m_StepSize;

        private Material m_Material;

        private BoxCollider m_Collider;

        private bool m_IsListening;

        private void Awake()
        {
            var renderer = GetComponent<MeshRenderer>();
            m_Material = new Material(renderer.material);
            renderer.material = m_Material;

            m_Collider = GetComponent<BoxCollider>();
            m_Collider.enabled = false;
        }

        private void OnDestroy()
        {
            Destroy(m_Material);
        }

        public void Setup(int note, int accuracy, float lengthPerSecond, float heightRange)
        {
            m_Note = note;
            m_MinimumNote = note - accuracy;
            m_MaximumNote = note + accuracy;

            var duration = transform.localScale.y / lengthPerSecond;
            // when duration is f.e. 5 seconds, we assume 60fps, the stepSize is 1/60 /  5;
            m_StepSize = (1f / 60f) / duration;

            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Clamp(transform.localPosition.y, -heightRange, heightRange), transform.localPosition.z);
        }

        public void StartListen()
        {
            if (m_IsListening)
                StopListen();

            m_IsListening = true;
            PitchPlatformerManager.Instance.PitchRecognizer.PitchDetected += OnPitchDetected;
            DisablePlatform();
            StartCoroutine(BuildPlatform());
        }

        public void StopListen()
        {
            if (!m_IsListening)
                return;

            PitchPlatformerManager.Instance.PitchRecognizer.PitchDetected -= OnPitchDetected;               
        }


        public void EnablePlatform()
        {
            m_Collider.enabled = true;
            m_Material.SetFloat("_DissolveValue", 1f);
            m_Material.SetColor("_MainColor", Color.green);
            m_Material.SetColor("_HologramColor", Color.green);
        }

        public void DisablePlatform()
        {
           
            m_Collider.enabled = false;
            m_Material.SetFloat("_DissolveValue", 0f);
            m_Material.SetColor("_MainColor", Color.green);
            m_Material.SetColor("_HologramColor", Color.red);
        }

        private void OnPitchDetected(PitchTracker sender, PitchTracker.PitchRecord pitchRecord)
        {
            PitchPlatformerManager.Instance.SetPitchValue(m_Note, pitchRecord.MidiNote);
            // recognized pitch is within bounds of min and max note
            if (pitchRecord.MidiNote >= m_MinimumNote && pitchRecord.MidiNote <= m_MaximumNote)
            {
                var currentValue = m_Material.GetFloat("_DissolveValue");
                m_Material.SetFloat("_DissolveValue", Mathf.Clamp01(currentValue + m_StepSize));
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
            EnablePlatform();
            PitchPlatformerEvents.OnPlatformFinished();
        }
    }
}