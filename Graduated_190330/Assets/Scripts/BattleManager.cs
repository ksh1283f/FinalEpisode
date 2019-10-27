using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Graduate.GameData.UnitData;
using UnityEngine;
using UnityEngine.SceneManagement;
using Graduate.Unit.Player;
using Graduate.Unit.Enemy;

public enum E_BattlePhase {
    None,
    Init,
    ShowLogo,
    Battle,
    End,
}

public enum E_SkillResourceType {
    Attack,
    Util,
    Defense,
}

public class BattleManager : Singletone<BattleManager> {
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
    public Action<bool, RewardData> OnGameEnd { get; set; }
    public Action<string, int, int, int> OnUpdatedUserInfoAfterGameEnd { get; set; } //userName, teamLevel, gold, exp

    public Action OnExecuteInit { get; set; } // 던전진행에 필요한 데이터 초기화
    public Action OnExecuteShowLogo { get; set; } // 로고 보여주기
    public Action OnExecuteStartBattle { get; set; } // 본격적인 전투 시작
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
    public Action OnUpdatedPlayerSkill{get; set;}

    public int AttackResourceCount { get; private set; } // 보유중인 공격 자원 개수
    public int UtilResourceCount { get; private set; } // 보유중인 유틸 자원 개수
    public int DefenseResourceCount { get; private set; } // 보유중인 방어 자원 개수
    public int AllAtk = 0;
    public int AllDef = 0;
    public int AllCri = 0;
    public int AllSpd = 0;
    public List<PhaseChecker> PhaseCheckerList = new List<PhaseChecker> ();
    public E_PhaseType PhaseType { get; private set; }

    [SerializeField] private E_BattlePhase battlePhase = E_BattlePhase.None;
    public E_BattlePhase BattlePhase {
        get { return battlePhase; }
        set {
            if (value == battlePhase)
                return;

            battlePhase = value;
            switch (battlePhase) {
                case E_BattlePhase.Init:
                    OnExecuteInit.Execute ();
                    break;

                case E_BattlePhase.ShowLogo:
                    OnExecuteShowLogo.Execute ();
                    break;

                case E_BattlePhase.Battle:
                    OnExecuteStartBattle.Execute ();
                    break;
                case E_BattlePhase.End:
                    OnExecuteBattleEnd.Execute ();
                    break;

            }
        }
    }

    private float remainTime = 0f;
    public float RemainTime {
        get { return remainTime; }
        set {
            if (value == remainTime)
                return;

            remainTime = value;
            OnCalculatedRemainTime.Execute (remainTime);
        }
    }

    [SerializeField]private Graduate.Unit.Enemy.EnemyUnit nowEnemy;
    public Graduate.Unit.Enemy.EnemyUnit NowEnemy 
    {
         get { return nowEnemy; }
         private set
         {
             if(value == nowEnemy)
                return;

            nowEnemy = value;

            // todo 적바뀔때마다 해줘야 하는 것들.. 체력바 갱신등등
         } 
    }
    bool isCorrespondPattern = false; // 패턴대응여부
    public float Cooltime { get; private set; }
    public bool IsCooldownComplite = true;
    public bool IsFirstPropertyCooldownComplite = true;
    public bool IsSecondPropertyCooldownComplite = true;
    public RewardData thisBattleRewardData { get; private set; }
    public Action<float> OnStartCooldown { get; set; }
    BattleBuff ActivatedBattleBuff;

    [SerializeField] List<Transform> playerPosList = new List<Transform>();
    [SerializeField] List<Graduate.Unit.Player.PlayerUnit> playerObjList = new List<Graduate.Unit.Player.PlayerUnit>();
    [SerializeField] List<Transform> EnemyPosList = new List<Transform>();
    

    Follow mainFollowCam;
    Coroutine monsterCasting;
    bool shouldGoToNext =false;
    
