using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;


[RequireComponent(typeof(TMP_Text))]
public class TextAnimator : MonoBehaviour
{
    [HideInInspector]
    public List<TextEffect> effects = new List<TextEffect>();

    Dictionary<TextEffect, bool[]> effectsTextMask = new Dictionary<TextEffect, bool[]>();
    Vector3[] baseVerticesPosition = new Vector3[0];

    TMP_Text textMesh;
    TMP_TextInfo textInfo;

    void Awake()
    {
        textMesh = GetComponent<TMP_Text>();

        ParseTags();

        textMesh.ForceMeshUpdate();
        textInfo = textMesh.textInfo;
    }

    void Update()
    {
        textMesh.ForceMeshUpdate();
        textInfo = textMesh.textInfo;

        foreach (var effect in effects)
        {
            bool[] textMask = effectsTextMask[effect];

            effect.AnimateEffect(ref textMesh, ref textInfo, textMask, baseVerticesPosition);
        }
    }

    private void ParseTags()
    {
        string raw = textMesh.text;
        string cleanText = raw;

        List<TextEffect> visitedEffects = new List<TextEffect>();
        Dictionary<TextEffect, List<(int startIndex, int endIndex)>> regionsOfEffects = new Dictionary<TextEffect, List<(int startIndex, int endIndex)>>();

        foreach (var effect in effects)
        {
            int searchIndex = 0;
            List<(int startIndex, int endIndex)> indexesOfRegion = new List<(int startIndex, int endIndex)>();

            while (true)
            {
                int start = cleanText.IndexOf(effect.startTag, searchIndex);
                if (start == -1) break;

                int end = cleanText.IndexOf(effect.endTag, start + effect.startTag.Length);
                if (end == -1) break;

                int regionStart = start;
                int regionEnd = end - effect.startTag.Length;

                indexesOfRegion.Add((regionStart, regionEnd));

                cleanText = cleanText.Remove(end, effect.endTag.Length);
                cleanText = cleanText.Remove(start, effect.startTag.Length);

                searchIndex = start;
            }

            foreach (var visitedEffect in visitedEffects)
            {
                List<(int startIndex, int endIndex)> currentEffectRegion = regionsOfEffects[visitedEffect];

                foreach ((int startIndex, int endIndex) in indexesOfRegion)
                {
                    for (int i = 0; i< currentEffectRegion.Count; i++)
                    {
                        (int startCurrentIndex, int endCurrentIndex) = currentEffectRegion[i];
                        if (startCurrentIndex >= startIndex) startCurrentIndex -= effect.startTag.Length;
                        if (startCurrentIndex >= endIndex) startCurrentIndex -= effect.endTag.Length;
                        if (endCurrentIndex >= startIndex) endCurrentIndex -= effect.startTag.Length;
                        if (endCurrentIndex >= endIndex) endCurrentIndex -= effect.endTag.Length;

                        currentEffectRegion[i] = (startCurrentIndex, endCurrentIndex);
                    }
                }
            }

            regionsOfEffects[effect] = indexesOfRegion;
            visitedEffects.Add(effect);
        }

        textMesh.text = cleanText;
        textMesh.ForceMeshUpdate();
        TMP_TextInfo textInfo = textMesh.textInfo;

        List<Vector3> verticesList = new List<Vector3>();
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
            {
                verticesList.Add(Vector3.zero);
                verticesList.Add(Vector3.zero);
                verticesList.Add(Vector3.zero);
                verticesList.Add(Vector3.zero);
                continue;
            }

            verticesList.Add(charInfo.bottomLeft);
            verticesList.Add(charInfo.topLeft);
            verticesList.Add(charInfo.topRight);
            verticesList.Add(charInfo.bottomRight);
        }
        baseVerticesPosition = verticesList.ToArray();

        foreach (var effect in effects)
        {
            bool[] textMask = new bool[cleanText.Length];
            List<(int startIndex, int endIndex)> indexesOfRegion = regionsOfEffects[effect];

            foreach ((int startIndex, int endIndex) in indexesOfRegion)
            {
                for (int i = startIndex; i < endIndex; i++)
                    textMask[i] = true;
            }

            effectsTextMask[effect] = textMask;
        }
    }


    public List<TextEffect> GetEffects() => effects;
}
