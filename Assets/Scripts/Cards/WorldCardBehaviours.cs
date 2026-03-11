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

        gameObject.transform.position = Vector3.zero;
        gameObject.transform.rotation = new Quaternion(0, 38.779f, 0, 0);
    }

    public void fGoToPosition(Transform newPos, float newSpeed)
    {
        // Mueve una carta a la posición pasada, este posición no puede ser la posición original
        targetPosition = newPos.position;
        targetRotation = newPos.rotation;
        speed = newSpeed;
        moving = true;
        timeCount = 0.0f;
        rotating = true;
        atDefaultTransform = false;
    }

    public void fReturnFromPosition(Transform startPos, float newSpeed)
    {
        // Posiciona una carta en otro lugar para después moverlo a su posición original
        gameObject.transform.position = startPos.position;
        gameObject.transform.rotation = startPos.rotation;

        targetPosition = defaultPosition;
        targetRotation = defaultRotation;
        speed = newSpeed;
        moving = true;
        timeCount = 0.0f;
        rotating = true;
        atDefaultTransform = true;
    }

    public void fDrawn(Transform deckPos)
    {
        // Mueve la carta desde el mazo hasta su posición original
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

    public void fToggleCard(bool reveal)
    {
        // Esconde o revela una carta dependiendo en el booleano pasado
        if (!reveal)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z); 
            atDefaultTransform = false;
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z); 
            atDefaultTransform = true;
        }
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
