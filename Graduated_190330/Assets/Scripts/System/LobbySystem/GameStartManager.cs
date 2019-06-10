using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.SceneManagement;


public class SelectionInfo
{
    public HeroInfo heroInfo = new HeroInfo();
    public bool isLeader;
    public HeroButton heroButton;
}
public partial class GameStartManager : MonoBehaviour
{
    static GameStartManager instance;
    public static GameStartManager Instance
    {
        get
        {
            return instance;
        }
    }
}
public partial class GameStartManager : MonoBehaviour
{
    Sprite[] sprites;
    Dictionary<string, Sprite> portraitDict = new Dictionary<string, Sprite>();
    Dictionary<E_Class, Sprite> classIconDict = new Dictionary<E_Class, Sprite>();
    public Dictionary<E_Class, Sprite> ClassIconDict
    {
        get
        {
            return classIconDict;
        }
    }
    public Dictionary<string, Sprite> PortraitDict
    {
        get
        {
            return portraitDict;
        }
    }
    List<Sprite> CharacterSpriteList = new List<Sprite>();

}
public partial class GameStartManager : MonoBehaviour
{
    public HeroDatabase heroDatabase = new HeroDatabase();
    public List<HeroButton> buttonList = new List<HeroButton>();

    public void SaveHeroesDatabase()
    {
        StreamWriter streamWriter = new StreamWriter(new FileStream(Application.dataPath + "/Resources/XML/Hero_Data.xml", FileMode.Create), System.Text.Encoding.UTF8);
        XmlSerializer serializer = new XmlSerializer(typeof(HeroDatabase));
        XmlSerializer xmlSerializer = new XmlSerializer(heroDatabase.GetType());

        xmlSerializer.Serialize(streamWriter, heroDatabase);
        streamWriter.Close();
    }
    public void LoadHeroesDatabase()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(HeroDatabase));
        FileStream stream = new FileStream(Application.dataPath + "/Resources/XML/Hero_Data.xml", FileMode.Open);
        heroDatabase = serializer.Deserialize(stream) as HeroDatabase;
        stream.Close();
    }
}
public partial class GameStartManager : MonoBehaviour
{
    public HeroButton heroButtonPrefab;
    public GridLayoutGroup gridLayout;
    HeroButton AddCharacterButton(HeroInfo heroInfo)
    {
        HeroButton temp = Instantiate(heroButtonPrefab,gridLayout.transform);
        buttonList.Add(temp);
        temp.name = count.ToString();
        count++;
        temp.heroInfo = heroInfo;
        for (int index = 0; index < CharacterSpriteList.Count; index++)
        {
            if (CharacterSpriteList[index].name == (heroInfo.name))
            {
                temp.heroImage.sprite = CharacterSpriteList[index];
                temp.classIcon.sprite = classIconDict[heroInfo.heroClass];
                break;
            }
        }
        return temp;
    }
    void SetGridLayoutFit()
    {
        int countInGrid = gridLayout.transform.childCount;
        RectTransform rectTransform = gridLayout.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, ((countInGrid / 9)* 43+4));
    }
}
public partial class GameStartManager : MonoBehaviour
{
    int count = 0;
    public List<SelectionInfo> selectionInfoList = new List<SelectionInfo>(3);
    public List<SelectionSlot> selectionSlotList = new List<SelectionSlot>();
    public bool isSelectLeaderMode = false;
    public GameObject leaderSelectionModeHighlight;
    public Button startButton;

    public void SetLeader(int leaderIndex)
    {
        for(int index=0;index< selectionInfoList.Count; index++)
        {
            selectionInfoList[index].isLeader = false;
            selectionSlotList[index].SetLeader(false);
        }
        selectionInfoList[leaderIndex].isLeader = true;
        selectionSlotList[leaderIndex].SetLeader(true);
    }
    public void AddHero(HeroInfo heroInfo, HeroButton heroButton)
    {
        SelectionInfo addHeroInfo = new SelectionInfo();
        addHeroInfo.heroInfo = heroInfo;
        addHeroInfo.isLeader = false;
        addHeroInfo.heroButton = heroButton;

        if (selectionInfoList.Count==0)
        {
            selectionInfoList.Add(addHeroInfo);
            selectionInfoList[0].isLeader = true;
        }
        else
        {//클래스 비교후 삽입
            int insertIndex = 0;
            for (int index = 0; index < selectionInfoList.Count; index++)
            {
                if (addHeroInfo.heroInfo.heroClass> selectionInfoList[index].heroInfo.heroClass)
                {
                    insertIndex = index;
                    break;
                }
                else
                {
                    insertIndex++;
                }
            }
            selectionInfoList.Insert(insertIndex,addHeroInfo);
        }
        SyncAllList();
    }
    public void RemoveHero(HeroInfo heroInfo,HeroButton heroButton)
    {
        SelectionInfo removeHeroInfo = selectionInfoList.Find(info => info.heroButton == heroButton);
        selectionInfoList.Remove(removeHeroInfo);
        removeHeroInfo.heroButton.Canceled();
        if (selectionInfoList.Count>0)
        {
            if(removeHeroInfo.isLeader)
            {
                selectionInfoList[0].isLeader = true;
            }
        }


        SyncAllList();
    }
    void SyncAllList()
    {
        for(int index=0;index < 3;index++)
        {
            if(index<selectionInfoList.Count)
            {
                selectionSlotList[index].SetHero(selectionInfoList[index].heroInfo);
                selectionSlotList[index].heroButton = selectionInfoList[index].heroButton;
                selectionSlotList[index].SetLeader(selectionInfoList[index].isLeader);
            }
            else
            {
                selectionSlotList[index].Clear();
            }
        }
        if(selectionInfoList.Count>=3)
        {
            startButton.interactable = true;
        }
        else
        {
            startButton.interactable = false;
        }

    }
   
