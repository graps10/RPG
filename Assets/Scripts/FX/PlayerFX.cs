using Cinemachine;
using UnityEngine;

public class PlayerFX : EntityFX
{
    [Header("Screen Shake FX")]
    [SerializeField] private float shakeMultiplier;
    public Vector3 shakeSwordImpact;
    public Vector3 shakeHighDamage;
    private CinemachineImpulseSource screenShake;

    [Header("After Image FX")]
    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private float colorLoseRate;
    [SerializeField] private float afterImageCooldown;
    private float afterImageCooldownTimer;

    [Space]
    [SerializeField] private ParticleSystem dustFX;

    protected override void Start()
    {
        base.Start();
        screenShake = GetComponent<CinemachineImpulseSource>();
    }
    void Update()
    {
        afterImageCooldownTimer -= Time.deltaTime;
    }

    public void ScreenShake(Vector3 _shakePower)
    {
        screenShake.m_DefaultVelocity = new Vector3(_shakePower.x * player.facingDir, _shakePower.y) * shakeMultiplier;
        screenShake.GenerateImpulse();
    }

    public void CreateAfterImage()
    {
        if (afterImageCooldownTimer < 0)
        {
            afterImageCooldownTimer = afterImageCooldown;
            GameObject newAfterImage = Instantiate(afterImagePrefab, transform.position, transform.rotation);
            newAfterImage.GetComponent<AfterImageFX>().SetupAfterImage(colorLoseRate, sr.sprite);
        }
    }

    public void PlayDustFX()
    {
        if (dustFX != null)
            dustFX.Play();
    }
}
