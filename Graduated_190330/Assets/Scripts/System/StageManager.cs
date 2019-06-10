using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.SceneManagement;
public partial class StageManager : MonoBehaviour
{ 
    static StageManager instance;
    public static StageManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<StageManager>();
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "StageManager";
                    instance = container.AddComponent<StageManager>();
                }
            }
            return instance;
        }
    }
    public static List<SelectionInfo> selectedPartyMemberList = new List<SelectionInfo>(3);
    public static List<SelectionInfo> enemyPartyList = new List<SelectionInfo>(3);
    public static string StageName="Winter";

    public List<TestUnit> playerList = new List<TestUnit>();
    public List<TestUnit> enemyList = new List<TestUnit>();

    void LoadPlayerData()
    {
        if(selectedPartyMemberList.Count==0)
        {
            Debug.Log("선택된 용사 리스트가 비어잇습니다.");
        }
        else
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Assets/Resources/XML/Ingame_Data.xml");
            foreach (SelectionInfo selectedHero in selectedPartyMemberList)
            {
                foreach (XmlNode node in xmlDoc.DocumentElement)
                {
                    if (node["name"].InnerText == selectedHero.heroInfo.name)
                    {
                        TestUnit testUnit = Instantiate<TestUnit>(Resources.Load<TestUnit>("Hero/Test/" + node["name"].InnerText));
                        playerList.Add(testUnit);

                        foreach (E_StatType stat in Enum.GetValues(typeof(E_StatType)))
                        {
                            testUnit.StatManager.AddStat(stat, new StatFloat(stat, stat.ToString(), float.Parse(node[stat.ToString()].InnerText)));
                        }
                        testUnit.heroClass = (E_Class)Enum.Parse(typeof(E_Class), node["class"].InnerText);
                        testUnit.id = node["id"].InnerText;
                        testUnit.heroName = node["name"].InnerText;
                        testUnit.level = int.Parse(node["level"].InnerText);
                    }
                }
            }
        }
    }
    void LoadEnemyData()
    {
        if (enemyPartyList.Count == 0)
        {
            Debug.Log("선택된 용사 리스트가 비어잇습니다.");
        }
        else
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Assets/Resources/XML/Ingame_Data.xml");
            foreach (SelectionInfo enemy in enemyPartyList)
            {
                foreach (XmlNode node in xmlDoc.DocumentElement)
                {
                    if (node["name"].InnerText == enemy.heroInfo.name)
                    {
                        TestUnit testUnit = Instantiate<TestUnit>(Resources.Load<TestUnit>("Hero/Test/" + node["name"].InnerText));
                        playerList.Add(testUnit);

                        foreach (E_StatType stat in Enum.GetValues(typeof(E_StatType)))
                        {
                            testUnit.StatManager.AddStat(stat, new StatFloat(stat, stat.ToString(), float.Parse(node[stat.ToString()].InnerText)));
                        }
                        testUnit.heroClass = (E_Class)Enum.Parse(typeof(E_Class), node["class"].InnerText);
                        testUnit.id = node["id"].InnerText;
                        testUnit.heroName = node["name"].InnerText;
                        testUnit.level = int.Parse(node["level"].InnerText);
                    }
                }
            }
        }
    }
    void Start ()
    {
        if (!instance)
        {
            instance = this;
        }
    }
}
