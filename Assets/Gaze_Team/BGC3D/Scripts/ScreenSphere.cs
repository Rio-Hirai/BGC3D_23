using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScreenSphere : MonoBehaviour
{
    public bool removeExistingColliders = true;
    public receiver server;
    // Start is called before the first frame update
    void Start()
    {
        CreateInvertedMeshCollider();
    }

    private void Update()
    {
        //if (server.target_p_id != 97)
        //{
        //    this.gameObject.SetActive(false);
        //}
        //else
        //{
        //    this.gameObject.SetActive(true);
        //}
    }

    public void CreateInvertedMeshCollider()
    {
        if (removeExistingColliders)
            RemoveExistingColliders();

        InvertMesh();

        gameObject.AddComponent<MeshCollider>();
    }

    private void RemoveExistingColliders()
    {
        Collider[] colliders = GetComponents<Collider>();
        for (int i = 0; i < colliders.Length; i++)
            DestroyImmediate(colliders[i]);
    }

    private void InvertMesh()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.triangles = mesh.triangles.Reverse().ToArray();
    }
}
