using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pocketboy.Common;
using TMPro;

namespace Pocketboy.Cupgame
{
    public class ShuffleMaster : Singleton<ShuffleMaster>
    {

        public GameObject[] Cups;
        public GameObject RoboyArm;
        public Material Mat_Ball;

        [SerializeField]
        private Button m_ScanButton;

        [SerializeField]
        private Button m_ShowBallButton;

        [SerializeField]
        private Button m_ShuffleButton;

        [SerializeField]
        private TextMeshProUGUI m_ResultText;

        [SerializeField]
        private TMP_Dropdown m_Difficulty;

        private bool m_BallSpawned;
        private int m_BallIndex;
        private List<Button> m_Buttons = new List<Button>();
        private float m_Counter = 0.0f;
        private float m_HoverTime = 3.0f;
        private float m_HandOffset = 0.03f;
        private Color m_ButtonColourDefault;


        private bool m_CupsMoveable = false;
        public bool CupsMoveable
        {
            get
            {
                return m_CupsMoveable;
            }
            set
            {
                m_CupsMoveable = value;
                if (value == true)
                {
                    for (int i = 0; i < Cups.Length; i++)
                    {
                        Cups[i].GetComponent<DragMe>().SetDragable(true);
                    }
                }
                if (value == false)
                {
                    for (int i = 0; i < Cups.Length; i++)
                    {
                        Cups[i].GetComponent<DragMe>().SetDragable(false);
                    }
                }
            }
        }



        private void Start()
        {
            m_ScanButton.onClick.AddListener(Scan);
            m_ScanButton.enabled = true;
            m_Buttons.Add(m_ScanButton);

           
            m_ShowBallButton.onClick.AddListener(ShowBallPosition);
            m_ShowBallButton.enabled = true;
            m_Buttons.Add(m_ShowBallButton);


            m_ShuffleButton.onClick.AddListener(Shuffle);
            m_ShuffleButton.enabled = true;
            m_Buttons.Add(m_ShuffleButton);

            m_ButtonColourDefault = m_ScanButton.GetComponent<Image>().color;
            m_BallSpawned = false;

        }

        private void Scan()
        {
            if (m_ScanButton.enabled)
            {
                Vibration.CreateOneShot(500, 10);
                ToggleButtons("OFF");
                StartCoroutine(StartLocating());

            }
        }

        private void ShowBallPosition()
        {
            if (m_ShowBallButton.enabled)
            {
                Vibration.CreateOneShot(100, 255);
                ToggleButtons("OFF");
                StartCoroutine(RevealBall());
            }
        }

        private void Shuffle()
        {
            float speed = -1f;
            int amount = -1;

            switch (m_Difficulty.value)
            {
                case 0:
                    speed = 0.5f;
                    amount = 10;
                    break;
                case 1:
                    speed = 0.25f;
                    amount = 15;
                    break;
                case 2:
                    speed = 0.15f;
                    amount = 20;
                    break;
                default:
                    break;
            }
            
            if (m_ShuffleButton.enabled)
            {
                Vibration.CreateOneShot(100, 255);
                ToggleButtons("OFF");
                m_ResultText.enabled = false;
                Wager.Instance.resetCurrentPosition();
                StartCoroutine(ShuffleCups(amount, speed));

            }
        }

        public IEnumerator RevealBall()
        {

            CupsMoveable = false;
            if (!m_BallSpawned)
            {
                SpawnBall(UnityEngine.Random.Range(0, 3));
                m_BallSpawned = true;
            }
            yield return StartCoroutine(LiftAllCups());
            yield return StartCoroutine(LowerAllCups());


        }

