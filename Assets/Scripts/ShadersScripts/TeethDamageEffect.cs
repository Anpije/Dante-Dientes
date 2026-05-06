using UnityEngine;
using System.Collections;

public class TeethDamageEffect : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform[] teethTransforms;  // GameObjects de cada diente
    [SerializeField] private Renderer[] teethRenderers;   // SkinnedMeshRenderers

    [Header("Color")]
    [SerializeField] private float flashDuration = 0.6f;

    [Header("Escala")]
    [SerializeField] private float scaleAmount = 0.15f;
    // Inspector: keyframes (0,0) (0.2,1) (0.8,1)  (1,0)
    [SerializeField]
    private AnimationCurve scaleCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private static readonly int FlashID =
        Shader.PropertyToID("_FlashIntensity");

    private MaterialPropertyBlock _mpb;
    private Vector3[] _baseScales;

    private void Awake()
    {
        _mpb = new MaterialPropertyBlock();
        _baseScales = new Vector3[teethTransforms.Length];
        for (int i = 0; i < teethTransforms.Length; i++)
            _baseScales[i] = teethTransforms[i].localScale;
    }

    public void TakeDamage()
    {
        StopAllCoroutines();
        ResetScale();
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            float t = elapsed / flashDuration;
            SetColor(1f - t);                        
            SetScale(scaleCurve.Evaluate(t));        
            elapsed += Time.deltaTime;
            yield return null;
        }
        SetColor(0f);
        ResetScale();
    }

    private void SetColor(float intensity)
    {
        _mpb.SetFloat(FlashID, intensity);
        foreach (var r in teethRenderers)
            r.SetPropertyBlock(_mpb);
    }

    private void SetScale(float t)
    {
        float factor = 1f + scaleAmount * t;
        for (int i = 0; i < teethTransforms.Length; i++)
            teethTransforms[i].localScale = _baseScales[i] * factor;
    }

    private void ResetScale()
    {
        for (int i = 0; i < teethTransforms.Length; i++)
            teethTransforms[i].localScale = _baseScales[i];
    }
}