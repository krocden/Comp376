using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave #", menuName = "Wave")]
public class Wave : ScriptableObject
{
    [SerializeField] private List<MonsterDetails> monsters = new List<MonsterDetails>();

    public List<MonsterDetails> GetMonsterDetails()
    {
        return monsters;
    }
}


[System.Serializable]
public class MonsterDetails
{
    public Monster monsterPrefab;
    public int monsterQuantity;
    public int monsterTier;
    public CommandType commandType;
    public int repeatNextXRows;
    public int repeatTimes;
}

public enum CommandType
{ 
    Monster,
    RepeatGroup
}