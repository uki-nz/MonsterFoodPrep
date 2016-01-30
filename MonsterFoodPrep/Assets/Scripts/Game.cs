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
    public List<GameObject> DishParts;

    private int currentDishIndex = 0;
    private float startTime;
    private List<Monster> monsters = new List<Monster>();

    public static Game game
    {
        get { return _game; }
    }
    private static Game _game;

    void Awake()
    {
        _game = this;
    }

    void Start()
    {
        SpawnDish(dishes[0]);
        startTime = Time.time;
    }

    void Update()
    {
        if (monsters.Count < monsterQuantity)
        {
            int rand = Random.Range(0, monsterPrefabs.Count);
            rand = Mathf.Clamp(rand, 0, monsterPrefabs.Count - 1);
            SpawnMonster(monsterPrefabs[rand]);
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
        Vector3 position = monsterSpawn.position + 
            new Vector3(extents.x * (Random.value - 0.5f), 0.0f, extents.z * (Random.value - 0.5f));

        bool overlap = true;
        while (overlap)
        {
            overlap = false;
            foreach (Monster m in monsters)
            {
                float dist = Vector3.Distance(position, m.transform.position);
                if (dist < 1f)
                {
                    overlap = true;
                    position = monsterSpawn.position +
             new Vector3(extents.x * (Random.value - 0.5f), 0.0f, extents.z * (Random.value - 0.5f));
                    break;
                }
            }
        }
        Quaternion rotation = Quaternion.AngleAxis(Random.Range(0, 360), new Vector3(0, 1, 0));
        GameObject gameObject = (GameObject)Instantiate(monster.gameObject, position, rotation);
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        Monster instance = gameObject.GetComponent<Monster>();
        instance.OnDeath += OnDeathEventhandler;
        monsters.Add(instance);
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
            if (prefab != null)
            {
                GameObject go = (GameObject)GameObject.Instantiate(prefab, monster.transform.position, monster.transform.rotation);
                StartCoroutine(RemoveCorpse(go));
                Destroy(monster.gameObject);
                Rigidbody[] bodies = go.GetComponentsInChildren<Rigidbody>();
                if (bodies.Length > 0)
                {
                    bodies[0].AddExplosionForce(200f, bodies[1].position + Random.onUnitSphere, 10f);
                }
            }
        }
        monsters.Remove(monster);
    }

    IEnumerator RemoveCorpse(GameObject go)
    {
        yield return new WaitForSeconds(3f);
        Destroy(go);

        if (currentDishIndex < DishParts.Count)
        {
            DishParts[currentDishIndex].SetActive(true);
            currentDishIndex++;
        }
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
