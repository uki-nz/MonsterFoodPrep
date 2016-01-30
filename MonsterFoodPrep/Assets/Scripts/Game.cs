using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
    public Dish[] dishes;
    public Transform UiCanvas;
    public Camera UiCamera;
    public GameObject[] KillEffects;
    public Transform dishSpawn;
    public Transform monsterSpawn;
    public Transform choppingBoard;
    public float timeLimit;
    public float respawnDelay;
    public float fallSpeed;
    public int monsterQuantity = 4;
    public List<Monster> monsterPrefabs;

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
    }


    IEnumerator Start()
    {
        //foreach (Dish dish in dishes)
        //{
        //    foreach (Monster monster in dish.monsters)
        //    {
        //        SpawnMonster(monster);
        //        yield return new WaitForSeconds(0.1f);
        //    }
        SpawnDish(dishes[0]);

        startTime = Time.time;
        while (true)
        {
            //float countdown = timeLimit - (Time.time - startTime);
            //if (countdown <= 0.0f)
            //{
            //    break;
            //}
            if (monsters.Count < monsterQuantity)
            {
                SpawnMonster(monsterPrefabs[Random.Range(0, monsterPrefabs.Count)]);
            }
            yield return new WaitForEndOfFrame();
        }
        //}
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
            instance.OnDeath += OnDeathEventhandler;
            monsters.Add(instance);
            
            spawns.Add(bounds);
            break;
        }
    }

    void OnDeathEventhandler(bool success, Monster monster)
    {
        Debug.Log("OnDeathHandler", this);

        GameObject prefab;

        // we handle chilli and octopus destruction differently
        if (monster.tag == "Chilli" || monster.tag == "Octopus")
        {
            StartCoroutine(RemoveCorpse(monster.gameObject));
        }
        else
        {
            if (success)
            {
                prefab = monster.rightPrefab;
                SpawnKillEffects(monster.transform.position);
            }
            else
            {
                prefab = monster.wrongPrefab;
            }

            GameObject go = (GameObject)GameObject.Instantiate(prefab, monster.transform.position, monster.transform.rotation);
            StartCoroutine(RemoveCorpse(go));
            Destroy(monster.gameObject);
            Rigidbody[] bodies = go.GetComponentsInChildren<Rigidbody>();
            if (bodies.Length > 0)
            {
                bodies[0].AddExplosionForce(200f, bodies[1].position + Random.onUnitSphere, 10f);
            }
        }

        monsters.Remove(monster);
    }

    IEnumerator RemoveCorpse(GameObject go)
    {
        yield return new WaitForSeconds(3f);
        Destroy(go);
        //Object.Instantiate(monster.dummyPrefab, dishSpawn.position, Quaternion.AngleAxis(Random.Range(0, 360), new Vector3(0, 1, 0)));
    }

    void SpawnKillEffects(Vector3 pos)
    {
        GameObject fx = KillEffects[Random.Range(0, KillEffects.Length)];
        GameObject go = (GameObject)GameObject.Instantiate(fx, Vector3.zero, Quaternion.identity);

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);
        RectTransform fxTransform = (RectTransform)go.transform;
        RectTransform canvasTransform = (RectTransform)UiCanvas;
        fxTransform.anchoredPosition = screenPoint;
        fxTransform.SetParent(UiCanvas);
    }
}
