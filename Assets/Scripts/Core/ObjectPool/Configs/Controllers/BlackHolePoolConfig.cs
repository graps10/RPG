using System.Collections.Generic;
using UnityEngine;

namespace Core.ObjectPool.Configs.Controllers
{
    [CreateAssetMenu(fileName = "BlackHolePoolConfig", menuName = "ObjectPool/Configs/Controllers/Black Hole")]
    public class BlackHolePoolConfig : BasePoolConfig
    {
        [Header("Black Hole Specific Settings")]
        [SerializeField] private int amountOfAttacks = 4;
        [SerializeField] private float cloneCooldown = 0.3f;
        [SerializeField] private float blackHoleDuration;
        [Space]
        [SerializeField] private float maxSize;
        [SerializeField] private float growSpeed;
        [SerializeField] private float shrinkSpeed;
        
        [Header("Clone & HotKey Settings")]
        [SerializeField] private HotKeyPoolConfig hotKeyConfig;
        [SerializeField] private List<KeyCode> keyCodeList;
        [SerializeField] private float cloneXSpawnOffset = 2f; 
        [SerializeField] private int cloneXSpawnChanceThreshold = 50;
        [SerializeField] private float finishBlackHoleDelay = 1f;
        [SerializeField] private Vector2 shrinkTargetScale = new(-1f, -1f);
        [SerializeField] private Vector3 hotkeySpawnOffset = new(0f, 3f, 0f);

        public int AmountOfAttacks => amountOfAttacks;
        public float CloneCooldown => cloneCooldown;
        public float BlackHoleDuration => blackHoleDuration;
        public float MaxSize => maxSize;
        public float GrowSpeed => growSpeed;
        public float ShrinkSpeed => shrinkSpeed;
        
        public BasePoolConfig HotKeyConfig => hotKeyConfig;
        public List<KeyCode> KeyCodeList => keyCodeList;
        public float CloneXSpawnOffset => cloneXSpawnOffset;
        public int CloneXSpawnChanceThreshold => cloneXSpawnChanceThreshold;
        public float FinishBlackHoleDelay => finishBlackHoleDelay;
        public Vector2 ShrinkTargetScale => shrinkTargetScale;
        public Vector3 HotkeySpawnOffset => hotkeySpawnOffset;
    }
}