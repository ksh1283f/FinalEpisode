using System.Collections;
using System.Collections.Generic;

public enum E_PropertyEffectType
{
    None,
    AdditionalAtkResource,
    AdditionalUtilResource,
    AdditionalDefResource,
    AdditionalAtk,
    AdditionalUtil,
    AdditionalDef,
    // 경험치나 골드 추가?
}

public enum E_PropertyType
{
    None,
    Atk,
    Util,
    Def,
}

public class CharacterProperty
{
    public string ImagePath { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public E_PropertyType PropertyType { get; private set; }
    public E_PropertyEffectType EffectType { get; private set; }
    public int EffectValue { get; private set; }

    public CharacterProperty(string imagePath, string name, string description, E_PropertyType propertyType, E_PropertyEffectType effectType, int effectValue)
    {
        ImagePath = imagePath;
        Name = name;
        Description = description;
        PropertyType = propertyType;
        EffectType = effectType;
        EffectValue = effectValue;
    }
}