    // none -> showLogo -> init -> Battle -> end
    void Start () {
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
        OnExecuteMonsterCasting += (pattern) => { monsterCasting = StartCoroutine (MonsterCast (pattern.CastTime)); };
        mainFollowCam = Camera.main.GetComponent<Follow>();

        int index = 0;
        bool isContainedWarrior = false;
        foreach (var item in UserManager.Instance.UserInfo.SelectedUnitDic) {
            
            PlayerUnit player=null;
            switch (item.Value.CharacterType)
            {
                case E_CharacterType.Warrior:
                case E_CharacterType.Mage:
                case E_CharacterType.Warlock:
                case E_CharacterType.Rogue:
                    player = Instantiate(playerObjList.Find(x => {return x.CharacterType == item.Value.CharacterType;}),playerPosList[index].position,playerPosList[index].rotation);
                    break;
            }

            if(player == null)
            {
                Debug.LogError("Player is null");
                return;
            }
            if( player.CharacterType == E_CharacterType.Warrior && !isContainedWarrior)
                isContainedWarrior = true;

            player.SetCharacterType (item.Value.CharacterType);
            unitDataList.Add (item.Value);
            player.HP = item.Value.Hp;
            player.Atk = item.Value.Atk;
            AllAtk += item.Value.Atk;
            AllDef += item.Value.Def;
            AllCri += item.Value.Cri;
            player.Def = item.Value.Def;
            MaxPlayerHealth += item.Value.Hp;
            player.Id = item.Key;

            // 특성 체크ㅋ    
            if (CharacterPropertyManager.Instance.SelectedUtilProperty != null
            && CharacterPropertyManager.Instance.SelectedUtilProperty.EffectType == E_PropertyEffectType.WarlockUtilMaserty_Healing
            && player.CharacterType == E_CharacterType.Warlock)
                AllCri += CharacterPropertyManager.Instance.SelectedUtilProperty.EffectValue;

            PlayerUnitList.Add(player);

            if(index == 0)
                mainFollowCam.target = player.transform;

            index++;
        }

        if(CharacterPropertyManager.Instance.SelectedUtilProperty!= null 
            && CharacterPropertyManager.Instance.SelectedUtilProperty.EffectType == E_PropertyEffectType.WarriorUtilMaserty_AdditionalDefense
            && isContainedWarrior)
        {
            int increasedValue = (int)((float)AllDef*((float)CharacterPropertyManager.Instance.SelectedUtilProperty.EffectValue/100f));
            AllDef += increasedValue;
        }

        OnUpdatedPlayerSkill.Execute();

        BattlePhase = E_BattlePhase.Init;
        PhaseCheckerList = FindObjectsOfType<PhaseChecker> ().ToList ();
        if (PhaseCheckerList != null) {
            for (int i = 0; i < PhaseCheckerList.Count; i++) {
                PhaseCheckerList[i].OnStartPhase += StartPhase;
            }
        }
    }

        void InitBattle () {
        /*
            - 자신의 출전 용병 정보들 
            - 선택된 dungeonMonsterData로 단수, 몬스터 정보, 제한시간 찾기
            - 단수에 맞는 몬스터 출현 정보
            - 단수에 따른 몬스터 스탯 보정
            - 보상정보 갱신
        */

        nowplayerHealth = MaxPlayerHealth;
        OnUpdateUserStat.Execute (unitDataList);

        // 2. 적 정보
        DungeonMonsterData dungeonMonsterData = UserManager.Instance.SelectedDungeonMonsterData;
        thisBattleRewardData = GameDataManager.Instance.RewardDataDic[dungeonMonsterData.Id];
        
        // 데이터에 맞게 가져오기
        int dataLength =dungeonMonsterData.MinionMonsterIds.Count+1;
        for (int i = 0; i < dataLength; i++)   // +1은 보스몹
        {
            // 적 생성
            string enemyObjPath = string.Empty;
            int enemyId = -1;
            bool isBoss=false;
            
            if(i == dataLength-1)   // 보스몹의 경우
            {
                enemyId = dungeonMonsterData.BossMonsterId;
                isBoss = true;
            }
            else
                enemyId = dungeonMonsterData.MinionMonsterIds[i];    
            
            if(enemyId == -1)
            {
                Debug.LogError("enemyId is null");
                return;
            }

            DungeonPattern patternData = GameDataManager.Instance.DungeonPatternDataDic[enemyId].ShallowCopy() as DungeonPattern;
            EnemyStatCorrectionData corData = GameDataManager.Instance.EnemyStatCorrectionDataDic[dungeonMonsterData.Id];
            enemyObjPath = patternData.PrefabPath;
            EnemyUnit enemy = Instantiate((GameObject)Resources.Load(enemyObjPath)).GetComponent<EnemyUnit>();
            enemy.transform.position = EnemyPosList[i].position;
            enemy.transform.rotation = EnemyPosList[i].rotation;
            if(isBoss)
                enemy.IsBoss = true;

            EnemyUnitList.Add(enemy);

            // 적 능력치 셋팅(단수에 맞게)
            float correctedHealth = (float) corData.HpCorrection / 100 + 1;
            int healthCorrected = (int) (correctedHealth <= 1 ? patternData.EnemyHealth : correctedHealth * patternData.EnemyHealth);
            if (!patternData.SetEnemyHealth (healthCorrected)) {
                Debug.LogError ("Invalid result, healthCorrected value: " + healthCorrected);
                BattlePhase = E_BattlePhase.End;
                return;
            }

            for (int j= 0; j < patternData.PatternList.Count; j++) 
            {
                EnemyPattern enemyPattern = patternData.PatternList[i];
                float corDamage = (float) corData.AtkCorrection / 100 + 1;
                int damageCorrected = (int) (corDamage <= 1 ? enemyPattern.Damage : enemyPattern.Damage * corDamage);
                enemyPattern.SetElements (EnemyPattern.E_ElementType.Damage, damageCorrected);
                if (!patternData.SetEnemyStat (enemyPattern)) {
                    Debug.LogError ("Invalid result, damageCorrected value: " + damageCorrected);
                    BattlePhase = E_BattlePhase.End;
                    return;
                }
            }

            enemy.Id = patternData.Id;
            enemy.HP = patternData.EnemyHealth;
            enemy.OnDamagedEnemy += CalculatedEnemyDamaged;
            enemy.OnDeathEnemy += OnEnemyDeath;
            enemy.Sequence = (E_EnemySequence)i;
        }

        SetTargetEnemy();
        // BattlePhase = E_BattlePhase.Battle;
        BattlePhase = E_BattlePhase.ShowLogo;
    }

