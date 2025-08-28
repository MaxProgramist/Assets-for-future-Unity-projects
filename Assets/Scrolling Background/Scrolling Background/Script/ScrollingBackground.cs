using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.5f;
    [SerializeField] private float backgroundScale = 1f;
    [SerializeField] private Vector2 direction = new Vector2(1,1);

    Renderer renderArea;

    private void Awake()
    {
        renderArea = GetComponent<Renderer>();
    }

    private void OnValidate()
    {
        if (renderArea == null)
            renderArea = GetComponent<Renderer>();

        if (renderArea != null && renderArea.sharedMaterial != null)
            renderArea.sharedMaterial.mainTextureScale = new Vector2(transform.localScale.x * backgroundScale, transform.localScale.y * backgroundScale);
    }

    void Update()
    {
        if (renderArea != null && renderArea.sharedMaterial != null)
            renderArea.material.mainTextureOffset = new Vector2(scrollSpeed * direction.x * Time.time, scrollSpeed * direction.y * Time.time);
    }
}
