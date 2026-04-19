using UnityEngine;

namespace Core.ObjectPool.Configs.Controllers
{
    [CreateAssetMenu(fileName = "SwordPoolConfig", menuName = "ObjectPool/Configs/Controllers/Sword")]
    public class SwordPoolConfig : BasePoolConfig
    {
        [Header("Sword Prefabs")]
        [SerializeField] private GameObject bouncePrefab;
        [SerializeField] private GameObject piercePrefab;
        [SerializeField] private GameObject spinPrefab;

        [Header("Common Info")]
        [SerializeField] private float freezeTimeDuration = 1f;
        [SerializeField] private float returnSpeed = 12f;
        [SerializeField] private Vector2 launchForce;
        
        [Header("Regular Sword")]
        [SerializeField] private float regularGravity = 0.5f;

        [Header("Bounce Info")]
        [SerializeField] private int bounceAmount = 4;
        [SerializeField] private float bounceGravity = 0.5f;
        [SerializeField] private float bounceSpeed = 20f;

        [Header("Pierce Info")]
        [SerializeField] private int pierceAmount = 2;
        [SerializeField] private float pierceGravity = 0.5f;
        [SerializeField] private float maxPierceDistance = 12f;

        [Header("Spin Info")]
        [SerializeField] private float hitCooldown = 0.35f;
        [SerializeField] private float maxTravelDistance = 7f;
        [SerializeField] private float spinDuration = 2f;
        [SerializeField] private float spinGravity = 1f;
        
        public GameObject BouncePrefab => bouncePrefab;
        public GameObject PiercePrefab => piercePrefab;
        public GameObject SpinPrefab => spinPrefab;
        
        public float FreezeTimeDuration => freezeTimeDuration;
        public float ReturnSpeed => returnSpeed;
        public Vector2 LaunchForce => launchForce;
        
        public float RegularGravity => regularGravity;

        public int BounceAmount => bounceAmount;
        public float BounceGravity => bounceGravity;
        public float BounceSpeed => bounceSpeed;

        public int PierceAmount => pierceAmount;
        public float PierceGravity => pierceGravity;
        public float MaxPierceDistance => maxPierceDistance;

        public float HitCooldown => hitCooldown;
        public float MaxTravelDistance => maxTravelDistance;
        public float SpinDuration => spinDuration;
        public float SpinGravity => spinGravity;
    }
}