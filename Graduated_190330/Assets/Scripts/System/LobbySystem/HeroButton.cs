using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class HeroButton : MonoBehaviour
{
    public HeroInfo heroInfo=new HeroInfo();

    public Image heroImage;
    public RectTransform heroImageRect;

    public Image backGroundImage;
    public Sprite noneSelectButtonImage;
    public Sprite selectedButtonImage;

    bool isSelected = false;

    public Image classIcon;

    void Start ()
    {
        transform.localScale = new Vector3(1, 1, 1);
        heroImage.SetNativeSize();
        Vector2 size = heroImageRect.sizeDelta;
        Vector2 pixelPivot = heroImage.sprite.pivot;
        Vector2 percentPivot = new Vector2(pixelPivot.x / size.x, pixelPivot.y / size.y);
        heroImageRect.pivot = percentPivot;

    }

    public void TrySelectHero()
    {
        if(!isSelected)
        {
            if (GameStartManager.Instance.selectionInfoList.Count<3)
            {
                GameStartManager.Instance.AddHero(heroInfo,this);
                isSelected = true;
                backGroundImage.sprite = selectedButtonImage;
            }
            else
            {
                isSelected = false;
                backGroundImage.sprite = noneSelectButtonImage;
                //선택 불가 출력
            }
        }
        else//이미 선택된 용사면 선택을 취소`
        {
            isSelected = false;
            backGroundImage.sprite = noneSelectButtonImage;
            GameStartManager.Instance.RemoveHero(heroInfo,this);
        }
    }
    public void Canceled()
    {
        isSelected = false;
        backGroundImage.sprite = noneSelectButtonImage;
    }
    private void Update()
    {
        name = heroInfo.name;
    }
}
