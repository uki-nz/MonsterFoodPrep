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
    public DishProgress dishPrefab;
    public MonsterSpawn[] monsterSpawns;
}
