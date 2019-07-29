using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Graduate.GameData.UnitData;
using UnityEngine.SceneManagement;

public enum E_BattlePhase
{
    None,
    Init,
    ShowLogo,
    Battle,
    End,
}

public enum E_SkillResourceType
{
    Attack,
    Util,
    Defense,
}


public class BattleManager : Singletone<BattleManager>
{
    /*
     * 전투 순서
     * 1. 페이즈 로고 연출
     * 2. 캐스팅 ui를 제외한 ui 셋팅
     * 3. 전투
     *  3.1 몹이 다가옴
     *  3.2 몹 캐스팅
     * 전투 작용
     */

    public const int MAX_SKILL_RESOURCE_COUNT = 5;
    public Action<bool> OnGameEnd { get; set; }
    public Action<string, int, int, int> OnUpdatedUserInfoAfterGameEnd { get; set; }    //userName, teamLevel, gold, exp

    public Action OnExecuteInit { get; set; }   // 던전진행에 필요한 데이터 초기화
    public Action OnExecuteShowLogo { get; set; }   // 로고 보여주기
    public Action OnExecuteStartBattle { get; set; }    // 본격적인 전투 시작
    public Action OnExecuteBattleEnd { get; set; }
    public Action<List<UnitData>> OnUpdateUserStat { get; set; }
    public Action<float> OnDamagedPlayer { get; set; }
    public Action<string, float> OnUpdateEnemyHpBar { get; set; }
    public Action<float> OnAttackEnemy { get; set; }
    public Action<bool> OnCorrespondPattern { get; set; }

    public Action<EnemyPattern> OnExecuteMonsterCasting { get; set; }
    public Action OnCastingEnd { get; set; }
    public Action<float> OnCastProgress { get; set; }
    public Action<float> OnCalculatedRemainTime { get; set; }

    public int AttackResourceCount { get; private set; }    // 보유중인 공격 자원 개수
    public int UtilResourceCount { get; private set; }  // 보유중인 유틸 자원 개수
    public int DefenseResourceCount { get; private set; }   // 보유중인 방어 자원 개수
    public List<PhaseChecker> PhaseCheckerList = new List<PhaseChecker>();
    public E_PhaseType PhaseType { get; private set; }

    private E_BattlePhase battlePhase = E_BattlePhase.None;
    public E_BattlePhase BattlePhase
    {
        get { return battlePhase; }
        set
        {
            if (value == battlePhase)
                return;

            battlePhase = value;
            switch (battlePhase)
            {
                case E_BattlePhase.Init:
                    OnExecuteInit.Execute();
                    break;

                case E_BattlePhase.ShowLogo:
                    OnExecuteShowLogo.Execute();
                    break;

                case E_BattlePhase.Battle:
                    OnExecuteStartBattle.Execute();
                    break;
                case E_BattlePhase.End:
                    OnExecuteBattleEnd.Execute();
                    break;

            }
        }
    }

    private float remainTime = 0f;
    public float RemainTime
    {
        get { return remainTime; }
        set
        {
            if (value == remainTime)
                return;

            remainTime = value;
            OnCalculatedRemainTime.Execute(remainTime);
        }
    }

    public Graduate.Unit.Enemy.EnemyUnit nowEnemy { get; private set; }
    bool isCorrespondPattern = false;   // 패턴대응여부
    public float Cooltime { get; private set; }
    public bool IsCooldownComplite = true;
    public Action<float> OnStartCooldown { get; set; }

    void Start()
    {
        // todo 현재 선택돤 던전의 데이터를 바탕으로 시간 셋팅
        RemainTime = UserManager.Instance.SelectedDungeonMonsterData.LimitTime;
        OnExecuteInit += InitBattle;
        OnExecuteShowLogo += BattleUI.Instance.ShowBattleLogo;
        OnExecuteStartBattle += BattleUI.Instance.ShowBattleUI;
        OnExecuteStartBattle += StartBattle;
        OnExecuteBattleEnd += UserManager.Instance.StartLoadLobbyScene;
        OnUpdatedUserInfoAfterGameEnd += UserManager.Instance.SetUserInfo;

        AttackResourceCount = 0;
        UtilResourceCount = 0;
        DefenseResourceCount = 0;
        Cooltime = 1.5f; //todo 임시값: 플레이어 능력치에 맞게 조정필요
        OnExecuteMonsterCasting += (pattern) => { StartCoroutine(MonsterCast(pattern.CastTime)); };

        BattlePhase = E_BattlePhase.ShowLogo;
        PhaseCheckerList = FindObjectsOfType<PhaseChecker>().ToList();
        if (PhaseCheckerList != null)
        {
            for (int i = 0; i < PhaseCheckerList.Count; i++)
            {
                PhaseCheckerList[i].OnStartPhase += StartPhase;
            }
        }
    }

