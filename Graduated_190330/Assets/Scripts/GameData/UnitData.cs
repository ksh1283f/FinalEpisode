namespace Graduate.GameData.UnitData
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class SerializableUnitData
    {
        public int Id;
        public int Hp;
        public int Atk;
        public int Def;
        public int Cri;
        public int Spd;
        public string IconName;
        public E_CharacterType CharacterType;
        public int Price;
        public string Description;
        public int Level;
        public int Exp;
    }

    public enum E_CharacterType
    {
        None,
        Warrior,    // 거름
        Mage,   // 법뻔뻔
        Warlock,    // 생석
        Rogue,  // 돚거
    }

    [Serializable]
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
        public int Price { get; private set; }
        public string Description { get; private set; }
        public int Level { get; private set; }
        public int Exp { get; private set; }

        public UnitData(int hp, int atk, int def, string iconName)
        {
            Hp = hp;
            Atk = atk;
            Def = def;
            IconName = iconName;
        }

        public UnitData(int id, int hp, int atk, int def, int cri, int spd, string iconName, E_CharacterType characterType, int price, string description, int level, int exp)
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
            Description = description;
            Level = level;
            Exp = exp;

        }

        /// 새로 구매하여 유닛의 아이디가 갱신된 경우
        public void UpdateUnitID(int unitID)
        {
            Id = unitID;
        }

        public void UpdateUnitLevel(int level)
        {
            if (level <= Level)
            {
                Debug.LogError("invalid level value");
                return;
            }

            Level = level;
        }

        public object ShallowCopy()
        {
            return this.MemberwiseClone();
        }
    }
}