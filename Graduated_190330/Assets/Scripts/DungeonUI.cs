using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class DungeonUI : uiSingletone<DungeonUI>, IBaseUI
{
    [SerializeField] Text titleText;
    [SerializeField] Text detailText;
    [SerializeField] Button btnStart;
    [SerializeField] Button btnCancel;

    [SerializeField] DungeonSelectContent content;  // 개수가 부족할 시 동적으로 생성할 용도

    [SerializeField] List<DungeonSelectContent> dungeonList;

    protected override void Awake()
    {
        uiType = E_UIType.DungeonSelect;
        base.Awake();

        dungeonList = new List<DungeonSelectContent>();
    }

    void Start()
    {
        if (btnStart != null)
            btnStart.onClick.AddListener(OnClickedStart);

        if (btnCancel != null)
            btnCancel.onClick.AddListener(OnClickedCancel);

        Close();
    }
    
    public override void Show(string[] dataList)
    {
        base.Show(dataList);
        if (dataList.Length != 1)
        {
            Debug.LogError("dataList's length is wrong, correct data count: " + 2 + " took data count: " + dataList.Length);
            return;
        }

        if (titleText != null)
            titleText.text = dataList[0];

        if (detailText != null)
            detailText.text = string.Empty;

        int clearStep = UserManager.Instance.UserInfo.BestDungeonStep;
        SetDungeonList(clearStep);

        if(!UserManager.Instance.UserInfo.TutorialClearList[(int)E_SimpleTutorialType.BattleIntro])
        {
            //show
            TutorialSimpleUI tutorialUI = UIManager.Instance.LoadUI(E_UIType.TutorialSimpleLobby) as TutorialSimpleUI;
            tutorialUI.Show(new string[]{"던전 선택 소개"});
            tutorialUI.SetTutorialType(E_SimpleTutorialType.BattleIntro);
        }
    }

    void SetDungeonList(int clearStep)
    {
        // 클리어 단수까지 생성
        dungeonList = GetComponentsInChildren<DungeonSelectContent>().ToList();
        if (clearStep >= dungeonList.Count)
        {
            for (int i = 0; i < clearStep + 1; i++)
            {
                if (i >= dungeonList.Count) // 기존에 있는 개수를 넘어갈 경우
                {
                    // 새로 생성
                    DungeonSelectContent newContent = Instantiate(content) as DungeonSelectContent;
                    newContent.transform.SetParent(content.transform.parent);
                    newContent.transform.localScale = Vector3.one;  // 스케일 보정
                    dungeonList.Add(newContent);
                }

                // 데이터 갱신
                // to parse from dungeonPatternData, rewardData
                DungeonPattern dungeonPatternData = DungeonStepManager.Instance.DungeonStepDic[i];
                DungeonMonsterData dungeonMonsterData = GameDataManager.Instance.DungeonMonsterDataDic[i];
                RewardData rewardData = GameDataManager.Instance.RewardDataDic[i];
                dungeonList[i].SetSimpleDataInfo(string.Concat(i + 1, "번째 던전"), i < clearStep ? true : false);
                dungeonList[i].SetDetailInfo(dungeonPatternData, rewardData);
                dungeonList[i].SetDungeonMonsterData(dungeonMonsterData);
                dungeonList[i].OnClickedShowDetail = OnClickedShowDetail;
            }
        }
        else
        {
            for (int i = 0; i < dungeonList.Count; i++)
            {
                if (i >= clearStep + 1)
                {
                    dungeonList[i].gameObject.SetActive(false);
                    continue;
                }

                DungeonPattern dungeonPatternData = DungeonStepManager.Instance.DungeonStepDic[i];
                DungeonMonsterData dungeonMonsterData = GameDataManager.Instance.DungeonMonsterDataDic[i];
                RewardData rewardData = GameDataManager.Instance.RewardDataDic[i];
                dungeonList[i].SetSimpleDataInfo(string.Concat(i + 1, "번째 던전"), i < clearStep ? true : false);
                dungeonList[i].SetDetailInfo(dungeonPatternData, rewardData);
                dungeonList[i].SetDungeonMonsterData(dungeonMonsterData);
                dungeonList[i].OnClickedShowDetail = OnClickedShowDetail;
            }
        }
    }

    void OnClickedShowDetail(string detailText, DungeonMonsterData patternData)
    {
        SoundManager.Instance.PlayButtonSound();
        UserManager.Instance.SelectedDungeonMonsterData = patternData;
        SetDetailInfo(detailText);
    }

    // 클릭되었을 때 실행하기
    void SetDetailInfo(string detail)
    {
        if (detailText == null)
        {
            Debug.LogError("detail text is null");
            return;
        }

        detailText.text = detail;
    }

    void OnClickedStart()
    {
        SoundManager.Instance.PlayButtonSound();
        btnStart.interactable = false;
        //todo 던전 단수에 맞게 데이터 조정- battlemanager 안의 initBattle 참조
        if (UserManager.Instance.SelectedDungeonMonsterData == null)
        {
            MessageUI message = UIManager.Instance.LoadUI(E_UIType.ShowMessage) as MessageUI;
            message.Show(new string[] { "유저 메세지", "선택된 던전이 없습니다." });
            return;
        }

        // 출전 용병이 없는 경우에는 전투 불가
        if(UserManager.Instance.UserInfo.SelectedUnitDic.Count == 0)
        {
            MessageUI message = UIManager.Instance.LoadUI(E_UIType.ShowMessage) as MessageUI;
            message.Show(new string[] { "유저 메세지", "출전할 용병이 없습니다.\n사무소에서 출전할 용병을 선택해주세요\n용병이 없는 경우 훈련소에서 용병을 고용할 수 있습니다." });
            return;
        }

        StartCoroutine(BattleSceneLoad());
    }

    void OnClickedCancel()
    {
        SoundManager.Instance.PlayButtonSound();
        UserManager.Instance.SelectedDungeonMonsterData = null; // 끄면 선택된 던전 정보 초기화
        if(!TutorialManager.Instance.IsTutorialComplete)
            TutorialManager.Instance.IsTutorialComplete = true;
        Close();
    }

    IEnumerator BattleSceneLoad()
    {
        string sceneName = UserManager.Instance.SelectedDungeonMonsterData.SceneName;

        LobbyUI lobbyUI = UIManager.Instance.LoadUI(E_UIType.Lobby) as LobbyUI;
        lobbyUI.Close();
        UserManager.Instance.UserSituation = E_UserSituation.Battle;
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        while (!ao.isDone)
            yield return null;
    }
}
