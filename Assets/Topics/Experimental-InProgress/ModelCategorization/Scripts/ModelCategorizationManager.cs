using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;

namespace Pocketboy.ModelCategorization
{
    public class ModelCategorizationManager : Singleton<ModelCategorizationManager>
    {
        [SerializeField]
        private List<CategorizationModel> Models = new List<CategorizationModel>();

        [SerializeField]
        private List<CategorizationPlatform> Platforms = new List<CategorizationPlatform>();

        private void Start()
        {
            if (LevelManager.Instance.Roboy != null)
            {
                foreach (var model in Models)
                {
                    var categorizationModel = Instantiate(model, LevelManager.Instance.Roboy.ARAnchor.transform);
                    categorizationModel.transform.localPosition = new Vector3(-0.3f, 0f, 0f);
                }

                foreach (var platform in Platforms)
                {
                    var categorizationPlatform = Instantiate(platform, LevelManager.Instance.Roboy.ARAnchor.transform);
                    categorizationPlatform.transform.localPosition = new Vector3(0.3f, 0f, 0f);
                }
            }
        }
    }
}


