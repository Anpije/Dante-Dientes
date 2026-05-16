using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using UnityEngine.Rendering.Universal;

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
        set { PlaySound(value); Effect = value; fAlterEffect(Effect); }
    }

    [Header("Stored Transforms")]
    public Vector3 inPlayPos = new Vector3();
    public Vector3 outPlayPos = new Vector3();
    public Transform target;

    [Header("Decal Settings")]
    [SerializeField] private Material _protectedDecalMaterial;
    [SerializeField] private Material _damageDecalMaterial;
    [SerializeField] private float _decalHeight = 0.3f;
    [SerializeField] private float _decalSize = 1f;

    [Header("Force Field")]
    [SerializeField] private GameObject _forceField;

    private LocalKeyword _keyPulsing;
    private LocalKeyword _keyProtected;
    private Coroutine _decalCoroutine;
    private DecalProjector _decalProjector;
    private GameObject _decalObject;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sfxShield;
    [SerializeField] private AudioClip sfxDamage;

    void Awake()
    {
        _MeshFilter = GetComponent<MeshFilter>();
        _Renderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();

        InitKeywords();

        inPlayPos = new Vector3(transform.position.x, 0f, transform.position.z);
        outPlayPos = new Vector3(transform.position.x, -0.1f, transform.position.z);
        transform.position = outPlayPos;

        CreateDecalProjector();

        if (_forceField) _forceField.SetActive(false);
    }

    void CreateDecalProjector()
    {
        _decalObject = new GameObject("EffectDecal");
        _decalObject.transform.SetParent(transform);
        _decalObject.transform.localPosition = new Vector3(0f, _decalHeight, 0f);
        _decalObject.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        _decalObject.transform.localScale = Vector3.one;

        _decalProjector = _decalObject.AddComponent<DecalProjector>();
        _decalProjector.size = new Vector3(_decalSize, _decalSize, 1f);
        _decalProjector.fadeFactor = 0f;
        _decalProjector.drawDistance = 100f;

        _decalObject.SetActive(false);
    }

    void InitKeywords()
    {
        if (_Renderer?.material?.shader == null) return;
        _keyPulsing = new LocalKeyword(_Renderer.material.shader, "_PULSING");
        _keyProtected = new LocalKeyword(_Renderer.material.shader, "_PROTECTED");
    }

    void ClearAllEffects()
    {
        if (_Renderer?.material != null)
        {
            _Renderer.material.SetKeyword(_keyPulsing, false);
            _Renderer.material.SetKeyword(_keyProtected, false);
        }

        if (_decalCoroutine != null)
        {
            StopCoroutine(_decalCoroutine);
            _decalCoroutine = null;
        }

        if (_decalProjector != null)
        {
            _decalProjector.fadeFactor = 0f;
            _decalObject.SetActive(false);
        }

        if (_forceField) _forceField.SetActive(false);
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

        if (Effect < 0)
            _Renderer.material.SetKeyword(_keyPulsing, true);
        else if (Effect > 0)
            _Renderer.material.SetKeyword(_keyProtected, true);

        yield return new WaitForSeconds(0.05f);
        transform.DOMove(inPlayPos, 0.15f);
    }

    private void PlaySound(int value)
    {
        if (value < Effect)
        {
            audioSource.clip = sfxDamage;
            audioSource.Play();
        }
        else
        {
            audioSource.clip = sfxShield;
            audioSource.Play();
        }
    }

    public void fAlterEffect(int newEffect)
    {
        ClearAllEffects();

        if (newEffect < 0)
        {
            // DAÑO
            if (_Renderer?.material != null)
                _Renderer.material.SetKeyword(_keyPulsing, true);

            _decalCoroutine = StartCoroutine(ShowDecal(_damageDecalMaterial));
        }
        else if (newEffect > 0)
        {
            // PROTECCIÓN
            if (_Renderer?.material != null)
                _Renderer.material.SetKeyword(_keyProtected, true);

            if (_forceField) _forceField.SetActive(true);  

            _decalCoroutine = StartCoroutine(ShowDecal(_protectedDecalMaterial));
        }
    }

    IEnumerator ShowDecal(Material decalMaterial)
    {
        if (decalMaterial == null)
        {
            Debug.LogWarning("Material de Decal no asignado");
            yield break;
        }

        _decalProjector.material = decalMaterial;
        _decalObject.SetActive(true);

        // Fade in
        float elapsed = 0f;
        float fadeInDuration = 0.3f;
        float visibleDuration = 1.5f;
        float fadeOutDuration = 0.5f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            _decalProjector.fadeFactor = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }

        _decalProjector.fadeFactor = 1f;

        // Mantenenos visible
        yield return new WaitForSeconds(visibleDuration);

        // Fade out
        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            _decalProjector.fadeFactor = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            yield return null;
        }

        _decalProjector.fadeFactor = 0f;
        _decalObject.SetActive(false);
        _decalCoroutine = null;
    }

    void OnDestroy()
    {
        ClearAllEffects();
        transform.DOKill();
    }
}