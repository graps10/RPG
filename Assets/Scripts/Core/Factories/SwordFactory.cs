using Controllers.Skill_Controllers.SwordSkill;
using Core.ObjectPool;
using Core.ObjectPool.Configs.Controllers;
using Skills.Skills;
using UnityEngine;

namespace Core.Factories
{
    public static class SwordFactory
    {
        public static SwordSkillController CreateSword(
            SwordType type, 
            Vector2 position, 
            Quaternion rotation, 
            Vector2 direction, 
            Player.Player player, 
            SwordPoolConfig config)
        {
            if (config == null)
                throw new System.ArgumentNullException(nameof(config));
            
            GameObject prefabToSpawn;
            float gravity;

            switch (type)
            {
                case SwordType.Bounce:
                    prefabToSpawn = config.BouncePrefab;
                    gravity = config.BounceGravity;
                    break;
                case SwordType.Pierce:
                    prefabToSpawn = config.PiercePrefab;
                    gravity = config.PierceGravity;
                    break;
                case SwordType.Spin:
                    prefabToSpawn = config.SpinPrefab;
                    gravity = config.SpinGravity;
                    break;
                default:
                    prefabToSpawn = config.Prefab; // regular prefab
                    gravity = config.RegularGravity;
                    break;
            }
            
            GameObject swordObject = PoolManager.Instance.Spawn(prefabToSpawn, position, rotation);
            
            if (swordObject.TryGetComponent(out SwordSkillController controller))
            {
                controller.SetupSword(direction, gravity, player, config.FreezeTimeDuration, config.ReturnSpeed);
                SetupControllerByType(controller, config);
            }
            
            return controller;
        }
        
        private static void SetupControllerByType(SwordSkillController controller, SwordPoolConfig config)
        {
            switch (controller)
            {
                case BounceSwordSkillController bounce:
                    bounce.SetupBounce(true, config.BounceAmount, config.BounceSpeed);
                    break;
                case PierceSwordSkillController pierce:
                    pierce.SetupPierce(config.PierceAmount);
                    break;
                case SpinSwordSkillController spin:
                    spin.SetupSpin(true, config.MaxTravelDistance, config.SpinDuration, config.HitCooldown);
                    break;
            }
        }
}

    public class SwordConfig
    {
        // Common properties
        public Vector2 Direction { get; set; }
        public float Gravity { get; set; }
        public Player.Player Player { get; set; }
        public float FreezeTimeDuration { get; set; }
        public float ReturnSpeed { get; set; }

        // Type-specific properties
        
        // Bounce
        public int BounceAmount { get; set; }
        public float BounceSpeed { get; set; }
        
        // Pierce
        public int PierceAmount { get; set; }
        
        // Spin
        public float MaxTravelDistance { get; set; }
        public float SpinDuration { get; set; }
        public float HitCooldown { get; set; }
    }
}