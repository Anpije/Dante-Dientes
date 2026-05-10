using System.Collections;
using UnityEngine.Rendering;
using DG.Tweening;
using UnityEngine;

public class ToothObject : MonoBehaviour
{
    [Header("Appearance")]
    MeshFilter _MeshFilter;
    Renderer _Renderer;
    [SerializeField] Mesh[] _Models = new Mesh[5];
    [SerializeField] Material[] _Materials = new Material[5];

    public enum Tooth { Molar, CentralIncisor, Canine, LateralIncisor, Rainbow, None }
    [Header("Details")]
    public Tooth toothType = Tooth.None;
    [SerializeField] private int Effect;
    public int effect
    {
        get
        { return Effect; }
        set
        {
            Effect = value;
            fAlterEffect(Effect);
        }
    }

    [Header("Stored Transforms")]
    public Vector3 inPlayPos = new Vector3();
    public Vector3 outPlayPos = new Vector3();
    public Transform target;
    private LocalKeyword keyPulsing;

    void Awake()
    {
        _MeshFilter = GetComponent<MeshFilter>();
        _Renderer = GetComponent<Renderer>();   
        inPlayPos = new Vector3(transform.position.x, 0, transform.position.z);
        outPlayPos = new Vector3(transform.position.x, -0.1f, transform.position.z);
        transform.position = outPlayPos;
    }

    public void fAddTooth(int newType)
    {
        toothType = (Tooth)newType;
        int ID = (int)toothType;
        _MeshFilter.mesh = _Models[ID];
        _Renderer.material = _Materials[ID];
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
        _Renderer.material = _Materials[toothID];
        yield return new WaitForSeconds(0.2f);
        transform.DOMove(inPlayPos, 0.15f);
        yield return new WaitForSeconds(0.15f);
        StopCoroutine("SwapTooth");
    }

    public void fAlterEffect(int newEffect)
    {
        // Des/Activar partículas o shader
        
        if (newEffect < 0)
        {
            keyPulsing = new LocalKeyword(_Renderer.material.shader, "_PULSING");
            _Renderer.material.SetKeyword(keyPulsing, true);
        }
        if (newEffect > 0)
        {
            keyPulsing = new LocalKeyword(_Renderer.material.shader, "_PULSING");
            _Renderer.material.SetKeyword(keyPulsing, false);
        }
        if (newEffect == 0)
        {
            keyPulsing = new LocalKeyword(_Renderer.material.shader, "_PULSING");
            _Renderer.material.SetKeyword(keyPulsing, false);
        }
    }
}
