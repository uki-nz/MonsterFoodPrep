using UnityEngine;
using System.Collections;

public class Knife : MonoBehaviour
{
    public Hand hand;

    void OnCollisionEnter(Collision collision)
    {
        hand.chopping = false;
        HingeJoint hingeJoint = hand.GetComponent<HingeJoint>();
        hingeJoint.connectedBody.constraints = RigidbodyConstraints.None;
    }
}
