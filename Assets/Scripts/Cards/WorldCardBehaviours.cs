using UnityEngine;

public class WorldCardBehaviours : MonoBehaviour
{
    public Vector3 defaultPosition;
    public Quaternion defaultRotation;
    public Vector3 targetPosition;
    public Quaternion targetRotation;
    public float speed;
    private bool moving = false;

    private bool rotating = false;
    private float timeCount = 0.0f;

    public bool atDefaultTransform;

    void Awake()
    {
        defaultPosition = gameObject.transform.position;
        defaultRotation = gameObject.transform.rotation;
    }

    public void fGoToPosition(Transform newPos, float newSpeed)
    {
        targetPosition = newPos.position;
        targetRotation = newPos.rotation;
        speed = newSpeed;
        moving = true;
        timeCount = 0.0f;
        rotating = true;
        atDefaultTransform = false;
    }

    public void fDrawn(Transform deckPos)
    {
        gameObject.transform.position = deckPos.position;
        gameObject.transform.rotation = deckPos.rotation;

        targetPosition = defaultPosition;
        targetRotation = defaultRotation;
        speed = 3;
        moving = true;
        timeCount = 0.0f;
        rotating = true;
        atDefaultTransform = true;
    }

    void Update()
    {
        if (moving)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
            if (transform.position == targetPosition) moving = false;
        }

        if (rotating)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, timeCount);
            timeCount = timeCount + Time.deltaTime;
            if (transform.rotation == targetRotation) rotating = false;
        }
    }
}
