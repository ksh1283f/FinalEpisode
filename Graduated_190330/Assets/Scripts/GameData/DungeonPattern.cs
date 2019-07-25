using System;
using System.Collections;
using System.Collections.Generic;

public class DungeonPattern
{
    public int Id { get; private set; }
    public string EnemyName { get; private set; }
    public int EnemyHealth { get; private set; }
    public int PatternListID { get; private set; }
    public List<EnemyPattern> PatternList { get; private set; }
    public string PatternDescription { get; private set; }
    public float PatternTerm { get; private set; }

    public DungeonPattern(int id, string enemyName, int enemyHealth, List<EnemyPattern> patternList, string patternDescription, float patternTerm)
    {
        this.Id = id;
        this.EnemyName = enemyName;
        this.EnemyHealth = enemyHealth;
        this.PatternList = patternList;
        this.PatternDescription = patternDescription;
        this.PatternTerm = patternTerm;
    }

    // obsolete, it is to remove
    public DungeonPattern(string enemyName, int enemyHealth, List<EnemyPattern> patternList, string patternDescription, float patternTerm)
    {
        this.EnemyName = enemyName;
        this.EnemyHealth = enemyHealth;
        this.PatternList = patternList;
        this.PatternDescription = patternDescription;
        this.PatternTerm = patternTerm;
    }

    // bool형인 이유는 monobehaviour를 상속받지 않는 클래스이기때문에 debug.log로 결과유무를 바로 확인할 수 없기 때문
    // 따라서 밖에서 반환값으로 결과를 출력해주어야한다.
    // ex) if (!SetEnemyStat(pattern)) Debug.LogError("There is no pattern in list.."); 
    public bool SetEnemyStat(EnemyPattern pattern)
    {
        EnemyPattern target = null;
        for (int i = 0; i < PatternList.Count; i++)
        {
            if (pattern.SkillId == PatternList[i].SkillId)
            {
                target = PatternList[i];
                return true;
            }
        }

        return false;
    }

    public bool SetEnemyHealth(int health)
    {
        if (health <= 0 || health > Int32.MaxValue)
            return false;

        EnemyHealth = health;
        return true;
    }

    public object ShallowCopy()
    {
        return this.MemberwiseClone();
    }
}
