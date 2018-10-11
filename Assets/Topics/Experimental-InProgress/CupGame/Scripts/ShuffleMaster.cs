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

        [SerializeField]
        private Button StartButton;
        private bool m_ball_spawned;
        private int m_ball_index;

        private float counter = 0.0f;
        private float hoverTime = 1.0f;

        private bool m_CupsMoveable;
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
            StartButton.onClick.AddListener(HandleStart);
            StartButton.enabled = true;
            m_ball_spawned = false;
            RoboyArm.SetActive(false);
        }

        private void HandleStart()
        {
            if (StartButton.enabled)
            {
                StartButton.enabled = false;
                StartButton.GetComponent<Image>().color = Color.red;
                StartCoroutine(StartGame());
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                StartCoroutine(ShowBall());
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                StartCoroutine(StartLocating());
            }

        }

        public IEnumerator StartGame()
        {

            CupsMoveable = false;
            if (!m_ball_spawned)
            {
                SpawnBall(Random.Range(0, 3));
                m_ball_spawned = true;
            }
            yield return StartCoroutine(LiftAllCups());
            yield return StartCoroutine(LowerAllCups());


        }

        public IEnumerator StartLocating()
        {
            RoboyArm.transform.position = new Vector3(Cups[0].transform.position.x, RoboyArm.transform.position.y, RoboyArm.transform.position.z);
            RoboyArm.SetActive(true);
            yield return StartCoroutine(LowerArm());
            RoboyArm.GetComponentInChildren<RadarSensor>().SensorActive = true;

            

            while (counter < hoverTime)
            {
                counter += Time.deltaTime;
                yield return null;
            }

            counter = 0.0f;
            yield return StartCoroutine(MoveArm(1));

            RoboyArm.GetComponentInChildren<RadarSensor>().SensorActive = true;
            while (counter < hoverTime)
            {
                counter += Time.deltaTime;
                yield return null;
            }
            counter = 0.0f;
            yield return StartCoroutine(MoveArm(2));
            RoboyArm.GetComponentInChildren<RadarSensor>().SensorActive = true;

        }

        public IEnumerator LiftAllCups()
        {
            int random = Random.Range(0, 2);
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
            int random = Random.Range(0, 2);
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
            StartButton.enabled = true;
            StartButton.GetComponent<Image>().color = Color.green;

        }

        public IEnumerator LiftCup(GameObject cup)
        {
            float offset = cup.GetComponent<Renderer>().bounds.size.y;
            offset += (0.1f * offset);
            float startingHeight = cup.transform.position.y;
            float endHeight = startingHeight + offset;
            float currentHeight = startingHeight;
            float currentDuration = 0.0f;
            float duration = 0.5f;
            while (currentDuration < duration)
            {
                currentHeight = Mathf.Lerp(startingHeight, endHeight, currentDuration / duration);
                cup.transform.position = new Vector3(cup.transform.position.x, currentHeight, cup.transform.position.z);
                currentDuration += Time.deltaTime;
                yield return null;
            }
            cup.transform.position = new Vector3(cup.transform.position.x, endHeight, cup.transform.position.z);
            yield return null;

        }

        public IEnumerator LowerCup(GameObject cup)
        {
            float offset = cup.GetComponent<Renderer>().bounds.size.y;
            offset += (0.1f * offset);
            float startingHeight = cup.transform.position.y;
            float endHeight = startingHeight - offset;
            float currentHeight = startingHeight;
            float currentDuration = 0.0f;
            float duration = 0.5f;
            while (currentDuration < duration)
            {
                currentHeight = Mathf.Lerp(startingHeight, endHeight, currentDuration / duration);
                cup.transform.position = new Vector3(cup.transform.position.x, currentHeight, cup.transform.position.z);
                currentDuration += Time.deltaTime;
                yield return null;
            }
            cup.transform.position = new Vector3(cup.transform.position.x, endHeight, cup.transform.position.z);
            yield return null;

        }

        private void SpawnBall(int cup_index)
        {
            var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ball.GetComponent<Renderer>().material.color = Random.ColorHSV();
            Vector3 pos = Cups[cup_index].transform.position;
            pos.y += Mathf.Abs(pos.y * 0.2f);
            ball.transform.position = pos;
            ball.layer = LayerMask.NameToLayer("Ignore");
            Cups[cup_index].GetComponent<DragMe>().HoldBall(ball);
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
            m_ball_spawned = false;
        }

        private void RemoveBall()
        {

            for (int i = 0; i < Cups.Length; i++)
            {
                Cups[i].GetComponent<DragMe>().LoseBall();
            }
            m_ball_spawned = false;
        }

        private IEnumerator LiftArm()
        {
            return null;
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

            yield return null;
        }

        private IEnumerator MoveArm(int index_cup)
        {
            float startingPosX = RoboyArm.transform.position.x;
            float currentPosX = RoboyArm.transform.position.x; ;
            float endPosX = Cups[index_cup].transform.position.x;
            float currentDuration = 0.0f;
            float duration = 1.0f;

            //No wave spawning while moving.
            RoboyArm.GetComponentInChildren<RadarSensor>().SensorActive = false;

            if (startingPosX == endPosX)
                yield return null;

            while (currentDuration < duration)
            {
                currentPosX = Mathf.Lerp(startingPosX, endPosX, currentDuration / duration);
                RoboyArm.transform.position = new Vector3(currentPosX, RoboyArm.transform.position.y, RoboyArm.transform.position.z);
                currentDuration += Time.deltaTime;
                yield return null;
            }
            RoboyArm.transform.position = new Vector3(endPosX, RoboyArm.transform.position.y, RoboyArm.transform.position.z);

            yield return null;
        }
    }
}
