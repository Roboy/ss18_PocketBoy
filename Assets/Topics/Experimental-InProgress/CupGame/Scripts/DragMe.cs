﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Pocketboy.Cupgame
{

    public class DragMe : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        public enum RotationDirection { left, right, none };
        public enum RotationLane { front, back, none };


        public GameObject PlayingField;
        public BouncingFLoor DeadZone;
        public bool HoldsBall { get { return m_HoldsBall; } }

        private bool m_IsTouched;

        private Vector2 m_TouchPositionScreenSpace;

        private Vector3 m_TouchPositionWorldSpace;

        private Rigidbody m_RigidBody;

        private bool m_KinematicState;

        private Vector3 m_OriginalPosition { get; set; }

        private bool m_Lifted;
        private bool m_HoldsBall;
        private bool m_Dragable = false;
        private GameObject m_ball = null;
        private Vector3 m_BoxSize = Vector3.one * 0.5f;
        private bool m_OffGameBoard = false;
        private GameObject m_AdjacentCup = null;

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

            m_OriginalPosition = transform.localPosition;
            m_Lifted = false;
            m_HoldsBall = false;
        }


        void Update()
        {
            if (m_IsTouched)
            {
                UpdateDrag();
            }
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            if (!m_Dragable)
                return;
            StartDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!m_Dragable)
                return;
            StopDrag();
        }

        private void OnTriggerEnter(Collider other)
        {
            
            
            if (!m_IsTouched)
                return;

           
            if (other.tag == "Cup")
            {
                
                m_AdjacentCup = other.gameObject;
                return;
            }

            if (other.tag == "GameBoard")
            {
                m_OffGameBoard = false;
            }

           

        }

        private void OnTriggerStay(Collider other)
        {
            if (!m_IsTouched)
                return;
            if (other.tag == "Cup")
            {
                m_AdjacentCup = other.gameObject;
                return;
            }

        }

        private void OnTriggerExit(Collider other)
        {
            if (!m_IsTouched)
                return;
            if (other.tag == "Cup")
            {
                m_AdjacentCup = null;
                return;
            }

            if (other.tag == "GameBoard")
            {
                m_OffGameBoard = true;
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
            m_OffsetOnTouch = (Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, m_DistanceToCameraOnTouch)) - transform.localPosition);
            if (m_ball != null)
            {
                m_ball.GetComponent<Renderer>().enabled = false;
            }


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

            if (m_OffGameBoard)
            {
                resetCurrentPosition();
                m_OffGameBoard = false;
            }
            else
            {
                StartCoroutine(SwapCups());
            }
            


        }

        private void UpdateDrag()
        {
            //See if the cup needs to be manipulated in the y-axis.
            StartCoroutine(CheckForElevation());

#if UNITY_EDITOR
            m_TouchPositionScreenSpace = Input.mousePosition;
#elif UNITY_ANDROID
            m_TouchPositionScreenSpace = Input.touches[0].position;
#endif

            if (m_Dragable)
            {
                m_TouchPositionWorldSpace = m_TouchPositionScreenSpace;
                m_TouchPositionWorldSpace.z = m_DistanceToCameraOnTouch; // transform touch position from 2d to 3d on the plane where the first touch occured.
                Vector3 NextPosition = Camera.main.ScreenToWorldPoint(m_TouchPositionWorldSpace) - m_OffsetOnTouch;
                //transform.position = new Vector3(NextPosition.x, transform.position.y, transform.position.z);
                transform.localPosition = new Vector3(NextPosition.x, transform.localPosition.y, transform.localPosition.z);
                if (m_ball != null)
                {
                    m_ball.transform.position = new Vector3(NextPosition.x, m_ball.transform.position.y, transform.position.z);
                }
            }
        }

        private IEnumerator CheckForElevation()
        {
            

            //gameObject.layer = LayerMask.NameToLayer("Ignore");
            //int layerMask = ~(1 << LayerMask.NameToLayer("Ignore"));

            //Collision happen.
            //if (Physics.OverlapBox(transform.position, m_BoxSize, Quaternion.identity, layerMask).Length > 0)
            if (m_AdjacentCup != null)
            {
                
                //If colliding and not elevated, increase the height, so that the cups stack
                if (!m_Lifted)
                {
                    
                    yield return StartCoroutine(ElevateCup(0, gameObject));
                }
                else
                {
                    //keep the current y value, do not change
                }
            }

            //No collisions occur.
            //if (Physics.OverlapBox(transform.position, m_BoxSize, Quaternion.identity, layerMask).Length == 0)
                if (m_AdjacentCup == null)
            {

                if (m_Lifted)
                {
                    //If the cup was lifted up, it has to come down
                    yield return StartCoroutine(ElevateCup(1, gameObject));
                }

            }

            //gameObject.layer = LayerMask.NameToLayer("Default");
            yield return null;
        }

        private IEnumerator ElevateCup(int mode, GameObject cup)
        {

            float startingHeight = cup.transform.localPosition.y;
            Vector3 currentPos = cup.transform.localPosition;
            float offset = gameObject.GetComponent<Renderer>().bounds.size.y;
            offset += (0.1f * offset);

            float endHeight = 0.0f;
            float currentDuration = 0.0f;
            float duration = 0.05f;

            //Increase of height.
            if (mode == 0)
            {
                m_Lifted = true;
                endHeight = startingHeight + offset;

            }

            //Decrease of height.
            if (mode == 1)
            {
                m_Lifted = false;
                endHeight = startingHeight - offset;
            }

            //Move the cup over time.
            while (currentDuration < duration)
            {
                currentPos = cup.transform.localPosition;
                currentPos.y = Mathf.Lerp(startingHeight, endHeight, currentDuration / duration);
                cup.transform.localPosition = currentPos;
                currentDuration += Time.deltaTime;
                yield return null;
            }

            //Eliminate accuracy error.
            cup.transform.localPosition = new Vector3(cup.transform.localPosition.x, endHeight, cup.transform.localPosition.z);
            yield return null;
        }


        private IEnumerator SwapCups()
        {
            ShuffleMaster.Instance.CupsMoveable = false;

            if (m_AdjacentCup  == null)
            {
                //no swapping needed.
                transform.localPosition = m_OriginalPosition;
                yield return null;
            }
            if (m_AdjacentCup != null)
            {
                RotationDirection dir = RotationDirection.none;
                RotationLane lan = RotationLane.none;
                //swapping needed
                if (m_Lifted)
                {

                    int index_a = Array.IndexOf(ShuffleMaster.Instance.Cups, gameObject);
                    int index_b = Array.IndexOf(ShuffleMaster.Instance.Cups, m_AdjacentCup);
                    int distance = Mathf.Abs(index_a - index_b);
                    if (index_a < index_b)
                    {
                        dir = RotationDirection.left;
                        lan = RotationLane.front;
                    }
                    if (index_b < index_a)
                    {
                        dir = RotationDirection.right;
                        lan = RotationLane.back;
                    }

                    //Cups are near to each other
                    if (distance == 1)
                    {
                        yield return StartCoroutine(ShuffleMaster.Instance.Cups[index_b].gameObject.GetComponent<DragMe>().MoveCupLinear(m_OriginalPosition, 0.5f));
                        yield return StartCoroutine(MoveCupLinear(ShuffleMaster.Instance.Cups[index_b].GetComponent<DragMe>().m_OriginalPosition, 0.5f));
                        GameObject other_cup = ShuffleMaster.Instance.Cups[index_b];
                        ShuffleMaster.Instance.Cups[index_b] = gameObject;
                        ShuffleMaster.Instance.Cups[index_a] = other_cup;
                        gameObject.GetComponent<DragMe>().resetOriginalPosition();
                        other_cup.GetComponent<DragMe>().resetOriginalPosition();
                        if (m_ball != null)
                        {
                            m_ball.GetComponent<Renderer>().enabled = true;
                        }

                    }
                    //Cups are far to each other
                    if (distance == 2)
                    {
                        yield return StartCoroutine(ShuffleMaster.Instance.Cups[index_b].gameObject.GetComponent<DragMe>().MoveCupCircular(m_OriginalPosition, 0.5f, dir, lan));
                        yield return StartCoroutine(MoveCupLinear(ShuffleMaster.Instance.Cups[index_b].GetComponent<DragMe>().m_OriginalPosition, 0.5f));
                        GameObject other_cup = ShuffleMaster.Instance.Cups[index_b];
                        ShuffleMaster.Instance.Cups[index_b] = gameObject;
                        ShuffleMaster.Instance.Cups[index_a] = other_cup;
                        gameObject.GetComponent<DragMe>().resetOriginalPosition();
                        other_cup.GetComponent<DragMe>().resetOriginalPosition();
                        if (m_ball != null)
                        {
                            m_ball.GetComponent<Renderer>().enabled = true;
                        }

                    }


                }
            }
            m_Lifted = false;
            m_AdjacentCup = null;
            //gameObject.layer = LayerMask.NameToLayer("Default");
            ShuffleMaster.Instance.CupsMoveable = true;
            

            yield return null;
        }

        public IEnumerator MoveCupLinear(Vector3 destination, float duration)
        {
            if (m_ball != null)
            {
                m_ball.GetComponent<Renderer>().enabled = false;
            }
            float currentDuration = 0.0f;
            Vector3 start = gameObject.transform.localPosition;

            while (currentDuration < duration)
            {
                gameObject.transform.localPosition = Vector3.Lerp(start, destination, currentDuration / duration);
                currentDuration += Time.deltaTime;
                yield return null;
            }

            gameObject.transform.localPosition = destination;

            if (m_ball != null)
            {
                m_ball.GetComponent<Renderer>().enabled = true;
                m_ball.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, m_ball.transform.localPosition.y, gameObject.transform.localPosition.z);
            }

        }

        public IEnumerator MoveCupCircular(Vector3 destination, float duration, RotationDirection dir, RotationLane lan)
        {
            if (m_ball != null)
            {
                m_ball.GetComponent<Renderer>().enabled = false;
            }
            float currentDuration = 0.0f;
            float currentAngle = 0.0f;
            float startingAngle = 0.0f;
            float resultingAngle = 0.0f;

            if (dir == RotationDirection.left)
            {
                startingAngle = 90.0f * Mathf.Deg2Rad;
                resultingAngle = 270.0f * Mathf.Deg2Rad;

            }
            if (dir == RotationDirection.right)
            {
                startingAngle = 270.0f * Mathf.Deg2Rad;
                resultingAngle = 90.0f * Mathf.Deg2Rad;
            }

            float distance = Vector3.Distance(destination, transform.localPosition) /2.0f;
            Vector3 start = gameObject.transform.localPosition;
            Vector3 pivotPoint = (transform.localPosition + destination) / 2.0f;

           

            while (currentDuration < duration)
            {
                currentAngle = Mathf.Lerp(startingAngle, resultingAngle, currentDuration / duration);
                Vector3 nextPos = gameObject.transform.localPosition;
                nextPos.x = pivotPoint.x + Mathf.Sin(currentAngle) * distance;
                if (lan == RotationLane.front)
                {
                    nextPos.z = pivotPoint.z + Mathf.Cos(currentAngle) * distance;
                }
                if (lan == RotationLane.back)
                {
                    nextPos.z = pivotPoint.z - Mathf.Cos(currentAngle) * distance;
                }
                gameObject.transform.localPosition = nextPos;
                currentDuration += Time.deltaTime;
                yield return null;
            }

            gameObject.transform.localPosition = destination;

            if (m_ball != null)
            {
                m_ball.GetComponent<Renderer>().enabled = true;
                m_ball.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, m_ball.transform.localPosition.y, gameObject.transform.localPosition.z);
            }
        }

        public void resetOriginalPosition()
        {
            m_OriginalPosition = gameObject.transform.localPosition;
        }

        public void HoldBall(GameObject ball)
        {
            m_HoldsBall = true;
            m_ball = ball;
        }

        public void LoseBall()
        {
            m_HoldsBall = false;
            m_ball = null;
        }

        public void SetDragable(bool b)
        {
            m_Dragable = b;
        }

       
        private void resetCurrentPosition()
        {
            transform.localPosition = m_OriginalPosition;
        }


        private void OnParticleCollision(GameObject other)
        {
            DeadZone.CheckResult();
        }

    }
}


