using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
    public Dish[] dishes;
    public Transform dishSpawn;
    public Transform monsterSpawn;
    public Transform choppingBoard;
    public float timeLimit;
    public float respawnDelay;
    public float fallSpeed;

    private float startTime;
    private List<Bounds> spawns = new List<Bounds>();
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
       // Cursor.visible = false;
    }

    IEnumerator Start()
    {
        foreach(Dish dish in dishes)
        {
            foreach(Monster monster in dish.monsters)
            {
                SpawnMonster(monster);
                yield return new WaitForSeconds(0.1f);
            }
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
               Quaternion.identity);
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    void SpawnMonster(Monster monster)
    {
        Vector3 extents = choppingBoard.GetComponent<Renderer>().bounds.extents;
        while (true)
        {
            Vector3 position = monsterSpawn.position + 
                new Vector3(extents.x * (Random.value - 0.5f), 0.0f, extents.z * (Random.value - 0.5f));

            Bounds bounds = monster.GetComponent<Renderer>().bounds;
            bounds.center = position;

            bool intersects = false;
            foreach(Bounds spawn in spawns)
            {
                if(bounds.Intersects(spawn))
                {
                    intersects = true;
                    break;
                }
            }

            if (intersects)
                continue;

            Quaternion rotation = Quaternion.AngleAxis(Random.Range(0, 360), new Vector3(0, 1, 0));
            GameObject gameObject = (GameObject)Instantiate(monster.gameObject, position, rotation);
            Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
            Monster instance = gameObject.GetComponent<Monster>();
            monsters.Add(monster);
            monster.OnDeath += OnDeathEventhandler;
            spawns.Add(bounds);
            break;
        }
    }

    void OnDeathEventhandler(bool success, Monster monster)
    {
        Debug.Log("OnDeathHandler", this);
        if (success)
        {
            GameObject go = (GameObject)GameObject.Instantiate(monster.dummyPrefab, monster.transform.position, monster.transform.rotation);
            StartCoroutine(RemoveCorpse(go));
            monsters.Remove(monster);
            Destroy(monster.gameObject);

        }
        else
        {
            GameObject go = (GameObject)GameObject.Instantiate(monster.deathPrefab, monster.transform.position, monster.transform.rotation);
            StartCoroutine(RemoveCorpse(go));
            monsters.Remove(monster);
            Destroy(monster.gameObject);
        }
        Object.Instantiate(monster.dummyPrefab, dishSpawn.position, Quaternion.AngleAxis(Random.Range(0, 360), new Vector3(0, 1, 0)));
    }

    IEnumerator RemoveCorpse(GameObject go)
    {
        yield return new WaitForSeconds(3f);
        Destroy(go);
    }
}
