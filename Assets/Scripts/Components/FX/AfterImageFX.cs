using Core.ObjectPool;
using UnityEngine;

namespace Components.FX
{
    public class AfterImageFX : PooledObject
    {
        private SpriteRenderer _sr;
        private float _colorLoseRate;

        private void Update()
        {
            float alpha = _sr.color.a - _colorLoseRate * Time.deltaTime;
            _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, alpha);

            if (_sr.color.a <= 0)
                ReturnToPool();
        }

        public void SetupAfterImage(float losingSpeed, Sprite spriteImage)
        {
            if(_sr == null)
                _sr = GetComponent<SpriteRenderer>();

            _sr.sprite = spriteImage;
            _colorLoseRate = losingSpeed;
            _sr.color = new Color(1f, 1f, 1f, 1f);
        }
        
        public override void ReturnToPool()
        {
            if (_sr != null)
                _sr.sprite = null;
                
            base.ReturnToPool();
        }
    }
}
