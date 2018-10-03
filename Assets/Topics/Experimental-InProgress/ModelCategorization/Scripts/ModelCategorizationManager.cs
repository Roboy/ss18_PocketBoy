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
            PlatformParent.transform.SetParent(LevelManager.Instance.Roboy.ARAnchor.transform);
            PlatformParent.transform.position = LevelManager.Instance.Roboy.transform.TransformPoint(new Vector3(0, 0f, 0.1f));

            //RobotPlatform.transform.SetParent(LevelManager.Instance.Roboy.ARAnchor.transform);
            //RobotPlatform.transform.position = LevelManager.Instance.Roboy.transform.TransformPoint(new Vector3(-0.3f, 0f, 0.2f));
            
            //DebatablePlatform.transform.SetParent(LevelManager.Instance.Roboy.ARAnchor.transform);
            //DebatablePlatform.transform.position = LevelManager.Instance.Roboy.transform.TransformPoint(new Vector3(0f, 0f, 0.5f));

            //NonRobotPlatform.transform.SetParent(LevelManager.Instance.Roboy.ARAnchor.transform);
            //NonRobotPlatform.transform.position = LevelManager.Instance.Roboy.transform.TransformPoint(new Vector3(0.3f, 0f, 0.2f));

        }

        private void SpawnModels()
        {
            int robotPlatformCount = 0;
            int detabablePlatformCount = 0;
            int nonRobotPlatformCount = 0;
            foreach (var model in ModelList.CategorizationModels)
            {
                var categorizationModel = Instantiate(CategorizationModelPrefab, LevelManager.Instance.Roboy.ARAnchor.transform);
                categorizationModel.transform.position = LevelManager.Instance.Roboy.transform.TransformPoint(new Vector3(0f, 0f, 0.35f));
                categorizationModel.Setup(model.ModelPrefab, model.Name, model.Explanation, model.ContentRelatedState);

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
            }
            RobotPlatform.SetContentCount(robotPlatformCount);
            DebatablePlatform.SetContentCount(detabablePlatformCount);
            NonRobotPlatform.SetContentCount(nonRobotPlatformCount);
        }
    }
}


