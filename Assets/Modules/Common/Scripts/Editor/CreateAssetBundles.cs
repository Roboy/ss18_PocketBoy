using UnityEditor;
using System.IO;

namespace Pocketboy.Utilities
{
    public class CreateAssetBundles
    {
        [MenuItem("Assets/Build AssetBundles")]
        static void BuildAllAssetBundles()
        {
            string assetBundleDirectory = "Assets/StreamingAssets";
            Directory.CreateDirectory(assetBundleDirectory);
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.Android);
        }
    }
}