using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


[CreateAssetMenu(menuName = "TextEffects/Waving Text")]
public class WavingEffect : TextEffect
{
    [SerializeField] private float frequency = 5;
    [SerializeField] private float amplitude = 5;
    [SerializeField] private float spreading = 1;

    public override string startTag => "<wav>";
    public override string endTag => "</wav>";

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

            Vector3[] vertices = textInfo.meshInfo[meshIndex].vertices;

            float offset = Mathf.Sin(Time.time * frequency + i * spreading) * amplitude;

            vertices[vertexIndex + 0] += new Vector3(0, offset, 0);
            vertices[vertexIndex + 1] += new Vector3(0, offset, 0);
            vertices[vertexIndex + 2] += new Vector3(0, offset, 0);
            vertices[vertexIndex + 3] += new Vector3(0, offset, 0);
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textMesh.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}
