using System.Collections.Generic;
using Enemies.Base;
using Managers;
using Skills;
using UnityEngine;

namespace Controllers.Skill_Controllers
{
    public class BlackHoleSkillController : MonoBehaviour
    {
        private const float Clone_X_Spawn_Offset = -2f;
        private const int Clone_X_Spawn_Chance_Threshold = 50;
        private const float Finish_Black_Hole_Delay = 1f;
        
        private static readonly Vector2 shrinkTargetScale = new(-1f, -1f);
        private static readonly Vector3 Hotkey_Spawn_Offset = new(0f, 3f, 0f);
    
        [SerializeField] private GameObject hotKeyPrefab;
        [SerializeField] private List<KeyCode> keyCodeList;

        public bool PlayerCanExitState { get; private set; }

        private float _maxSize;
        private float _growSpeed;
        private float _shrinkSpeed;
        private float _blackHoleTimer;

        private bool _canGrow = true;
        private bool _canShrink;
        private bool _canCreateHotKeys = true;
        private bool _cloneAttackReleased;
        private bool _playerCanDisapear = true;
    
        private int _amountOfAttacks;
        private float _cloneAttackCooldown;
        private float _cloneAttackTimer;

        private List<Transform> _targets = new();
        private List<GameObject> _createdHotKeys = new();
    
        public void SetupBlackHole(float maxSize, float growSpeed, float shrinkSpeed, 
            int amountOfAttacks, float cloneAttackCooldown, float blackHoleDuration)
        {
            _maxSize = maxSize;
            _growSpeed = growSpeed;
            _shrinkSpeed = shrinkSpeed;
            _amountOfAttacks = amountOfAttacks;
            _cloneAttackCooldown = cloneAttackCooldown;
        
            _blackHoleTimer = blackHoleDuration;

            if(SkillManager.Instance.Clone.IsCrystalInsteadOfClone())
                _playerCanDisapear = false;
        }

        private void Update()
        {
            _cloneAttackTimer -= Time.deltaTime;
            _blackHoleTimer -= Time.deltaTime;

            if(_blackHoleTimer < 0)
            {
                _blackHoleTimer = Mathf.Infinity;

                if(_targets.Count > 0)
                    ReleaseCloneAttack();
                else
                    FinishBlackHoleAbility();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                ReleaseCloneAttack();
            }

            CloneAttackLogic();

            if (_canGrow && !_canShrink)
            {
                transform.localScale = Vector2.Lerp(transform.localScale, 
                    new Vector2(_maxSize, _maxSize), _growSpeed * Time.deltaTime);
            }

            if (_canShrink)
            {
                transform.localScale = Vector2.Lerp(transform.localScale, 
                    shrinkTargetScale, _shrinkSpeed * Time.deltaTime);
            
                if (transform.localScale.x < 0)
                    Destroy(gameObject);
            }
        }

        private void ReleaseCloneAttack()
        {
            if(_targets.Count <= 0) return;

            DestroyHotKeys();
            _cloneAttackReleased = true;
            _canCreateHotKeys = false;

            if(_playerCanDisapear)
            {
                _playerCanDisapear = false;
                PlayerManager.Instance.PlayerGameObject.Fx.MakeTransparent(true);
            }
        }

        private void CloneAttackLogic()
        {
            if (_cloneAttackTimer < 0 && _cloneAttackReleased && _amountOfAttacks > 0)
            {
                _cloneAttackTimer = _cloneAttackCooldown;
                int randomIndex = Random.Range(0, _targets.Count);

                float xOffset;
                if (Random.Range(0, 100) > Clone_X_Spawn_Chance_Threshold)
                    xOffset = Clone_X_Spawn_Offset;
                else
                    xOffset = -Clone_X_Spawn_Offset;

                if(SkillManager.Instance.Clone.IsCrystalInsteadOfClone())
                {
                    SkillManager.Instance.Crystal.CreateCrystal();
                    SkillManager.Instance.Crystal.CurrentCrystalChooseRandomTarget();
                }
                else
                    SkillManager.Instance.Clone.CreateClone(_targets[randomIndex], new Vector3(xOffset, 0));

                _amountOfAttacks--;

                if (_amountOfAttacks <= 0)
                    Invoke(nameof(FinishBlackHoleAbility), Finish_Black_Hole_Delay);
            }
        }

        private void FinishBlackHoleAbility()
        {
            DestroyHotKeys();
            PlayerCanExitState = true;

            _canShrink = true;
            _cloneAttackReleased = false;
        }

        public void AddEnemyToList(Transform _enemyTransform) => _targets.Add(_enemyTransform);

        private void DestroyHotKeys()
        {
            if(_createdHotKeys.Count < 0) return;

            foreach (var hotKey in _createdHotKeys)
                Destroy(hotKey);
        }

        private void OnTriggerEnter2D(Collider2D collision) 
        {
            if(collision.GetComponent<Enemy>() != null)
            {
                collision.GetComponent<Enemy>().FreezeTime(true);
                CreateHotKey(collision);
            }
        }

        private void OnTriggerExit2D(Collider2D collision) 
        {
            if(collision.GetComponent<Enemy>() != null)
                collision.GetComponent<Enemy>().FreezeTime(false);
        }

        private void CreateHotKey(Collider2D collision)
        {
            if(keyCodeList.Count <= 0) 
            {
                Debug.Log("Not enough how keys in a key code list");
                return;
            }

            if(!_canCreateHotKeys) return;

            GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + 
                                                             Hotkey_Spawn_Offset, Quaternion.identity);
            _createdHotKeys.Add(newHotKey);

            KeyCode chooseKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
            keyCodeList.Remove(chooseKey);

            BlackHoleHotKeyController newHotKeyScript = newHotKey.GetComponent<BlackHoleHotKeyController>();
        
            newHotKeyScript.SetupHotKey(chooseKey, collision.transform, this);
        }
    }
}
