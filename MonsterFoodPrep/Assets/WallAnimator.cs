using UnityEngine;
using System.Collections;

public class WallAnimator : MonoBehaviour {

    Material mat;

	// Use this for initialization
	void Start () {
        mat = GetComponent<Renderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 newOffset = mat.mainTextureOffset;
        newOffset.x += Time.deltaTime * 0.1f;
        mat.mainTextureOffset = newOffset;
	}
}