    Graduate.Unit.Enemy.EnemyUnit SetTargetEnemy()
    {
        Graduate.Unit.Enemy.EnemyUnit tempEnemy = null;
        for (int i = 0; i < EnemyUnitList.Count; i++) {
            if(EnemyUnitList[i] == null || EnemyUnitList[i].EnemyUnitState == Graduate.Unit.E_UnitState.Death)
            {
                continue;
            }
                
            if (tempEnemy == null) {
                tempEnemy = EnemyUnitList[i];
                continue;
            }

            if (tempEnemy.Sequence > EnemyUnitList[i].Sequence)
            {
                tempEnemy = EnemyUnitList[i];
            }
                
        }

        // todo 타겟이 갱신될때마다 해줄 필요
        NowEnemy = tempEnemy;
        dungeonPattern = GameDataManager.Instance.DungeonPatternDataDic[NowEnemy.Id];
        if(dungeonPattern == null)
        {
            Debug.LogError("dungeonPattern is null");
            return null;
        }

        float hpValue = NowEnemy.HP / dungeonPattern.EnemyHealth;
        OnUpdateEnemyHpBar.Execute (dungeonPattern.EnemyName, hpValue);
        return tempEnemy;
    }

    void StartPhase (E_PhaseType phaseType) {
        PhaseType = phaseType;
    }


    public void GenerateSkillResource (E_SkillResourceType skillResourceType) {
        if (PhaseType == E_PhaseType.None)
            return;

        if (!IsCooldownComplite)
            return;

        int createCount = 1; // 만들어질 자원 변수
        int createdCount = 0; // 만들어진 자원 변수

        switch (skillResourceType) {
            case E_SkillResourceType.Attack:
                if (AttackResourceCount == MAX_SKILL_RESOURCE_COUNT)
                    return;

                if (CharacterPropertyManager.Instance.SelectedCommonProperty != null &&
                    CharacterPropertyManager.Instance.SelectedCommonProperty.EffectType == E_PropertyEffectType.AdditionalAtkResource)
                    createCount++;

                AttackResourceCount += createCount;
                if (this.AttackResourceCount >= MAX_SKILL_RESOURCE_COUNT)
                    this.AttackResourceCount = MAX_SKILL_RESOURCE_COUNT;

                createdCount = this.AttackResourceCount;
                break;

            case E_SkillResourceType.Util:
                if (UtilResourceCount == MAX_SKILL_RESOURCE_COUNT)
                    return;

                if (CharacterPropertyManager.Instance.SelectedCommonProperty != null &&
                    CharacterPropertyManager.Instance.SelectedCommonProperty.EffectType == E_PropertyEffectType.AdditionalUtilResource)
                    createCount++;

                UtilResourceCount += createCount;
                if (this.UtilResourceCount >= MAX_SKILL_RESOURCE_COUNT)
                    this.UtilResourceCount = MAX_SKILL_RESOURCE_COUNT;

                createdCount = UtilResourceCount;
                break;

            case E_SkillResourceType.Defense:
                if (DefenseResourceCount == MAX_SKILL_RESOURCE_COUNT)
                    return;

                if (CharacterPropertyManager.Instance.SelectedCommonProperty != null &&
                    CharacterPropertyManager.Instance.SelectedCommonProperty.EffectType == E_PropertyEffectType.AdditionalDefResource)
                    createCount++;

                DefenseResourceCount += createCount;
                if (this.DefenseResourceCount >= MAX_SKILL_RESOURCE_COUNT)
                    this.DefenseResourceCount = MAX_SKILL_RESOURCE_COUNT;
                createdCount = DefenseResourceCount;
                break;
        }

        BattleUI.Instance.SetSkillResorce (skillResourceType, true, createdCount);
        OnStartCooldown.Execute (Cooltime);
        EffectManager.Instance.StartEffect(E_SkillEffectType.UserGenerateResourceEffect,effectTrans.position);
        Debug.Log (skillResourceType + ": " + createCount);
    }

