using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class Monster : MonoBehaviour
{
    // ENUMS
    public enum MonState
    {
        Spawning = 0,
        Chopped,    // playing chopped animation
        Dead,
        Derpy,      // disoriented, wandering in circles
        Escaping
    };
    public enum MovementPattern
    {
        Circle,
        RunToPoint,
        RunToPointAndPause,
        Fast,
        Miniboss,
        Boss
    };
    // DEFINING DELEGATES
    private delegate float MovementDelegate(MonsterController mc);
    public delegate void OnDeathEvent(bool success, Monster monster);
    public delegate void OnChopEvent(bool success, Monster monster);
    // DEFINING EVENTS
    public event OnDeathEvent OnDeath;
    public event OnChopEvent OnChop;
    // VARS
    public GameObject rightPrefab;
    public GameObject wrongPrefab;
    public GameObject dummyPrefab;
    private CharacterController controller;
    public MovementPattern movementOptions;
    public List<Knife.ChopMode> ChopsToKill = new List<Knife.ChopMode>();
    public int scoreValue = 10;
    private int chopCount = 0;  // must init to 0 in Start() if we pool
    private Quaternion rotation;
    private float escapeAngle;
    private float randomSteer = 0;
    private MonState state = MonState.Spawning;
    // PROPERTIES
    public MonState State
    {
        get;
        protected set;
    }

    public float moveSpeed;
    public float turnSpeed;
    public float walkTimeMin;
    public float walkTimeMax;
    public float walkDistanceMin;
    public float walkDistanceMax;

    private Vector3 face;
    private Vector3 moveDirection;
    private Vector3 size;
    Vector3 direction;

    float time = 0.0f;

    bool start;

    void Start()
    {
        size = transform.localScale;
    }

    void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded)
        {
            if (state == MonState.Spawning)
            {
                state = MonState.Derpy;
            }

            if (!start)
            {
                Debug.Log("hit");
                StartCoroutine(LookAround());
                start = true;
            }

            if(direction != Vector3.zero)
            {
                face = Vector3.RotateTowards(face, direction, turnSpeed * Time.deltaTime, 0.0f);
                face.y = 0.0f;
                transform.forward = face.normalized;
               
            }
            moveDirection = face * moveSpeed;
        }
  
        moveDirection += Physics.gravity;
        controller.Move(moveDirection * Time.deltaTime);
        transform.localScale = new Vector3(size.x, size.y - (size.y * (Mathf.PingPong(time, 0.25f) * 0.2f)), size.z);
        time += Time.deltaTime;
    }

    IEnumerator LookAround()
    {
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(Walk());
    }

    IEnumerator Walk()
    {
        while (true)
        {
            Vector2 random = Random.insideUnitCircle * Random.Range(walkDistanceMin, walkDistanceMax);
            direction = new Vector3(random.x, 0.0f, random.y);
            yield return new WaitForSeconds(Random.Range(walkTimeMin, walkTimeMax));
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.GetComponent<CharacterController>())
        {
            direction = Vector3.zero;
            StartCoroutine(Walk());
        }
    }

    public void Chop(Knife.ChopMode chop)
    {
        if (state < MonState.Derpy) return;

        if (ChopsToKill.Count == 0) return;

        print("CHOPPED");
        if (ChopsToKill[chopCount] == chop)
        {
            chopCount++;

            if (OnChop != null)
            {
                OnChop(true, this);
            }

            if (ChopsToKill.Count == chopCount)
            {
                // done chopping, kill

                // run mesh-divider
                // update game class kill count
                Debug.Log("KILLED", this);
                state = MonState.Chopped;
                if (OnDeath != null)
                {
                    OnDeath(true, this);
                }

                // after interval, remove pieces, put stuff on plate  
                Debug.Log("Success! Awarded pts : " + scoreValue.ToString());
            }
        }
        else
        {
            // chopped wrongly, kill but no rewards
            if (OnChop != null)
            {
                OnChop(false, this);
            }

            // run mesh divider
            // update game class fail count
            state = MonState.Chopped;
            if (OnDeath!= null)
            {
                OnDeath(false, this);
            }

            // leave chopped bits where they are
            Debug.Log("Fail!");
        }
    }

    IEnumerator RemoveCorpse(GameObject toRemove)
    {
        yield return new WaitForSeconds(3f);
        Destroy(toRemove);
    }
}