    void StartPhase(E_PhaseType phaseType)
    {
        PhaseType = phaseType;
    }

    #region 버블 생성 관련

    public void GenerateSkillResource(E_SkillResourceType skillResourceType)
    {
        if (PhaseType == E_PhaseType.None)
            return;

        if (!IsCooldownComplite)
            return;

        int createCount = 1;    // 만들어질 자원 변수
        int createdCount = 0;   // 만들어진 자원 변수

        switch (skillResourceType)
        {
            case E_SkillResourceType.Attack:
                if (AttackResourceCount == MAX_SKILL_RESOURCE_COUNT)
                    return;

                if (CharacterPropertyManager.Instance.SelectedProperty != null
                && CharacterPropertyManager.Instance.SelectedProperty.PropertyType == E_PropertyType.Atk)
                    createCount++;

                AttackResourceCount += createCount;
                if (this.AttackResourceCount >= MAX_SKILL_RESOURCE_COUNT)
                    this.AttackResourceCount = MAX_SKILL_RESOURCE_COUNT;

                createdCount = this.AttackResourceCount;
                break;

            case E_SkillResourceType.Util:
                if (UtilResourceCount == MAX_SKILL_RESOURCE_COUNT)
                    return;

                if (CharacterPropertyManager.Instance.SelectedProperty != null
                && CharacterPropertyManager.Instance.SelectedProperty.PropertyType == E_PropertyType.Util)
                    createCount++;

                UtilResourceCount += createCount;
                if (this.UtilResourceCount >= MAX_SKILL_RESOURCE_COUNT)
                    this.UtilResourceCount = MAX_SKILL_RESOURCE_COUNT;

                createdCount = UtilResourceCount;
                break;

            case E_SkillResourceType.Defense:
                if (DefenseResourceCount == MAX_SKILL_RESOURCE_COUNT)
                    return;

                if (CharacterPropertyManager.Instance.SelectedProperty != null
                && CharacterPropertyManager.Instance.SelectedProperty.PropertyType == E_PropertyType.Def)
                    createCount++;

                DefenseResourceCount += createCount;
                if (this.DefenseResourceCount >= MAX_SKILL_RESOURCE_COUNT)
                    this.DefenseResourceCount = MAX_SKILL_RESOURCE_COUNT;
                createdCount = DefenseResourceCount;
                break;
        }

        BattleUI.Instance.SetSkillResorce(skillResourceType, true, createdCount);
        OnStartCooldown.Execute(Cooltime);
        Debug.Log(skillResourceType + ": " + createCount);
    }

    public void DeleteSkillResource(E_SkillResourceType skillResourceType)
    {
        if (PhaseType == E_PhaseType.None)
            return;

        // 쿨타임인지 아닌지
        if (!IsCooldownComplite)
            return;

        int count = 0;
        switch (skillResourceType)
        {
            case E_SkillResourceType.Attack:
                if (AttackResourceCount == 0)
                    return;

                count = AttackResourceCount;

                Debug.Log(skillResourceType + "," + "used count" + ": " + count + ", remain count: " + AttackResourceCount);
                int damage = CalculateAttackDamage();
                nowEnemy.GetDamaged(damage);
                AttackResourceCount = 0;
                break;

            case E_SkillResourceType.Util:
                if (UtilResourceCount == 0)
                    return;

                count = UtilResourceCount;
                UtilResourceCount = 0;
                Debug.Log(skillResourceType + "," + "used count" + ": " + count + ", remain count: " + UtilResourceCount);
                if (thisPattern != null && thisPattern.SkillType == E_UserSkillType.Util)
                {
                    isCorrespondPattern = true;
                }
                // 패턴에 맞게 처리

                break;

            case E_SkillResourceType.Defense:
                if (DefenseResourceCount == 0)
                    return;

                count = DefenseResourceCount;
                DefenseResourceCount = 0;
                Debug.Log(skillResourceType + "," + "used count" + ": " + count + ", remain count: " + DefenseResourceCount);
                if (thisPattern != null && thisPattern.SkillType == E_UserSkillType.Defense)
                {
                    isCorrespondPattern = true;
                }

                break;
        }
        StartUnitAttackAni();
        BattleUI.Instance.SetSkillResorce(skillResourceType, false, count);
        OnStartCooldown.Execute(Cooltime);
        // 관련 함수 처리
    }

