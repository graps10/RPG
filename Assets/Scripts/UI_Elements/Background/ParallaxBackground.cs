using UnityEngine;

namespace UI_Elements.Background
{
    public class ParallaxBackground : MonoBehaviour
    {
        [SerializeField] private GameObject cam;
        [SerializeField] private SpriteRenderer length;
        [Header("Parallax Settings")]
        [SerializeField] private float parallaxEffect; 
        [SerializeField] private float parallaxEffectY;

        private float _xPos;
        private float _yPos;
        private float _bounds;
   
        private void Start()
        {
            _xPos = transform.position.x;
            _yPos = transform.position.y;
            _bounds = length.bounds.size.x;
        }
        
        private void LateUpdate()
        {
            float distanceMovedX = cam.transform.position.x * (1 - parallaxEffect);
            float distanceToMoveX = cam.transform.position.x * parallaxEffect;
            
            float distanceToMoveY = cam.transform.position.y * parallaxEffectY;
            
            transform.position = new Vector3(_xPos + distanceToMoveX, _yPos + distanceToMoveY, transform.position.z);
            
            if (distanceMovedX > _xPos + _bounds)
                _xPos += _bounds;
            else if (distanceMovedX < _xPos - _bounds)
                _xPos -= _bounds;
        }
    }
}
