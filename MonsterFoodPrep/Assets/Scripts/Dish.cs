using UnityEngine;
using System.Collections;

[System.Serializable]
public struct MonsterInfo
{
    public Monster monsterPrefab;
    public Transform spawnPoint;
}

[System.Serializable]
public struct MonsterWave
{
    public MonsterInfo[] mobs;
}

public class Dish : MonoBehaviour
{
    public MonsterWave[] monsterWaves;
}