    public void DeleteSkillResource (E_SkillResourceType skillResourceType) {
        if (PhaseType == E_PhaseType.None)
            return;

        // 쿨타임인지 아닌지
        if (!IsCooldownComplite)
            return;

        int count = 0;
        switch (skillResourceType) {
            case E_SkillResourceType.Attack:
                if (AttackResourceCount == 0)
                    return;

                count = AttackResourceCount;

                Debug.Log (skillResourceType + "," + "used count" + ": " + count + ", remain count: " + AttackResourceCount);
                int damage = CalculateAttackDamage ();
                NowEnemy.GetDamaged (damage);
                AttackResourceCount = 0;
                break;

            case E_SkillResourceType.Util:
                if (UtilResourceCount == 0)
                    return;

                count = UtilResourceCount;
                UtilResourceCount = 0;
                Debug.Log (skillResourceType + "," + "used count" + ": " + count + ", remain count: " + UtilResourceCount);
                if (thisPattern != null && thisPattern.SkillType == E_UserSkillType.Util) {
                    isCorrespondPattern = true;
                }
                // 패턴에 맞게 처리
                EffectManager.Instance.StartEffect(E_SkillEffectType.UserUseUtilSkillEffect,effectTrans.position);
                break;

            case E_SkillResourceType.Defense:
                if (DefenseResourceCount == 0)
                    return;

                count = DefenseResourceCount;
                DefenseResourceCount = 0;
                Debug.Log (skillResourceType + "," + "used count" + ": " + count + ", remain count: " + DefenseResourceCount);
                if (thisPattern != null && thisPattern.SkillType == E_UserSkillType.Defense) {
                    isCorrespondPattern = true;
                }
                EffectManager.Instance.StartEffect(E_SkillEffectType.UserUseDefendSkillEffect,effectTrans.position);
                break;
        }
        StartUnitAttackAni ();
        BattleUI.Instance.SetSkillResorce (skillResourceType, false, count);
        OnStartCooldown.Execute (Cooltime);
        // 관련 함수 처리
    }

    public void DeleteSkillResource (E_UserSkillType skillType) {
        if (PhaseType == E_PhaseType.None)
            return;

        // 쿨타임인지 아닌지
        if (!IsCooldownComplite)
            return;

        int countFirst = 0;
        int countSecond = 0;
        switch (skillType) {
            case E_UserSkillType.AttackAndUtil:
                if (AttackResourceCount == 0)
                    return;

                if (UtilResourceCount == 0)
                    return;

                countFirst = AttackResourceCount;
                countSecond = UtilResourceCount;
                AttackResourceCount = 0;
                UtilResourceCount = 0;
                if (thisPattern != null && thisPattern.SkillType == E_UserSkillType.AttackAndUtil) {
                    isCorrespondPattern = true;
                    EffectManager.Instance.StartEffect(E_SkillEffectType.UserUseComplexSkillEffect,effectTrans.position);
                }
                break;

        }
        StartUnitAttackAni ();
        BattleUI.Instance.SetSkillResorce (E_SkillResourceType.Attack, false, countFirst);
        BattleUI.Instance.SetSkillResorce (E_SkillResourceType.Util, false, countSecond);
        OnStartCooldown.Execute (Cooltime);
    }

    public List<Graduate.Unit.Player.PlayerUnit> PlayerUnitList = new List<Graduate.Unit.Player.PlayerUnit> ();
    public List<Graduate.Unit.Enemy.EnemyUnit> EnemyUnitList = new List<Graduate.Unit.Enemy.EnemyUnit> ();
    public int aliveEnemyCount {
        get {
            int count = 0;
            for (int i = 0; i < EnemyUnitList.Count; i++) {
                if (EnemyUnitList[i].EnemyUnitState == Graduate.Unit.E_UnitState.Death)
                    continue;

                count++;
            }

            return count;
        }
    }
    
    DungeonPattern dungeonPattern;
    EnemyPattern thisPattern;
    List<UnitData> unitDataList = new List<UnitData> ();
    [SerializeField]int MaxPlayerHealth = 0;
    [SerializeField]int nowplayerHealth = 0;
    // [SerializeField] 

    Graduate.Unit.E_UnitState playerState = Graduate.Unit.E_UnitState.None;
    public Graduate.Unit.E_UnitState PlayerState {
        get { return playerState; }
        set {
            if (value == playerState)
                return;

            playerState = value;
            switch (playerState) {
                case Graduate.Unit.E_UnitState.Death:
                    // 게임 끝
                    break;
            }
        }
    }

    void OnEnemyDeath (bool isDeath) {
        PlayerState = Graduate.Unit.E_UnitState.None;
        if(monsterCasting != null) 
        {
            Debug.LogError("StopCoroutine(monsterCasting);");
            StopCoroutine(monsterCasting);
        }
            
        OnCastingEnd.Execute();
        if (aliveEnemyCount == 0) {
            StopCoroutine (CalculateRemainCount ());
            // ui상으로 보여주기

            OnGameEnd.Execute (true, thisBattleRewardData);
            CalculateReward (true);
            isBattleEnd = true;
            //BattlePhase = E_BattlePhase.End;
        } else {
            NowEnemy = null;
            for (int i = 0; i < EnemyUnitList.Count; i++) {
                // if (EnemyUnitList[i].EnemyUnitState == Graduate.Unit.E_UnitState.Death)
                //     continue;

                if (NowEnemy == null) {
                    // NowEnemy = EnemyUnitList[i];
                    // float hpValue = nowEnemy.HP
                    SetTargetEnemy();
                    // NowEnemy = enemy;
                    // float hpValue = NowEnemy.HP / dungeonPattern.EnemyHealth;
                    // OnUpdateEnemyHpBar.Execute (dungeonPattern.EnemyName, hpValue);
                    continue;
                }

                if (NowEnemy.Sequence > EnemyUnitList[i].Sequence)
                    NowEnemy = EnemyUnitList[i];
            }

            //StartBattle ();

        }

    }

