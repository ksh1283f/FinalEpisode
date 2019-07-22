using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graduate.GameData.UnitData;
using UnityEngine;
using UnityEngine.UI;
public enum E_SelectedUnitListSetType
{
    None,
    Insert, // 비어있을때 새로운 유닛 투입
    Remove, // 하나라도 있을때 기존 유닛 제거
    Trade,  // 선택한 유닛끼리 교체
}

public class UserInformationUI : uiSingletone<UserInformationUI>
{
    [SerializeField] Button btnTrade;
    [SerializeField] Button btnInsert;
    [SerializeField] Button btnDelete; // 유저 기본정보 확인
    [SerializeField] Button btnBack;
    [SerializeField] Text titleText;
    [SerializeField] Text detailInfo;

    public List<CharacterSimpleInfo> simpleInfoList;
    public List<SelectCharacterIcon> selectIconList;

    UnitData selectedUnitInSimpleList;
    UnitData selectedUnitInSelectList;

    protected override void Awake()
    {
        uiType = E_UIType.UserInformation;
        base.Awake();

        simpleInfoList = GetComponentsInChildren<CharacterSimpleInfo>().ToList();
        selectIconList = GetComponentsInChildren<SelectCharacterIcon>().ToList();

        if (btnTrade != null)
            btnTrade.onClick.AddListener(OnClickedBtnTrade);
        if (btnInsert != null)
            btnInsert.onClick.AddListener(OnClickedBtnInsert);
        if (btnDelete != null)
            btnDelete.onClick.AddListener(OnClickedBtnDelete);
        if (btnBack != null)
            btnBack.onClick.AddListener(OnClickedBtnBack);
    }

    public override void Show(string[] dataList)
    {
        base.Show();
        if (dataList.Length != 1)
        {
            Debug.LogError("dataList's length is wrong, correct data count: " + 2 + " took data count: " + dataList.Length);
            return;
        }

        if (titleText != null)
            titleText.text = dataList[0];

        if (detailInfo != null)
            detailInfo.text = string.Empty;

        // todo 데이터 갱신 작업
        SetSimpleInfoList();
        SetSelectedList();
    }

    void Start()
    {
        Close();
    }

    // 보유중인 용병 리스트 갱신
    void SetSimpleInfoList()
    {
        UserInfo userInfo = UserManager.Instance.UserInfo;
        int index = 0;
        // init
        for (int i = 0; i < simpleInfoList.Count; i++)
            simpleInfoList[i].gameObject.SetActive(false);

        foreach (var item in userInfo.UnitDic)
        {
            simpleInfoList[index].gameObject.SetActive(true);
            simpleInfoList[index].SetData(item.Value);
            simpleInfoList[index].OnClickedContent = OnClickedSimpleInfo;
            index++;
        }
    }

    void SetSelectedList()
    {
        UserInfo userInfo = UserManager.Instance.UserInfo;
        for (int i = 0; i < selectIconList.Count; i++)
        {
            selectIconList[i].OnClickedContent = OnClickedSelectedIcon;
            selectIconList[i].SetUnitData(null);
        }

        // todo 선택된 용병들 갱신
        int index = 0;
        foreach (var item in userInfo.SelectedUnitDic)
        {
            selectIconList[index].SetUnitData(item.Value);
            index++;
        }
    }

    string GetDataInfo(UnitData data)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Lv. ");
        sb.Append(data.Level);
        sb.AppendLine();
        sb.Append("Atk: ");
        sb.Append(data.Atk);
        sb.AppendLine();
        sb.Append("Def: ");
        sb.Append(data.Def);
        sb.AppendLine();
        sb.Append("Cri: ");
        sb.Append(data.Cri);
        sb.AppendLine();
        sb.Append("Spd: ");
        sb.Append(data.Spd);
        sb.AppendLine();
        sb.Append(data.Description);

