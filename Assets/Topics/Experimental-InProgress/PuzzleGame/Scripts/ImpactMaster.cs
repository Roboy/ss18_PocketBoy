using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;

namespace Pocketboy.PuzzleGame
{

    public class ImpactMaster : Singleton<ImpactMaster>
    {

        public List<GameObject> SimulatedObjects = new List<GameObject>();
        public float Force, Radius, Upwards;
        public GameObject RoboyToExplode;

        private Dictionary<string, Pose> m_InitialPositions;
        private Collider[] m_Colliders;
        private Vector3 m_ExplosionPosition;

        // Use this for initialization
        void Start()
        {

            m_InitialPositions = new Dictionary<string, Pose>();
            m_ExplosionPosition = RoboyToExplode.transform.position;

            foreach (PuzzlePart pp in RoboyToExplode.GetComponentsInChildren<PuzzlePart>())
            {
                SimulatedObjects.Add(pp.gameObject);
            }

            foreach (GameObject go in SimulatedObjects)
            {
                m_InitialPositions.Add(go.name, new Pose(go.transform.position, go.transform.rotation));
                go.GetComponent<PuzzlePart>().setInitRotation();
            }
        }

        // Update is called once per frame
        void Update()
        {

            if (Input.GetKeyDown(KeyCode.D))
            {
                StartCoroutine(Explode());
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Reset();
            }

        }

        public IEnumerator  Explode()
        {
            int layermask = LayerMask.GetMask("PuzzlePieces");

            m_Colliders = Physics.OverlapSphere(Vector3.zero, Radius, layermask);

            foreach (Collider co in m_Colliders)
            {
                Rigidbody rb = co.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = true;
                    rb.isKinematic = false;
                    rb.AddExplosionForce(Force, m_ExplosionPosition, Radius, Upwards, ForceMode.Impulse);
                }

            }

            float counter = 0.0f;
            float duration = 5.0f;

            while (counter < duration)
            {
                counter += Time.deltaTime;
                yield return null;
            }

            foreach (Collider co in m_Colliders)
            {
                Rigidbody rb = co.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = false;
                    rb.isKinematic = true;
                    PuzzlePart pp = rb.gameObject.GetComponent<PuzzlePart>();
                    if (pp != null)
                    {
                        pp.Initialize();
                        pp.SetMoveability(true);
                    }
                    
                }

            }

            PuzzleMaster.Instance.EnableRoboyTarget();
            yield return null;
        }
        public void Reset()
        {
            foreach (GameObject simulatedPiece in SimulatedObjects)
            {
                simulatedPiece.transform.position = m_InitialPositions[simulatedPiece.name].position;
                simulatedPiece.transform.rotation = m_InitialPositions[simulatedPiece.name].rotation;
            }
        }
    }
}