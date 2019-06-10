using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassButton : MonoBehaviour
{
    public E_Class classInfo = 0;
    public Sprite selectedImage;
    public Sprite noneSelectedImage;
    public Image tapImage;
    public RectTransform rect;
    
    bool isSelected=false;
    public void Clicked()
    {
        GameStartManager.Instance.ClassSet(this);
    }

    public void IsSelected(bool isSelect)
    {
        if(isSelect)
        {
            isSelected = true;
            tapImage.sprite = selectedImage;
            rect.anchoredPosition += new Vector2(0, 2);
            rect.sizeDelta = new Vector2(32, 18);
        }
        else
        {
            isSelected = false;
            tapImage.sprite = noneSelectedImage;
            rect.anchoredPosition -= new Vector2(0, 2);
            rect.sizeDelta = new Vector2(32, 14);
        }
    }

	// Use this for initialization
	void Start ()
    {
        tapImage = GetComponent<Image>();
        rect = GetComponent<RectTransform>();

    }
}
