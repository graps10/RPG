using System.Collections;
using Core.ObjectPool;
using Core.ObjectPool.Configs;
using Core.ObjectPool.Configs.FX;
using Managers;
using TMPro;
using UI_Elements;
using UnityEngine;
using PoolManager = Core.ObjectPool.PoolManager;

namespace Components.FX
{
    public class EntityFX : MonoBehaviour
    {
        private const float Hit_Return_To_Pool_Delay = 0.5f;
        private const float Effect_Repeat_Interval = 0.3f;
        
        private const float Text_Spawn_Cooldown = 0.1f;
        
        private static Vector2 popupXPositionRange = new(-1f, 1f);
        private static Vector2 popupYPositionRange = new(1.5f, 3f);
        
        private static Vector2 hitZRotationRange = new(-90f, 90);
        private static Vector2 hitXPositionRange = new(-0.5f, 0.5f);
        private static Vector2 hitYPositionRange = new(-0.5f, 0.5f);
        
        private static Vector2 critHitZRotationRange = new(-45, 45);
        private static float critHitYRotation = 180f;
        
        private static readonly Color transparentColor = Color.clear;
        private static readonly Color visibleColor = Color.white;
        private static readonly Color blinkColor = Color.red;
        
        [Header("Popup Text FX")]
        [SerializeField] private PopupTextPoolConfig textConfig;
        private float _lastTextSpawnTime;
        
        [Header("Hits FX")]
        [SerializeField] private HitFXPoolConfig hitFXConfig;
        [SerializeField] private CritHitFXPoolConfig criticalHitFXConfig;

        [Header("Flash FX")]
        [SerializeField] private float flashDuration;
        [SerializeField] private Material hitMat;

        [Header("Ailment Colors")]
        [SerializeField] private Color[] igniteColor;
        [SerializeField] private Color[] chillColor;
        [SerializeField] private Color[] shockColor;

        [Header("Ailment Particles")]
        [SerializeField] private ParticleSystem igniteFx;
        [SerializeField] private ParticleSystem chillFx;
        [SerializeField] private ParticleSystem shockFx;
    
        protected Player.Player player;
        protected SpriteRenderer sr;

        private Material _originalMat;
        private GameObject _myHealthBar;

        protected virtual void Start()
        {
            sr = GetComponentInChildren<SpriteRenderer>();
            player = PlayerManager.Instance.PlayerGameObject;

            _originalMat = sr.material;

            _myHealthBar = GetComponentInChildren<HealthBar>().gameObject;
        }

        public void CreatePopUpText(string text)
        {
            if (Time.time < _lastTextSpawnTime + Text_Spawn_Cooldown) return;
            _lastTextSpawnTime = Time.time;
            
            float randomX = Random.Range(popupXPositionRange.x, popupXPositionRange.y);
            float randomY = Random.Range(popupYPositionRange.x, popupYPositionRange.y);

            Vector3 spawnPosition = transform.position + new Vector3(randomX, randomY, 0);
            GameObject newTextObj = PoolManager.Instance.Spawn(textConfig.Prefab, spawnPosition, Quaternion.identity);
            
            if (newTextObj.TryGetComponent(out PopUpTextFX popUpText)) 
                popUpText.Setup(text, textConfig);
        }

        public void MakeTransparent(bool transparent)
        {
            if (transparent)
            {
                _myHealthBar.SetActive(false);
                sr.color = transparentColor;
            }
            else
            {
                _myHealthBar.SetActive(true);
                sr.color = visibleColor;
            }
        }

        public void CreateHitFX(Transform target, bool critical)
        {
            float zRotation = Random.Range(hitZRotationRange.x, hitZRotationRange.y);
            float xPosition = Random.Range(hitXPositionRange.x, hitXPositionRange.y);
            float yPosition = Random.Range(hitYPositionRange.x, hitYPositionRange.y);

            Vector3 hitFXRotation = new Vector3(0, 0, zRotation);
            
            GameObject prefabToSpawn = hitFXConfig.Prefab;

            if (critical)
            {
                prefabToSpawn = criticalHitFXConfig.Prefab;

                float yRotation = 0f;
                zRotation = Random.Range(critHitZRotationRange.x, critHitZRotationRange.y);

                if (GetComponent<Entity.Entity>().FacingDir == -1)
                    yRotation = critHitYRotation;

                hitFXRotation = new Vector3(0, yRotation, zRotation);
            }
            
            Vector3 spawnPosition = target.position + new Vector3(xPosition, yPosition, 0);
            Quaternion spawnRotation = Quaternion.Euler(hitFXRotation);
            
            GameObject newHitFX = PoolManager.Instance.Spawn(prefabToSpawn, spawnPosition, spawnRotation);
            PoolManager.Instance.ReturnWithDelay(newHitFX, Hit_Return_To_Pool_Delay);
        }

        public void IgniteFxFor(float seconds)
        {
            igniteFx.Play();

            InvokeRepeating(nameof(IgniteColorFx), 0, Effect_Repeat_Interval);
            InvokeCancelColorChange(seconds);
        }

        public void ChillFxFor(float seconds)
        {
            chillFx.Play();

            InvokeRepeating(nameof(ChillColorFx), 0, Effect_Repeat_Interval);
            InvokeCancelColorChange(seconds);
        }

        public void ShockFxFor(float seconds)
        {
            shockFx.Play();

            InvokeRepeating(nameof(ShockColorFx), 0, Effect_Repeat_Interval);
            InvokeCancelColorChange(seconds);
        }

        public void InvokeBlinkEffect(float repeatRate)
        {
            InvokeRepeating(nameof(RedColorBlink), 0, repeatRate);
        }
        
        private void RedColorBlink()
        {
            sr.color = sr.color != visibleColor ? visibleColor : blinkColor;
        }

        private IEnumerator FlashFX()
        {
            sr.material = hitMat;

            Color currentColor = sr.color;
            sr.color = visibleColor;

            yield return new WaitForSeconds(flashDuration);

            sr.color = currentColor;

            sr.material = _originalMat;
        }

        public void InvokeCancelColorChange(float seconds)
        {
            Invoke(nameof(CancelColorChange), seconds);
        }

        private void CancelColorChange()
        {
            CancelInvoke();
            sr.color = visibleColor;

            igniteFx.Stop();
            chillFx.Stop();
            shockFx.Stop();
        }

        private void IgniteColorFx()
        {
            sr.color = sr.color != igniteColor[0] ? igniteColor[0] : igniteColor[1];
        }

        private void ChillColorFx()
        {
            sr.color = sr.color != chillColor[0] ? chillColor[0] : chillColor[1];
        }

        private void ShockColorFx()
        {
            sr.color = sr.color != shockColor[0] ? shockColor[0] : shockColor[1];
        }
    }
}
