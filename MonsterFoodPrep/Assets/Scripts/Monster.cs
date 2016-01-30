using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour
{
    public float moveSpeed;
    public float turnSpeed;

    private Vector3 target;
    private Vector3 moveDirection;

    void Start()
    {
        StartCoroutine(LookAround());
    }

    void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();
     
        if (controller.isGrounded)
        {
            transform.forward = Vector3.RotateTowards(transform.forward, target - transform.position, turnSpeed * Time.deltaTime, 0.0f);
            if (Vector3.Distance(transform.position, target) > 0.1f)
            {
                moveDirection = transform.forward * moveSpeed;
            }
        }
        moveDirection += Physics.gravity;
        controller.Move(moveDirection * Time.deltaTime);
    }

    IEnumerator LookAround()
    {
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(Walk());
    }

    IEnumerator Walk()
    {   
        while(true)
        {
            Vector3 offset = Random.insideUnitSphere;
            offset.y = 0.0f;
            target = transform.position + offset;
            yield return new WaitForSeconds(1.0f);
        }
    }
}