    void StartHealOverTime()
    {
        if(CharacterPropertyManager.Instance.SelectedUtilProperty == null || CharacterPropertyManager.Instance.SelectedUtilProperty.EffectType != E_PropertyEffectType.MageUtilMaserty_HOT)
            return;

        int mageNum = 0;
        for (int i = 0; i < PlayerUnitList.Count; i++)
        {
            if(PlayerUnitList[i].CharacterType == E_CharacterType.Mage)
                mageNum++;
        }

        // 마법사가 없으면 종료
        if(mageNum  == 0)
            return;

        string path = CharacterPropertyManager.Instance.SelectedUtilProperty.SkillEffectPath;
        EffectManager.Instance.StartEffect(path, effectTrans.position);

        StopCoroutine(HealOverTime(mageNum));
        StartCoroutine(HealOverTime(mageNum));
    }

    IEnumerator HealOverTime(float mageNum)
    {
        
        while (!isBattleEnd || nowplayerHealth >=0) {

            yield return new WaitForSeconds (1f);
            float healValue = ((float)CharacterPropertyManager.Instance.SelectedUtilProperty.EffectValue * mageNum/100);
            healValue *= MaxPlayerHealth;
            Debug.LogError("HealValue: "+healValue);
            DamageFloatManager.Instance.ShowDamageFont (PlayerUnitList[0].gameObject, healValue, E_DamageType.Heal);
            nowplayerHealth += (int)healValue;
            if(nowplayerHealth > MaxPlayerHealth)   // 최대생명력을 넘지 않게
                nowplayerHealth = MaxPlayerHealth;

            float playerHealthPer = (float) nowplayerHealth / (float) MaxPlayerHealth;
            OnDamagedPlayer.Execute (playerHealthPer);
        }   
    }

    void StartCountTime () {
        StopCoroutine (CalculateRemainCount ());
        StartCoroutine (CalculateRemainCount ());
    }

    IEnumerator CalculateRemainCount () {
        while (!isBattleEnd) {
            if (RemainTime < 0) {
                isBattleEnd = true;
                // StopCoroutine(Battle());
                PlayerState = Graduate.Unit.E_UnitState.Death;
                OnGameEnd.Execute (false, thisBattleRewardData); // 결과 ui를 보여준다
                CalculateReward (false); // 결과를 계산하여 유저데이터에 갱신
                //BattlePhase = E_BattlePhase.End;    // 로비씬 로딩

                yield break;
            }

            RemainTime -= 1;
            yield return new WaitForSeconds (1f);
        }
    }

    void StartBattle () {
        StartCountTime ();
        // 마법사 패시브 
        StartHealOverTime();
        StopCoroutine (Battle ());
        StartCoroutine (Battle ());
    }

    public bool IsEncounterEnemy = false;
    public Graduate.Unit.Enemy.EnemyUnit Target;
    public bool isBattleEnd = false;
    IEnumerator Battle() {
        while (!isBattleEnd)
        {   
            Target = SetTargetEnemy();
            IsEncounterEnemy = false;

            // 1.아군 앞으로
            ChangeUnitsState (Graduate.Unit.E_UnitState.Move);
            Debug.Log("1. 캐릭터 이동");

            // 2.적 조우
            while (!IsEncounterEnemy)
                yield return null;
            Debug.Log("2. 적 조우");

            while (PhaseType == E_PhaseType.None)
                yield return null;

            ChangeUnitsState(Graduate.Unit.E_UnitState.Idle);
            Debug.Log("3. 공격대기");

            //todo 튜토리얼 유무: 튜토리얼중이면 대기하기
            // show tutorial simple ui
            TutorialSimpleUI tutorialUI = null;
            if (!UserManager.Instance.UserInfo.TutorialClearList[(int) E_SimpleTutorialType.Battle]) 
            {
                tutorialUI = UIManager.Instance.LoadUI (E_UIType.TutorialSimpleBattle) as TutorialSimpleUI;
                tutorialUI.Show (new string[] { "전투 소개" });
                tutorialUI.SetTutorialType (E_SimpleTutorialType.Battle);
                Debug.Log("3-1. 전투 튜토리얼");
            }
            // if tutorial simple ui is activated, wait for inactivating..
            while (tutorialUI != null && tutorialUI.gameObject.activeSelf)
                yield return null;

            ChangeUnitsState (Graduate.Unit.E_UnitState.Attack);
            Debug.Log("4. 전투 시작!");

            // 3.적 캐스팅
            int index = 0;
            // 적이 죽거나 내가죽거나
            while (Target.EnemyUnitState != Graduate.Unit.E_UnitState.Death)
            {
                Debug.Log("battle...");
                if (isBattleEnd)
                {
                    Debug.Log("전투 끝!");
                    yield break;
                }
                
                Debug.Log("4-1. 다음 패턴 대기시간 :"+dungeonPattern.PatternTerm);
                yield return new WaitForSeconds (dungeonPattern.PatternTerm);
                if(Target.EnemyUnitState == Graduate.Unit.E_UnitState.Death)
                    break;

                thisPattern = dungeonPattern.PatternList[index];
                OnExecuteMonsterCasting.Execute (thisPattern);
                Debug.Log("4-2. 캐스팅... :"+thisPattern.CastTime);
                yield return new WaitForSeconds (thisPattern.CastTime);
                if(Target.EnemyUnitState == Graduate.Unit.E_UnitState.Death)
                    break;

                // 4.대응
                bool result = false;
                if (isCorrespondPattern) {
                    result = true;
                    isCorrespondPattern = false;
                } else {
                    // 데미지 입기
                    if (Target.EnemyUnitState != Graduate.Unit.E_UnitState.Death)
                        CalculatedPlayerDamaged (thisPattern);
                }
                OnCorrespondPattern.Execute (result);

                // 5.처리
                if (index + 1 <= dungeonPattern.PatternList.Count - 1)
                    index++;
                else
                    index = 0; // 처음으로
            }

            yield return null;
        }
    }

