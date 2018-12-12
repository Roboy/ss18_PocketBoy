using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.ModelCategorization
{
    [CreateAssetMenu(fileName = "CategorizationModelAsset", menuName = "Pocketboy/ModelCategorization/CategorizationModelAsset")]
    public class CategorizationModelAsset : ScriptableObject
    {
        public GameObject ModelPrefab;

        public string Name;

        public string Explanation;

        public ContentRelated ContentRelatedState = ContentRelated.No;

    }
}

