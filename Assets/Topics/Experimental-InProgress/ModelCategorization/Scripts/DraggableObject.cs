using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GoogleARCore;

namespace Pocketboy.ModelCategorization
{
#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;
#endif

    /// <summary>
    /// Makes an object draggable in mobile via IPointerInterface.
    /// </summary>
    public class DraggableObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
       
        private bool m_IsTouched;

        private Vector2 m_TouchPositionScreenSpace;

        private Vector3 m_TouchPositionWorldSpace;

        private Rigidbody m_RigidBody;

        private bool m_KinematicState;

        /// <summary>
        /// Offset when on touch down between the touch position and the position to avoid a snap to the center.
        /// </summary>
        private Vector3 m_OffsetOnTouch;

        /// <summary>
        /// When moving the object you need to define the touch position in 3D space, 
        /// the distance between the camera and the position is used as z-coordinate for the world touch position.
        /// </summary>
        private float m_DistanceToCameraOnTouch = 0f;

        void Start()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            if (m_RigidBody)
            {
                m_KinematicState = m_RigidBody.isKinematic;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            StartDrag();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            StopDrag();
        }

        // Update is called once per frame
        void Update()
        {
            if (m_IsTouched)
            {
                UpdateDrag();
            }
        }

        void StartDrag()
        {
            m_IsTouched = true;

            if (m_RigidBody)
            {
                m_RigidBody.isKinematic = true;
            }

            m_DistanceToCameraOnTouch = (Camera.main.transform.position - transform.position).magnitude;
#if UNITY_EDITOR
            m_OffsetOnTouch = (Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_DistanceToCameraOnTouch)) - transform.position);
#elif UNITY_ANDROID
            m_OffsetOnTouch = (Camera.main.ScreenToWorldPoint(new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, m_DistanceToCameraOnTouch)) - transform.position);
#endif
        }

        void StopDrag()
        {
            m_IsTouched = false;
            m_DistanceToCameraOnTouch = 0f;
            m_OffsetOnTouch = Vector3.zero;

            if (m_RigidBody)
            {
                m_RigidBody.isKinematic = m_KinematicState;
            }
        }

        void UpdateDrag()
        {
#if UNITY_EDITOR
            m_TouchPositionScreenSpace = Input.mousePosition;
#elif UNITY_ANDROID
            m_TouchPositionScreenSpace = Input.touches[0].position;
#endif
            m_TouchPositionWorldSpace = m_TouchPositionScreenSpace;
            m_TouchPositionWorldSpace.z = m_DistanceToCameraOnTouch; // transform touch position from 2d to 3d on the plane where the first touch occured
            transform.position = Camera.main.ScreenToWorldPoint(m_TouchPositionWorldSpace) - m_OffsetOnTouch;
        }

    }
}


