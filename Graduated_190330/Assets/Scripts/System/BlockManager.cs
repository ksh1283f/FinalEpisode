#define NOT_SHOW_UTIL

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum E_BlockSkillType
{
    None,
    Attack,
    Util,
    Heal,
}

public class BlockPanel
{
    public List<Block> blockPanel;
    public int lastBlockNextIndex;
    public List<Transform> blockPanelPosisionList;

    public BlockPanel(List<Block> blockPanel, int lastBlockNextIndex, List<Transform> blockPanelPosisionList)
    {
        this.blockPanel = blockPanel;
        this.lastBlockNextIndex = lastBlockNextIndex;
        this.blockPanelPosisionList = blockPanelPosisionList;
    }
}

public class BlockManager : MonoBehaviour
{
    public Block block;
    static BlockManager instance = null;
    public static BlockManager Instance
    {
        get
        {
            if (instance)
            {
                return instance;
            }
            else
            {
                instance = FindObjectOfType<BlockManager>();
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "GameManager";
                    instance = container.AddComponent<BlockManager>();
                }
                return instance;
            }
        }
    }
    List<Unit> playerUnitList = null;
    List<int> dropTable = new List<int>();

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public const int BlockMaxCount = 7;
    private const string ATTACK_SKILL_BAR = "AttackSkillBar";
    private const string UTIL_SKILL_BAR = "UtilSkillBar";
    private const string HEAL_SKILL_BAR = "HealSkillBar";

    public RectTransform TransAttackBlockPool { get; private set; }
    public RectTransform TransUtilBlockPool { get; private set; }
    public RectTransform TransHealBlockPool { get; private set; }

    void Start()
    {
        TransAttackBlockPool = transform.Find(ATTACK_SKILL_BAR).GetComponent<RectTransform>();
        TransUtilBlockPool = transform.Find(UTIL_SKILL_BAR).GetComponent<RectTransform>();
        TransHealBlockPool = transform.Find(HEAL_SKILL_BAR).GetComponent<RectTransform>();

        InitializeBlockPool();
        InitializeBlockPanel();
        playerUnitList = GameManager.Instance.PlayerUnitList;
        StartGenerateBlock();
    }

    /*
     * 블록 풀 생성
     * 블록 풀 초기화
     * 블록 풀에서 꺼내기
     * 블록 풀에 반납(반납할 블록)
     */
    List<Block> healblockPool;

    List<Block> attackblockPool;
    List<Block> utilblockPool;

    Dictionary<E_BlockSkillType, List<Block>> poolDic;

    void InitializeBlockPool()
    {
        healblockPool = new List<Block>();
        attackblockPool = new List<Block>();
        utilblockPool = new List<Block>();
        poolDic = new Dictionary<E_BlockSkillType, List<Block>>();

        block = Resources.Load<Block>("Prefabs/System/BlockPanel/Block");
        lastHealBlockNextIndex = 0;
        lastAttackBlockNextIndex = 0;
        lastUtilBlockNextIndex = 0;

        for (int index = 0; index < BlockMaxCount; index++)
        {
            attackblockPool.Add(Instantiate<Block>(block));
            attackblockPool[index].transform.SetParent(transform, false);
            attackblockPool[index].SetBlockData(0, null, E_SkillType.Normal, E_BlockSkillType.Attack, E_PlayerType.None);
            attackblockPool[index].gameObject.SetActive(false);

            utilblockPool.Add(Instantiate<Block>(block));
            utilblockPool[index].transform.SetParent(transform, false);
            utilblockPool[index].SetBlockData(0, null, E_SkillType.Normal, E_BlockSkillType.Util, E_PlayerType.None);
            utilblockPool[index].gameObject.SetActive(false);

            healblockPool.Add(Instantiate<Block>(block));
            healblockPool[index].transform.SetParent(transform, false);
            healblockPool[index].SetBlockData(0, null, E_SkillType.Normal, E_BlockSkillType.Heal, E_PlayerType.None);
            healblockPool[index].gameObject.SetActive(false);
        }

        poolDic.Add(E_BlockSkillType.Attack, attackblockPool);
        poolDic.Add(E_BlockSkillType.Util, utilblockPool);
        poolDic.Add(E_BlockSkillType.Heal, healblockPool);
    }
    Block Pop(E_BlockSkillType blockSkillType)
    {
        List<Block> blockPool = poolDic[blockSkillType];

        foreach (Block block in blockPool)
        {
            if (!block.gameObject.activeInHierarchy)
            {
                block.gameObject.SetActive(true);
                return block;
            }
        }
        return null;
    }
    void Push(Block block)
    {
        block.gameObject.SetActive(false);
    }

    /* 
     * 블록 떨구기(떨굴 위치,떨구기
     * 블록 합치기
     * 블록 자동 생성 시작
     * 블록 자동 생성 중지
     */


    public List<Block> healBlockPanel = new List<Block>();
    public int lastHealBlockNextIndex = 0;
    public List<Transform> healBlockPanelPostionList = new List<Transform>();

    public List<Block> attackBlockPanel = new List<Block>();
    public int lastAttackBlockNextIndex = 0;
    public List<Transform> attackBlockPanelPositionList = new List<Transform>();

    public List<Block> utilBlockPanel = new List<Block>();
    public int lastUtilBlockNextIndex = 0;
    public List<Transform> utilBlockPanelPositionList = new List<Transform>();

    public Dictionary<E_BlockSkillType, BlockPanel> blockPanelDic = new Dictionary<E_BlockSkillType, BlockPanel>();

    void InitializeBlockPanel()
    {
        BlockPanel healPanel = new BlockPanel(healBlockPanel, lastHealBlockNextIndex, healBlockPanelPostionList);
        BlockPanel attackPanel = new BlockPanel(attackBlockPanel, lastAttackBlockNextIndex, attackBlockPanelPositionList);
        #if !NOT_SHOW_UTIL
        BlockPanel utilPanel = new BlockPanel(utilBlockPanel, lastUtilBlockNextIndex, utilBlockPanelPositionList);
        #endif

        blockPanelDic.Add(E_BlockSkillType.Attack, attackPanel);
        #if !NOT_SHOW_UTIL
        blockPanelDic.Add(E_BlockSkillType.Util, utilPanel);
        #endif
        blockPanelDic.Add(E_BlockSkillType.Heal, healPanel);
    }

    void StartGenerateBlock()
    {
        // 플레이어 유닛리스트를 검색하여 안에 있는 유닛들의 클래스를 체크
        // 그 클래스에 맞는 스킬만 나오게 한다.


        StartCoroutine(GenerateSkillBlock(E_BlockSkillType.Attack));
        //StopCoroutine(GenerateSkillBlock(E_BlockSkillType.Attack)); // 끝난 코루틴은 종료
        #if !NOT_SHOW_UTIL
        StartCoroutine(GenerateSkillBlock(E_BlockSkillType.Util));
        //StopCoroutine(GenerateSkillBlock(E_BlockSkillType.Util));
        #endif
        StartCoroutine(GenerateSkillBlock(E_BlockSkillType.Heal));
        //StopCoroutine(GenerateSkillBlock(E_BlockSkillType.Heal));
    }
    void StopGenerateBlock()
    {
        StopCoroutine(GenerateSkillBlock(E_BlockSkillType.Attack));
        #if !NOT_SHOW_UTIL
        StopCoroutine(GenerateSkillBlock(E_BlockSkillType.Util));
        #endif
        StopCoroutine(GenerateSkillBlock(E_BlockSkillType.Heal));
    }

    //블록 주기 생성
    public float blockGeneratePeriod = 0;
    float currentGeneratingPeriod = 0;
    IEnumerator GenerateSkillBlock(E_BlockSkillType blockSkillType)
    {
        BlockPanel blockPanel = blockPanelDic[blockSkillType];
        if (blockPanel == null)
        {
            Debug.LogError("blockPanel is null");
            yield break;
        }

        while (true)
        {
            //블록이 가득차지 않았다면
            if (blockPanel.lastBlockNextIndex < BlockMaxCount)
            {
                //주기를 검사한다. [드랍을 위해서]
                if (currentGeneratingPeriod < blockGeneratePeriod)
                {
                    currentGeneratingPeriod += Time.deltaTime;
                }
                else
                {
                    //쿨탐 초기화하고
                    currentGeneratingPeriod = 0;

                    //이건 드랍할 유닛 임시 설정. 그냥 랜덤임 나중에 지워야할 테스트 코드
                    Unit randomUnit;
                    E_PlayerType playerType = E_PlayerType.None;
                    // 힐이 가능한 유닛이 몇명?
                    // 힐이 가능한 유닛이 누구?

                    
                    //if (Random.value >= 0.5f/*dropTable[lastBlockNextIndex]==0*/)
                    // todo 범용적으로 쓰일수 있도록 변경하기(사용가능한 스킬을 누가 쓸수 있는지)
                    List<Unit> skillUsableList = new List<Unit>();
                    if(blockSkillType == E_BlockSkillType.Heal)
                    {
                        for (int i = 0; i < GameManager.Instance.PlayerUnitList.Count; i++)
                        {
                            if(GameManager.Instance.PlayerUnitList[i].ClassType.IsHealable())
                                skillUsableList.Add(GameManager.Instance.PlayerUnitList[i]);
                        }
                    }
                    else if (blockSkillType == E_BlockSkillType.Attack) // 힐러는 공격못하게(나중에 바뀔수도)
                    {
                        for (int i = 0; i < GameManager.Instance.PlayerUnitList.Count; i++)
                        {
                            if(!GameManager.Instance.PlayerUnitList[i].ClassType.IsHealable())
                                skillUsableList.Add(GameManager.Instance.PlayerUnitList[i]);
                        }
                    }
                    else
                    {
                        skillUsableList = GameManager.Instance.PlayerUnitList;
                    }

                    float resultRandomValue = Random.value;
                    float termValue = 1f / skillUsableList.Count;

                    int resultIndex = Random.Range(0, skillUsableList.Count);
                    randomUnit = skillUsableList[resultIndex];
                    playerType = randomUnit.PlayerType;

                    Block tempBlock = Pop(blockSkillType);
                    tempBlock.SetBlockData(1, randomUnit, E_SkillType.Normal, blockSkillType, playerType);
                    tempBlock.DropDownTargetTransform = blockPanel.blockPanelPosisionList[blockPanel.lastBlockNextIndex];
                    blockPanel.blockPanel.Add(tempBlock);
                    tempBlock.StartDropDown();
                    tempBlock.StartDropCombine(blockSkillType);

                    blockPanel.lastBlockNextIndex++;
                }
            }
            else
            {
                Debug.LogError("풀: " + blockSkillType.ToString());
                currentGeneratingPeriod = 0;
            }


            yield return null;
        }
    }

    //재배열만 해준다.
    public void Combine(params Block[] blocks)
    {
        int headIndex = 0;                  //맨앖
        int currentChainCount = 1;
        int remainBlocks = blocks.Length;   //7
        int remainBlocksChainLevel = blocks.Length % 3;


        for (int index = 0; index < blocks.Length; index++)
        {

            if (!blocks[index].gameObject.activeInHierarchy)
            {
                break;
            }
            //헤더블록은 현재의 헤더 블록
            blocks[index].HeadBlock = blocks[headIndex];    //0번째

            //체인수설정
            //재설정될 블록이 3개 이상이라면
            if (remainBlocks >= 3)
            {
                blocks[index].ChainLevel = 3;

            }
            else
            {
                blocks[index].ChainLevel = remainBlocksChainLevel;

                if (remainBlocksChainLevel == 2)
                {

                }

            }

            //헤더블록 재설정
            if (currentChainCount == 3)
            {
                headIndex += 3;
                currentChainCount = 1;
                remainBlocks -= 3;
            }
            else
            {
                currentChainCount++;

            }
        }
    }
    public void BlockUse(Block block)
    {
        if (block.targetUnit == null)
            return;

        if (block.targetUnit.EnemyRange == E_Range.OutOfRange)
            return;

        //블록헤더의 정보를 토대로 시전자에게 스킬 인자 전달.
        //블록헤더의 체인값만큼 이후열의 블록들 소멸 실시
        //체인값 이후 열부터 드롭다운 실시
        //드롭다운이 완료되면 앞과 뒤를 비교하여 같을경우 콤바인 진행
        //
        E_BlockSkillType blockSkillType = block.blockSkillType;
        BlockPanel blockPanel = blockPanelDic[blockSkillType];

        Block headBlock = block.HeadBlock;              //사용된 블록의 헤더 블록
        int usedBlockHeadIndex = blockPanel.blockPanel.IndexOf(headBlock);//사용된 블록의 헤더블록의 인덱스
        int usingChain = block.ChainLevel;              //사용된 블록의 체인수

        //스킬 시전(스킬 타입, 체인수)

        block.targetUnit.AddSkillQueue(block.skillType, block.ChainLevel);
        //블록 풀에 사용한 블록만큼 반환한다.
        //헤더 블록을 기준으로 체인갯수만큼 반환
        foreach (Block usingBlock in blockPanel.blockPanel.GetRange(usedBlockHeadIndex, usingChain))
        {
            Push(usingBlock);
        }
        blockPanel.blockPanel.RemoveRange(usedBlockHeadIndex, usingChain);

        //드랍이 가능한 마지막 인덱스를 체인값만큼 빼서 재설정.
        blockPanel.lastBlockNextIndex -= usingChain;






        //합쳐질 블록 계산
        if (blockPanel.lastBlockNextIndex > usedBlockHeadIndex)
        {
            if (usedBlockHeadIndex != 0)
            {
                //사용된 블록의 앞에 체인의 합이 3이면 합칠 필요가 없다.
                if (blockPanel.blockPanel[usedBlockHeadIndex - 1].ChainLevel != 3)
                {
                    //같지 않다면 합칠 필요가 없다.
                    if ((blockPanel.blockPanel[usedBlockHeadIndex - 1].targetUnit == blockPanel.blockPanel[usedBlockHeadIndex].targetUnit) && (blockPanel.blockPanel[usedBlockHeadIndex - 1].skillType == blockPanel.blockPanel[usedBlockHeadIndex].skillType))
                    {
                        //사용된 블록 앞에 쌓여있는 블록의 헤더 인덱스
                        int frontHeadBlockIndex = blockPanel.blockPanel.IndexOf(blockPanel.blockPanel[usedBlockHeadIndex - 1].HeadBlock);
                        int totalCombineBlockCount = blockPanel.blockPanel[frontHeadBlockIndex].ChainLevel + blockPanel.blockPanel[usedBlockHeadIndex].ChainLevel; //1 + 3 = 4 


                        //내려오는 블록중 마지막 블록 찾기
                        //
                        //Debug.Log((usingBlockHeadIndex + blockPanel[usingBlockHeadIndex].chainLevel) + "-"+(blockPanel.Count));

                        for (int index = usedBlockHeadIndex + blockPanel.blockPanel[usedBlockHeadIndex].ChainLevel; index < blockPanel.blockPanel.Count; index++)
                        {
                            //후열이 드랍중이 아니여야 합칠 수 있음.
                            if (!blockPanel.blockPanel[index].canUse)
                            {
                                break;
                            }
                            else
                            {
                                //
                                if ((blockPanel.blockPanel[usedBlockHeadIndex].targetUnit == blockPanel.blockPanel[index].targetUnit) && (blockPanel.blockPanel[usedBlockHeadIndex].skillType == blockPanel.blockPanel[index].skillType))
                                {
                                    totalCombineBlockCount++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        //Debug.Log(frontHeadBlockIndex + " " +totalCombineBlockCount);
                        blockPanel.blockPanel[usedBlockHeadIndex].StartTryCombine(blockPanel.blockPanel.GetRange(frontHeadBlockIndex, totalCombineBlockCount).ToArray());
                    }
                }
            }
        }
        //드롭다운 시킨다.
        int dropDownBlockCount = blockPanel.blockPanel.Count - usedBlockHeadIndex;
        for (int index = usedBlockHeadIndex; index < usedBlockHeadIndex + dropDownBlockCount; index++)
        {
            blockPanel.blockPanel[index].DropDownTargetTransform = blockPanel.blockPanelPosisionList[index];
            blockPanel.blockPanel[index].StartDropDown();
        }


    }

    // todo 블록의 스킬타입 변환(공격, 회복, 유틸은 어떻게?)
    E_BlockSkillType UnitClassToBlockSkillType(Unit unit)
    {
        if (unit == null)
            return E_BlockSkillType.None;

        // TODO 유닛에게 유틸성 기술이 있는지?
        // TODO 스킬에 관해서 좀더 구체적인것이 필요하다
        //체크용

        E_Class classType = unit.ClassType;
        switch (classType)
        {
            // 원딜
            case E_Class.Wizard:
            case E_Class.Hunter:
            case E_Class.Archer:
                return E_BlockSkillType.Attack;

            // 근딜
            case E_Class.Paladin:
            case E_Class.Warrior:
                return E_BlockSkillType.Util;

            // 힐러
            case E_Class.Priest:
                return E_BlockSkillType.Heal;
        }

        return E_BlockSkillType.None;
    }

    [ContextMenu("DebugHeight")]
    public void DebugHeight()
    {
        Debug.LogError(BlockManager.Instance.GetComponent<RectTransform>().rect.height);
    }
}