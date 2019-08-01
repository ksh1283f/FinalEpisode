using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonStepManager : Singletone<DungeonStepManager>
{
    // 던전 단계별로 어떤 몬스터가 등장하는지, 공격력과 체력, 보상 등은 얼마인지 셋팅
    public Dictionary<int, DungeonPattern> DungeonStepDic { get; private set; }

    void Awake()
    {
        DungeonStepDic = new Dictionary<int, DungeonPattern>();
        SetStepData();
    }

    // 단수에 따라서 체력, 공격력 수치 변경
    void SetStepData()
    {
        // todo 190725 추가된 던전 몬스터 데이터로 변경(단수마다 적용하도록)
        foreach (var item in GameDataManager.Instance.DungeonMonsterDataDic)
        {
            DungeonMonsterData monsterData = item.Value.ShallowCopy() as DungeonMonsterData;
            DungeonPattern pattern = GameDataManager.Instance.DungeonPatternDataDic[monsterData.MonsterId].ShallowCopy() as DungeonPattern;

            float cor = GameDataManager.Instance.EnemyStatCorrectionDataDic[monsterData.Id].HpCorrection;
            // 20 -> 1.20
            cor = cor / 100 + 1;   // 소수점으로 변경
            int healthCorrected = (int)(cor <= 1 ? pattern.EnemyHealth : cor * pattern.EnemyHealth);
            if (!pattern.SetEnemyHealth(healthCorrected))
            {
                Debug.LogError("invalid Health value! : " + healthCorrected);
                return;
            }

            DungeonStepDic.Add(monsterData.Id, pattern);
        }
    }
}