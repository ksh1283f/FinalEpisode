namespace Graduate.GameData.UnitData
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UnitData
    {
        public int Hp { get; private set; }
        public int Atk { get; private set; }
        public int Def { get; private set; }
        public string IconName {get; private set;}

        public UnitData(int hp, int atk, int def, string iconName)
        {
            Hp = hp;
            Atk = atk;
            Def = def;
            IconName = iconName;
        }
    }
}