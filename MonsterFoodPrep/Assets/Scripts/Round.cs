using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Round : MonoBehaviour {

    public int number;
    public float timeLimit;
    public Dish[] dishes;
    public Transform dishSpawn;
    public Transform monsterSpawn;
    public Vector3 monsterSpawnArea;

    private List<Monster> monsters = new List<Monster>();
    private List<Bounds> spawnBounds = new List<Bounds>();

    public delegate void RoundComplete();
    public event RoundComplete roundComplete;

    public float startTime;

    
    /*
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
                */

    IEnumerator Start()
    {
        //Show round number
        yield return new WaitForSeconds(1.0f);

        foreach(Dish dish in dishes)
        {
            SpawnDish(dish);
            foreach(Monster monster in dish.monsters)
            {
                SpawnMonster(monster);
                yield return new WaitForSeconds(0.1f);
            }

            while(true)
            {  
                //check time and monsters
                yield return null;
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
        Vector3 area = monsterSpawnArea * 0.5f;
        Vector3 position = monsterSpawn.position +
            new Vector3(area.x * Random.Range(-1.0f, 1.0f), 0.0f, area.z * Random.Range(-1.0f, 1.0f));
        Quaternion rotation = Quaternion.AngleAxis(Random.Range(0, 360), new Vector3(0, 1, 0));
        GameObject gameObject = (GameObject)Instantiate(monster.gameObject, position, rotation);
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        Monster instance = gameObject.GetComponent<Monster>();
        monsters.Add(instance);
    }
}
