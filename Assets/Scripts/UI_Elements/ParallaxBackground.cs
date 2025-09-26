using UnityEngine;

namespace UI_Elements
{
    public class ParallaxBackground : MonoBehaviour
    {
        [SerializeField] private GameObject cam;
        [SerializeField] private SpriteRenderer length;
        [SerializeField] private float parallaxEffect;

        private float _xPos;
        private float _bounds;
   
        private void Start()
        {
            _xPos = transform.position.x;
            _bounds = length.bounds.size.x;
        }

        private void Update()
        {
            float distanceMoved = cam.transform.position.x * (1 - parallaxEffect);

            float distanceToMove = cam.transform.position.x * parallaxEffect;
            transform.position = new Vector3(_xPos + distanceToMove, transform.position.y);

            if (distanceMoved > _xPos + _bounds)
                _xPos += _bounds;
            else if (distanceMoved < _xPos - _bounds)
                _xPos -= _bounds;
        }
    }
}
