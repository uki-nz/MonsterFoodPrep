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

    private bool chopping;
    private float y;
    private int rotation;
    private Vector3 choppingPoint;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        y = hand.transform.position.y;
    }

    void Start()
    {
        Game.instance.onGameOver += delegate ()
        {
            Destroy(this);
        };
    }

    void Update()
    {
        Rigidbody rigidbody = hand.GetComponent<Rigidbody>();
        HingeJoint hingeJoint = hand.GetComponent<HingeJoint>();

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(new Ray(hand.transform.position, Vector3.down), out raycastHit))
            {
                chopping = true;
                audioSource.Play();
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                choppingPoint = raycastHit.point;
            }
        }

        if (chopping)
        {
            rigidbody.MovePosition(Vector3.Lerp(hand.transform.position, choppingPoint, Time.deltaTime * chopSpeed));
            if (Vector3.Distance(hand.transform.position, choppingPoint) < 0.1f)
            {
                chopping = false;
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            }
        }
        else
        {
            Vector3 screenPosition = Input.mousePosition;
            screenPosition.z = Vector3.Distance(hand.transform.position, Camera.main.transform.position);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            worldPosition.y = y;
            rigidbody.MovePosition(Vector3.Lerp(hand.transform.position, worldPosition, Time.deltaTime * moveSpeed));

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            rotation = Mathf.Clamp(rotation + ((scroll > 0.0f) ? Mathf.CeilToInt(scroll) : Mathf.FloorToInt(scroll)), -2, 2);
            rigidbody.MoveRotation(Quaternion.AngleAxis((90 / 2) * rotation, new Vector3(0, 1, 0)));
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (chopping)
        {
            Monster monster = collider.GetComponent<Monster>();
            if (monster)
            {
                monster.Chop((ChopMode)Mathf.Abs(rotation));
            }
            //chopping = false;
        }
    }
}
