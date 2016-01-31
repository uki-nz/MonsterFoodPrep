using UnityEngine;
using System.Collections;

[System.Serializable]
public struct MonsterSpawn
{
    public Monster monster;
    public Transform transform;
}

public class Dish : MonoBehaviour
{
    public MonsterSpawn[] monsterSpawns;
}
