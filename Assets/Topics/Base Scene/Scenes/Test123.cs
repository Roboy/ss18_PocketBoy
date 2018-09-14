using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test123 : MonoBehaviour {

    BoxCollider boxCol;

    private void Start()
    {
        //var boxcollider = gameObject.AddComponent<BoxCollider>();
        //foreach (Transform child in transform)
        //{
        //    boxcollider.bounds.Encapsulate(child.GetComponent<MeshFilter>().sharedMesh.bounds);
        //}
        addBoundsToAllChildren();
    }

    public void addBoundsToAllChildren()
    {
        if (boxCol == null)
        {
            boxCol = gameObject.GetComponent(typeof(BoxCollider)) as BoxCollider;
            if (boxCol == null)
            {
                boxCol = gameObject.AddComponent<BoxCollider>();
            }
        }
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

        var allDescendants = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform desc in allDescendants)
        {
            Renderer childRenderer = desc.GetComponent<Renderer>();
            if (childRenderer != null)
            {
                bounds.Encapsulate(childRenderer.bounds);
            }
            boxCol.center = bounds.center - transform.position;
            boxCol.size = bounds.size;
            Debug.Log(desc.name);
        }
    }
}
