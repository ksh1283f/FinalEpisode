using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class CharacterPropertyUI : uiSingletone<CharacterPropertyUI>, IBaseUI
{
    [SerializeField] Text titleText;

    [SerializeField] PropertyImages commonProperties;
    [SerializeField] PropertyImages utilProperties;
    [SerializeField] PropertyImages healingProperties;

	[SerializeField] Button btnOk;
    [SerializeField] Button btnPrev;
    [SerializeField] Button btnNext;

    [SerializeField] List<GameObject> propertyPageList = new List<GameObject>();
    [SerializeField]int pageIndex = 0;

    public Action<E_DetailPropertyType> OnChangeToggleValue { get; set; }
    protected override void Awake()
    {
        uiType = E_UIType.CharacterProperty;
        base.Awake();
    }

    void Start()
    {
        if(btnOk != null)
            btnOk.onClick.AddListener(() => { OnBtnOk(); });

        if(btnPrev != null)
            btnPrev.onClick.AddListener(()=>{ OnBtnPrev(); });

        if(btnNext != null)
            btnNext.onClick.AddListener(()=>{ OnBtnNext(); });

        Close();
    }

	void OnBtnOk()
	{
        SoundManager.Instance.PlayButtonSound();
		Close();
	}

    void OnBtnPrev()
    {
        SoundManager.Instance.PlayButtonSound();
        ActivatedPropertyPage(pageIndex-1);
    }

    void OnBtnNext()
    {
        SoundManager.Instance.PlayButtonSound();
        ActivatedPropertyPage(pageIndex+1);
    }
	
    public override void Show(string[] dataList)
    {
        base.Show(dataList);
        if (dataList == null)
            return;

        string title = dataList[0];
		if(titleText != null)
			titleText.text = title;
        
        pageIndex = 0;  // default
        ActivatedPropertyPage(pageIndex);
        // 튜토리얼
        if(!UserManager.Instance.UserInfo.TutorialClearList[(int)E_SimpleTutorialType.BattleProperty])
        {
            //show
            TutorialSimpleUI tutorialUI = UIManager.Instance.LoadUI(E_UIType.TutorialSimpleLobby) as TutorialSimpleUI;
            tutorialUI.Show(new string[]{"연구소 소개"});
            tutorialUI.SetTutorialType(E_SimpleTutorialType.BattleProperty);
        }
    }

    void UpdateData(E_BattlePropertyType type)
    {
        if (UserManager.Instance.UserInfo == null)
        {
            Debug.LogError("UserInfo is nul");
            return;
        }

        switch (type)
        {
            case E_BattlePropertyType.Common:
                foreach (var item in CharacterPropertyManager.Instance.CommonPropertyDic)
                    commonProperties.UpdateProperties(item.Value, item.Key);
                break;

            case E_BattlePropertyType.Util:
                foreach (var item in CharacterPropertyManager.Instance.UtilPropertyDic)
                    utilProperties.UpdateProperties(item.Value, item.Key);
                break;

            case E_BattlePropertyType.Healing:
                foreach (var item in CharacterPropertyManager.Instance.HealingPropertyDic)
                    healingProperties.UpdateProperties(item.Value, item.Key);
                break;
        }
    }

    void ActivatedPropertyPage(int index)
    {
/*{
    None,
    Common,=1
    Util,=2
    Healing,=3
} */
        if(index < 0)   // prev의 경우
            index = propertyPageList.Count-1;
        else if (index >=propertyPageList.Count) // next의 경우
            index = 0;

        pageIndex =index;
        for (int i = 0; i < propertyPageList.Count; i++)
        {
            if (i == pageIndex)
                propertyPageList[i].SetActive(true);
            else
                propertyPageList[i].SetActive(false);
        }

        E_BattlePropertyType type = (E_BattlePropertyType)(pageIndex+1);
        UpdateData(type);
    }
}