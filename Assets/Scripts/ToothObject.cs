using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ToothObject : MonoBehaviour
{
    [Header("Appearance")]
    MeshFilter _MeshFilter;
    MeshRenderer _MeshRenderer;
    [SerializeField] Mesh[] _Models = new Mesh[5];
    [SerializeField] Material[] _Materials = new Material[5];

    public enum Tooth { Incisor, Canine, PreMol, Molar, Rainbow, None }
    [Header("Details")]
    public Tooth toothType = Tooth.None;
    public int effect = 0;

    [Header("Stored Transforms")]
    public Vector3 inPlayPos = new Vector3();
    public Vector3 outPlayPos = new Vector3();
    public Transform target;

    void Awake()
    {
        _MeshFilter = GetComponent<MeshFilter>();
        _MeshRenderer = GetComponent<MeshRenderer>();   
        inPlayPos = new Vector3(transform.position.x, 0, transform.position.z);
        outPlayPos = new Vector3(transform.position.x, -0.1f, transform.position.z);
        transform.position = outPlayPos;
    }

    public void fAddTooth(int newType)
    {
        toothType = (Tooth)newType;
        int ID = (int)toothType;
        _MeshFilter.mesh = _Models[ID];
        _MeshRenderer.material = _Materials[ID];
        transform.DOMove(inPlayPos, 0.15f);
    }

    public void fModifyTooth(int newType)
    {
        toothType = (Tooth)newType;
        int ID = (int)toothType;
        StartCoroutine(SwapTooth(ID));
    }

    public void fDeleteTooth()
    {
        toothType = Tooth.None;
        transform.DOMove(outPlayPos, 0.15f);
    }

    IEnumerator SwapTooth(int toothID)
    {
        transform.DOMove(outPlayPos, 0.15f);
        _MeshFilter.mesh = _Models[toothID];
        _MeshRenderer.material = _Materials[toothID];
        yield return new WaitForSeconds(0.2f);
        transform.DOMove(inPlayPos, 0.15f);
        yield return new WaitForSeconds(0.15f);
        StopCoroutine("SwapTooth");
    }

    public void fAlterEffect(int alter)
    {
        effect += alter;
        // Des/Activar partículas o shader
        /*
        if (effect > 0)
            // efecto positivo
        else if (effect < 0)
            // efecto negativo
        */
    }
}
