using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSimpleUI : uiSingletone<TutorialSimpleUI> {
    [SerializeField] Text titleText;
    [SerializeField] Text contentText;
    [SerializeField] Button btnNext;
    [SerializeField] Button btnPrev;
    [SerializeField] Image image;

    public Dictionary<E_SimpleTutorialType, Dictionary<int, TutorialSimpleData>> presentTutorialDic = new Dictionary<E_SimpleTutorialType, Dictionary<int, TutorialSimpleData>>();
    public E_SimpleTutorialType PresentTutorialType {get; private set;}
    private int dicIndex =0;
    private string title;
    [SerializeField] E_UIType thisTutorialSimpleType;
    protected override void Awake () {
        uiType = thisTutorialSimpleType;
        base.Awake ();

        if (titleText == null)
            return;

        if (contentText == null)
            return;

        if (btnNext == null)
            return;

        if (btnPrev == null)
            return;

        if (image == null)
            return;

        if(presentTutorialDic == null)
            presentTutorialDic = new Dictionary<E_SimpleTutorialType, Dictionary<int, TutorialSimpleData>>();
        btnPrev.onClick.AddListener(() => { OnBtnPrev(); });
        btnNext.onClick.AddListener(() => { OnBtnNext(); });

        Debug.Log("Tutorial simple ui start");
        Close();
    }

    // 튜토리얼을 켜야하는 상황이면 켠다
    [ContextMenu("show")]
    public override void Show (string[] dataList) {

        base.Show (dataList);
        if (dataList == null)
            return;

        if (dataList.Length != 1) {
            Debug.LogError ("dataList's length is wrong, correct data count: " + 2 + " took data count: " + dataList.Length);
            return;
        }

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        title = dataList[0];
        dicIndex = 0;
    }

    void OnBtnPrev () {
        if(dicIndex == 0)
            return;

        SoundManager.Instance.PlayButtonSound();

        SetTutorialContents(title, presentTutorialDic[PresentTutorialType][dicIndex-1].Dialogue, presentTutorialDic[PresentTutorialType][dicIndex-1].ImagePath);
        dicIndex--;
    }

    void OnBtnNext () {
        if(dicIndex == presentTutorialDic[PresentTutorialType].Count-1)
        {
            UserManager.Instance.SetTutorialClearState(PresentTutorialType);
            Close();
            return;
        }
        
        SoundManager.Instance.PlayButtonSound();
        SetTutorialContents(title, presentTutorialDic[PresentTutorialType][dicIndex+1].Dialogue, presentTutorialDic[PresentTutorialType][dicIndex+1].ImagePath);
        dicIndex++;
    }

    //ui 갱신
    void SetTutorialContents (string title, string content, string imagePath) {
        titleText.text = title;
        contentText.text = content;
        image.sprite = Resources.Load<Sprite>(imagePath);
    }

    // ui처음 세팅할떄 호출
    public void SetTutorialType(E_SimpleTutorialType type)
    {
        if(type == E_SimpleTutorialType.None)
            return;
        
        PresentTutorialType = type;
        if(!presentTutorialDic.ContainsKey(PresentTutorialType))
            presentTutorialDic.Add(PresentTutorialType, new Dictionary<int, TutorialSimpleData>());
        
        for (int i = 0; i < GameDataManager.Instance.SimpleTutorialDataDic[PresentTutorialType].Count; i++)
        {
            presentTutorialDic[PresentTutorialType].Add(GameDataManager.Instance.SimpleTutorialDataDic[PresentTutorialType][i].DetailId,
                                                        GameDataManager.Instance.SimpleTutorialDataDic[PresentTutorialType][i]);
        }

        TutorialSimpleData data = presentTutorialDic[PresentTutorialType][dicIndex];
        SetTutorialContents(title, data.Dialogue, data.ImagePath);
    }

    public void ChangeUI_Type(bool isLobby)
    {
        if(isLobby)
            uiType = E_UIType.TutorialSimpleLobby;
        else
            uiType = E_UIType.TutorialSimpleBattle;
    }
}