        public IEnumerator StartLocating()
        {
            CupsMoveable = false;
            RoboyArm.transform.localPosition = new Vector3(Cups[0].transform.localPosition.x + m_HandOffset, RoboyArm.transform.localPosition.y, RoboyArm.transform.localPosition.z);
            RoboyArm.SetActive(true);
            RoboyArm.GetComponentInChildren<RadarSensor>().SensorActive = false;
            yield return StartCoroutine(LowerArm());
            RoboyArm.GetComponentInChildren<RadarSensor>().ToggleLight("ON");


            while (m_Counter < m_HoverTime)
            {
                m_Counter += Time.deltaTime;
                yield return null;
            }
            m_Counter = 0.0f;


            yield return StartCoroutine(MoveArm(1));


            while (m_Counter < m_HoverTime)
            {
                m_Counter += Time.deltaTime;
                yield return null;
            }
            m_Counter = 0.0f;


            yield return StartCoroutine(MoveArm(2));

            while (m_Counter < m_HoverTime)
            {
                m_Counter += Time.deltaTime;
                yield return null;
            }
            m_Counter = 0.0f;

            RoboyArm.GetComponentInChildren<RadarSensor>().ToggleLight("OFF");
            yield return StartCoroutine(RiseArm());
            yield return StartCoroutine(MoveArm(0));
            

            RoboyArm.GetComponentInChildren<RadarSensor>().SensorActive = false;
            RoboyArm.SetActive(false);
            ToggleButtons("ON");
            CupsMoveable = true;


        }

        public IEnumerator LiftAllCups()
        {
            int random = UnityEngine.Random.Range(0, 2);
            if (random == 0)
            {
                for (int i = 0; i < Cups.Length; i++)
                {
                    //lift each cup, starting from the left one until the ball is shown, then stop
                    yield return StartCoroutine(LiftCup(Cups[i]));
                }
            }
            if (random == 1)
            {
                for (int i = Cups.Length; i > 0; i--)
                {
                    //lift each cup, starting from the right one until the ball is shown, then stop
                    yield return StartCoroutine(LiftCup(Cups[i - 1]));
                }
            }


        }

        public IEnumerator LowerAllCups()
        {
            int random = UnityEngine.Random.Range(0, 2);
            if (random == 0)
            {
                for (int i = 0; i < Cups.Length; i++)
                {
                    //lift each cup, starting from the left one until the ball is shown, then stop
                    yield return StartCoroutine(LowerCup(Cups[i]));
                }
            }
            if (random == 1)
            {
                for (int i = Cups.Length; i > 0; i--)
                {
                    //lift each cup, starting from the right one until the ball is shown, then stop
                    yield return StartCoroutine(LowerCup(Cups[i - 1]));
                }
            }

            CupsMoveable = true;
            ToggleButtons("ON");
            

        }

        public IEnumerator LiftCup(GameObject cup)
        {
            float offset = cup.GetComponent<Renderer>().bounds.size.y;
            offset += (0.1f * offset);
            float startingHeight = cup.transform.localPosition.y;
            float endHeight = startingHeight + offset;
            float currentHeight = startingHeight;
            float currentDuration = 0.0f;
            float duration = 0.5f;
            while (currentDuration < duration)
            {
                currentHeight = Mathf.Lerp(startingHeight, endHeight, currentDuration / duration);
                cup.transform.localPosition = new Vector3(cup.transform.localPosition.x, currentHeight, cup.transform.localPosition.z);
                currentDuration += Time.deltaTime;
                yield return null;
            }
            cup.transform.localPosition = new Vector3(cup.transform.localPosition.x, endHeight, cup.transform.localPosition.z);
            yield return null;

        }

        public IEnumerator LowerCup(GameObject cup)
        {
            float offset = cup.GetComponent<Renderer>().bounds.size.y;
            offset += (0.1f * offset);
            float startingHeight = cup.transform.localPosition.y;
            float endHeight = startingHeight - offset;
            float currentHeight = startingHeight;
            float currentDuration = 0.0f;
            float duration = 0.5f;
            while (currentDuration < duration)
            {
                currentHeight = Mathf.Lerp(startingHeight, endHeight, currentDuration / duration);
                cup.transform.localPosition = new Vector3(cup.transform.localPosition.x, currentHeight, cup.transform.localPosition.z);
                currentDuration += Time.deltaTime;
                yield return null;
            }
            cup.transform.localPosition = new Vector3(cup.transform.localPosition.x, endHeight, cup.transform.localPosition.z);
            yield return null;

        }

