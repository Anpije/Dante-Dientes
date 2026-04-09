using DG.Tweening;
using UnityEngine;

public class WorldCardBehaviours : MonoBehaviour
{
    public Vector3 defaultPosition;
    public Quaternion defaultRotation;
    public Vector3 targetPosition;
    public Quaternion targetRotation;

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
        atDefaultTransform = false;
        transform.DOMove(targetPosition, 0.15f, false);
        transform.DORotateQuaternion(targetRotation, 0.15f);
    }

    public void fReturnFromPosition(Transform startPos, float newSpeed)
    {
        // Posiciona una carta en otro lugar para después moverlo a su posición original
        gameObject.transform.position = startPos.position;
        gameObject.transform.rotation = startPos.rotation;

        targetPosition = defaultPosition;
        targetRotation = defaultRotation;

        transform.DOMove(targetPosition, 0.15f, false);
        transform.DORotateQuaternion(targetRotation, 0.15f);
        atDefaultTransform = true;
    }

    public void fDrawn(Transform deckPos)
    {
        // Mueve la carta desde el mazo hasta su posición original
        gameObject.transform.position = deckPos.position;
        gameObject.transform.rotation = deckPos.rotation;

        targetPosition = defaultPosition;
        targetRotation = defaultRotation;

        transform.DOMove(targetPosition, 0.15f, false);
        transform.DORotateQuaternion(targetRotation, 0.15f);
        atDefaultTransform = true;
    }

    public void fToggleCard(bool reveal)
    {
        // Esconde o revela una carta dependiendo en el booleano pasado
        if (!reveal)
        {
            transform.DOMove(new Vector3(transform.position.x, transform.position.y - 1, transform.position.z), 0.15f, false); 
            atDefaultTransform = false;
        }
        else
        {
            transform.DOMove(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), 0.15f, false); 
            atDefaultTransform = true;
        }
    }
}
