using UnityEngine;
using System.Collections;

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
    private static MovementDelegate[] movementDelegates =
    {
        Circle,
        RunToPoint,
        RunToPointAndPause,
        Fast,
        Miniboss,
        Boss
    };
    public delegate void OnDeathEvent(bool success);
    // DEFINING EVENTS
    public event OnDeathEvent OnDeath;
    // VARS
    private CharacterController controller;
    public MovementPattern movementOptions;
    public List<ChopMode> ChopsToKill = new List<ChopMode>();
    public float movementSpeed = 1f; // baseline
    [Range(0.01f, 1f)]
    public float turningSpeed = 1f;
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

    void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded)
        {
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

<<<<<<< HEAD
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("hit");
        StartCoroutine(LookAround());
=======
    void OnCollisionEnter(Collision col)
    {
        switch (state)
        {
            case MonState.Spawning:
                {
                    if (col.collider.tag == "Table" || col.collider.tag == "Chopping Block")
                    {
                        state = MonState.Derpy;
                    }
                }
                break;
            case MonState.Dead:
            case MonState.Chopped:
                // ignore collisions when already dead
                break;
            default:
                {
                    if (col.collider.tag == "Knife")
                    {
                        // we need to query the Hand/Knife for the chopping mode
                        // then call Chop(mode)
                    }
                }
                break;
        }
    }

    private static float Circle(MonsterController mc)
    {
        switch (mc.state)
        {
            case MonState.Derpy:
                {
                    // run in circles
                    mc.randomSteer += Random.Range(-0.2f, 0.2f) * Time.fixedDeltaTime;
                    mc.randomSteer = Mathf.Clamp(mc.randomSteer, -0.05f, 0.05f);
                    return (((Time.time * mc.turningSpeed) + mc.randomSteer) % 1f);
                }
            case MonState.Escaping:
                {
                    // continuously run towards chosen escape angle
                    return mc.escapeAngle;
                }
            default:
                break;
        }

        return 0;
    }
    private static float RunToPoint(MonsterController mc)
    {
        Debug.LogError("Error: Movement Function (Standard2) has not yet been implemented");
        return 0;
    }
    private static float RunToPointAndPause(MonsterController mc)
    {
        Debug.LogError("Error: Movement Function (Slow) has not yet been implemented");
        return 0;
    }
    private static float Fast(MonsterController mc)
    {
        Debug.LogError("Error: Movement Function (Fast) has not yet been implemented");
        return 0;
    }
    private static float Miniboss(MonsterController mc)
    {
        Debug.LogError("Error: Movement Function (Miniboss) has not yet been implemented");
        return 0;
    }
    private static float Boss(MonsterController mc)
    {
        Debug.LogError("Error: Movement Function (Boss) has not yet been implemented");
        return 0;
>>>>>>> origin/master
    }
}
