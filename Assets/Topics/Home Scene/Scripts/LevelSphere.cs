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
        private float RotationSpeed = 30f;

        private Material m_HologramMaterial;

        private Coroutine m_AnimationCoroutine;

        private void Start()
        {
            if (HologramRenderer.material.HasProperty("_DissolveValue"))
            {
                m_HologramMaterial = HologramRenderer.material;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            LevelSphereCollider collider;
            if ((collider = other.GetComponent<LevelSphereCollider>()) != null)
            {
                m_AnimationCoroutine = StartCoroutine(LoadingAnimation(collider.LoadDuration));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            m_HologramMaterial.SetFloat("_DissolveValue", 0f);
            if (m_AnimationCoroutine != null)
            {
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
                transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
                yield return null;
            }
            m_HologramMaterial.SetFloat("_DissolveValue", 1f);
            SceneLoader.Instance.LoadScene(SceneName);
        }
        
    }
}


