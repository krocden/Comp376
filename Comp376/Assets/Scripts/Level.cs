using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level #", menuName = "Level")]
public class Level : ScriptableObject
{
    public int maximumSpawners;
    public int addSpawnerWaveInterval;
    public int numberOfSpawnersToAdd;
}
