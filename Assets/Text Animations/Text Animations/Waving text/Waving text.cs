using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class WavingText : MonoBehaviour
{
    [SerializeField] private OffsetType offsetType;
    [SerializeField] private float amplitude = 5f;
    [SerializeField] private float frequency = 5f;
    [SerializeField] private float phaseShift = 0.25f;

    enum OffsetType
    {
        Horizontal = 0,
        Vertical = 1,
        Both = 2
    }

    TMP_Text textMesh;

    void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
    }

    void Update()
    {
        textMesh.ForceMeshUpdate();
        TMP_TextInfo textInfo = textMesh.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            float wave = Mathf.Sin(Time.time * frequency + i * phaseShift) * amplitude;

            Vector3 offset;
            switch (offsetType)
            {
                case OffsetType.Horizontal:
                    offset = new Vector3(wave, 0, 0);
                    break;
                case OffsetType.Vertical:
                    offset = new Vector3(0, wave, 0);
                    break;
                default:
                    offset = new Vector3(wave, wave, 0);
                    break;
            }

            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textMesh.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}
