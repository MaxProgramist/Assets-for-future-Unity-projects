using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


[CreateAssetMenu(menuName = "TextEffects/Shaking Text")]
public class ShakingEffect : TextEffect
{
    [SerializeField] private float maxOffset = 5;

    public override string startTag => "<sh>";
    public override string endTag => "</sh>";

    public override void AnimateEffect(ref TMP_Text textMesh, ref TMP_TextInfo textInfo, bool[] textMask, Vector3[] baseVerticesPosition)
    {
        string text = textMesh.text;
        int lenghtOfText = textInfo.characterCount;

        for (int i = 0; i < lenghtOfText; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;
            if (!textMask[i]) continue;

            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            int meshIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int baseIndex = i * 4;

            Vector3[] vertices = textInfo.meshInfo[meshIndex].vertices;

            Vector3 offset = new Vector3(Random.Range(-maxOffset, maxOffset), Random.Range(-maxOffset, maxOffset));

            vertices[vertexIndex + 0] = baseVerticesPosition[baseIndex + 0] + offset;
            vertices[vertexIndex + 1] = baseVerticesPosition[baseIndex + 1] + offset;
            vertices[vertexIndex + 2] = baseVerticesPosition[baseIndex + 2] + offset;
            vertices[vertexIndex + 3] = baseVerticesPosition[baseIndex + 3] + offset;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textMesh.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}