    IEnumerator MonsterCast (float castTime)
    {
        float startTime = 0;
        float progress = 0;
        while (progress < 1) 
        {
            if (NowEnemy != null && NowEnemy.EnemyUnitState == Graduate.Unit.E_UnitState.Death) {
                OnCastingEnd.Execute ();
                yield break;
            }

            startTime += Time.deltaTime / castTime;
            progress = Mathf.Lerp (0, 1, startTime);

            OnCastProgress.Execute (progress);
            yield return null;
        }

        OnCastingEnd.Execute ();
    }
    [SerializeField] Transform effectTrans;
    void ChangeUnitsState (Graduate.Unit.E_UnitState unitState)
    {
        Debug.LogError (gameObject.name + unitState);
        if (PlayerUnitList == null) 
        {
            Debug.LogError ("PlayerUnitList is null");
            return;
        }

        for (int i = 0; i < PlayerUnitList.Count; i++)
            PlayerUnitList[i].PlayerUnitState = unitState;

        if(unitState == Graduate.Unit.E_UnitState.Attack)
        {
            effectTrans.position = new Vector3(PlayerUnitList[0].transform.position.x+1,
                            PlayerUnitList[0].transform.position.y,
                            PlayerUnitList[0].transform.position.z);
        }
            

    }

    void StartUnitAttackAni () {
        if (PlayerUnitList == null) {
            Debug.LogError ("PlayerUnitList is null");
            return;
        }

        for (int i = 0; i < PlayerUnitList.Count; i++)
            PlayerUnitList[i].StartAniAttack ();
    }

    int CalculateAttackDamage () {
        // 1. 버블 소모값 + 공격력 + 크리확률
        // 버블대미지는 공격력 총합에 비례
        int calculatedDamage = 0;
        // todo 초기화할때 계산한 총 데미지량을 이용하기(현재는 비효율적인 방법)
        for (int i = 0; i < unitDataList.Count; i++)
            calculatedDamage += unitDataList[i].Atk;

        calculatedDamage += AttackResourceCount * calculatedDamage;

        float cri = UnityEngine.Random.Range (0f, 1f);
        float criValue = (float)AllCri / 100; 
        if (criValue >= cri)
        {
            Debug.LogError("Critical!");
            calculatedDamage *= 2;
        }
            
        // todo 이펙트
        // 생흡 효과 유무
        if(ActivatedBattleBuff != null && ActivatedBattleBuff.BattleBuffType == E_BattleBuffType.DrainHealthPerDamage)
        {
            if(UserManager.Instance.UserInfo.SelectedUnitList.Find(x=>x.CharacterType == E_CharacterType.Warlock) != null)
            {
                float percentVal = (float)ActivatedBattleBuff.EffectValue/100f;
                float healValue = Convert.ToInt32((float)calculatedDamage*percentVal);
                
                Debug.LogError("Leech HealValue: "+healValue);
                DamageFloatManager.Instance.ShowDamageFont (PlayerUnitList[0].gameObject, healValue, E_DamageType.Heal);
                nowplayerHealth += (int)healValue;
                if(nowplayerHealth > MaxPlayerHealth)   // 최대생명력을 넘지 않게
                    nowplayerHealth = MaxPlayerHealth;

                float playerHealthPer = (float) nowplayerHealth / (float) MaxPlayerHealth;
                OnDamagedPlayer.Execute (playerHealthPer);

                ActivatedBattleBuff = null;

                //effect
                string path = CharacterPropertyManager.Instance.SelectedHealingProperty.SkillEffectPath;
                EffectManager.Instance.StartEffect(path, effectTrans.position);
            }
        }

        Debug.Log ("[Test]Damage from player: " + calculatedDamage);
        EffectManager.Instance.StartEffect(E_SkillEffectType.UserUseAttackSkillEffect,effectTrans.position);
        return calculatedDamage;
    }

