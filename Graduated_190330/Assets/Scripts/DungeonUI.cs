using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DungeonUI : uiSingletone<DungeonUI>, IBaseUI
{
    [SerializeField] Text titleText;
    [SerializeField] Text detailText;
    [SerializeField] Button btnStart;
    [SerializeField] Button btnCancel;

    List<DungeonSelectContent> dungeonList;

    protected override void Awake()
    {
        uiType = E_UIType.DungeonSelect;
        base.Awake();

        dungeonList = new List<DungeonSelectContent>();
    }

    void Start()
    {
        if(btnStart != null)
            btnStart.onClick.AddListener(OnClickedStart);
            
        if( btnCancel != null)
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
    }

    void SetDungeonList(int clearStep)
    {
        // 클리어 단수까지 생성

    }

    // 클릭되었을 때 실행하기
    void SetDetailInfo(string name, int hp, int atk, int exp, int gold)
    {
        if (detailText == null)
        {
            Debug.LogError("detail text is null");
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.Append(name);
        sb.AppendLine();
        sb.AppendLine();
        sb.Append("<몬스터 정보>");
        sb.AppendLine();
        sb.Append("체력: ");
        sb.Append(hp);
        sb.Append("공격력: +");
        sb.Append(atk);
        sb.Append("%");
        sb.AppendLine();
        sb.AppendLine();
        sb.AppendLine();

        sb.Append("<보상 정보>");
        sb.AppendLine();
        sb.Append("exp: ");
        sb.Append(exp);
        sb.AppendLine();
        sb.Append("gold: ");
        sb.Append(gold);

        detailText.text = sb.ToString();
    }

    void OnClickedStart()
    {
        //todo 던전 단수에 맞게 데이터 조정- battlemanager 안의 initBattle 참조
        StartCoroutine(BattleSceneLoad());
    }

    void OnClickedCancel()
    {
        Close();
    }

    IEnumerator BattleSceneLoad()
    {
        LobbyUI lobbyUI = UIManager.Instance.LoadUI(E_UIType.Lobby) as LobbyUI;
        lobbyUI.Close();
        UserManager.Instance.UserSituation = E_UserSituation.Battle;
        AsyncOperation ao = SceneManager.LoadSceneAsync("Battle_True");
        while (!ao.isDone)
            yield return null;
    }
}
