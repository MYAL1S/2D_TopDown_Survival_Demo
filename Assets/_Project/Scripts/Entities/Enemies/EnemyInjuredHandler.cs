using System.Collections;
using UnityEngine;

[RequireComponent(typeof(InjuredEvent))]
[DisallowMultipleComponent]
public class EnemyInjuredHandler : MonoBehaviour
{
    [SerializeField]
    private Color flashColor = Color.white;

    [SerializeField]
    [Min(0.01f)]
    private float flashDuration = 0.08f;

    private InjuredEvent injuredEvent;
    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        injuredEvent = GetComponent<InjuredEvent>();
        CacheSpriteRenderers();
    }

    private void OnEnable()
    {
        injuredEvent.OnInjured += InjuredEvent_OnInjured;
    }

    private void OnDisable()
    {
        injuredEvent.OnInjured -= InjuredEvent_OnInjured;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        RestoreSpriteColors();
    }

    private void InjuredEvent_OnInjured(InjuredEvent eventSource, int damage)
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        SetSpriteColors(flashColor);
        yield return new WaitForSeconds(flashDuration);
        RestoreSpriteColors();
        flashCoroutine = null;
    }

    private void CacheSpriteRenderers()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        originalColors = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalColors[i] = spriteRenderers[i].color;
        }
    }

    private void SetSpriteColors(Color color)
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
            {
                spriteRenderers[i].color = color;
            }
        }
    }

    private void RestoreSpriteColors()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
            {
                spriteRenderers[i].color = originalColors[i];
            }
        }
    }
}
