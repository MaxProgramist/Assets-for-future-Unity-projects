using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shakingtext : MonoBehaviour
{
    [SerializeField] private Vector2 maxDistance;
    [SerializeField] private float timeBtwFrames;

    TMP_Text textMesh;
    List<Vector3[]> originalVertices = new List<Vector3[]>();

    float currentTimeBtwFrames = 0;

    void Start()
    {
        textMesh = GetComponent<TMP_Text>();
        textMesh.ForceMeshUpdate();
        TMP_TextInfo textInfo = textMesh.textInfo;

        originalVertices.Clear();
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var copy = new Vector3[textInfo.meshInfo[i].vertices.Length];
            System.Array.Copy(textInfo.meshInfo[i].vertices, copy, copy.Length);
            originalVertices.Add(copy);
        }
    }

    void Update()
    {
        if (currentTimeBtwFrames < timeBtwFrames) {
            currentTimeBtwFrames += Time.deltaTime;
            return;
        }

        textMesh.ForceMeshUpdate();
        TMP_TextInfo textInfo = textMesh.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

            Vector3[] verticesOfLetter = textInfo.meshInfo[materialIndex].vertices;
            Vector3[] originalVerticesOfLetter = originalVertices[materialIndex];

            Vector3 offset = new Vector3(Random.Range(-maxDistance.x, maxDistance.x), Random.Range(-maxDistance.y, maxDistance.y));

            verticesOfLetter[vertexIndex + 0] = originalVerticesOfLetter[vertexIndex + 0] + offset;
            verticesOfLetter[vertexIndex + 1] = originalVerticesOfLetter[vertexIndex + 1] + offset;
            verticesOfLetter[vertexIndex + 2] = originalVerticesOfLetter[vertexIndex + 2] + offset;
            verticesOfLetter[vertexIndex + 3] = originalVerticesOfLetter[vertexIndex + 3] + offset;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textMesh.UpdateGeometry(meshInfo.mesh, i);
        }

        currentTimeBtwFrames = 0;
    }
}
