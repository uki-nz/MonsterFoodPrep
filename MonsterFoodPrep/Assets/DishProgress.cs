using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DishProgress : MonoBehaviour {

    public List<GameObject> foodParts;
    private float progress;
    public float Progress
    {
        get
        {
            return progress;
        }
        set
        {
            progress = value;
            SetProgress(progress);
        }
    }    

	// Use this for initialization
	void Start ()
    {
        foreach(GameObject go in foodParts)
        {
            go.SetActive(false);
        }
	}

    /// <summary>
    /// dish parts appear according to dish progress
    /// </summary>
    /// <param name="prog">value from 0 to 1</param>
    public void SetProgress(float prog)
    {
        prog = Mathf.Clamp01(prog);
        progress = prog;
        int numParts = (int)(prog * foodParts.Count);

        for (int i = 0; i < numParts; i++)
        {
            foodParts[i].SetActive(true);
        }
    }
}
