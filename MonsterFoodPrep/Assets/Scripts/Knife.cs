using UnityEngine;
using System.Collections;

public class Knife : MonoBehaviour
{

    public enum ChopMode
    {
        Vertical,
        Diagonal,
        Horizontal
    }

    public float moveSpeed = 10.0f;
    public float chopSpeed = 40.0f;

    public Transform hand;

    [HideInInspector]
    public bool chopping;

    Vector3 choppingPoint;

    float y;
    float distance;

    float rotation;

    void Awake()
    {
        Transform transform = hand.transform;
        y = transform.position.y;
    }

    void Update()
    {
        Transform transform = hand.transform;
        Rigidbody rigidbody = hand.GetComponent<Rigidbody>();
        HingeJoint hingeJoint = hand.GetComponent<HingeJoint>();

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(new Ray(transform.position, Vector3.down), out raycastHit))
            {
                chopping = true;
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                choppingPoint = raycastHit.point + new Vector3(0, GetComponent<Renderer>().bounds.extents.y, 0);
            }
        }

        if (chopping)
        {
            rigidbody.MovePosition(Vector3.Lerp(transform.position, choppingPoint, Time.deltaTime * chopSpeed));
            if(Vector3.Distance(transform.position, choppingPoint) < 0.1f)
            {
                chopping = false;
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            }
        }
        else
        {
            /*
            RaycastHit raycastHit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit))
            {
    
                Vector3 point = raycastHit.point;
                point.y = y;
                Debug.Log("point");
                rigidbody.MovePosition(Vector3.Lerp(transform.position, point, Time.deltaTime * moveSpeed));
            }*/

            /*
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            scroll = (scroll > 0.0f) ? Mathf.Ceil(scroll) : Mathf.Floor(scroll);
            rotation = Mathf.Clamp(rotation + scroll, -2, 2);
            rigidbody.MoveRotation(Quaternion.AngleAxis((90 / 2) * rotation, new Vector3(0, 1, 0)));
            */
           
            Vector3 screenPosition = Input.mousePosition;
            screenPosition.z = Vector3.Distance(transform.position, Camera.main.transform.position);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            worldPosition.y = y;
            rigidbody.MovePosition(Vector3.Lerp(transform.position, worldPosition, Time.deltaTime * moveSpeed));
            
        }
    }

<<<<<<< HEAD
    void OnTriggerEnter(Collider collider)
    {
        Monster monster = collider.GetComponent<Monster>();
=======
    void OnTriggerEnter(Collider other)
    {
        Monster monster = other.GetComponent<Monster>();
>>>>>>> origin/master
        if(monster)
        {
            //monster.Chop((ChopMode)Mathf.Abs(rotation));
        }
    }
}
