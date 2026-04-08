using Core.ObjectPool;
using Core.ObjectPool.Configs.FX;
using TMPro;
using UnityEngine;

namespace Components.FX
{
    public class PopUpTextFX : PooledObject
    {
        private const float Alpha_Disappearance_Threshold = 0.5f;
        
        private PopupTextPoolConfig _config;
        private TextMeshPro _myText;
    
        private float _textTimer;
        private float _currentSpeed;
        private float _defaultSpeed;

        private void Awake() => _myText = GetComponent<TextMeshPro>();

        public void Setup(string text, PopupTextPoolConfig newConfig)
        {
            _config = newConfig;
            _myText.text = text;
            
            _defaultSpeed = _config.Speed;
            _currentSpeed = _defaultSpeed;
            _textTimer = _config.LifeTime;
            
            _myText.color = new Color(_myText.color.r, _myText.color.g, _myText.color.b, 1f);
        }
    
        public override void ReturnToPool()
        {
            _currentSpeed = _defaultSpeed;
            _myText.color = new Color(_myText.color.r, _myText.color.g, _myText.color.b, 1);
            
            transform.position = Vector2.zero;
            base.ReturnToPool();
        }

        private void Update()
        {
            transform.position = Vector2.MoveTowards(transform.position, 
                new Vector2(transform.position.x, transform.position.y + 1), _currentSpeed * Time.deltaTime);
        
            _textTimer -= Time.deltaTime;

            if (_textTimer < 0)
            {
                float alpha = _myText.color.a - _config.ColorDisappearanceSpeed * Time.deltaTime;

                _myText.color = new Color(_myText.color.r, _myText.color.g, _myText.color.b, alpha);

                if (_myText.color.a < Alpha_Disappearance_Threshold)
                    _currentSpeed = _config.DisappearanceSpeed;

                if (_myText.color.a <= 0)
                    ReturnToPool();
            }
        }
    }
}
