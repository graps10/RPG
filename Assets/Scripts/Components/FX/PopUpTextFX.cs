using Core.ObjectPool;
using Managers;
using TMPro;
using UnityEngine;

namespace Components.FX
{
    public class PopUpTextFX : MonoBehaviour, ISpawnedPooledObject
    {
        private const float Alpha_Disappearance_Threshold = 0.5f;
        
        [SerializeField] private float speed;
        [SerializeField] private float disappearanceSpeed;
        [SerializeField] private float colorDisappearanceSpeed;
        [SerializeField] private float lifeTime;
    
        private TextMeshPro _myText;
    
        private float _textTimer;
        private float _defaultSpeed;

        private void Awake() => _myText = GetComponent<TextMeshPro>();

        public void OnSpawn()
        {
            _defaultSpeed = speed;
            _textTimer = lifeTime;
        }
    
        public void OnReturnToPool()
        {
            transform.position = Vector2.zero;
            speed = _defaultSpeed;
            _myText.color = new Color(_myText.color.r, _myText.color.g, _myText.color.b, 1);
        }

        private void Update()
        {
            transform.position = Vector2.MoveTowards(transform.position, 
                new Vector2(transform.position.x, transform.position.y + 1), speed * Time.deltaTime);
        
            _textTimer -= Time.deltaTime;

            if (_textTimer < 0)
            {
                float alpha = _myText.color.a - colorDisappearanceSpeed * Time.deltaTime;

                _myText.color = new Color(_myText.color.r, _myText.color.g, _myText.color.b, alpha);

                if (_myText.color.a < Alpha_Disappearance_Threshold)
                    speed = disappearanceSpeed;

                if (_myText.color.a <= 0)
                    PoolManager.Instance.Return("text", gameObject);
            }
        }
    }
}