        private void SpawnBall(int cup_index)
        {
            var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ball.GetComponent<Renderer>().material = Mat_Ball;
            Vector3 pos = Cups[cup_index].transform.position;
            Vector3 scale = Cups[cup_index].transform.localScale;
            ball.transform.localScale = Vector3.one * 0.083f;
            ball.AddComponent<BoxCollider>();
            pos.y += Mathf.Abs(ball.transform.GetComponent<BoxCollider>().bounds.size.y / 2.0f);
            Destroy(ball.GetComponent<BoxCollider>());
            ball.transform.position = pos;
            ball.transform.parent = transform;
            ball.name = "Ball";
            ball.layer = LayerMask.NameToLayer("Ignore");
            Cups[cup_index].GetComponent<DragMe>().HoldBall(ball);
            Cups[cup_index].GetComponentInChildren<BouncingFLoor>().iHaveBall = true;
        }

        private IEnumerator ShowBall()
        {

            for (int i = 0; i < Cups.Length; i++)
            {
                if (Cups[i].GetComponent<DragMe>().HoldsBall == true)
                {
                    yield return StartCoroutine(LiftCup(Cups[i]));
                    yield return StartCoroutine(LowerCup(Cups[i]));
                }
            }
            //m_ball_spawned = false;
        }

        private void RemoveBall()
        {

            for (int i = 0; i < Cups.Length; i++)
            {
                Cups[i].GetComponent<DragMe>().LoseBall();
                Cups[i].GetComponentInChildren<BouncingFLoor>().iHaveBall = false;
            }
            m_BallSpawned = false;
        }



        private IEnumerator LowerArm()
        {

            float startingAngleX = 0.0f;
            float currentAngleX = 0.0f;
            float endAngleX = -90.0f;
            float currentDuration = 0.0f;
            float duration = 1.0f;

            while (currentDuration < duration)
            {
                currentAngleX = Mathf.Lerp(startingAngleX, endAngleX, currentDuration / duration);
                RoboyArm.transform.eulerAngles = new Vector3(currentAngleX, RoboyArm.transform.eulerAngles.y, RoboyArm.transform.eulerAngles.z);
                currentDuration += Time.deltaTime;
                yield return null;
            }


            RoboyArm.transform.eulerAngles = new Vector3(endAngleX, RoboyArm.transform.eulerAngles.y, RoboyArm.transform.eulerAngles.z);
            RoboyArm.GetComponentInChildren<RadarSensor>().SensorActive = true;
            yield return null;
        }

        private IEnumerator RiseArm()
        {

            float startingAngleX = -90.0f;
            float currentAngleX = 0.0f;
            float endAngleX = 0.0f;
            float currentDuration = 0.0f;
            float duration = 1.0f;
            //No active sensing when putting the arm up
            RoboyArm.GetComponentInChildren<RadarSensor>().SensorActive = false;

            while (currentDuration < duration)
            {
                currentAngleX = Mathf.Lerp(startingAngleX, endAngleX, currentDuration / duration);
                RoboyArm.transform.eulerAngles = new Vector3(currentAngleX, RoboyArm.transform.eulerAngles.y, RoboyArm.transform.eulerAngles.z);
                currentDuration += Time.deltaTime;
                yield return null;
            }


            RoboyArm.transform.eulerAngles = new Vector3(endAngleX, RoboyArm.transform.eulerAngles.y, RoboyArm.transform.eulerAngles.z);

            yield return null;
        }

