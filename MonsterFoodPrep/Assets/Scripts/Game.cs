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
    private List<Monster> monsters = new List<Monster>();

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
            foreach(Monster monster in dish.monsters)
            {

                SpawnMonster(monster);
            }

            yield return new WaitForSeconds(0.5f);
            SpawnDish(dish);

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
               Quaternion.identity);//Quaternion.AngleAxis(Random.Range(-45.0f, 45.0f), new Vector3(0, 0, 1))

        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        //rigidbody.angularVelocity = Random.insideUnitSphere * 360.0f;
    }

    void SpawnMonster(Monster monster)
    {
        GameObject gameObject = (GameObject)Instantiate(
               monster.gameObject,
               monsterSpawn.position + new Vector3(
                   spawnArea.x * Random.value - 0.5f,
                   spawnArea.y * Random.value - 0.5f,
                   spawnArea.z * Random.value - 0.5f),
               Quaternion.AngleAxis(Random.RandomRange(0, 360), new Vector3(0, 1, 0)));
        Monster instance = gameObject.GetComponent<Monster>();
        monsters.Add(monster);
    }
}
