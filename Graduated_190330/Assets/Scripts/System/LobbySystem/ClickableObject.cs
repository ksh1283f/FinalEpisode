using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    private void OnMouseDown()
    {
        if (spriteRenderer == null)
            return;

        if (UIManager.Instance.openedUiDic.Count == 1)
            spriteRenderer.color = new Color32(150, 150, 150, 255);
    }
    private void OnMouseUp()
    {
        if (spriteRenderer == null)
            return;

        if (UIManager.Instance.openedUiDic.Count == 1)
            spriteRenderer.color = new Color32(255, 255, 255, 255);
    }

    // Use this for initialization
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }
}
