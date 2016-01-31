using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
    public Round[] rounds;
    public GameObject countdownTimer;
    public Text countdownText;
    public Image countdownImage;
    public Transform UiCanvas;
    public Camera UiCamera;
    public GameObject[] KillEffects;
    public Transform dishSpawn;
    public float respawnDelay = 5f;
    public float completeDelay = 1f;

    public static Game game
    {
        get { return _game; }
    }
    private static Game _game;

    void Awake()
    {
        _game = this;
    }

    IEnumerator Start()
    {
        foreach(Round round in rounds)
        {
            countdownTimer.SetActive(false);
            yield return new WaitForSeconds(1.0f);

            countdownTimer.SetActive(true);
            StartCoroutine(Countdown(round.timeLimit));

            foreach (Dish dish in round.dishes)
            {
                int count = 0;
                Dish dishInstance = SpawnDish(dish);
                List<Monster> monsters = new List<Monster>();
                foreach (MonsterSpawn monsterSpawn in dish.monsterSpawns)
                {
                    Monster monsterInstance = SpawnMonster(monsterSpawn.monster, monsterSpawn.transform);
                    monsterInstance.OnDeath += delegate (bool success, Monster monster)
                    {
                        if (success)
                            count++;

                        monsters.Remove(monster);
                    };
                    monsterInstance.OnDeath += OnDeath;
                    monsters.Add(monsterInstance);
                    yield return new WaitForSeconds(0.05f);
                }

                while (true)
                {
                    if (count == dish.monsterSpawns.Length)
                        break;

                    yield return new WaitForEndOfFrame();
                }

                yield return new WaitForSeconds(completeDelay);
                Destroy(dishInstance.gameObject);
            }
        }
    }

    IEnumerator Countdown(float time)
    {
        float start = Time.time;
        while (true)
        {
            float timeLeft = time - (Time.time - start);
            countdownText.text = timeLeft.ToString("0.0");
            countdownImage.fillAmount = timeLeft / time;
            if (timeLeft <= 0.0f)
            {
                //Game over
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    Dish SpawnDish(Dish dish)
    {
        GameObject gameObject = (GameObject)Instantiate(
               dish.gameObject,
               dishSpawn.position,
               Quaternion.identity);
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.down * 10f;
        return gameObject.GetComponent<Dish>();
    }

    Monster SpawnMonster(Monster monster, Transform transform)
    {
        Quaternion rotation = Quaternion.AngleAxis(Random.Range(0, 360), new Vector3(0, 1, 0));
        GameObject gameObject = (GameObject)Instantiate(monster.gameObject, transform.position, rotation);
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        return gameObject.GetComponent<Monster>();
    }

    void OnDeath(bool success, Monster monster)
    {
        Debug.Log("OnDeathHandler", this);

        GameObject prefab;

        // we handle chilli and octopus destruction differently
        if (monster.tag == "Chilli" || monster.tag == "Octopus")
        {
            Destroy(monster.gameObject, 3f);
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
                Destroy(go, 3f);
                Destroy(monster.gameObject);
                Rigidbody[] bodies = go.GetComponentsInChildren<Rigidbody>();
                if (bodies.Length > 0)
                {
                    bodies[0].AddExplosionForce(200f, bodies[1].position + Random.onUnitSphere, 10f);
                }
            }
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
