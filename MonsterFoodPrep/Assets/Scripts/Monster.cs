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
    // DEFINING EVENTS
    public event OnDeathEvent OnDeath;
    // VARS
    public GameObject deathPrefab;
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

    private Vector3 target;
    private Vector3 moveDirection;
    Vector3 direction;

    bool start;

    void Start()
    {
    }

    void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded)
        {
            state = MonState.Derpy;
            direction = Vector3.RotateTowards(direction, target - transform.position, turnSpeed * Time.deltaTime, 0.0f);
            direction.y = 0.0f;

            if(direction.magnitude > 0.0f)
                transform.forward = direction;

            if (Vector3.Distance(transform.position, target) > 0.1f)
                moveDirection = direction * moveSpeed;
        }
        moveDirection += Physics.gravity;
        controller.Move(moveDirection * Time.deltaTime);
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
            Vector3 offset = Random.insideUnitSphere;
            offset.y = 0.0f;
            target = transform.position + offset;
            yield return new WaitForSeconds(1.0f);
        }
    }

    public void Chop(Knife.ChopMode chop)
    {
        if (state < MonState.Derpy) return;

                
        print("CHOPPED");
        if (ChopsToKill.Count > (int)chop && ChopsToKill[chopCount] == chop)
        {
            chopCount++;

            if (ChopsToKill.Count == chopCount)
            {
                // done chopping, kill

                // run mesh-divider
                // update game class kill count
                Debug.Log("KILLED", this);
                state = MonState.Chopped;
                //if (OnDeath != null)
                {
                    print("CALLING ON DEATH");
                    OnDeath(true, this);
                }
                //Debug.Log("KILLED", this);
                //GameObject go = (GameObject) GameObject.Instantiate(deathPrefab, transform.position, transform.rotation);
                //StartCoroutine(RemoveCorpse(go));
                //Destroy(gameObject);
                // after interval, remove pieces, put stuff on plate  
                Debug.Log("Success! Awarded pts : " + scoreValue.ToString());
            }
        }
        else
        {
            // chopped wrongly, kill but no rewards

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

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //StartCoroutine(LookAround());
    }
    
}
