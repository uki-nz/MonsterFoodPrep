using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour
{
    public Round[] rounds;
    public GameObject gameOverScreen;
    public GameObject countdownTimer;
    public Text countdownText;
    public Image countdownImage;
    public Transform UiCanvas;
    public Camera UiCamera;
    public GameObject[] KillEffects;
    AudioSource audio;
    public AudioClip chopSound;
    public AudioClip deathSound;
    public GameObject chopSuccessfulPopup;
    public GameObject chopWastePopup;
    public GameObject chopPopup;
    public Transform dishSpawn;
    public Transform foodSpawn;
    public GameObject finishedDish;
    public float respawnDelay = 5f;
    public float completeDelay = 1f;
    private DishProgress currentDish;

    public delegate void OnGameOver();
    public event OnGameOver onGameOver;

    [HideInInspector]
    public int monstersRemaining;
    [HideInInspector]
    public int monstersRequired;
    [HideInInspector]
    public bool gameOver;
    [HideInInspector]
    public static Game instance;

    List<Monster> monsters;

    void Awake()
    {
        instance = this;
        audio = GetComponent<AudioSource>();
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
                Dish dishInstance = SpawnDish(dish);

                monstersRequired = dish.monsterSpawns.Length;
                monstersRemaining = monstersRequired;
                monsters = new List<Monster>();

                GameObject go = (GameObject)GameObject.Instantiate(dishInstance.dishPrefab.gameObject, foodSpawn.position, foodSpawn.rotation);
                currentDish = go.GetComponent<DishProgress>();

                foreach (MonsterSpawn monsterSpawn in dish.monsterSpawns)
                {
                    Monster monsterInstance = SpawnMonster(monsterSpawn);
                    monsters.Add(monsterInstance);
                    yield return new WaitForSeconds(0.05f);
                }

                while (true)
                {
                    if (currentDish != null)
                    {
                        float val = (((float)monstersRequired - monstersRemaining) / monstersRequired);
                        if (val > 0)
                            Debug.Log("Dish is "+val*100+"% complete", this);
                        currentDish.SetProgress(val);
                    }

                    if (monstersRemaining == 0)
                        break;

                    yield return new WaitForEndOfFrame();
                }

                //finishedDish.SetActive(true);

                yield return new WaitForSeconds(completeDelay);

                //finishedDish.SetActive(false);

                Destroy(dishInstance.gameObject);
                Destroy(currentDish.gameObject);
            }
        }

        GameOver();
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
                foreach(Monster monster in monsters)
                {
                    monster.OnDeath -= OnDeath;
                }

                GameOver();
                           
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    void GameOver()
    {
        gameOver = true;
        if (onGameOver != null)
            onGameOver();

        gameOverScreen.SetActive(true);
    }

    public void PlayAgain()
    {
        Application.LoadLevel("Main");
    }

    public void ReturnToMenu()
    {
        Application.LoadLevel("Menu");
    }

    Dish SpawnDish(Dish dish)
    {
        GameObject gameObject = (GameObject)Instantiate(
               dish.gameObject,
               dishSpawn.position,
               Quaternion.identity);
        return gameObject.GetComponent<Dish>();
    }

    Monster SpawnMonster(MonsterSpawn monsterSpawn)
    {
        Quaternion rotation = Quaternion.AngleAxis(Random.Range(0, 360), new Vector3(0, 1, 0));
        GameObject gameObject = (GameObject)Instantiate(monsterSpawn.monster.gameObject, monsterSpawn.transform.position, rotation);
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        Monster instance = gameObject.GetComponent<Monster>();
        instance.OnChop += OnChopEventHandler;
        instance.OnDeath += OnDeath;
        instance.OnDeath += OnDeathEventhandler;
        instance.monsterSpawn = monsterSpawn;
        return gameObject.GetComponent<Monster>();
    }

    IEnumerator RespawnMonster(MonsterSpawn monsterSpawn, float time)
    {
        yield return new WaitForSeconds(time);
        SpawnMonster(monsterSpawn);
    }

    void OnDeath(bool success, Monster monster)
    {
        if (success)
            monstersRemaining--;
        else
            StartCoroutine(RespawnMonster(monster.monsterSpawn, respawnDelay));

        monsters.Remove(monster);
    }

    private void OnChopEventHandler(bool success, Monster monster)
    {
        if (success)
        {
            Debug.Log(monster.name+" MONSTER STATE!!! " + monster.State);

            if (monster.State == Monster.MonState.Dead) return;
            
            GameObject fx = chopPopup;
            GameObject go = (GameObject)GameObject.Instantiate(fx, Vector3.zero, Quaternion.identity);

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, monster.transform.position);
            RectTransform fxTransform = (RectTransform)go.transform;
            RectTransform canvasTransform = (RectTransform)UiCanvas;
            fxTransform.anchoredPosition = screenPoint;
            fxTransform.SetParent(UiCanvas);
        }
        
    }

    void OnDeathEventhandler(bool success, Monster monster)
    {
        Debug.Log("OnDeathHandler", this);

        GameObject prefab;

        // we handle chilli and octopus destruction differently
        if (monster.tag == "Chilli" || monster.tag == "Octopus")
        {
            Destroy(monster.gameObject, 3f);
            SpawnKillEffects(monster.transform.position, success);
        }
        else
        {
            if (success)
            {
                prefab = monster.rightPrefab;
                SpawnKillEffects(monster.transform.position, true);
            }
            else
            {
                prefab = monster.wrongPrefab;
                SpawnKillEffects(monster.transform.position, false);
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

    void SpawnKillEffects(Vector3 pos, bool success)
    {
        GameObject fx;
        if (success)
        {
            fx = chopSuccessfulPopup;
        }
        else
        {
            fx = chopWastePopup;
        }
        GameObject go = (GameObject)GameObject.Instantiate(fx, Vector3.zero, Quaternion.identity);

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);
        RectTransform fxTransform = (RectTransform)go.transform;
        RectTransform canvasTransform = (RectTransform)UiCanvas;
        fxTransform.anchoredPosition = screenPoint;
        fxTransform.SetParent(UiCanvas);
        
        audio.PlayOneShot(deathSound);
    }
}
