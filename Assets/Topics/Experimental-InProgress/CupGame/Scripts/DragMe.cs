using System;
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

        private bool m_IsTouched;

        private Vector2 m_TouchPositionScreenSpace;

        private Vector3 m_TouchPositionWorldSpace;

        private Rigidbody m_RigidBody;

        private bool m_KinematicState;

        private Vector3 m_OriginalPosition { get; set; }
        private bool m_Lifted;


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
            m_Lifted = false;
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

            StartCoroutine(SwapCups());


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
            m_TouchPositionWorldSpace = m_TouchPositionScreenSpace;
            m_TouchPositionWorldSpace.z = m_DistanceToCameraOnTouch; // transform touch position from 2d to 3d on the plane where the first touch occured.
            Vector3 NextPosition = Camera.main.ScreenToWorldPoint(m_TouchPositionWorldSpace) - m_OffsetOnTouch;
            transform.position = new Vector3(NextPosition.x, transform.position.y, transform.position.z);
        }

        private IEnumerator CheckForElevation()
        {


            gameObject.layer = LayerMask.NameToLayer("Ignore");
            int layerMask = ~(1 << LayerMask.NameToLayer("Ignore"));

            //Collision happen.
            if (Physics.OverlapBox(transform.position, Vector3.one, Quaternion.identity, layerMask).Length > 0)
            {

                //If colliding and not elevated, increase the height, so that the cups stack
                if (transform.position.y == m_OriginalPosition.y)
                {
                    yield return StartCoroutine(ElevateCup(0, gameObject));
                }
                else
                {
                    //keep the current y value, do not change
                }
            }

            //No collisions occur.
            if (Physics.OverlapBox(transform.position, Vector3.one, Quaternion.identity, layerMask).Length == 0)
            {

                if (m_Lifted)
                {
                    //If the cup was lifted up, it has to come down
                    yield return StartCoroutine(ElevateCup(1, gameObject));
                }

            }

            gameObject.layer = LayerMask.NameToLayer("Default");
            yield return null;
        }

        private IEnumerator ElevateCup(int mode, GameObject cup)
        {

            float startingHeight = cup.transform.position.y;
            Vector3 currentPos = cup.transform.position;
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
                currentPos = cup.transform.position;
                currentPos.y = Mathf.Lerp(startingHeight, endHeight, currentDuration / duration);
                cup.transform.position = currentPos;
                currentDuration += Time.deltaTime;
                yield return null;
            }

            //Eliminate accuracy error.
            cup.transform.position = new Vector3(cup.transform.position.x, endHeight, cup.transform.position.z);
            yield return null;
        }


        private IEnumerator SwapCups()
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore");
            int layerMask = ~(1 << LayerMask.NameToLayer("Ignore"));
            Collider[] hitted_Objects = Physics.OverlapBox(transform.position, Vector3.one, Quaternion.identity, layerMask);

            if (hitted_Objects.Length == 0)
            {
                //no swapping needed.
            }
            if (hitted_Objects.Length > 0)
            {
                RotationDirection dir = RotationDirection.none;
                RotationLane lan = RotationLane.none;
                //swapping needed
                if (m_Lifted)
                {

                    int index_a = Array.IndexOf(ShuffleMaster.Instance.Cups, gameObject);
                    int index_b = Array.IndexOf(ShuffleMaster.Instance.Cups, hitted_Objects[0].gameObject);
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

                    }

                    //determine offsets
                    //code here


                }
            }
            m_Lifted = false;
            gameObject.layer = LayerMask.NameToLayer("Default");

            yield return null;
        }

        public IEnumerator MoveCupLinear(Vector3 destination, float duration)
        {
            float currentDuration = 0.0f;
            Vector3 start = gameObject.transform.position;

            while (currentDuration < duration)
            {
                gameObject.transform.position = Vector3.Lerp(start, destination, currentDuration / duration);
                currentDuration += Time.deltaTime;
                yield return null;
            }

            gameObject.transform.position = destination;



        }

        public IEnumerator MoveCupCircular(Vector3 destination, float duration, RotationDirection dir, RotationLane lan)
        {
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

            float distance = Vector3.Distance(ShuffleMaster.Instance.Cups[1].transform.position, transform.position);
            Vector3 start = gameObject.transform.position;
            Vector3 pivotPoint = ShuffleMaster.Instance.Cups[1].transform.position;


            while (currentDuration < duration)
            {
                currentAngle = Mathf.Lerp(startingAngle, resultingAngle, currentDuration / duration);
                Vector3 nextPos = gameObject.transform.position;
                nextPos.x = pivotPoint.x + Mathf.Sin(currentAngle) * distance;
                if (lan == RotationLane.front)
                {
                    nextPos.z = pivotPoint.x + Mathf.Cos(currentAngle) * distance;
                }
                if (lan == RotationLane.back)
                {
                    nextPos.z = pivotPoint.x - Mathf.Cos(currentAngle) * distance;
                }
                gameObject.transform.position = nextPos;
                currentDuration += Time.deltaTime;
                yield return null;
            }

            gameObject.transform.position = destination;


        }

        public void resetOriginalPosition()
        {
            m_OriginalPosition = gameObject.transform.position;
        }
    }
}


