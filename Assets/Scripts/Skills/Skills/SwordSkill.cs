using Controllers.Skill_Controllers;
using Managers;
using Player.States;
using UI_Elements;
using UnityEngine;
using UnityEngine.UI;

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

        [Header("Bounce Info")]
        [SerializeField] private SkillTreeSlot bounceUnlockButton;
        [SerializeField] private int bounceAmount;
        [SerializeField] private float bounceGravity;
        [SerializeField] private float bounceSpeed;

        [Header("Pierce Info")]
        [SerializeField] private SkillTreeSlot pierceUnlockButton;
        [SerializeField] private int pierceAmount;
        [SerializeField] private float pierceGravity;

        [Header("Spin Info")]
        [SerializeField] private SkillTreeSlot spinUnlockButton;
        [SerializeField] private float hitCooldown = 0.35f;
        [SerializeField] private float maxTravelDistance = 7;
        [SerializeField] private float spinDuration = 2;
        [SerializeField] private float spinGravity = 1;
        
        [Header("Skill Info")]
        [SerializeField] private SkillTreeSlot swordUnlockButton;
        [SerializeField] private GameObject swordPrefab;
        [SerializeField] private Vector2 launchForce;
        [SerializeField] private float swordGravity;
        [SerializeField] private float freezeTimeDuration;
        [SerializeField] private float returnSpeed;

        [Header("Passive Skills")]
        [SerializeField] private SkillTreeSlot timeStopUnlockButton;
        [SerializeField] private SkillTreeSlot vulnerableUnlockButton;
        
        [Header("Aim Dots")]
        [SerializeField] private int numberOfDots;
        [SerializeField] private float spaceBetweenDots;
        [SerializeField] private GameObject dotPrefab;
        [SerializeField] private Transform dotsParent;

        private bool _swordUnlocked;
        private bool _timeStopUnlocked;
        private bool _vulnerableUnlocked;

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
            swordGravity = swordType switch
            {
                SwordType.Bounce => bounceGravity,
                SwordType.Pierce => pierceGravity,
                SwordType.Spin => spinGravity,
                _ => swordGravity
            };
        }

        protected override void Update()
        {
            bool isAiming = 
                PlayerManager.Instance.PlayerGameObject.StateMachine.CurrentState 
                == PlayerManager.Instance.PlayerGameObject.AimSword;

            if (isAiming && Input.GetKey(KeyCode.Mouse1))
            {
                DotsActive(true);
                
                for (int i = 0; i < _dots.Length; i++)
                    _dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
            }
            else
                DotsActive(false);
            
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                _finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, 
                    AimDirection().normalized.y * launchForce.y);
            }
        }

        public void CreateSword()
        {
            GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
            SwordSkillController newSwordScript = newSword.GetComponent<SwordSkillController>();

            switch (swordType)
            {
                case SwordType.Bounce:
                    newSwordScript.SetupBounce(true, bounceAmount, bounceSpeed);
                    break;
                case SwordType.Pierce:
                    newSwordScript.SetupPierce(pierceAmount);
                    break;
                case SwordType.Spin:
                    newSwordScript.SetupSpin(true, maxTravelDistance, spinDuration, hitCooldown);
                    break;
            }

            newSwordScript.SetupSword(_finalDir, swordGravity, player, freezeTimeDuration, returnSpeed);

            player.AssignNewSword(newSword);

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
            _dots = new GameObject[numberOfDots];
            for (int i = 0; i < numberOfDots; i++)
            {
                _dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
                _dots[i].SetActive(false);
            }
        }
        
        private Vector2 DotsPosition(float t)
        {
            Vector2 position = (Vector2)player.transform.position + new Vector2(
                AimDirection().normalized.x * launchForce.x,
                AimDirection().normalized.y * launchForce.y) * t + Physics2D.gravity * (Gravity_Scale * swordGravity * (t * t));

            return position;
        }

        #endregion
    }
}