using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pocketboy.Common;

namespace Pocketboy.Cupgame
{
    public class ShuffleMaster : Singleton<ShuffleMaster>
    {

        public GameObject[] Cups;
        public GameObject RoboyArm;
        public Material mat_Ball;

        [SerializeField]
        private Button ScanButton;

        [SerializeField]
        private Button ShowBallButton;

        [SerializeField]
        private Button ShuffleButton;

        private bool m_ball_spawned;
        private int m_ball_index;

        private float m_counter = 0.0f;
        private float m_hoverTime = 3.0f;
        private float m_handOffset = 0.03f;
        private Color m_buttonColourDefault;


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
            ScanButton.onClick.AddListener(Scan);
            ScanButton.enabled = true;
            m_buttonColourDefault = ScanButton.GetComponent<Image>().color;
            ShowBallButton.onClick.AddListener(ShowBallPosition);
            ShowBallButton.enabled = true;
            ShuffleButton.onClick.AddListener(Shuffle);
            ShuffleButton.enabled = true;
            m_ball_spawned = false;

        }

        private void Scan()
        {
            if (ScanButton.enabled)
            {
                Vibration.CreateOneShot(500, 10);
                ScanButton.enabled = false;
                ScanButton.GetComponent<Image>().color = Color.gray;
                StartCoroutine(StartLocating());

            }
        }

        private void ShowBallPosition()
        {
            if (ShowBallButton.enabled)
            {
                Vibration.CreateOneShot(100, 255);
                ShowBallButton.enabled = false;
                ShowBallButton.GetComponent<Image>().color = Color.gray;
                StartCoroutine(StartGame());
            }
        }

        private void Shuffle()
        {
            float speed = 0.25f;
            int amount = 20;
            if (ShuffleButton.enabled)
            {
                Vibration.CreateOneShot(100, 255);
                ShuffleButton.enabled = false;
                ShuffleButton.GetComponent<Image>().color = Color.gray;

                StartCoroutine(ShuffleCups(amount, speed));

            }
        }

        public IEnumerator StartGame()
        {

            CupsMoveable = false;
            if (!m_ball_spawned)
            {
                SpawnBall(UnityEngine.Random.Range(0, 3));
                m_ball_spawned = true;
            }
            yield return StartCoroutine(LiftAllCups());
            yield return StartCoroutine(LowerAllCups());


        }

        public IEnumerator StartLocating()
        {
            CupsMoveable = false;
            RoboyArm.transform.localPosition = new Vector3(Cups[0].transform.localPosition.x + m_handOffset, RoboyArm.transform.localPosition.y, RoboyArm.transform.localPosition.z);
            RoboyArm.SetActive(true);
            RoboyArm.GetComponentInChildren<RadarSensor>().SensorActive = false;
            yield return StartCoroutine(LowerArm());
            RoboyArm.GetComponentInChildren<RadarSensor>().ToggleLight("ON");


            while (m_counter < m_hoverTime)
            {
                m_counter += Time.deltaTime;
                yield return null;
            }
            m_counter = 0.0f;


            yield return StartCoroutine(MoveArm(1));


            while (m_counter < m_hoverTime)
            {
                m_counter += Time.deltaTime;
                yield return null;
            }
            m_counter = 0.0f;


            yield return StartCoroutine(MoveArm(2));

            while (m_counter < m_hoverTime)
            {
                m_counter += Time.deltaTime;
                yield return null;
            }
            m_counter = 0.0f;

            RoboyArm.GetComponentInChildren<RadarSensor>().ToggleLight("OFF");
            yield return StartCoroutine(RiseArm());
            yield return StartCoroutine(MoveArm(0));

            RoboyArm.GetComponentInChildren<RadarSensor>().SensorActive = false;
            ScanButton.enabled = true;
            ScanButton.GetComponent<Image>().color = m_buttonColourDefault;
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
            ShowBallButton.enabled = true;
            ShowBallButton.GetComponent<Image>().color = m_buttonColourDefault;
            Wager.Instance.checkForWin();

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
            ball.GetComponent<Renderer>().material = mat_Ball;
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
            m_ball_spawned = false;
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
            float endPosX = Cups[index_cup].transform.localPosition.x + m_handOffset;
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

            ShuffleButton.enabled = true;
            ShuffleButton.GetComponent<Image>().color = m_buttonColourDefault;
            yield return null;
        }

        private IEnumerator ShuffleCups(int amount, float speed)
        {
            for (int i = 0; i < amount; i++)
            {

                yield return StartCoroutine(ShuffleRandom(speed));

            }

            yield return null;
        }

    }
}
