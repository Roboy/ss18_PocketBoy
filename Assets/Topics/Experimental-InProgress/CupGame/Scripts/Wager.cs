using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Pocketboy.Common;
using TMPro;

namespace Pocketboy.Cupgame
{
    public class Wager : Singleton<Wager>, IPointerUpHandler, IPointerDownHandler
    {
        private bool m_IsTouched = false;
        private bool m_Dragable = true;
        public List<BoxCollider> BetZones;
        public TextMeshProUGUI ResultText;
        

        [SerializeField]
        private GameObject ZoneToBetOn = null;

        private Vector2 m_TouchPositionScreenSpace;
        private Vector3 m_TouchPositionWorldSpace;

        private Vector3 m_OriginalPosition { get; set; }

        private bool m_Lifted;
        private bool m_HoldsBall;

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

        // Use this for initialization
        void Start() {
            m_OriginalPosition = transform.localPosition;
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

        private void StartDrag(PointerEventData eventData)
        {
            m_IsTouched = true;
            ToggleBetZones("ON");
            ToggleLight("ON");

            m_DistanceToCameraOnTouch = (Camera.main.transform.position - transform.position).magnitude;
            m_OffsetOnTouch = (Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, m_DistanceToCameraOnTouch)) - transform.localPosition);

        }

        private void UpdateDrag()
        {
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

            }
        }


        private void StopDrag()
        {
            m_IsTouched = false;
            m_DistanceToCameraOnTouch = 0f;
            m_OffsetOnTouch = Vector3.zero;
            ToggleBetZones("OFF");
            ToggleLight("OFF");

           
            if (ZoneToBetOn !=null)
            {
                //Place coins at the resprective cup position
                transform.position = ZoneToBetOn.transform.position;
                StartCoroutine(checkForWin());
                return;
            }

            if (m_OffGameBoard)
            {
                resetCurrentPosition();
                m_OffGameBoard = false;
                return;
            }
            //resetCurrentPosition();




        }

     

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "BetZone")
            {
                ZoneToBetOn = other.gameObject;
                
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "BetZone")
            {
                ZoneToBetOn = other.gameObject;
                
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "BetZone")
            {
                ZoneToBetOn = null;
                
            }
            
            if (other.tag == "GameBoard")
            {
                m_OffGameBoard = true;
            }
        }

        public void ToggleBetZones(string operation)
        {
            foreach (BoxCollider bc in BetZones)
            {
                if (operation == "ON") { bc.enabled = true; }
                if (operation == "OFF") { bc.enabled = false; }
                
            }
        }

        public void ToggleLight(string operation)
        {
            foreach (BoxCollider bc in BetZones)
            {
                Light li = bc.gameObject.GetComponent<Light>();
                if (operation == "ON") {li.enabled = true; }
                if (operation == "OFF") { li.enabled = false; }

            }
        }

        public void resetCurrentPosition()
        {
            transform.localPosition = m_OriginalPosition;
        }

        public void resetOriginPosition()
        {
            m_OriginalPosition = transform.localPosition;
        }

        public IEnumerator checkForWin()
        {
            //Show the text on GUI
            ResultText.text = "checking";
            ResultText.enabled = true;

            if (ZoneToBetOn == null)
            {
                
                ResultText.text = "You Lose!";
                yield return null;
            }

            StartCoroutine(ShuffleMaster.Instance.RevealBall());
            float counter = 0.0f;
            while (counter < 1.5f)
            {
                counter += Time.deltaTime;
                yield return null;
            }

            GameObject Cup = ZoneToBetOn.gameObject.transform.parent.gameObject;
            if (Cup.GetComponent<DragMe>().HoldsBall)
            {
                ResultText.text = "You Win!";
            }
            else
            {
                ResultText.text = "You Lose!";
            }
        }


    }
}