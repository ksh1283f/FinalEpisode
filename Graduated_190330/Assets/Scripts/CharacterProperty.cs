using System.Collections;
using System.Collections.Generic;

public enum E_PropertyEffectType
{
    None,
    AdditionalAtkResource,
    AdditionalUtilResource,
    AdditionalDefResource,
    WarriorUtilMaserty_AdditionalDefense,
    MageUtilMaserty_HOT,
    WarlockUtilMaserty_IncreaseCri,
    RogueUtilMaserty_Clocking,
    WarriorHealingMaserty_DecreaseDamageFromEnemy,
    MageHealingMaserty_Invincible,
    WarlockHealingMaserty_DrainHealthPerDamage,
    RogueHealingMaserty_CheatDeath,
    WarriorHealingMastery_SpellReflection,
}

public enum E_DetailPropertyType
{
    None,
    Atk,
    Util,
    Def,
}

public class CharacterProperty
{
    public int Id {get; private set;}
    public string ImagePath { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public E_BattlePropertyType BattlePropertyType {get; private set;}
    public E_DetailPropertyType PropertyType { get; private set; }
    public E_PropertyEffectType EffectType { get; private set; }
    public int EffectValue { get; private set; }
    public int CoolTime {get; private set;}
    public string SkillEffectPath {get; private set;}

    public CharacterProperty(int id, string imagePath, string name, string description,E_BattlePropertyType battlePropertyType, E_DetailPropertyType propertyType, E_PropertyEffectType effectType, int effectValue, int coolTime, string skillEffectPath)
    {
        Id = id;
        ImagePath = imagePath;
        Name = name;
        Description = description;
        BattlePropertyType = battlePropertyType;
        PropertyType = propertyType;
        EffectType = effectType;
        EffectValue = effectValue;
        CoolTime = coolTime;
        SkillEffectPath = skillEffectPath;
    }
}