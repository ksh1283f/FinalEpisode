public enum E_UnitType
{
    Caster,
    Target
}

public enum E_AbnormalStatus
{
    Entangle = 12,
    Groggy = 8,
    Dead = 15
}
public enum E_motion
{
    Normal = 0,
    Laugh = 1,
    Sleepy = 2,
    Shout = 3,
    Angry = 4,
    Groggy = 5,
}

public enum E_StatType
{

    //체력
    MaxHealth,
    MinHealth,
    CurrentHealth,

    //공격력
    AttackPoint,

    //치명타
    CriticalRate,
    CriticalMultiplier,

    //SP
    MaxSpecialPoint,
    MinSpecialPoint,
    CurrentSpecialPoint,


    //방어 속성
    PhysicalDefense = 255,
    MagicalDefense,
    TrueTypeDefense,
    //피해감소
    DamageReduceRate,

    //흡혈율
    BloodSuckingRate = 9,

    //명중률
    MaxAccuracy,
    MinAccuracy,
    CurrentAccuracy,
    //회피율
    MaxEvasionRate,
    MinEvasionRate,
    CurrentEvasionRate,

    //사거리
    MaxRange,
    MinRange,
    CurrentRange,

    //평타속도
    MaxAttackSpeed,
    MinAttackSpeed,
    CurrentAttackSpeed,

    //관통력
    PhysicalPenetration = 268,
    MagicalPenetration,
    //이동속도
    MoveSpeed = 22,
    //넉백 저항
    KnockbackResistance,
    //시간 가속율
    TimeAccelerationRate,
    //모션 가속율
    MotionAccelerationRate,
    //크기 배율
    ScaleMultiplier,

}

/// <summary>
/// 버프 타입
/// </summary>
public enum E_BuffType
{
    None,
    Attack,
    Defence,
    technical
}
/// <summary>
/// 버프 정렬
/// </summary>
public enum E_BuffOrder
{
    None,
    positive,
    negative,
    neutral
}
/// <summary>
/// 적용 대상
/// </summary>
public enum E_ApplyTargetFilter
{
    Caster,
    Target
}

public enum E_EffectType
{
    None = 254,
    Physics,
    Magical,
    TrueType,
    Heals,
}
public enum E_FloatingType
{
    None,
    FullPenetrationDamage,
    CriticalDamage,
    ShieldDamage,
    NonpenetratingDamage,
    Heal
}

public enum E_Range
{
    /// <summary>
    /// 최소 사거리 내
    /// </summary>
    WithInMinRange,
    /// <summary>
    /// 사거리 안
    /// </summary>
    WithInMaxRange,
    /// <summary>
    /// 사거리 밖
    /// </summary>
    OutOfRange,
}

public enum E_GroupTag
{
    Player,
    Enemy
}
public enum E_Race
{
    Human,
    Monster,
    Boss
}

public enum E_SkillType
{
    Normal,
    Special
}

public enum E_Class
{
    None,
    Wizard,
    Hunter,
    Archer,
    Priest,
    Paladin,
    Warrior,
    Monster
}

public enum E_AttackPriority
{
    First,
    Second,
    Third,
}