    void CalculatedEnemyDamaged (int damage) {
        if (NowEnemy == null)
            return;

        if (dungeonPattern == null)
            return;

        if (NowEnemy.EnemyUnitState == Graduate.Unit.E_UnitState.Death)
            return;

        // todo  이펙트
        // 뎀증 특성 유무 계산

        //ui 갱신
        DamageFloatManager.Instance.ShowDamageFont (NowEnemy.gameObject, damage, E_DamageType.Normal);
        float enemyHealthPer = (float) NowEnemy.HP / (float) dungeonPattern.EnemyHealth;
        if (enemyHealthPer <= 0)
            enemyHealthPer = 0;
        OnAttackEnemy.Execute (enemyHealthPer);
    }

    void CalculatedPlayerDamaged (EnemyPattern pattern) {
        // todo 뎀감 계산
        int damage = pattern.Damage;
        int finalDamage = damage;
        finalDamage -= AllDef / 10; // 캐릭터들 방어력 적용

        string skillEffectPath = pattern.SkillEffectPath;
        EffectManager.Instance.StartEffect(skillEffectPath, effectTrans.position);
        
        // todo 뎀감 특성 유무 계산
        if(ActivatedBattleBuff != null)
        {
            switch (ActivatedBattleBuff.BattleBuffType)
            {
                case E_BattleBuffType.DecreasDamage:
                    float percentValue = (float)ActivatedBattleBuff.EffectValue/100f;
                    string decreaseEffectPath = CharacterPropertyManager.Instance.SelectedHealingProperty.SkillEffectPath;
                    EffectManager.Instance.StartEffect(decreaseEffectPath, effectTrans.position);
                    finalDamage = Convert.ToInt32((float)finalDamage*percentValue);
                    break;

                case E_BattleBuffType.Invincible:
                    string InvinciblePath = CharacterPropertyManager.Instance.SelectedHealingProperty.SkillEffectPath;
                    EffectManager.Instance.StartEffect(InvinciblePath, effectTrans.position);
                    finalDamage = 0;
                    break;
            }
        }

        // if (finalDamage <= 0)
        //     finalDamage = 1; // 최소 1 데미지는 들어가야함
        // todo 이펙트

        DamageFloatManager.Instance.ShowDamageFont (PlayerUnitList[0].gameObject, finalDamage, E_DamageType.Normal);
        nowplayerHealth -= finalDamage;
        if (nowplayerHealth <= 0) {
            if (ActivatedBattleBuff != null && ActivatedBattleBuff.BattleBuffType == E_BattleBuffType.CheatDeath)
            {
                float percentValue = (float)ActivatedBattleBuff.EffectValue/100f;
                nowplayerHealth = Convert.ToInt32(MaxPlayerHealth*percentValue);
                string path = CharacterPropertyManager.Instance.SelectedUtilProperty.SkillEffectPath;
                EffectManager.Instance.StartEffect(path, effectTrans.position);
                Debug.LogError("Cheat death is activated!");
            }
            else
            {
                StopCoroutine (CalculateRemainCount ());
                nowplayerHealth = 0;
                PlayerState = Graduate.Unit.E_UnitState.Death;
                CalculateReward (false);
                OnGameEnd.Execute (false, thisBattleRewardData);
                isBattleEnd = true;
                //BattlePhase = E_BattlePhase.End;
            }
            
        }

        float playerHealthPer = (float) nowplayerHealth / (float) MaxPlayerHealth;
        OnDamagedPlayer.Execute (playerHealthPer);
        Debug.Log ("[Test] Damage from monster: " + finalDamage);
        ActivatedBattleBuff = null;
    }

    void CalculateReward (bool isClear) {
        // <bool,string, int, int>
        string userName = UserManager.Instance.UserInfo.UserName;
        int teamLevel = UserManager.Instance.UserInfo.TeamLevel;
        int gold = UserManager.Instance.UserInfo.Gold;
        int exp = UserManager.Instance.UserInfo.Exp;

        //아이템?

        if (isClear) 
        {
            // 유저정보
            gold += thisBattleRewardData.Gold;
            exp += thisBattleRewardData.Exp;
            if (exp >= 100)
            {
                int tempExp = exp;
                for (int i = 0; i < tempExp / 100; i++) {
                    teamLevel++;
                    exp -= 100;
                }
            }

            // 유닛정보
            int unitRewardExp = 0;
            for (int i = 0; i < unitDataList.Count; i++) {
                unitRewardExp = unitDataList[i].Exp + thisBattleRewardData.Exp;
                if (unitRewardExp < 100) {
                    unitDataList[i].UpdateExp (unitRewardExp);
                    UserManager.Instance.SetMyUnitList (unitDataList[i]);
                    continue;
                }

                for (int j = 0; j < unitRewardExp / 100;) {
                    int updatedLevel = unitDataList[i].Level + 1;
                    int statAugmenter = GameDataManager.Instance.StatAugmenterDataDic[unitDataList[i].Level - 1].Augmenter;
                    // 스탯 상승
                    int updatedHp = unitDataList[i].Hp + statAugmenter;
                    int updatedAtk = unitDataList[i].Atk + statAugmenter;
                    int updatedDef = unitDataList[i].Def + statAugmenter;
                    int updatedCri = unitDataList[i].Cri + statAugmenter;
                    int updatedSpd = unitDataList[i].Spd + statAugmenter;
                    // 판매 가격 상승
                    int updatedPrice = unitDataList[i].Price + updatedLevel * 10;
                    unitDataList[i].UpdateUnitStat (updatedHp, updatedAtk, updatedDef, updatedCri, updatedSpd);
                    unitDataList[i].UpdateLevel (updatedLevel);
                    unitDataList[i].UpdatePrice (updatedPrice);
                    unitRewardExp -= 100;
                }

                unitDataList[i].UpdateExp (unitRewardExp);
                Debug.LogError ("unitId: " + unitDataList[i].Id);
                UserManager.Instance.SetMyUnitList (unitDataList[i]);
                
            }
        } else {
            // 졌을때는 보상정보를 보여주지않는다
        }

        // 변경정보 갱신
        UserInfo userInfo = UserManager.Instance.UserInfo;
        
        int thisDungeonStep = isClear? thisBattleRewardData.DungeonId + 1 :  thisBattleRewardData.DungeonId;
        if (thisDungeonStep == userInfo.BestDungeonStep + 1) // 기록단수 보다 높으면 경신 
            userInfo.BestDungeonStep++;

        UserManager.Instance.SetUserInfo (userInfo);
        OnUpdatedUserInfoAfterGameEnd.Execute (userName, teamLevel, exp, gold);
    }

