using UnityEngine;
using System.Collections;

public class TutorialLine : MonoBehaviour
{
    Transform parent;
    Vector3 position;
    Quaternion rotation;

    void Awake()
    {
        parent = transform.parent;
        position = transform.localPosition;
        rotation = transform.localRotation;
        transform.parent = null;
    }

    void Update()
    {
        transform.position = parent.position + position;
        transform.rotation = rotation;
    }
}
