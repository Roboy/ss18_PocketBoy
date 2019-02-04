using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    public class VertexCount : MonoBehaviour
    {
        public int GetVertexCount()
        {
            var allMeshes = GetComponentsInChildren<MeshFilter>();
            int vertexCount = 0;
            foreach (var mesh in allMeshes)
            {
                vertexCount += mesh.sharedMesh.vertexCount;
            }
            return vertexCount;
        }
    }
}

