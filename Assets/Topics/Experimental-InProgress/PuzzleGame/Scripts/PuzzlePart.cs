using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Pocketboy.Common;

namespace Pocketboy.PuzzleGame
{
    /// <summary>
    /// This is a moveable body part which needs to be pulled to the correct position by the user.
    /// </summary>
    public class PuzzlePart : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// Unique ID of a puzzle part, is used to check if it is moved to the correct position.
        /// </summary>
        public int ID;

        public bool Moveable = false;

        private bool m_IsTouched;

        private Vector2 m_TouchPositionScreenSpace;

        private Vector3 m_TouchPositionWorldSpace;

        private Rigidbody m_RigidBody;

        private bool m_KinematicState;
        private bool m_TargetReached = false;
        private bool m_OutsideArea = false;
        private Quaternion m_InitRotation;
        private Vector3 m_Intermediateposition;
        private GameObject m_LastHittedTarget = null;


        /// <summary>
        /// Offset when on touch down between the touch position and the position to avoid a snap to the center.
        /// </summary>
        private Vector3 m_OffsetOnTouch;

        /// <summary>
        /// When moving the object you need to define the touch position in 3D space, 
        /// the distance between the camera and the position is used as z-coordinate for the world touch position.
        /// </summary>
        private float m_DistanceToCameraOnTouch = 0f;

        public void setInitRotation()
        {
            m_InitRotation = transform.rotation;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            StartDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            StopDrag();
        }

        private void Update()
        {
            if (m_IsTouched)
            {
                if (Moveable)
                {
                    UpdateDrag();

                }
            }
        }

        void StartDrag(PointerEventData eventData)
        {
            if (Moveable)
            {
                m_IsTouched = true;
                m_Intermediateposition = transform.position;
                transform.rotation = m_InitRotation;

                m_DistanceToCameraOnTouch = (Camera.main.transform.position - transform.position).magnitude;
                m_OffsetOnTouch = (Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, m_DistanceToCameraOnTouch)) - transform.position);
            }
        }

        void StopDrag()
        {
            if (Moveable)
            {
                m_IsTouched = false;
                m_DistanceToCameraOnTouch = 0f;
                m_OffsetOnTouch = Vector3.zero;

                if (!m_TargetReached)
                {
                    m_RigidBody.isKinematic = false;
                    m_RigidBody.useGravity = true;
                }
                if (m_TargetReached)
                {
                    if (m_LastHittedTarget != null)
                    {
                        this.transform.position = m_LastHittedTarget.transform.position;
                        this.GetComponent<Collider>().enabled = false;
                        m_LastHittedTarget.GetComponent<Collider>().enabled = false;
                        PuzzleMaster.Instance.ColorTargetPart(this.gameObject, "Default");
                        PuzzleMaster.Instance.IncrementNumberOfCorrectParts();
                        PuzzleMaster.Instance.CheckForCompletion();

                    }
                }

                if (m_OutsideArea)
                {
                    transform.position = m_Intermediateposition;
                    m_OutsideArea = false;
                }
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

        public void Initialize()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            if (m_RigidBody)
            {
                m_KinematicState = m_RigidBody.isKinematic;
            }
        }

        public void SetMoveability(bool value)
        {
            Moveable = value;
        }

        #region Trigger

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "RoboyPart")
            {
                m_LastHittedTarget = other.gameObject;

                if (other.GetComponent<RoboyPartTarget>().ID == this.ID)
                {
                    m_TargetReached = true;
                    PuzzleMaster.Instance.ColorTargetPart(this.gameObject, "Correct");
                }
                else
                {
                    m_TargetReached = false;
                    PuzzleMaster.Instance.ColorTargetPart(this.gameObject, "Incorrect");
                }
            }




        }

        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "RoboyPart")
            {
                m_LastHittedTarget = other.gameObject;

                if (other.GetComponent<RoboyPartTarget>().ID == this.ID)
                {
                    m_TargetReached = true;
                    PuzzleMaster.Instance.ColorTargetPart(this.gameObject, "Correct");
                }
                else
                {
                    m_TargetReached = false;
                    PuzzleMaster.Instance.ColorTargetPart(this.gameObject, "Incorrect");
                }
            }



        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Deadzone")
            {
                m_OutsideArea = true;
            }
            m_LastHittedTarget = null;
            m_TargetReached = false;
            PuzzleMaster.Instance.ColorTargetPart(this.gameObject, "Default");

        }

        #endregion

        #region Collider
        private void OnCollisionEnter(Collision collision)
        {
            if (Moveable)
            {
                if (collision.transform.tag == "Floor")
                {
                    StayPut();
                }
            }

        }

        #endregion


        private void StayPut()
        {
            m_RigidBody.isKinematic = true;
            m_RigidBody.useGravity = false;
        }
    }
}

