using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class NavMeshVisualizer : MonoBehaviour
{
    [SerializeField] Material _visualisationMaterial;
    [SerializeField] Vector3 _generatedMeshOffset = new(0, 0.05f, 0);

    GameObject _meshVisualisation;
    void OnEnable()
    {
        _meshVisualisation = new("NavMesh Visualization");
        MeshRenderer renderer = _meshVisualisation.AddComponent<MeshRenderer>();
        MeshFilter filter = _meshVisualisation.AddComponent<MeshFilter>();
        Mesh navMesh = new Mesh();

        NavMeshTriangulation traingulation = NavMesh.CalculateTriangulation();

        navMesh.SetVertices(traingulation.vertices);
        navMesh.SetIndices(traingulation.indices, MeshTopology.Triangles, 0);

        renderer.sharedMaterial = _visualisationMaterial;
        filter.mesh = navMesh;
        _meshVisualisation.transform.position = _generatedMeshOffset;
    }
}
