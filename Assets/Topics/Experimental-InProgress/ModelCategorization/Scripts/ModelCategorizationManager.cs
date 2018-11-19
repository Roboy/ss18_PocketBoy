using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;
using TMPro;
using UnityEngine.EventSystems;

namespace Pocketboy.ModelCategorization
{
    public class ModelCategorizationManager : Singleton<ModelCategorizationManager>
    {
        [SerializeField]
        private CategorizationModel CategorizationModelPrefab;

        [SerializeField]
        private CategorizationModelListAsset ModelList;

        [SerializeField]
        private Transform PlatformParent;

        [SerializeField]
        private CategorizationPlatform RobotPlatform;

        [SerializeField]
        private CategorizationPlatform DebatablePlatform;

        [SerializeField]
        private CategorizationPlatform NonRobotPlatform;

        [SerializeField]
        private List<Transform> SpawnPositions = new List<Transform>();

        [Header("UI Stuff")]

        [SerializeField]
        private GameObject ObjectInformation;

        [SerializeField]
        private GameObject NameParent;

        [SerializeField]
        private TextMeshProUGUI NameText;

        [SerializeField]
        private GameObject ExplanationParent;

        [SerializeField]
        private TextMeshProUGUI ExplanationText;

        private float m_ObjectInformationDisplayTime = 5f;

        private GameObject m_CurrentInformationGO;

        private Coroutine m_CurrentShowRoutine;

        private void Start()
        {
            PositionPlatforms();
            SpawnModels();                
        }

        public void ShowObjectName(string objectName)
        {
            //ObjectInformation.gameObject.SetActive(true);
            //NameParent.SetActive(true);
            //NameText.text = objectName;
            //m_CurrentInformationGO = NameParent;

            if (m_CurrentShowRoutine != null)
                StopCoroutine(m_CurrentShowRoutine);
            m_CurrentShowRoutine = StartCoroutine(ShowInformation(NameParent, NameText, objectName));
        }

        public void ShowObjectExplanation(string explanation)
        {
            //ObjectInformation.gameObject.SetActive(true);
            //ExplanationParent.SetActive(true);
            //ExplanationText.text = explanation;
            //m_CurrentInformationGO = ExplanationParent;

            if (m_CurrentShowRoutine != null)
                StopCoroutine(m_CurrentShowRoutine);
            m_CurrentShowRoutine = StartCoroutine(ShowInformation(ExplanationParent, ExplanationText, explanation));
        }

        private void HideObjectInformation()
        {
            ObjectInformation.gameObject.SetActive(false);
            if (m_CurrentInformationGO != null)
                m_CurrentInformationGO.SetActive(false);
        }

        private IEnumerator ShowInformation(GameObject informationGO, TextMeshProUGUI informationText, string information)
        {
            ObjectInformation.gameObject.SetActive(true);
            informationGO.SetActive(true);
            informationText.text = information;
            yield return new WaitForSeconds(m_ObjectInformationDisplayTime);
            ObjectInformation.gameObject.SetActive(false);
            informationGO.SetActive(false);
        }

        private void PositionPlatforms()
        {
            LevelManager.Instance.RegisterGameObjectWithRoboy(PlatformParent.gameObject, new Vector3(0f, 0f, 0f));
            PlatformParent.transform.forward = -RoboyManager.Instance.transform.forward;
        }

        private void SpawnModels()
        {
            int robotPlatformCount = 0;
            int detabablePlatformCount = 0;
            int nonRobotPlatformCount = 0;
            int spawnPositionIndex = 0;
            foreach (var model in ModelList.CategorizationModels)
            {
                if (spawnPositionIndex >= SpawnPositions.Count)
                    break;

                var categorizationModel = Instantiate(CategorizationModelPrefab);
                LevelManager.Instance.RegisterGameObjectWithRoboy(categorizationModel.gameObject);
                categorizationModel.Setup(model.ModelPrefab, model.Name, model.Explanation, model.ContentRelatedState, SpawnPositions[spawnPositionIndex]);

                switch (model.ContentRelatedState)
                {
                    case ContentRelated.Yes:
                        robotPlatformCount++;
                        break;
                    case ContentRelated.Debatable:
                        detabablePlatformCount++;
                        break;
                    case ContentRelated.No:
                        nonRobotPlatformCount++;
                        break;
                }
                spawnPositionIndex++;
            }
            RobotPlatform.SetContentCount(robotPlatformCount);
            DebatablePlatform.SetContentCount(detabablePlatformCount);
            NonRobotPlatform.SetContentCount(nonRobotPlatformCount);
        }
    }
}


