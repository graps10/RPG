using Cinemachine;
using Core.ObjectPool;
using Core.ObjectPool.Configs.FX;
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
        [SerializeField] private AfterImagePoolConfig afterImageConfig;
        [SerializeField] private ParticleSystem dustFX;
        
        private CinemachineImpulseSource _screenShake;
        private float _afterImageCooldownTimer;

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
                _afterImageCooldownTimer = afterImageConfig.AfterImageCooldown;
        
                GameObject newAfterImage = PoolManager.Instance.Spawn(
                    afterImageConfig.Prefab, 
                    transform.position, 
                    transform.rotation
                );
                
                if (newAfterImage.TryGetComponent(out AfterImageFX afterImageScript))
                    afterImageScript.SetupAfterImage(afterImageConfig.ColorLoseRate, sr.sprite);
            }
        }

        public void PlayDustFX()
        {
            if (dustFX != null)
                dustFX.Play();
        }
    }
}
