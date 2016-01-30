using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
    public Dish[] dishes;
    public Transform dishSpawn;
    public Transform monsterSpawn;
    public Vector3 spawnArea;
    public int spawnCount;
    public float spawnDelay;
    public float respawnDelay;
    public float timeLimit;

    private float startTime;

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
        foreach(Dish dish in dishes)
        {
            List<Monster> monsters = new List<Monster>();
            foreach(Monster ingredient in dish.ingredients)
            {
                /*
                GameObject gameObject = (GameObject)Instantiate(
                   ingredient.gameObject,
                   monsterSpawn.position + new Vector3(
                       spawnArea.x * Random.value - 0.5f,
                       spawnArea.y * Random.value - 0.5f,
                       spawnArea.z * Random.value - 0.5f),
                   Random.rotation);
                Monster monster = gameObject.GetComponent<Monster>();
                monsters.Add(monster);
                yield return new WaitForSeconds(spawnDelay);
                */
            }

            startTime = Time.time;
            while (true)
            {
                float countdown = timeLimit - (Time.time - startTime);
                if (countdown <= 0.0f)
                {
                    break;
                }
                if (monsters.Count == 0)
                {
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }

    void SpawnDish(Dish dish)
    {
        GameObject gameObject = (GameObject)Instantiate(
               dish.gameObject,
               dishSpawn.position,
               Quaternion.identity);

        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        rigidbody.angularVelocity = Random.onUnitSphere;
    }

    void SpawnMonster(Monster monster)
    {

    }

    void CreateMonster()
    {
        /*
        while (true)
        {

            MonsterType monsterType = monsterTypes[Random.Range(0, monsterTypes.Length - 1)];
            if (Random.value <= monsterType.chance)
            {
                GameObject instance = (GameObject)Instantiate(
                    monsterType.prefab,
                    monsterSpawn.position + new Vector3(
                        spawnArea.x * Random.value - 0.5f,
                        spawnArea.y * Random.value - 0.5f,
                        spawnArea.z * Random.value - 0.5f),
                    Random.rotation);
                Monster monster = instance.GetComponent<Monster>();
                //monster.onDeath += Invoke("CreateMonster", respawnDelay);
                //monster.onPrepared;
                break;
            }
        }*/
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(timeLimit);
    }
}
