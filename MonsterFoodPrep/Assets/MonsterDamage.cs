using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterDamage : MonoBehaviour {
    
    public List<Rigidbody> breakable;
    private int damageLevel = 0;
    public bool hideRenderer = true;
    private List<GameObject> rememberToDestroy;

	// Use this for initialization
	void Start () {
        rememberToDestroy = new List<GameObject>();

        if (hideRenderer)
        {
            // line below is a hack, don't try this at home!
            GetComponent<Renderer>().enabled = false;
        }

        Monster mon = GetComponent<Monster>();
        mon.OnChop += OnChopHandler;
        mon.OnDeath += OnDeathHandler;
    }
	
	// Update is called once per frame
	void Update ()
    {	
	}

    void OnChopHandler(bool success, Monster monster)
    {
        if (!success) return;

        if (breakable.Count <= damageLevel) return;
        Rigidbody part = breakable[damageLevel];

        part.useGravity = true;
        part.GetComponent<Collider>().enabled = true;
        part.transform.parent = null;
        rememberToDestroy.Add(part.gameObject);
        breakable[damageLevel] = null;
        damageLevel++;
    }

    void OnDeathHandler(bool success, Monster monster)
    {
        if (success) return;

        foreach(Rigidbody part in breakable)
        {
            if (part != null)
            {
                part.useGravity = true;
                part.GetComponent<Collider>().enabled = true;
                part.transform.parent = null;
                rememberToDestroy.Add(part.gameObject);
            }
        }
    }

    void OnDestroy()
    {
        foreach(GameObject go in rememberToDestroy)
        {
            Destroy(go);
        }
    }
}
