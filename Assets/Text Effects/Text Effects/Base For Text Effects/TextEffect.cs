using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;


public abstract class TextEffect : ScriptableObject
{
    public abstract string startTag { get; }
    public abstract string endTag { get; }


    public abstract void AnimateEffect(ref TMP_Text textMesh, ref TMP_TextInfo textInfo, bool[] textMask, Vector3[] baseVerticesPosition);
}