        return sb.ToString();
    }

    void OnClickedSimpleInfo(UnitData data, bool isSelected)
    {
        if (data == null)
        {
            Debug.LogError("unitData is null");
            return;
        }

        // 다른 선택된 리스트 선택 해제하기
        for (int i = 0; i < simpleInfoList.Count; i++)
        {
            if (data.Id == simpleInfoList[i].unitData.Id)
                continue;

            simpleInfoList[i].IsSelected = false;
        }

        if (!isSelected)
        {
            detailInfo.text = string.Empty;
            selectedUnitInSimpleList = null;
            return;
        }

        selectedUnitInSimpleList = data;
        detailInfo.text = GetDataInfo(selectedUnitInSimpleList);
    }

    void OnClickedSelectedIcon(UnitData data, bool isSelected)
    {
        if (data == null)
        {
            Debug.LogError("unitData is null");
            return;
        }

        // 다른 선택된 리스트 선택 해제하기
        for (int i = 0; i < selectIconList.Count; i++)
        {
            if (selectIconList[i].unitData == null)
            {
                selectIconList[i].IsSelected = false;
                continue;
            }

            if (data.Id == selectIconList[i].unitData.Id)
                continue;

            selectIconList[i].IsSelected = false;
        }

        if (!isSelected)
        {
            detailInfo.text = string.Empty;
            selectedUnitInSelectList = null;
            return;
        }

        selectedUnitInSelectList = data;
        detailInfo.text = GetDataInfo(selectedUnitInSelectList);
    }

    void OnClickedBtnTrade()
    {
        // <예외>
        // 선택된 캐릭터가 두 리스트 중 하나라도 없는 경우
        if (selectedUnitInSimpleList == null)
        {
            MessageUI message = UIManager.Instance.LoadUI(E_UIType.ShowMessage) as MessageUI;
            message.Show(new string[] { "유저 메세지", "보유 용병중에서 선택된 용병이 없습니다." });
            Debug.LogError("보유 용병중에서 선택된 용병이 없습니다.");
            return;
        }

        if (selectedUnitInSelectList == null)
        {
            MessageUI message = UIManager.Instance.LoadUI(E_UIType.ShowMessage) as MessageUI;
            message.Show(new string[] { "유저 메세지", "교환될 용병이 없습니다." });
            Debug.LogError("교환될 용병이 없습니다.");
            return;
        }

        // 이미 출전중인 유닛일 경우의 예외처리
        if(UserManager.Instance.UserInfo.SelectedUnitDic.ContainsKey(selectedUnitInSimpleList.Id))
        {
            MessageUI message = UIManager.Instance.LoadUI(E_UIType.ShowMessage) as MessageUI;
            message.Show(new string[] { "유저 메세지", "교환하려는 용병이 이미 출전중입니다." });
            Debug.LogError("교환하려는 용병이 이미 출전중입니다.");
            return;
        }

        // todo trading process
        // 1. remove
        UserManager.Instance.SetMySelectedUnitList(selectedUnitInSelectList, E_SelectedUnitListSetType.Remove);
        
        // 2. insert
        UserManager.Instance.SetMySelectedUnitList(selectedUnitInSimpleList, E_SelectedUnitListSetType.Insert);

        // 3. ui update
        // 보유 리스트도 업데이트 하기
        for (int i = 0; i < selectIconList.Count; i++)  // 초기화
            selectIconList[i].SetUnitData(null);
            
        int index = 0;
        foreach (var item in UserManager.Instance.UserInfo.SelectedUnitDic)
        {
            selectIconList[index].SetUnitData(item.Value);
            index++;
        }

        for (int i = 0; i < simpleInfoList.Count; i++)
            simpleInfoList[i].SetData(simpleInfoList[i].unitData);

        selectedUnitInSelectList = null;
        selectedUnitInSimpleList = null;
    }

    void OnClickedBtnInsert()
    {
        //<예외>
        // 보유리스트중 선택된 것이 없는 경우
        if (selectedUnitInSimpleList == null)
        {
            MessageUI message = UIManager.Instance.LoadUI(E_UIType.ShowMessage) as MessageUI;
            message.Show(new string[] { "유저 메세지", "보유 용병중에서 선택된 용병이 없습니다." });
            Debug.LogError("보유 용병중에서 선택된 용병이 없습니다.");
            return;
        }

        if (UserManager.Instance.UserInfo.SelectedUnitDic.ContainsKey(selectedUnitInSimpleList.Id))
        {
            MessageUI message = UIManager.Instance.LoadUI(E_UIType.ShowMessage) as MessageUI;
            message.Show(new string[] { "유저 메세지", "이미 출전하는 용병입니다." });
            Debug.LogError("이미 출전하는 용병입니다.");
            return;
        }

        // 선택리스트가 가득 찬 경우 체크
        bool isReady = false;
        for (int i = 0; i < selectIconList.Count; i++)
        {
            if (selectIconList[i].unitData == null)
            {
                isReady = true;
                break;
            }
        }

        if (isReady)
        {
            UserManager.Instance.SetMySelectedUnitList(selectedUnitInSimpleList, E_SelectedUnitListSetType.Insert);
            int index = 0;
            foreach (var item in UserManager.Instance.UserInfo.SelectedUnitDic)
            {
                selectIconList[index].SetUnitData(item.Value);
                index++;
            }

            for (int i = 0; i < simpleInfoList.Count; i++)
            {
                if (simpleInfoList[i].unitData.Id == selectedUnitInSimpleList.Id)
                {
                    simpleInfoList[i].SetData(selectedUnitInSimpleList);
                    break;
                }
            }
        }
        else
        {
            MessageUI message = UIManager.Instance.LoadUI(E_UIType.ShowMessage) as MessageUI;
            message.Show(new string[] { "유저 메세지", "출전리스트가 가득 찼습니다." });
            Debug.LogError("출전리스트가 가득 찼습니다.");
            return;
        }
        
        selectedUnitInSelectList = null;
        selectedUnitInSimpleList = null;
    }

    void OnClickedBtnDelete()
    {
        //<예외>
        // 선택리스트에 아무것도 없는 경우
        UserInfo userInfo = UserManager.Instance.UserInfo;
        if (userInfo.SelectedUnitDic.Count == 0)
        {
            MessageUI message = UIManager.Instance.LoadUI(E_UIType.ShowMessage) as MessageUI;
            message.Show(new string[] { "유저 메세지", "출전할 용병이 없습니다." });
            Debug.LogError("출전할 용병이 없습니다.");
            return;
        }

        // 선택리스트 중에서 선택된 것이 없는 경우
        if (selectedUnitInSelectList == null)
        {
            MessageUI message = UIManager.Instance.LoadUI(E_UIType.ShowMessage) as MessageUI;
            message.Show(new string[] { "유저 메세지", "선택된 용병이 없습니다." });
            Debug.LogError("선택된 용병이 없습니다.");
            return;
        }

        UserManager.Instance.SetMySelectedUnitList(selectedUnitInSelectList, E_SelectedUnitListSetType.Remove);

        int index = 0;
        for (int i = 0; i < selectIconList.Count; i++)  // 초기화
            selectIconList[i].SetUnitData(null);

        foreach (var item in userInfo.SelectedUnitDic)
        {
            selectIconList[index].SetUnitData(item.Value);  // 남은 데이터 갱신
            index++;
        }

        for (int i = 0; i < simpleInfoList.Count; i++)
        {
            if(UserManager.Instance.UserInfo.SelectedUnitDic.ContainsKey(simpleInfoList[i].unitData.Id))
                continue;

            simpleInfoList[i].SetData(simpleInfoList[i].unitData);  // 출전리스트에 빠진 용병들 정보 다시 갱신
        }
        selectedUnitInSelectList = null;
        selectedUnitInSimpleList = null;
    }

    void OnClickedBtnBack()
    {
        selectedUnitInSelectList = null;
        selectedUnitInSimpleList = null;
        Close();
    }
}
