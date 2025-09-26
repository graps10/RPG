using Cinemachine;
using UnityEngine;

namespace Components.FX
{
    public class PlayerFX : EntityFX
    {
        [Header("Screen Shake FX")]
        [SerializeField] private float shakeMultiplier;
        [SerializeField] private Vector3 shakeSwordImpact;
        [SerializeField] private Vector3 shakeHighDamage;
        
        [Header("After Image FX")]
        [SerializeField] private GameObject afterImagePrefab;
        [SerializeField] private float colorLoseRate;
        [SerializeField] private float afterImageCooldown;
        private float _afterImageCooldownTimer;

        [Space]
        [SerializeField] private ParticleSystem dustFX;
        
        private CinemachineImpulseSource _screenShake;

        protected override void Start()
        {
            base.Start();
            _screenShake = GetComponent<CinemachineImpulseSource>();
        }
        
        private void Update()
        {
            _afterImageCooldownTimer -= Time.deltaTime;
        }

        public void ScreenShake(Vector3 shakePower)
        {
            _screenShake.m_DefaultVelocity = new Vector3(shakePower.x * player.FacingDir, shakePower.y) * shakeMultiplier;
            _screenShake.GenerateImpulse();
        }
        
        public Vector3 GetShakeSwordImpact() => shakeSwordImpact;
        public Vector3 GetShakeHighDamage() => shakeHighDamage;

        public void CreateAfterImage()
        {
            if (_afterImageCooldownTimer < 0)
            {
                _afterImageCooldownTimer = afterImageCooldown;
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
}
