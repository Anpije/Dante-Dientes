using System.Collections;
using UnityEngine;

public class ToothObject : MonoBehaviour
{
    MeshFilter _MeshFilter;
    [SerializeField] Mesh[] _Models = new Mesh[4];

    public enum Tooth { Incisor, Canine, PreMol, Molar, None }
    public Tooth toothType = Tooth.None;

    public int effect = 0;

    public Vector3 inPlayPos = new Vector3();
    public Vector3 outPlayPos = new Vector3();
    Vector3 target;

    void Awake()
    {
        _MeshFilter = GetComponent<MeshFilter>();
    }

    void Start()
    {
        inPlayPos = new Vector3(transform.position.x, 0, transform.position.z);
        outPlayPos = new Vector3(transform.position.x, -0.1f, transform.position.z);
        transform.position = outPlayPos;
    }

    public void fAddTooth(Tooth newType)
    {
        toothType = newType;
        int ID = (int)toothType;
        StartCoroutine(AddTooth(ID));
    }

    public void fModifyTooth(Tooth newType)
    {
        toothType = newType;
        int ID = (int)toothType;
        StartCoroutine(SwapTooth(ID));
    }

    public void fDeleteTooth()
    {
        toothType = Tooth.None;
        StartCoroutine("RemoveTooth");
    }

    IEnumerator AddTooth(int toothID)
    {
        _MeshFilter.mesh = _Models[toothID];
        for (int i = 0; i < 60; i++)
        {
            transform.position += new Vector3(0, 0.1f/60, 0);
            yield return new WaitForSeconds(1/60);
        }
        transform.position = inPlayPos;
        StopCoroutine("AddTooth");
    }

    IEnumerator SwapTooth(int toothID)
    {
        for (int i = 0; i < 60; i++)
        {
            transform.position -= new Vector3(0, 0.1f/60, 0);
            yield return new WaitForSeconds(1/60);
        }
        _MeshFilter.mesh = _Models[toothID];
        yield return new WaitForSeconds(0.01f);
        for (int i = 0; i < 60; i++)
        {
            transform.position += new Vector3(0, 0.1f/60, 0);
            yield return new WaitForSeconds(1/60);
        }
        transform.position = inPlayPos;
        StopCoroutine("SwapTooth");
    }

    IEnumerator RemoveTooth()
    {
        for (int i = 0; i < 60; i++)
        {
            transform.position -= new Vector3(0, 0.1f/60, 0);
            yield return new WaitForSeconds(1/60);
        }
        transform.position = outPlayPos;
        StopCoroutine("RemoveTooth");
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
