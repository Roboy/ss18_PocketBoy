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
            if (LevelManager.Instance.Roboy != null)
            {
                PositionPlatforms();
                SpawnModels();                
            }
        }

        public void ShowObjectInformation(string name, string explanation)
        {

        }

        private void PositionPlatforms()
        {
            PlatformParent.transform.SetParent(LevelManager.Instance.GetAnchorTransform());
            PlatformParent.transform.position = LevelManager.Instance.Roboy.transform.TransformPoint(new Vector3(0.6f, 0f, 0f));       
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

                var categorizationModel = Instantiate(CategorizationModelPrefab, LevelManager.Instance.Roboy.ARAnchor.transform);
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