    public void ToggleSelectLeaderMode()
    {
        if(isSelectLeaderMode)
        {
            isSelectLeaderMode = false;
            leaderSelectionModeHighlight.SetActive(false);
        }
        else
        {
            isSelectLeaderMode = true;
            leaderSelectionModeHighlight.SetActive(true);
        }


    }
    public void SelectLeaderModeEnd()
    {
        isSelectLeaderMode = false;
        leaderSelectionModeHighlight.SetActive(false);
    }
}
public partial class GameStartManager : MonoBehaviour
{
    void Start()
    {
        if (Instance == null)
        {
            instance = this;
        }
        sprites = Resources.LoadAll<Sprite>("LobbySystem/Sprites/AllHero");
        List<Sprite> tempSpriteList = new List<Sprite>(sprites);
        foreach (Sprite sprite in tempSpriteList.FindAll(temp => temp.name[temp.name.Length - 1] == 'F'))
        {
            portraitDict.Add(sprite.name, sprite);
        }
        CharacterSpriteList = new List<Sprite>(tempSpriteList.FindAll(temp => temp.name[temp.name.Length - 1] != 'F'));

        tempSpriteList = new List<Sprite>(Resources.LoadAll<Sprite>("LobbySystem/Sprites/WindowUI"));
        tempSpriteList = tempSpriteList.FindAll(s => s.name.Substring(0, 4) == "Icon");
        classIconDict.Add(E_Class.Wizard, tempSpriteList.Find(s => s.name.Substring(4) == E_Class.Wizard.ToString()));
        classIconDict.Add(E_Class.Hunter, tempSpriteList.Find(s => s.name.Substring(4) == E_Class.Hunter.ToString()));
        classIconDict.Add(E_Class.Archer, tempSpriteList.Find(s => s.name.Substring(4) == E_Class.Archer.ToString()));
        classIconDict.Add(E_Class.Priest, tempSpriteList.Find(s => s.name.Substring(4) == E_Class.Priest.ToString()));
        classIconDict.Add(E_Class.Paladin, tempSpriteList.Find(s => s.name.Substring(4) == E_Class.Paladin.ToString()));
        classIconDict.Add(E_Class.Warrior, tempSpriteList.Find(s => s.name.Substring(4) == E_Class.Warrior.ToString()));

        LoadHeroesDatabase();
        foreach (HeroInfo heroInfo in heroDatabase.list)
        {
            if(heroInfo.canUse)
            {
                AddCharacterButton(heroInfo);
            }
        }
        SetGridLayoutFit();
        //SaveHeroesDatabase();


        foreach(ClassButton button in classList)
        {
            button.IsSelected(false);
        }
        selectedClass = classList[0];
        selectedClass.IsSelected(true);
    }
}
public partial class GameStartManager : MonoBehaviour
{
    public List<ClassButton> classList = new List<ClassButton>();
    ClassButton selectedClass;
    public void ClassSet(ClassButton classButton)
    {
        selectedClass.IsSelected(false);
        selectedClass = classButton;
        selectedClass.IsSelected(true);

        foreach(HeroButton heroButton in buttonList)
        {
            if(selectedClass.classInfo == E_Class.None)
            {
                heroButton.gameObject.SetActive(true);
            }
            else
            {
                if(heroButton.heroInfo.heroClass==classButton.classInfo)
                {
                    heroButton.gameObject.SetActive(true);
                }
                else
                {
                    heroButton.gameObject.SetActive(false);
                }
            }
        }
        SetGridLayoutFit();
    }
}
//게임 시작 버튼
public partial class GameStartManager : MonoBehaviour
{
    public void StartGame()
    {
        StageManager.selectedPartyMemberList = selectionInfoList;
        StageManager.StageName = "Winter";
        SceneManager.LoadScene("Stage");
    }
}

