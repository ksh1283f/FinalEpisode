using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillResource : MonoBehaviour
{
    Image image;
    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetImageColor(Color color)
    {
        if (image == null)
            return;

        image.color = color;
    }
}
