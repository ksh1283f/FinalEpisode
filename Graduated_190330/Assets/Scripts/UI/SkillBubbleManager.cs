using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillBubbleManager : MonoBehaviour
{
    [SerializeField] E_SkillResourceType SkillResourceType;
    public List<SkillResource> skillResourceList = new List<SkillResource>();

    private void Start()
    {
        skillResourceList = GetComponentsInChildren<SkillResource>().ToList();
        InitSkillBubble();
    }

    /// <summary>
    /// 갯수에 맞게 버블을 생성한다
    /// </summary>
    /// <param name="createCount"></param>
    public void CreateBubble(int createCount)
    {
        int count = skillResourceList.Count;
        for (int i = 0; i < count && createCount >0; i++)
        {
            if (skillResourceList[i].gameObject.activeSelf)
            {
                createCount--;
                continue;
            }

            if (i == count)
            {
                createCount--;
                return;
            }

            skillResourceList[i].gameObject.SetActive(true);
            createCount--;
            
        }
    }

    /// <summary>
    /// 소모기능은 모든 자원을 소모한다
    /// </summary>
    public void UseSkillBubble(int skillCount)
    {
        int count = skillCount; // ex 2
        for (int i = count - 1; i >= 0; i--)    // ? => i=2
        {
            if (!skillResourceList[i].gameObject.activeSelf)
                continue;

            skillResourceList[i].gameObject.SetActive(false);
        }
    }

    void InitSkillBubble()
    {
        for (int i = 0; i < skillResourceList.Count; i++)
        {
            if (!skillResourceList[i].gameObject.activeSelf)
                continue;

            skillResourceList[i].gameObject.SetActive(false);
        }
    }
}
