using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.ModelCategorization
{
    [CreateAssetMenu(fileName = "CategorizationModelListAsset", menuName = "ModelCategorization/CategorizationModelListAsset", order = 1)]
    public class CategorizationModelListAsset : ScriptableObject
    {
        public List<CategorizationModelAsset> CategorizationModels = new List<CategorizationModelAsset>();
    }
}


