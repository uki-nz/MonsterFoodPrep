using UnityEngine;
using System.Collections;

public class FxPop : MonoBehaviour {

	// Use this for initialization
	void Start () {
        iTween.ScaleTo(gameObject, Vector3.one, 0.5f);
        StartCoroutine(Kill());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator Kill()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
