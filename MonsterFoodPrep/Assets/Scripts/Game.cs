using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour
{

    [System.Serializable]
    public struct MonsterType
    {
        public GameObject prefab;
        [Range(0.0f, 1.0f)]
        public float chance;
    }
    public MonsterType[] monsterTypes;
    public int spawnCount;
    public float spawnDelay;
    public float respawnDelay;
    public Transform spawnLocation;
    public Vector3 spawnArea;
    public float timeLimit;
 

    private float startTime;
    private List<Monster> monsters;

    public static Game game
    {
        get
        {
            return _game;
        }
    }
    private static Game _game;

    void OnValidate()
    {
    }

    void Awake()
    {
        _game = this;
    }

    IEnumerator Start()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            CreateMonster();
            yield return new WaitForSeconds(spawnDelay);
        }

        startTime = Time.time;
        while(true)
        {
            float countdown = timeLimit - (Time.time - startTime);
            if (countdown <= 0.0f)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }

    }

    void CreateMonster()
    {
        while (true)
        {
            MonsterType monsterType = monsterTypes[Random.Range(0, monsterTypes.Length - 1)];
            if (Random.value <= monsterType.chance)
            {
                GameObject instance = (GameObject)Instantiate(
                    monsterType.prefab,
                    spawnLocation.position + new Vector3(
                        spawnArea.x * Random.value - 0.5f,
                        spawnArea.y * Random.value - 0.5f,
                        spawnArea.z * Random.value - 0.5f),
                    Random.rotation);
                Monster monster = instance.GetComponent<Monster>();
                //monster.onDeath += Invoke("CreateMonster", respawnDelay);
                break;
            }
        }
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(timeLimit);
    }
}
