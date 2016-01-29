using UnityEngine;
using System.Collections;

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
    public Transform spawnLocation;
    public Vector3 spawnArea;
    public float timeLimit;
    private float time;

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
        DontDestroyOnLoad(gameObject); //maybe have different game objects per level
    }

    IEnumerator Start()
    {
       

        while(true)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator Spawn()
    {
        for (int i = 0; i < spawnCount; i++)
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
                    break;
                }
            }
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(timeLimit);
    }
}
