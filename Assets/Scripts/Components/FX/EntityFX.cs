using System.Collections;
using Core.ObjectPool;
using Managers;
using TMPro;
using UI_Elements;
using UnityEngine;

namespace Components.FX
{
    public class EntityFX : MonoBehaviour
    {
        private const float Hit_Return_To_Pool_Delay = 0.5f;
        private const float Effect_Repeat_Interval = 0.3f;
        
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
        
        [Header("Hits FX")]
        [SerializeField] private GameObject hitFXPrefab;
        [SerializeField] private GameObject criticalHitFXPrefab;

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
            float randomX = Random.Range(popupXPositionRange.x, popupXPositionRange.y);
            float randomY = Random.Range(popupYPositionRange.x, popupYPositionRange.y);

            Vector3 positionOffset = new Vector3(randomX, randomY, 0);
            GameObject newText = PoolManager.Instance.Spawn(PoolNames.POPUP_TEXT, 
                transform.position + positionOffset, Quaternion.identity);

            if (newText)
                newText.GetComponent<TextMeshPro>().text = text;
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

            GameObject hitFX = hitFXPrefab;

            if (critical)
            {
                hitFX = criticalHitFXPrefab;

                float yRotation = 0f;
                zRotation = Random.Range(critHitZRotationRange.x, critHitZRotationRange.y);

                if (GetComponent<Entity.Entity>().FacingDir == -1)
                    yRotation = critHitYRotation;

                hitFXRotation = new Vector3(0, yRotation, zRotation);
            }
            
            GameObject newHitFX = PoolManager.Instance.Spawn(PoolNames.HIT_FX, 
                target.position + new Vector3(xPosition, yPosition), Quaternion.identity, hitFX);

            newHitFX.transform.Rotate(hitFXRotation);

            PoolManager.Instance.Return(PoolNames.HIT_FX, newHitFX, Hit_Return_To_Pool_Delay);
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
