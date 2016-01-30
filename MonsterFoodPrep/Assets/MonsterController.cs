/**

Collision Detection, State Machine & AI script

*/


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ChopMode
{
    Vertical,
    Diagonal,
    Horizontal
}
public class MonsterController : MonoBehaviour {
    
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
    // CONSTS
    public const float TWO_PI = Mathf.PI * 2;

    // Use this for initialization
    void Start () {
        rotation = new Quaternion();
        escapeAngle = Mathf.Sin(Random.value);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (state >= MonState.Derpy)
        {
            //Vector3 oldPos = transform.position;
            MovementDelegate f = movementDelegates[(int)movementOptions];
            float angle = f(this) * 360;
            rotation.eulerAngles = new Vector3(0, Mathf.Clamp(angle, -360f, 360f), 0);
            transform.rotation = rotation;
            transform.position = transform.position + transform.forward * Time.fixedDeltaTime * movementSpeed;
        }
	}

    void Chop(ChopMode chop)
    {
        if (ChopsToKill[chopCount] == chop)
        {
            chopCount++;

            if (ChopsToKill.Count == chopCount)
            {
                // done chopping, kill

                // run mesh-divider
                // update game class kill count
                OnDeath(true);
                // after interval, remove pieces, put stuff on plate  
                Debug.Log("Success! Awarded pts : " + scoreValue.ToString());
            }
        }
        else
        {
            // chopped wrongly, kill but no rewards

            // run mesh divider
            // update game class fail count
            OnDeath(false);
            // leave chopped bits where they are
            Debug.Log("Fail!");
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
    }

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
}
