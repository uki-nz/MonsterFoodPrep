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
    public MovementPattern movementOptions;
    public GameObject deathSound;
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

    public float moveSpeed = 1.0f;
    public float turnSpeed = 45.0f;
    public float idleTimeMin = 5.0f;
    public float idleTimeMax = 10.0f;
    public bool canWalk = true;

    private CharacterController controller;
    private Vector3 moveDirection;
    private bool falling;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        StartCoroutine(Idle());
    }

    void Update()
    {
        if(!controller.isGrounded)
        {
            moveDirection += Physics.gravity * Time.deltaTime;
        }
        controller.Move(moveDirection * Time.deltaTime);
    }

    IEnumerator Idle()
    {
        while(true)
        {
            if (controller.isGrounded)
            {
                StartCoroutine(Looking());
                yield return new WaitForSeconds(Random.Range(idleTimeMin, idleTimeMax));
                StopCoroutine(Looking());
                yield return StartCoroutine(Walking());
            }
            yield return null;
        }
    }

    IEnumerator Looking()
    {
        while(true)
        {
            Vector3 direction = Random.onUnitSphere;
            direction.y = 0.0f;
            float dot = Vector3.Dot(transform.forward, direction);
            while (dot < 0.5f)
            {
                Debug.Log(dot);
                LookTowards(direction);
                dot = Vector3.Dot(transform.forward, direction);
                yield return new WaitForEndOfFrame();
            }
            //Debug.Log()
        }
    }

    IEnumerator Walking()
    {
        while(true)
        {
            yield return StartCoroutine(MoveInDirection(Random.onUnitSphere));
        }
    }

    IEnumerator MoveInDirection(Vector3 direction)
    {
        direction.y = 0.0f;
        while(true)
        {
            LookTowards(direction);
            Vector3 move = transform.forward * moveSpeed;
            moveDirection.x = move.x;
            moveDirection.z = move.z;
            yield return new WaitForEndOfFrame();
        }
    }

    void LookTowards(Vector3 direction)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.GetComponent<CharacterController>())
        {
            //StartCoroutine(Walk());
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
                OnChop(true, this);

            if (ChopsToKill.Count == chopCount)
            {
                // done chopping, kill

                // run mesh-divider
                // update game class kill count
                Debug.Log("KILLED", this);
                state = MonState.Chopped;
                if (OnDeath != null)
                    OnDeath(true, this);

                // after interval, remove pieces, put stuff on plate  
                Debug.Log("Success! Awarded pts : " + scoreValue.ToString());
            }
        }
        else
        {
            // chopped wrongly, kill but no rewards
            if (OnChop != null)
                OnChop(false, this);

            // run mesh divider
            // update game class fail count
            state = MonState.Chopped;
            if (OnDeath!= null)
                OnDeath(false, this);

            // leave chopped bits where they are
            Debug.Log("Fail!");
        }
    }
}
