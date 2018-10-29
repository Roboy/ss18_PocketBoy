using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;

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

        private void Start()
        {
            PositionPlatforms();
            SpawnModels();                
        }

        public void ShowObjectInformation(string name, string explanation)
        {

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


