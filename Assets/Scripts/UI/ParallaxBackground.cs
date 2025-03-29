using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
   [SerializeField] private GameObject cam;
   [SerializeField] private SpriteRenderer length;
   [SerializeField] private float parallaxEffect;

   private float xPos;
   private float bounds;
   
    void Start()
    {
        xPos = transform.position.x;
        bounds = length.bounds.size.x;
    }
    void Update()
    {
        float distanceMoved = cam.transform.position.x * (1-parallaxEffect);

        float distanceToMove = cam.transform.position.x * parallaxEffect;
        transform.position = new Vector3(xPos + distanceToMove, transform.position.y);

        if(distanceMoved > xPos + bounds)
            xPos += bounds; 
        else if (distanceMoved < xPos - bounds)
            xPos -= bounds;
    }
}