        private IEnumerator MoveArm(int index_cup)
        {
            float startingPosX = RoboyArm.transform.localPosition.x;
            float currentPosX = RoboyArm.transform.localPosition.x; ;
            float endPosX = Cups[index_cup].transform.localPosition.x + m_HandOffset;
            float currentDuration = 0.0f;
            float duration = 1.0f;

            //No wave spawning while moving.
            RoboyArm.GetComponentInChildren<RadarSensor>().SensorActive = false;

            if (startingPosX == endPosX)
                yield return null;

            while (currentDuration < duration)
            {
                currentPosX = Mathf.Lerp(startingPosX, endPosX, currentDuration / duration);
                RoboyArm.transform.localPosition = new Vector3(currentPosX, RoboyArm.transform.localPosition.y, RoboyArm.transform.localPosition.z);
                currentDuration += Time.deltaTime;
                yield return null;
            }
            RoboyArm.transform.localPosition = new Vector3(endPosX, RoboyArm.transform.localPosition.y, RoboyArm.transform.localPosition.z);
            RoboyArm.GetComponentInChildren<RadarSensor>().SensorActive = true;

            yield return null;
        }

        private IEnumerator ShuffleRandom(float speed)
        {
            int ignore = UnityEngine.Random.Range(0, 3);
            GameObject Cup01 = null;
            GameObject Cup02 = null;
            float counter = 0.0f;

            for (int i = 0; i < Cups.Length; i++)
            {
                if (i == ignore)
                {
                    continue;
                }

                if (Cup01 == null)
                {
                    Cup01 = Cups[i];
                    continue;
                }

                if (Cup02 == null)
                {
                    Cup02 = Cups[i];
                    break;
                }
            }

            Vector3 Pos01 = Cup01.transform.localPosition;
            Vector3 Pos02 = Cup02.transform.localPosition;

            int decider = UnityEngine.Random.Range(0, 2);

            if (decider == 0)
            {
                StartCoroutine(Cup01.GetComponent<DragMe>().MoveCupCircular(Pos02, speed, DragMe.RotationDirection.right, DragMe.RotationLane.back));
                StartCoroutine(Cup02.GetComponent<DragMe>().MoveCupCircular(Pos01, speed, DragMe.RotationDirection.left, DragMe.RotationLane.front));
            }
            if (decider == 1)
            {
                StartCoroutine(Cup01.GetComponent<DragMe>().MoveCupCircular(Pos02, speed, DragMe.RotationDirection.left, DragMe.RotationLane.back));
                StartCoroutine(Cup02.GetComponent<DragMe>().MoveCupCircular(Pos01, speed, DragMe.RotationDirection.right, DragMe.RotationLane.front));
            }

            

            while (counter < speed)
            {
                counter += Time.deltaTime;
                yield return null;
            }

            int index_01 = Array.IndexOf(Cups, Cup01);
            int index_02 = Array.IndexOf(Cups, Cup02);

            Cups[index_01] = Cup02;
            Cups[index_02] = Cup01;
            Cup01.GetComponent<DragMe>().resetOriginalPosition();
            Cup02.GetComponent<DragMe>().resetOriginalPosition();

            yield return null;
        }

        private IEnumerator ShuffleCups(int amount, float speed)
        {
            for (int i = 0; i < amount; i++)
            {

                yield return StartCoroutine(ShuffleRandom(speed));

            }
            ToggleButtons("ON");
            yield return null;
        }

        private void ToggleButtons(string operation)
        {
            if (m_Buttons.Count == 0)
                return;

            if (operation == "ON")
            {
                foreach (Button bt in m_Buttons)
                {
                    bt.enabled = true;
                    bt.GetComponent<Image>().color = m_ButtonColourDefault;
                }
               
            }
            if (operation == "OFF")
            {
                foreach (Button bt in m_Buttons)
                {
                    bt.enabled = false;
                    bt.GetComponent<Image>().color = Color.grey;
                }

            }



        }

    }
}
