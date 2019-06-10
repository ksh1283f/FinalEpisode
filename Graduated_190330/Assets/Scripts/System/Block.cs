using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    public E_SkillType skillType { get; private set; }
    public Block HeadBlock = null;
    public int ChainLevel = 0;
    public Unit targetUnit { get; private set; }
    public bool canUse { get; private set; }
    public E_BlockSkillType blockSkillType { get; private set; }
    public E_PlayerType PlayerType { get; private set; }

    Transform dropDownTargetTransform;
    public Transform DropDownTargetTransform
    {
        get
        {
            return dropDownTargetTransform;
        }
        set
        {
            dropDownTargetTransform = value;
        }
    }

    public void SetBlockData(int chainLevel, Unit targetUnit, E_SkillType skillType, E_BlockSkillType blockSkillType, E_PlayerType playerType)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
            gameObject.AddComponent<RectTransform>();

        Vector2 blockPos = Vector2.zero;

        switch (blockSkillType)
        {
            case E_BlockSkillType.Attack:
                blockPos.x = BlockManager.Instance.TransAttackBlockPool.localPosition.x;
                blockPos.y = BlockManager.Instance.TransAttackBlockPool.localPosition.y + 300;
                break;

            case E_BlockSkillType.Util:
                blockPos.x = BlockManager.Instance.TransUtilBlockPool.localPosition.x;
                blockPos.y = BlockManager.Instance.TransUtilBlockPool.localPosition.y + 300;
                break;

            case E_BlockSkillType.Heal:
                blockPos.x = BlockManager.Instance.TransHealBlockPool.localPosition.x;
                blockPos.y = BlockManager.Instance.TransHealBlockPool.localPosition.y + 300;
                break;
        }

        rectTransform.anchoredPosition = new Vector3(blockPos.x, blockPos.y);

        canUse = true;
        this.targetUnit = targetUnit;
        this.ChainLevel = chainLevel;
        this.skillType = skillType;
        this.blockSkillType = blockSkillType;
        HeadBlock = this;
        PlayerType = playerType;

        //디버그
        if (targetUnit)
        {
            Image image = GetComponent<Image>();
            if (image == null)
                return;

            switch (PlayerType)
            {
                case E_PlayerType.None:
                    image.color = Color.black;
                    break;

                case E_PlayerType.Leader:
                    image.color = Color.red;
                    break;

                case E_PlayerType.Sub1:
                    image.color = Color.yellow;
                    break;

                case E_PlayerType.Sub2:
                    image.color = Color.green;
                    break;
            }
        }
    }
    private void Start()
    {
    }
    private void OnMouseDown()
    {
        if (canUse)
        {
            BlockManager.Instance.BlockUse(this);
        }
        //연결 블록에 대한 블록매니저에 소멸 요청. 
        //블록 소멸 요청
    }

    /// <summary>
    /// 블록 드롭 시작.
    /// </summary>
    public void StartDropDown()
    {
        if (canUse)
        {
            StartCoroutine(DropDown());
        }
    }
    IEnumerator DropDown()
    {
        canUse = false;
        while (Vector3.Distance(transform.position, dropDownTargetTransform.transform.position) >= 0.3f)
        {
            transform.position = Vector3.Lerp(transform.position, dropDownTargetTransform.position, 0.35f);
            yield return null;
        }
        transform.position = dropDownTargetTransform.position;
        canUse = true;
    }
    public void StartTryCombine(params Block[] blocks)
    {
        StopCoroutine(TryCombine());
        StartCoroutine(TryCombine(blocks));
    }

    IEnumerator TryCombine(params Block[] blocks)
    {
        //List<Block> blockPanel = BlockManager.Instance.healBlockPanel;

        while (!canUse)
        {
            yield return null;
        }
        BlockManager.Instance.Combine(blocks);
    }

    public void StartDropCombine(E_BlockSkillType blockSkillType)
    {
        StartCoroutine(DropDownCombine(blockSkillType));
    }
    IEnumerator DropDownCombine(E_BlockSkillType blockSkillType)
    {
        while (!canUse)
        {
            yield return null;
        }
        //List<Block> blockPanel = BlockManager.Instance.healBlockPanel;
        List<Block> blockPanel = BlockManager.Instance.blockPanelDic[blockSkillType].blockPanel;
        int thisBlockIndex = blockPanel.IndexOf(this);

        //이 블록이 맨앞 인덱스가 아니며
        if (thisBlockIndex > 0)
        {
            //그 앞의 블록이 있되 3체인 미만이며
            if (blockPanel[thisBlockIndex - 1].ChainLevel < 3)
            {
                //이블럭과 앞블럭의 헤더가 동일 블럭이면
                if ((blockPanel[thisBlockIndex - 1].targetUnit == targetUnit) && (blockPanel[thisBlockIndex - 1].skillType == skillType))
                {
                    BlockManager.Instance.Combine(blockPanel.GetRange(blockPanel.IndexOf(blockPanel[thisBlockIndex - 1].HeadBlock), blockPanel[thisBlockIndex - 1].ChainLevel + 1).ToArray());
                }

            }

        }
    }
}