    public void DeleteSkillResource(E_UserSkillType skillType)
    {
        if (PhaseType == E_PhaseType.None)
            return;

        // 쿨타임인지 아닌지
        if (!IsCooldownComplite)
            return;

        int countFirst = 0;
        int countSecond = 0;
        switch (skillType)
        {
            case E_UserSkillType.AttackAndUtil:
                if (AttackResourceCount == 0)
                    return;

                if (UtilResourceCount == 0)
                    return;

                countFirst = AttackResourceCount;
                countSecond = UtilResourceCount;
                AttackResourceCount = 0;
                UtilResourceCount = 0;
                if (thisPattern != null && thisPattern.SkillType == E_UserSkillType.AttackAndUtil)
                {
                    isCorrespondPattern = true;
                }
                break;

        }
        StartUnitAttackAni();
        BattleUI.Instance.SetSkillResorce(E_SkillResourceType.Attack, false, countFirst);
        BattleUI.Instance.SetSkillResorce(E_SkillResourceType.Util, false, countSecond);
        OnStartCooldown.Execute(Cooltime);
    }
    #endregion

    #region 전투 진행
    public List<Graduate.Unit.Player.PlayerUnit> PlayerUnitList = new List<Graduate.Unit.Player.PlayerUnit>();
    public List<Graduate.Unit.Enemy.EnemyUnit> EnemyUnitList = new List<Graduate.Unit.Enemy.EnemyUnit>();
    public int aliveEnemyCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < EnemyUnitList.Count; i++)
            {
                if (EnemyUnitList[i].EnemyUnitState == Graduate.Unit.E_UnitState.Death)
                    continue;

                count++;
            }

            return count;
        }
    }
    #region 임시 데이터
    DungeonPattern dungeonPattern;
    EnemyPattern thisPattern;
    List<EnemyPattern> enemyPatterns;
    List<UnitData> unitDataList = new List<UnitData>();
    int MaxPlayerHealth = 0;
    int nowplayerHealth = 0;

    Graduate.Unit.E_UnitState playerState = Graduate.Unit.E_UnitState.None;
    public Graduate.Unit.E_UnitState PlayerState
    {
        get { return playerState; }
        set
        {
            if (value == playerState)
                return;

            playerState = value;
            switch (playerState)
            {
                case Graduate.Unit.E_UnitState.Death:
                    // 게임 끝
                    break;
            }
        }
    }
    #endregion

    void OnEnemyDeath(bool isDeath)
    {
        PlayerState = Graduate.Unit.E_UnitState.None;
        if (aliveEnemyCount == 0)
        {
            StopCoroutine(CalculateRemainCount());
            CalculateReward(true);
            OnGameEnd.Execute(true);
            isBattleEnd = true;
            BattlePhase = E_BattlePhase.End;
        }
        else
        {
            nowEnemy = null;
            for (int i = 0; i < EnemyUnitList.Count; i++)
            {
                if (EnemyUnitList[i].EnemyUnitState == Graduate.Unit.E_UnitState.Death)
                    continue;

                if (nowEnemy == null)
                {
                    nowEnemy = EnemyUnitList[i];
                    continue;
                }

                if (nowEnemy.Sequence > EnemyUnitList[i].Sequence)
                    nowEnemy = EnemyUnitList[i];
            }
            StartBattle();

        }

    }

    void InitBattle()
    {
        // TODO: unit 데이터 초기화
        // TODO: dungeon 데이터 초기화
        PlayerUnitList = FindObjectsOfType<Graduate.Unit.Player.PlayerUnit>().ToList();
        EnemyUnitList = FindObjectsOfType<Graduate.Unit.Enemy.EnemyUnit>().ToList();

        //임시 데이터
        #region 임시 데이터
        for (int i = 0; i < PlayerUnitList.Count; i++)
        {
            //todo 새로운 생성자 사용하기
            // UnitData(int id, int hp, int atk, int def, 
            //      int cri, int spd, string iconName, E_CharacterType characterType)
            UnitData unitData = new UnitData(50, 10, 10, string.Empty);
            unitDataList.Add(unitData);
            PlayerUnitList[i].HP = unitData.Hp;
            PlayerUnitList[i].Atk = unitData.Atk;
            PlayerUnitList[i].Def = unitData.Def;
            MaxPlayerHealth += unitData.Hp;
        }
        nowplayerHealth = MaxPlayerHealth;
        OnUpdateUserStat.Execute(unitDataList);

        enemyPatterns = new List<EnemyPattern>();
        enemyPatterns.Add(new EnemyPattern(0, "고통", E_UserSkillType.Defense, 4f, "agony", 10));
        enemyPatterns.Add(new EnemyPattern(1, "부패", E_UserSkillType.Util, 4f, "curruption", 10));
        enemyPatterns.Add(new EnemyPattern(2, "생명력 착취", E_UserSkillType.Defense, 4f, "생명력 흡수", 8));
        enemyPatterns.Add(new EnemyPattern(3, "혼돈의 화살", E_UserSkillType.AttackAndUtil, 4f, "caos balt", 30));
        enemyPatterns.Add(new EnemyPattern(4, "유령 출몰", E_UserSkillType.Util, 4f, "Haunt", 20));
        enemyPatterns.Add(new EnemyPattern(6, "황천의 차원문", E_UserSkillType.AttackAndUtil, 5f, "Nether portal", 80));

        dungeonPattern = new DungeonPattern("Wizard", 1000, enemyPatterns, "잡몹스킬", 5f);
        for (int i = 0; i < EnemyUnitList.Count; i++)
        {
            EnemyUnitList[i].HP = dungeonPattern.EnemyHealth;
            EnemyUnitList[i].OnDamagedEnemy += CalculatedEnemyDamaged;
            EnemyUnitList[i].OnDeathEnemy += OnEnemyDeath;
        }

        Graduate.Unit.Enemy.EnemyUnit enemy = null;
        for (int i = 0; i < EnemyUnitList.Count; i++)
        {
            if (enemy == null)
            {
                enemy = EnemyUnitList[i];
                continue;
            }

            if (enemy.Sequence > EnemyUnitList[i].Sequence)
                enemy = EnemyUnitList[i];
        }

        nowEnemy = enemy;
        float hpValue = nowEnemy.HP / dungeonPattern.EnemyHealth;
        OnUpdateEnemyHpBar.Execute(dungeonPattern.EnemyName, hpValue);


        #endregion
        BattlePhase = E_BattlePhase.Battle;
    }

    void StartCountTime()
    {
        StopCoroutine(CalculateRemainCount());
        StartCoroutine(CalculateRemainCount());
    }

    IEnumerator CalculateRemainCount()
    {
        while (!isBattleEnd)
        {
            // todo 190727 던전별 데이터에 제한시간을 추가, 받아와서 처리해야함
            if (RemainTime < 0)
            {
                isBattleEnd = true;
                // 기존의 게임 끝났을때(플레이어가 죽었을 때)의 처리 -> 참고
                // if (nowplayerHealth <= 0)
                // {
                //     nowplayerHealth = 0;
                //     PlayerState = Graduate.Unit.E_UnitState.Death;
                //     CalculateReward(false);
                //     OnGameEnd.Execute(false);
                //     isBattleEnd = true;
                //     BattlePhase = E_BattlePhase.End;
                // }
                yield break;
            }

            RemainTime -= 1;
            yield return new WaitForSeconds(1f);
        }
    }

    void StartBattle()
    {
        StartCountTime();
        StopCoroutine(Battle());
        StartCoroutine(Battle());
    }

    public bool IsEncounterEnemy = false;
    public Graduate.Unit.Enemy.EnemyUnit Target;
    public bool isBattleEnd = false;
    IEnumerator Battle()
    {
        while (!isBattleEnd)
        {
            Target = nowEnemy;
            IsEncounterEnemy = false;

            // 1.아군 앞으로
            ChangeUnitsState(Graduate.Unit.E_UnitState.Move);

            // 2.적 조우
            while (!IsEncounterEnemy)
                yield return null;

            while (PhaseType == E_PhaseType.None)
                yield return null;

            ChangeUnitsState(Graduate.Unit.E_UnitState.Attack);

            // 3.적 캐스팅
            int index = 0;
            // 적이 죽거나 내가죽거나
            while (Target.EnemyUnitState != Graduate.Unit.E_UnitState.Death)
            {
                if (isBattleEnd)
                    yield break;

                yield return new WaitForSeconds(dungeonPattern.PatternTerm);
                thisPattern = enemyPatterns[index];
                OnExecuteMonsterCasting.Execute(thisPattern);
                yield return new WaitForSeconds(thisPattern.CastTime);


                // 4.대응
                bool result = false;
                if (isCorrespondPattern)
                {
                    result = true;
                    isCorrespondPattern = false;
                }
                else
                {
                    // 지 입기
                    CalculatedPlayerDamaged(thisPattern.Damage);
                    Debug.Log("[Test] Damage from monster: " + thisPattern.Damage);
                }
                OnCorrespondPattern.Execute(result);

                // 5.처리
                if (index + 1 <= enemyPatterns.Count - 1)
                    index++;
                else
                    index = 0;  // 처음으로
            }

            yield return null;
        }
    }

    IEnumerator MonsterCast(float castTime)
    {
        float startTime = 0;
        float progress = 0;
        while (progress < 1)
        {
            if (nowEnemy != null && nowEnemy.EnemyUnitState == Graduate.Unit.E_UnitState.Death)
            {
                OnCastingEnd.Execute();
                yield break;
            }

            startTime += Time.deltaTime / castTime;
            progress = Mathf.Lerp(0, 1, startTime);

            OnCastProgress.Execute(progress);
            yield return null;
        }

        OnCastingEnd.Execute();
    }

    void ChangeUnitsState(Graduate.Unit.E_UnitState unitState)
    {
        Debug.LogError(gameObject.name + unitState);
        if (PlayerUnitList == null)
        {
            Debug.LogError("PlayerUnitList is null");
            return;
        }

        for (int i = 0; i < PlayerUnitList.Count; i++)
            PlayerUnitList[i].PlayerUnitState = unitState;
    }

    void StartUnitAttackAni()
    {
        if (PlayerUnitList == null)
        {
            Debug.LogError("PlayerUnitList is null");
            return;
        }

        for (int i = 0; i < PlayerUnitList.Count; i++)
            PlayerUnitList[i].StartAniAttack();
    }
    #endregion

    int CalculateAttackDamage()
    {
        // 1. 버블 소모값 + 공격력 + 크리확률
        // 버블대미지는 공격력 총합에 비례
        int calculatedDamage = 0;
        for (int i = 0; i < unitDataList.Count; i++)
            calculatedDamage += unitDataList[i].Atk;

        calculatedDamage += AttackResourceCount * calculatedDamage;

        float cri = UnityEngine.Random.Range(0f, 1f);
        float criValue = 0f;    // 크리확률 받아오는 작업 필요
        if (criValue < cri)
            calculatedDamage *= 2;
        Debug.Log("[Test]Damage from player: " + calculatedDamage);
        return calculatedDamage;
    }

    void CalculatedEnemyDamaged(int damge)
    {
        if (nowEnemy == null)
            return;

        if (dungeonPattern == null)
            return;

        if (nowEnemy.EnemyUnitState == Graduate.Unit.E_UnitState.Death)
            return;

        //ui 갱신
        float enemyHealthPer = (float)nowEnemy.HP / (float)dungeonPattern.EnemyHealth;
        if (enemyHealthPer <= 0)
            enemyHealthPer = 0;
        OnAttackEnemy.Execute(enemyHealthPer);
    }

    void CalculatedPlayerDamaged(int damage)
    {
        nowplayerHealth -= damage;
        if (nowplayerHealth <= 0)
        {
            StopCoroutine(CalculateRemainCount());
            nowplayerHealth = 0;
            PlayerState = Graduate.Unit.E_UnitState.Death;
            CalculateReward(false);
            OnGameEnd.Execute(false);
            isBattleEnd = true;
            BattlePhase = E_BattlePhase.End;
        }

        float playerHealthPer = (float)nowplayerHealth / (float)MaxPlayerHealth;
        OnDamagedPlayer.Execute(playerHealthPer);
    }

    void CalculateReward(bool isClear)
    {
        // <bool,string, int, int>
        string userName = UserManager.Instance.UserInfo.UserName;
        int teamLevel = UserManager.Instance.UserInfo.TeamLevel;
        int gold = UserManager.Instance.UserInfo.Gold;
        int exp = UserManager.Instance.UserInfo.Exp;
        //아이템?

        if (isClear)
        {
            gold += 100;
            exp += 20;
            if (exp >= 100)    // todo exp 테이블로 값 맞추기 필요
            {
                int tempExp = exp;
                for (int i = 0; i < tempExp / 100; i++)
                {
                    teamLevel++;
                    exp -= 100;
                }
            }
        }
        else
        {

        }
        OnUpdatedUserInfoAfterGameEnd.Execute(userName, teamLevel, exp, gold);
        OnGameEnd.Execute(isClear);

    }


}
