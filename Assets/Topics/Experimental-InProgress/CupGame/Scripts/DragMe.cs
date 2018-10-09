using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Pocketboy.Cupgame
{

    public class DragMe : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        public GameObject PlayingField;

        private bool m_IsTouched;

        private Vector2 m_TouchPositionScreenSpace;

        private Vector3 m_TouchPositionWorldSpace;

        private Rigidbody m_RigidBody;

        private bool m_KinematicState;

        private Vector3 m_OriginalPosition;

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

            m_OriginalPosition = transform.position;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            StartDrag(eventData);
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

        void StartDrag(PointerEventData eventData)
        {
            m_IsTouched = true;

            if (m_RigidBody)
            {
                m_RigidBody.isKinematic = true;
            }

            m_DistanceToCameraOnTouch = (Camera.main.transform.position - transform.position).magnitude;
            m_OffsetOnTouch = (Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, m_DistanceToCameraOnTouch)) - transform.position);
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            
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

            transform.position = new Vector3(transform.position.x, m_OriginalPosition.y, transform.position.z);
            gameObject.GetComponent<BoxCollider>().isTrigger = false;
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
            Vector3 NextPosition = Camera.main.ScreenToWorldPoint(m_TouchPositionWorldSpace) - m_OffsetOnTouch;
            transform.position = new Vector3(NextPosition.x, transform.position.y, NextPosition.z);
        }

        private void OnTriggerEnter(Collider other)
        {
            float newHeight = Mathf.Abs(other.gameObject.transform.position.y);
           
            transform.position = new Vector3(transform.position.x, transform.position.y + newHeight, transform.position.z);
        }

    }
}


