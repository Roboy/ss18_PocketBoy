using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    public class LevelSphere : MonoBehaviour
    {
        public string SceneToLoad { get { return SceneName; } }

        [SerializeField]
        private string SceneName;

        [SerializeField]
        private MeshRenderer HologramRenderer;

        [SerializeField]
        private float IdleRotationSpeed = 10f;

        [SerializeField]
        private float LoadRotationSpeed = 80;

        private float m_CurrentRotationSpeed = 0f;

        private Material m_HologramMaterial;

        private Coroutine m_AnimationCoroutine;

        private void Start()
        {
            if (HologramRenderer.material.HasProperty("_DissolveValue"))
            {
                m_HologramMaterial = HologramRenderer.material;
            }
            m_CurrentRotationSpeed = IdleRotationSpeed;
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, m_CurrentRotationSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            LevelSphereCollider collider;
            if ((collider = other.GetComponent<LevelSphereCollider>()) != null)
            {
                m_AnimationCoroutine = StartCoroutine(LoadingAnimation(collider.LoadDuration));
                m_CurrentRotationSpeed = LoadRotationSpeed;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            m_HologramMaterial.SetFloat("_DissolveValue", 0f);
            if (m_AnimationCoroutine != null)
            {
                m_CurrentRotationSpeed = IdleRotationSpeed;
                StopCoroutine(m_AnimationCoroutine);
            }
        }

        private IEnumerator LoadingAnimation(float duration)
        {
            float currentDuration = 0f;          
            while (currentDuration < duration)
            {
                m_HologramMaterial.SetFloat("_DissolveValue", Mathf.Lerp(0f, 1f, currentDuration / duration));
                currentDuration += Time.deltaTime;
                yield return null;
            }
            m_HologramMaterial.SetFloat("_DissolveValue", 1f);       
            SceneLoader.Instance.LoadScene(SceneName);
        }
        
    }
}