    public void LoadLobbyScene () {
        isBattleEnd = true;
        BattlePhase = E_BattlePhase.End;
    }
    public Action<float, bool> StartUtilCoolDown {get; set;}
    public Action<float, bool> StartHealCoolDown {get; set;}
    public void ExecuteUtilPropertySkill()
    {
        // maybe clocking
        if(!IsFirstPropertyCooldownComplite)
            return;

        if(nowEnemy == null)
            return;

        if(nowEnemy.IsBoss)
            return; // 보스는 해당안됨
      
        // 적 패스
        if(CharacterPropertyManager.Instance.SelectedUtilProperty != null
        && CharacterPropertyManager.Instance.SelectedUtilProperty.EffectType == E_PropertyEffectType.RogueUtilMaserty_Clocking)
        {
            // 해당 용병이 출전했을때만 사용가능
            if(UserManager.Instance.UserInfo.SelectedUnitList.Find(x=>x.CharacterType == E_CharacterType.Rogue) != null)
            {
                nowEnemy.EnemyUnitState = Graduate.Unit.E_UnitState.Death;
                string path = CharacterPropertyManager.Instance.SelectedUtilProperty.SkillEffectPath;
                EffectManager.Instance.StartEffect(path, effectTrans.position);

                float time = CharacterPropertyManager.Instance.SelectedUtilProperty.CoolTime;
                StartUtilCoolDown.Execute(time, true);
            }  
        }

        
    }

    public void ExecuteHealPropertySkill()
    {
        if(!IsSecondPropertyCooldownComplite)
            return;

        // 특성에 맞게 다르게
        if(CharacterPropertyManager.Instance.SelectedHealingProperty != null)
        {
            BattleBuff buff = null;
            CharacterProperty property = CharacterPropertyManager.Instance.SelectedHealingProperty;
            // 여기서 특성별로 다르게
            switch (CharacterPropertyManager.Instance.SelectedHealingProperty.EffectType)
            {
                case E_PropertyEffectType.WarriorHealingMaserty_DecreaseDamageFromEnemy:
                    if(UserManager.Instance.UserInfo.SelectedUnitList.Find(x=>x.CharacterType == E_CharacterType.Warrior) != null)
                        buff = new BattleBuff(E_BattleBuffType.DecreasDamage, property.EffectValue);
                    break;

                case E_PropertyEffectType.MageHealingMaserty_Invincible:
                    if(UserManager.Instance.UserInfo.SelectedUnitList.Find(x=>x.CharacterType == E_CharacterType.Mage) != null)
                        buff = new BattleBuff(E_BattleBuffType.Invincible, property.EffectValue);
                    break;

                case E_PropertyEffectType.WarlockHealingMaserty_DrainHealthPerDamage:
                    if(UserManager.Instance.UserInfo.SelectedUnitList.Find(x=>x.CharacterType == E_CharacterType.Warlock) != null)
                        buff = new BattleBuff(E_BattleBuffType.DrainHealthPerDamage, property.EffectValue);
                    break;

                case E_PropertyEffectType.RogueHealingMaserty_CheatDeath:
                    if(UserManager.Instance.UserInfo.SelectedUnitList.Find(x=>x.CharacterType == E_CharacterType.Rogue) != null)
                        buff = new BattleBuff(E_BattleBuffType.CheatDeath, property.EffectValue);
                    break;
            }

            if (buff == null)
            {
                Debug.LogError("There is no valid unit to use propertySkill");
                return;
            }

            ActivatedBattleBuff = buff;
            
        }

        float time = CharacterPropertyManager.Instance.SelectedHealingProperty.CoolTime;
        StartHealCoolDown.Execute(time, false);
    }
}