using UnityEngine;
using System.Collections;

public class Knife : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public float rotationSpeed = 10.0f;

    float y;
    float distance;

    Quaternion rotation;

    Collider lastCollider;

    void Awake()
    {
        y = transform.position.y; ;
        distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        rotation = transform.rotation;
    }

    void Update()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        Vector3 screenPosition = Input.mousePosition;
        screenPosition.z = distance;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        worldPosition.y = y;

        if (Input.GetMouseButton(0))
        {
            rigidbody.velocity = new Vector3(0, -32, 0);
        }
        else
        {
            float horizontal = Input.GetAxis("Horizontal");
            if (horizontal != 0.0f)
            {
                rotation *= Quaternion.AngleAxis(Time.deltaTime * rotationSpeed * horizontal, new Vector3(0.0f, 1.0f, 0.0f));
            }

            rigidbody.isKinematic = false;
            rigidbody.velocity = new Vector3();
            rigidbody.MovePosition(Vector3.Lerp(transform.position, worldPosition, Time.deltaTime * moveSpeed));
            rigidbody.MoveRotation(rotation);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;

        MeshDivider meshDivider = collision.gameObject.GetComponent<MeshDivider>();
        if (meshDivider)
        {
            meshDivider.Divide(
                new Plane(
                    collision.transform.InverseTransformDirection(transform.right),
                    collision.transform.InverseTransformPoint(collision.contacts[0].point))
                    );
            rigidbody.detectCollisions = false;
        }
    }
}
