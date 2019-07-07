namespace Graduate.GameData.UnitData
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public enum E_CharacterType
    {
        None,
        Warrior,    // 거름
        Mage,   // 법뻔뻔
        Warlock,    // 생석
        Rogue,  // 돚거
    }

    public class UnitData
    {
        public int Id { get; private set; }
        public int Hp { get; private set; }
        public int Atk { get; private set; }
        public int Def { get; private set; }
        public int Cri { get; private set; }
        public int Spd { get; private set; }
        public string IconName { get; private set; }
        public E_CharacterType CharacterType { get; private set; }
        public int Price {get; private set;}

        public UnitData(int hp, int atk, int def, string iconName)
        {
            Hp = hp;
            Atk = atk;
            Def = def;
            IconName = iconName;
        }

        public UnitData(int id, int hp, int atk, int def, int cri, int spd, string iconName, E_CharacterType characterType, int price)
        {
            Id = id;
            Hp = hp;
            Atk = atk;
            Def = def;
            Cri = cri;
            Spd = spd;
            IconName = iconName;
            CharacterType = characterType;
            Price = price;
        }
    }
}