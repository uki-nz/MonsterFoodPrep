using UnityEngine;
using System.Collections;

public class FxPop : MonoBehaviour {

    public float duration = 0.5f;

	// Use this for initialization
	void Start () {
        iTween.ScaleTo(gameObject, Vector3.one, duration);
        StartCoroutine(Kill());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator Kill()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
