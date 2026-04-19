using Core.Factories;
using Core.ObjectPool;
using Core.ObjectPool.Configs.Controllers;
using Core.ObjectPool.Configs.FX;
using Managers;
using UI_Elements;
using UnityEngine;

namespace Skills.Skills
{
    public enum SwordType
    {
        Regular,
        Bounce,
        Pierce,
        Spin
    }
    public class SwordSkill : Skill
    {
        private const float Gravity_Scale = 0.5f;
        
        [SerializeField] private SwordType swordType = SwordType.Regular;

        [Header("Skill Info")] [SerializeField] private SwordPoolConfig swordConfig;
        
        [Header("Bounce Info")] [SerializeField] private SkillTreeSlot bounceUnlockButton;
        [Header("Pierce Info")] [SerializeField] private SkillTreeSlot pierceUnlockButton;
        [Header("Spin Info")] [SerializeField] private SkillTreeSlot spinUnlockButton;
        [Header("Skill Info")] [SerializeField] private SkillTreeSlot swordUnlockButton;
        
        [Header("Passive Skills")]
        [SerializeField] private SkillTreeSlot timeStopUnlockButton;
        [SerializeField] private SkillTreeSlot vulnerableUnlockButton;

        [Header("Aim Dots")] [SerializeField] private DotPoolConfig dotsConfig;

        private bool _swordUnlocked;
        private bool _timeStopUnlocked;
        private bool _vulnerableUnlocked;
        
        private float _swordGravity;

        private Vector2 _finalDir;
        private GameObject[] _dots;

        private void OnEnable()
        {
            swordUnlockButton.OnUnlocked += UnlockRegular;
            pierceUnlockButton.OnUnlocked += UnlockPierce;
            bounceUnlockButton.OnUnlocked += UnlockBounce;;
            spinUnlockButton.OnUnlocked += UnlockSpin;
            
            timeStopUnlockButton.OnUnlocked += UnlockTimeStop;
            vulnerableUnlockButton.OnUnlocked += UnlockVulnerable;
        }

        private void OnDisable()
        {
            if(swordUnlockButton != null)
                swordUnlockButton.OnUnlocked -= UnlockRegular;
            
            if(pierceUnlockButton != null)
                pierceUnlockButton.OnUnlocked -= UnlockPierce;
            
            if(bounceUnlockButton != null)
                bounceUnlockButton.OnUnlocked -= UnlockBounce;;
            
            if(spinUnlockButton != null)
                spinUnlockButton.OnUnlocked -= UnlockSpin;
            
            if(timeStopUnlockButton != null)
                timeStopUnlockButton.OnUnlocked -= UnlockTimeStop;
            
            if(vulnerableUnlockButton != null)
                vulnerableUnlockButton.OnUnlocked -= UnlockVulnerable;
        }
        
        protected override void Start()
        {
            base.Start();

            GenerateDots();
            SetupGravity();
        }

        private void SetupGravity()
        {
            _swordGravity = swordType switch
            {
                SwordType.Bounce => swordConfig.BounceGravity,
                SwordType.Pierce => swordConfig.PierceGravity,
                SwordType.Spin => swordConfig.SpinGravity,
                _ => swordConfig.RegularGravity
            };
        }

        protected override void Update()
        {
            bool isAiming = 
                PlayerManager.Instance.PlayerGameObject.StateMachine.CurrentState 
                == PlayerManager.Instance.PlayerGameObject.AimSword;

            if (isAiming && Input.GetKey(KeyCode.Mouse1))
            {
                for (int i = 0; i < _dots.Length; i++)
                {
                    Vector2 dotPos = DotsPosition(i * dotsConfig.SpaceBetweenDots);
                    
                    float distanceToDot = Vector2.Distance(player.transform.position, dotPos);
                    bool isTooFar = false;
                    
                    if (swordType == SwordType.Spin && distanceToDot > swordConfig.MaxTravelDistance)
                        isTooFar = true;
                    else if (swordType == SwordType.Pierce && distanceToDot > swordConfig.MaxPierceDistance)
                        isTooFar = true;
                    
                    if (isTooFar)
                    {
                        _dots[i].SetActive(false);
                    }
                    else
                    {
                        _dots[i].SetActive(true);
                        _dots[i].transform.position = dotPos;
                    }
                }
            }
            else
            {
                DotsActive(false);
            }
            
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                _finalDir = new Vector2(AimDirection().normalized.x * swordConfig.LaunchForce.x, 
                    AimDirection().normalized.y * swordConfig.LaunchForce.y);
            }
        }

        public void CreateSword()
        {
            var swordController = SwordFactory.CreateSword(
                swordType, 
                player.transform.position, 
                transform.rotation, 
                _finalDir, 
                player, 
                swordConfig
            );
            
            player.AssignNewSword(swordController.gameObject);
            DotsActive(false);
        }

        protected override void CheckUnlock()
        {
            UnlockRegular();
            UnlockPierce();
            UnlockBounce();
            UnlockSpin();
            
            UnlockTimeStop();
            UnlockVulnerable();
        }

        #region Unlock

        public bool IsSwordUnlocked() => _swordUnlocked;
        public bool IsTimeStopUnlocked() => _timeStopUnlocked;
        public bool IsVulnerableUnlocked() => _vulnerableUnlocked;
        
        private void UnlockRegular() 
            => TryUnlock(swordUnlockButton, ref _swordUnlocked, 
                () => swordType = SwordType.Regular);
        
        private void UnlockPierce() 
            => TryUnlock(pierceUnlockButton, ref _swordUnlocked, 
                () => swordType = SwordType.Pierce);
        
        private void UnlockBounce() 
            => TryUnlock(bounceUnlockButton, ref _swordUnlocked, 
                () => swordType = SwordType.Bounce);
        
        private void UnlockSpin() 
            => TryUnlock(spinUnlockButton, ref _swordUnlocked, 
                () => swordType = SwordType.Spin);
        
        private void UnlockTimeStop() 
            => TryUnlock(timeStopUnlockButton, ref _timeStopUnlocked);
        
        private void UnlockVulnerable() 
            => TryUnlock(vulnerableUnlockButton, ref _vulnerableUnlocked);

        #endregion
        
        #region Aim Region

        private Vector2 AimDirection()
        {
            Vector2 playerPosition = player.transform.position;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePosition - playerPosition;

            return direction;
        }

        public void DotsActive(bool isActive)
        {
            for (int i = 0; i < _dots.Length; i++)
                _dots[i].SetActive(isActive);
        }
        
        private void GenerateDots()
        {
            _dots = new GameObject[dotsConfig.NumberOfDots];
            for (int i = 0; i < dotsConfig.NumberOfDots; i++)
            {
                _dots[i] = PoolManager.Instance.Spawn(dotsConfig.Prefab, player.transform.position, Quaternion.identity);
                _dots[i].SetActive(false);
            }
        }
        
        private Vector2 DotsPosition(float t)
        {
            Vector2 aimDirection = AimDirection().normalized;
            Vector2 launchVelocity = new Vector2(
                aimDirection.x * swordConfig.LaunchForce.x,
                aimDirection.y * swordConfig.LaunchForce.y);
            
            if (swordType == SwordType.Pierce || swordType == SwordType.Spin)
            {
                return (Vector2)player.transform.position + launchVelocity * t;
            }
            
            Vector2 position = (Vector2)player.transform.position 
                               + launchVelocity * t 
                               + Physics2D.gravity * (Gravity_Scale * _swordGravity * (t * t));

            return position;
        }

        #endregion
    }
}