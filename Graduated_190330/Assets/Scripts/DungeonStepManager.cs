using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonStepManager : Singletone<DungeonStepManager>
{
    // 던전 단계별로 어떤 몬스터가 등장하는지, 공격력과 체력, 보상 등은 얼마인지 셋팅
    public Dictionary<int, DungeonPattern> DungeonStepDic { get; private set; }

    void Start()
    {
        DungeonStepDic = new Dictionary<int, DungeonPattern>();
        SetStepData();
    }

    // 단수에 따라서 체력, 공격력 수치 변경
    void SetStepData()
    {
        int index = 0;
        foreach (var item in GameDataManager.Instance.DungeonPatternDataDic)
        {
            DungeonPattern pattern = item.Value.ShallowCopy() as DungeonPattern;
            // 보정값 계산
            int healthCor = pattern.EnemyHealth * GameDataManager.Instance.EnemyStatCorrectionDataDic[index].Correction;
            if (!pattern.SetEnemyHealth(healthCor))
            {
                Debug.LogError("invalid Health value! : " + healthCor);
                return;
            }

            // EnemyPattern customEnemyPattern = 
            // for (int i = 0; i < pattern.PatternList.Count; i++)
            // {
            //     pattern.SetEenemyStat()
            // }
            // if()

            DungeonStepDic.Add(index, pattern);
        }



    }
}
