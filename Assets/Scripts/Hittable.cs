using System.Collections;
using UnityEngine;

/// <summary>
/// Put this monobehaviour on an object to make it respond to attacks.
/// Also comes with some neat effects.
/// Flash effect is set up with 2D in mind, but can be made to work for 3D with some modifications.
/// </summary>
public class Hittable : MonoBehaviour
{
    [System.Flags]
    public enum HitType
    {
        None = 0,
        Color = 1 << 0,
        Shake = 1 << 1,
    }

    /// <summary>
    /// Subscribe to this event to get notified of when this hittable is attacked.
    /// </summary>
    public event System.Action<int> OnHit;

    /// <summary>
    /// Subscribe to this event to get notified of when this hittable dies.
    /// </summary>
    public event System.Action OnDeath;

    [SerializeField] private HitType hitType = HitType.None;
    [SerializeField] private bool disableHitEffect;
    [SerializeField] private ParticleSystem customHitEffect;
    [SerializeField] private AudioClip customHitSound;

    private Vector2 spriteOffset;
    protected Color defaultColor = Color.white;
    private Material defaultMaterial;
    private Material flashMaterial;

    private SpriteRenderer sprite;

    private Coroutine damageShakeRoutine;
    private Coroutine hitFlashRoutine;

    private float gameTime = 0;
    [SerializeField] private bool destroyOnDeath = false;

    [SerializeField] private bool isInvincible;
    public bool IsInvincible { get => isInvincible; set => isInvincible = value; }

    [SerializeField] private int health = 1;

    [SerializeField, ReadOnly] private int currentHealth;
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }

    #region Unity lifetime methods

    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();

        spriteOffset = sprite.transform.localPosition;
        defaultMaterial = sprite.material;
        flashMaterial = new Material(Shader.Find("Shader Forge/SpriteFlash"));
        CurrentHealth = health;
    }

    private void Update()
    {
        gameTime += Time.deltaTime;
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Deal damage to this hittable.
    /// Causes effects. To deal damage without triggering effects or events, see DealDamage(int)
    /// </summary>
    /// <param name="damage"></param>
    public void Hit(int damage)
    {
        if (currentHealth <= 0)
            return;

        DealDamage(damage);

        OnHit?.Invoke(damage);

        if (hitType.HasFlag(HitType.Color))
        {
            if (hitFlashRoutine != null)
                StopCoroutine(hitFlashRoutine);
            hitFlashRoutine = StartCoroutine(HitFlashFade(0.4f));
        }

        if (hitType.HasFlag(HitType.Shake))
        {
            if (damageShakeRoutine != null)
                StopCoroutine(damageShakeRoutine);
            damageShakeRoutine = StartCoroutine(DamageShake(0.4f));
        }
    }

    /// <summary>
    /// Deal damage quietly to this hittable, without causing events or effecs to trigger.
    /// Useful for damage over time.
    /// </summary>
    /// <param name="damage"></param>
    public void DealDamage(int damage)
    {
        if (isInvincible || currentHealth <= 0)
            return;

        damage = Mathf.Clamp(damage, 0, currentHealth);
        currentHealth -= damage;

        if (currentHealth <= 0)
            Kill();
    }

    /// <summary>
    /// Instantly kill this hittable.
    /// </summary>
    public void Kill()
    {
        currentHealth = 0;

        OnDeath?.Invoke();
        if (destroyOnDeath)
            Destroy(gameObject);
    }

    /// <summary>
    /// Bring this hittable back to life.
    /// </summary>
    public void Revive()
    {
        currentHealth = health;
    }

    #endregion

    #region Private methods

    private IEnumerator DamageShake(float time)
    {
        float t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime / time;
            float at = 1 - t;
            float shakeAmt = 0.5f * (1 - at * at);
            Vector2 shakeOffset = new(
                (Mathf.PerlinNoise(gameTime * 40.0f, 0) - 0.5f) * 2.0f * shakeAmt,
                (Mathf.PerlinNoise(0, gameTime * 40.0f) - 0.5f) * 2.0f * shakeAmt);
            sprite.transform.localPosition = spriteOffset + shakeOffset;
            yield return null;
        }

        sprite.transform.localPosition = spriteOffset;
    }

    private IEnumerator HitFlashFade(float time)
    {
        sprite.material = flashMaterial;
        flashMaterial.SetFloat("_FlashAmt", 1);

        float t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime / time;
            float at = 1 - t;
            float alpha = 1 - at * at * at * at;

            flashMaterial.SetFloat("_FlashAmt", alpha);

            yield return null;
        }

        sprite.material = defaultMaterial;
    }

    #endregion
}
