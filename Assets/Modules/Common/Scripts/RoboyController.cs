using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    /// <summary>
    /// TODO:: Add functionality e.g. startAnimationXY, SayXY
    /// </summary>
    public class RoboyController : MonoBehaviour
    {
        [SerializeField]
        private Transform Head;
        private Coroutine m_cor;

        void Start()
        {
            var cameraPosition = Camera.main.transform.position;
            cameraPosition.y = transform.position.y;
            transform.LookAt(cameraPosition);
        }

        void Update()
        {
            Head.LookAt(Camera.main.transform.position);
            if (Input.GetKeyDown(KeyCode.A))
            {
                m_cor = StartCoroutine(EmotionManager.Instance.mouthMoving());
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                if(m_cor !=null)
                StopCoroutine(m_cor);
                EmotionManager.Instance.ResetMouth();
            }
        }
    }
}


