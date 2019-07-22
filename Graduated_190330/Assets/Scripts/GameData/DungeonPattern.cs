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
}
