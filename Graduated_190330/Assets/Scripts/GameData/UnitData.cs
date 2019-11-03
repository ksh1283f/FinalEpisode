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
        public string portraitPath;
    }

    public enum E_CharacterType
    {
        None,
        Warrior,    // 거름
        Mage,   // 법뻔뻔
        Warlock,    // 생석
        Rogue,  // 돚거
        EnumDataCount,  // 타입 개수
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
        public string PortraitPath {get; private set;}

        public UnitData(int hp, int atk, int def, string iconName)
        {
            Hp = hp;
            Atk = atk;
            Def = def;
            IconName = iconName;
        }

        public UnitData(int id, int hp, int atk, int def, int cri, int spd, string iconName, E_CharacterType characterType, int price, string description, int level, int exp, string imagePath)
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
            PortraitPath = imagePath;
        }

        /// 새로 구매하여 유닛의 아이디가 갱신된 경우
        public void UpdateUnitID(int unitID)
        {
            Id = unitID;
        }

        public void UpdateLevel(int level)
        {
            if(level < Level)
            {
                Debug.LogError("Invalid Level: it is lower than present level");
                return;
            }

            Level = level;
        }
        
        public void UpdateExp(int gainedExp)
        {
            // Exp += gainedExp;
            // int splitValue = 100;   // todo 추후에 데이터로 받아오는 방식으로도 작업할 수 있다.
            // int levelUpValue = Exp / splitValue;
            // Level += levelUpValue;  // level-up
            // Exp = Exp % splitValue;
            if(gainedExp < 1)
            {
                Debug.LogError("exp value can't be negative");
                return;
            }

            Exp = gainedExp;
        }

        public void UpdateUnitStat(int hp, int atk, int def, int cri, int spd)
        {
            Hp = hp;
            Atk = atk;
            Def = def;
            Cri = cri;
            Spd = spd;
        }

        public void UpdatePrice(int price)
        {
            Price = price;
        }

        public object ShallowCopy()
        {
            return this.MemberwiseClone();
        }
    }
}