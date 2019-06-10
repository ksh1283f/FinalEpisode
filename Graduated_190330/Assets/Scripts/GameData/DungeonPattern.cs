using System.Collections;
using System.Collections.Generic;

public class DungeonPattern 
{
    public string EnemyName{get; private set;}
    public int EnemyHealth { get; private set; }
    public List<EnemyPattern> PatternList { get; private set; }
    public string PatternDescription { get; private set; }
    public float PatternTerm { get; private set; }

    public DungeonPattern(string enemyName, int enemyHealth, List<EnemyPattern> patternList, string patternDescription, float patternTerm)
    {
        this.EnemyName = enemyName;
        this.EnemyHealth = enemyHealth;
        this.PatternList = patternList;
        this.PatternDescription = patternDescription;
        this.PatternTerm = patternTerm;
    }
}
