using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour
{
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

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("hit");
        StartCoroutine(LookAround());
    }
}
