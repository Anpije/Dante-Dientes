using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;

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
        get { return Effect; }
        set { Effect = value; fAlterEffect(Effect); }
    }

    [Header("Stored Transforms")]
    public Vector3 inPlayPos = new Vector3();
    public Vector3 outPlayPos = new Vector3();
    public Transform target;

    [Header("Force Field")]
    [SerializeField] private GameObject _forceField; 

    [Header("Label Settings")]
    [SerializeField] private GameObject _labelQuad;           
    [SerializeField] private Texture2D _protectedTexture;    
    [SerializeField] private Texture2D _damagedTexture;      
    [SerializeField] private float _labelRiseAmount = 0.15f;  // cuánto sube el Quad hasta desaparecer depende que tan comorto de tiempo
    [SerializeField] private float _labelFadeInDuration = 0.25f;
    [SerializeField] private float _labelVisibleDuration = 1.2f;
    [SerializeField] private float _labelFadeOutDuration = 0.4f;

    // Posición local inicial del Quad que se guarda en el Awake para resetear siempre al mismo punto
    private Vector3 _labelStartLocalPos;
    private Material _labelMaterial;   
    private Renderer _labelRenderer;
    private Coroutine _labelCoroutine;

    private LocalKeyword _keyPulsing;
    private LocalKeyword _keyProtected;

    void Awake()
    {
        _MeshFilter = GetComponent<MeshFilter>();
        _Renderer = GetComponent<Renderer>();

        InitKeywords();

        inPlayPos = new Vector3(transform.position.x, 0f, transform.position.z);
        outPlayPos = new Vector3(transform.position.x, -0.1f, transform.position.z);
        transform.position = outPlayPos;

        // Force Field
        if (_forceField) _forceField.SetActive(false);

        if (_labelQuad)
        {
            _labelRenderer = _labelQuad.GetComponent<Renderer>();
            _labelMaterial = new Material(_labelRenderer.sharedMaterial);
            _labelRenderer.material = _labelMaterial;
            _labelStartLocalPos = _labelQuad.transform.localPosition;
            _labelMaterial.SetFloat("_Opacity", 0f);
            _labelQuad.SetActive(false);
        }
    }

    void InitKeywords()
    {
        if (_Renderer?.material?.shader == null) return;
        _keyPulsing = new LocalKeyword(_Renderer.material.shader, "_PULSING");
        _keyProtected = new LocalKeyword(_Renderer.material.shader, "_PROTECTED");
    }

    void ClearAllEffects()
    {
        // Keywords del diente
        if (_Renderer?.material != null)
        {
            _Renderer.material.SetKeyword(_keyPulsing, false);
            _Renderer.material.SetKeyword(_keyProtected, false);
        }

        // Force Field
        if (_forceField) _forceField.SetActive(false);

        // Label animado
        if (_labelCoroutine != null)
        {
            StopCoroutine(_labelCoroutine);
            _labelCoroutine = null;
        }
        if (_labelQuad)
        {
            _labelQuad.transform.DOKill();
            _labelMaterial?.SetFloat("_Opacity", 0f);
            _labelQuad.SetActive(false);
        }
    }

    public void fAddTooth(int newType)
    {
        toothType = (Tooth)newType;
        int ID = (int)toothType;

        _MeshFilter.mesh = _Models[ID];
        _Renderer.material = _Materials[ID];
        InitKeywords();

        transform.DOMove(inPlayPos, 0.15f);
    }

    public void fModifyTooth(int newType)
    {
        toothType = (Tooth)newType;
        StartCoroutine(SwapTooth((int)toothType));
    }

    public void fDeleteTooth()
    {
        toothType = Tooth.None;
        ClearAllEffects();
        transform.DOMove(outPlayPos, 0.15f);
    }

    IEnumerator SwapTooth(int toothID)
    {
        transform.DOMove(outPlayPos, 0.15f);
        yield return new WaitForSeconds(0.15f);

        _MeshFilter.mesh = _Models[toothID];
        _Renderer.material = _Materials[toothID];
        InitKeywords();

        if (Effect < 0) _Renderer.material.SetKeyword(_keyPulsing, true);
        else if (Effect > 0) _Renderer.material.SetKeyword(_keyProtected, true);

        yield return new WaitForSeconds(0.05f);
        transform.DOMove(inPlayPos, 0.15f);
    }

    public void fAlterEffect(int newEffect)
    {
        ClearAllEffects();

        if (newEffect < 0)
        {
            // DAÑO
            if (_Renderer?.material != null)
                _Renderer.material.SetKeyword(_keyPulsing, true);

            _labelCoroutine = StartCoroutine(ShowLabel(_damagedTexture));
        }
        else if (newEffect > 0)
        {
            // PROTECCIÓN
            if (_Renderer?.material != null)
                _Renderer.material.SetKeyword(_keyProtected, true);

            if (_forceField) _forceField.SetActive(true);

            _labelCoroutine = StartCoroutine(ShowLabel(_protectedTexture));
        }
    }

    IEnumerator ShowLabel(Texture2D texture)
    {
        if (_labelQuad == null || _labelMaterial == null || texture == null)
            yield break;

        _labelMaterial.SetTexture("_LabelTex", texture);
        _labelMaterial.SetFloat("_Opacity", 0f);
        _labelQuad.transform.localPosition = _labelStartLocalPos;
        _labelQuad.SetActive(true);

        // Fade in
        float elapsed = 0f;
        while (elapsed < _labelFadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _labelFadeInDuration);
            _labelMaterial.SetFloat("_Opacity", t);
            yield return null;
        }
        _labelMaterial.SetFloat("_Opacity", 1f);

        Vector3 riseTarget = _labelStartLocalPos + new Vector3(0f, _labelRiseAmount, 0f);
        _labelQuad.transform.DOLocalMove(riseTarget, _labelVisibleDuration + _labelFadeOutDuration)
                             .SetEase(Ease.OutQuad);

        yield return new WaitForSeconds(_labelVisibleDuration);

        // Fade out
        elapsed = 0f;
        float startOpacity = _labelMaterial.GetFloat("_Opacity");
        while (elapsed < _labelFadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _labelFadeOutDuration);
            _labelMaterial.SetFloat("_Opacity", Mathf.Lerp(startOpacity, 0f, t));
            yield return null;
        }

        _labelMaterial.SetFloat("_Opacity", 0f);
        _labelQuad.transform.DOKill();
        _labelQuad.SetActive(false);
        _labelCoroutine = null;
    }

    void OnDestroy()
    {
        ClearAllEffects();
        transform.DOKill();

        // Limpiamos las instancias del material
        if (_labelMaterial != null)
            Destroy(_labelMaterial);
    }
}