using System.Collections.Generic;
using Core.ObjectPool;
using Core.ObjectPool.Configs.Controllers;
using Enemies.Base;
using Managers;
using Skills;
using UnityEngine;

namespace Controllers.Skill_Controllers
{
    public class BlackHoleSkillController : PooledObject
    {
        public bool PlayerCanExitState { get; private set; }
        
        private BlackHolePoolConfig _config;
        private float _blackHoleTimer;

        private bool _canGrow = true;
        private bool _canShrink;
        
        private bool _canCreateHotKeys = true;
        
        private bool _cloneAttackReleased;
        private bool _playerCanDisappear = true;
    
        private int _amountOfAttacks;
        private float _cloneAttackCooldown;
        private float _cloneAttackTimer;

        private List<Transform> _targets = new();
        private List<GameObject> _createdHotKeys = new();
        private List<KeyCode> _availableKeyCodes = new();
    
        public void SetupBlackHole(BlackHolePoolConfig config)
        {
            _config = config;
            
            _amountOfAttacks = _config.AmountOfAttacks;
            _blackHoleTimer = _config.BlackHoleDuration;
            
            _canGrow = true;
            _canShrink = false;
            _canCreateHotKeys = true;
            _cloneAttackReleased = false;
            PlayerCanExitState = false;
            
            _availableKeyCodes = new List<KeyCode>(_config.KeyCodeList);
            
            transform.localScale = Vector3.zero;

            _playerCanDisappear = !SkillManager.Instance.Clone.IsCrystalInsteadOfClone();
        }
        
        public override void ReturnToPool()
        {
            ClearHotKeys();
            
            _targets.Clear();
            _availableKeyCodes.Clear();

            PlayerCanExitState = false;
            transform.localScale = Vector3.zero; 
            
            base.ReturnToPool();
        }

        private void Update()
        {
            if (_config == null) return;
            
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
                ReleaseCloneAttack();

            CloneAttackLogic();

            if (_canGrow && !_canShrink)
            {
                transform.localScale = Vector2.Lerp(transform.localScale, 
                    new Vector2(_config.MaxSize, _config.MaxSize), _config.GrowSpeed * Time.deltaTime);
            }

            if (_canShrink)
            {
                transform.localScale = Vector2.Lerp(transform.localScale, 
                    _config.ShrinkTargetScale, _config.ShrinkSpeed * Time.deltaTime);
            
                if (transform.localScale.x < 0)
                    ReturnToPool();
            }
        }

        private void ReleaseCloneAttack()
        {
            if(_targets.Count <= 0) return;

            ClearHotKeys();
            _cloneAttackReleased = true;
            _canCreateHotKeys = false;

            if(_playerCanDisappear)
            {
                _playerCanDisappear = false;
                PlayerManager.Instance.PlayerGameObject.Fx.MakeTransparent(true);
            }
        }

        private void CloneAttackLogic()
        {
            if (_cloneAttackTimer < 0 && _cloneAttackReleased && _amountOfAttacks > 0)
            {
                _cloneAttackTimer = _config.CloneCooldown;
                int randomIndex = Random.Range(0, _targets.Count);

                float xOffset = Random.Range(0, 100) > _config.CloneXSpawnChanceThreshold 
                    ? _config.CloneXSpawnOffset 
                    : -_config.CloneXSpawnOffset;

                if(SkillManager.Instance.Clone.IsCrystalInsteadOfClone())
                {
                    SkillManager.Instance.Crystal.CreateCrystal();
                    SkillManager.Instance.Crystal.CurrentCrystalChooseRandomTarget();
                }
                else
                    SkillManager.Instance.Clone.CreateClone(_targets[randomIndex], new Vector3(xOffset, 0));

                _amountOfAttacks--;

                if (_amountOfAttacks <= 0)
                    Invoke(nameof(FinishBlackHoleAbility), _config.FinishBlackHoleDelay);
            }
        }

        private void FinishBlackHoleAbility()
        {
            ClearHotKeys();
            PlayerCanExitState = true;

            _canShrink = true;
            _cloneAttackReleased = false;
        }

        public void AddEnemyToList(Transform _enemyTransform) => _targets.Add(_enemyTransform);

        private void ClearHotKeys()
        {
            if(_createdHotKeys.Count <= 0) return;

            foreach (var hotKey in _createdHotKeys)
            {
                if (hotKey != null && hotKey.activeInHierarchy)
                    PoolManager.Instance.Return(hotKey);
            }
            
            _createdHotKeys.Clear();
        }

        private void OnTriggerEnter2D(Collider2D collision) 
        {
            if(collision.TryGetComponent(out Enemy enemy))
            {
                enemy.FreezeTime(true);
                CreateHotKey(collision);
            }
        }

        private void OnTriggerExit2D(Collider2D collision) 
        {
            if(collision.TryGetComponent(out Enemy enemy))
                enemy.FreezeTime(false);
        }

        private void CreateHotKey(Collider2D collision)
        {
            if(_availableKeyCodes.Count <= 0 || !_canCreateHotKeys) return;

            GameObject newHotKey = PoolManager.Instance.Spawn(
                _config.HotKeyConfig.Prefab, 
                collision.transform.position + _config.HotkeySpawnOffset, 
                Quaternion.identity
            );
            
            _createdHotKeys.Add(newHotKey);

            KeyCode chooseKey = _availableKeyCodes[Random.Range(0, _availableKeyCodes.Count)];
            _availableKeyCodes.Remove(chooseKey);

            if (newHotKey.TryGetComponent(out BlackHoleHotKeyController newHotKeyScript))
                newHotKeyScript.SetupHotKey(chooseKey, collision.transform, this);
        }
    }
}
