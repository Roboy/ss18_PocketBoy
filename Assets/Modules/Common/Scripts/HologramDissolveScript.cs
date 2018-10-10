using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Pocketboy/HologramDissolveScript")]
public class HologramDissolveScript : MonoBehaviour {

    [SerializeField, Tooltip("The mesh that is to be transformed in to a dissolvable hologram.")]
    private MeshFilter HoloMeshFilter;

    [SerializeField, HideInInspector]
    bool m_Initialized;

    public Material HologramMaterial { get; private set; }

    Vector3 m_DissolveStart;
    Vector3 m_DissolveEnd;

    float m_HighestVertex;
    float m_LowestVertex;

    private void Start()
    {
        if (m_Initialized)
            return;

        if (HoloMeshFilter == null)
        {
            HoloMeshFilter = GetComponent<MeshFilter>();
        }

        GetShaderValue();
        HologramMaterial = new Material(Shader.Find("Custom/HologramDissolveShader"));
        MeshRenderer renderer;
        if (HologramMaterial != null && (renderer = HoloMeshFilter.GetComponent<MeshRenderer>()) != null)
        {
            HologramMaterial.SetVector("_DissolveStart", m_DissolveStart);
            HologramMaterial.SetVector("_DissolveEnd", m_DissolveEnd);
            HologramMaterial.SetFloat("_DissolveValue", 0f);
            HologramMaterial.SetFloat("_HighestVertex", m_HighestVertex);
            HologramMaterial.SetFloat("_LowestVertex", m_LowestVertex);
            renderer.material = HologramMaterial;
            m_Initialized = true;
        }       
    }

    void GetShaderValue()
    {
        if (HoloMeshFilter == null)
            return;

        var vertices = HoloMeshFilter.sharedMesh.vertices;
        float min = 0f;
        float max = 0f;
        foreach (var vertex in vertices)
        {
            if (vertex.y > max)
            {
                max = vertex.y;
            }
            if (vertex.y < min)
            {
                min = vertex.y;
            }
        }
        m_DissolveStart = new Vector3(0f, min - 0.01f, 0f);
        m_DissolveEnd = new Vector3(0f, max + 0.1f, 0f);
        m_HighestVertex = max;
        m_LowestVertex = min;

        Debug.Log(m_HighestVertex);
        Debug.Log(m_LowestVertex);
    }
}
