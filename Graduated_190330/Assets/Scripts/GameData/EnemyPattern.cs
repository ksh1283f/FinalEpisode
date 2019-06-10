using System.Collections;
using System.Collections.Generic;

// todo 임시로 여기에 작성, 나중에 enum형식이 정리되어있는 파일로 옮기거나 따로 정리할것
public enum E_UserSkillType
{
    Attack = 1,
    Util,
    AttackAndUtil,
    Defense,
}

public class EnemyPattern
{
    public int ID { get; private set; }
    public string Name { get; private set; }
    public E_UserSkillType SkillType { get; private set; }
    public float CastTime { get; private set; }
    public string SkillDescription { get; private set; }
    public float PatternTerm { get; private set; }
    public int Damage { get; private set; }

    public EnemyPattern(int id, string name, E_UserSkillType skillType, float castTime, string skillDescription, int damage)
    {
        this.ID = id;
        this.Name = name;
        this.SkillType = skillType;
        this.CastTime = castTime;
        this.SkillDescription = skillDescription;
        this.Damage = damage;
    }
}