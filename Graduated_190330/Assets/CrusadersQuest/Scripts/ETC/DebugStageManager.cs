using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;

namespace CrusadersQuestReplica
{
    public class DebugStageManager : MonoBehaviour
    {
        public List<Unit> playerUnitList = new List<Unit>();
        public List<Unit> enemyUnitList = new List<Unit>();
        void LoadPlayerData()
        {
            if (playerUnitList.Count == 0)
            {
                Debug.Log("선택된 용사 리스트가 비어잇습니다.");
            }
            else
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("Assets/Resources/XML/Ingame_Data.xml");
                foreach (Unit unit in playerUnitList)
                {
                    foreach (XmlNode node in xmlDoc.DocumentElement)
                    {
                        if (node["name"].InnerText == unit.name)
                        {
                            foreach (E_StatType stat in Enum.GetValues(typeof(E_StatType)))
                            {
                                //unit.statManager.AddStat(stat, new StatFloat(stat, stat.ToString(), float.Parse(node[stat.ToString()].InnerText)));
                            }
                            //unit.heroClass = (E_Class)Enum.Parse(typeof(E_Class), node["class"].InnerText);
                            //unit.id = node["id"].InnerText;
                            //unit.heroName = node["name"].InnerText;
                            //unit.level = int.Parse(node["level"].InnerText);
                        }
                    }
                }
            }
        }
        void LoadEnemyData()
        {
            if (playerUnitList.Count == 0)
            {
                Debug.Log("선택된 용사 리스트가 비어잇습니다.");
            }
            else
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("Assets/Resources/XML/Ingame_Data.xml");
                foreach (Unit unit in enemyUnitList)
                {
                    foreach (XmlNode node in xmlDoc.DocumentElement)
                    {
                        if (node["name"].InnerText == unit.name)
                        {
                            foreach (E_StatType stat in Enum.GetValues(typeof(E_StatType)))
                            {
                                //unit.statManager.AddStat(stat, new StatFloat(stat, stat.ToString(), float.Parse(node[stat.ToString()].InnerText)));
                            }
                            //unit.heroClass = (E_Class)Enum.Parse(typeof(E_Class), node["class"].InnerText);
                            //unit.id = node["id"].InnerText;
                            //unit.heroName = node["name"].InnerText;
                            //unit.level = int.Parse(node["level"].InnerText);
                        }
                    }
                }
            }
        }
        private void Start()
        {
            LoadPlayerData();
            LoadEnemyData();
        }
    }
}

