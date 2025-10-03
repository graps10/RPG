using Controllers.Skill_Controllers.SwordSkill;
using Skills.Skills;
using UnityEngine;

namespace Core.Factories
{
    public static class SwordFactory
    {
        public static SwordSkillController CreateSword(SwordType type, GameObject templatePrefab, 
        Vector2 position, Quaternion rotation, SwordConfig config)
    {
        if (templatePrefab == null)
            throw new System.ArgumentNullException(nameof(templatePrefab));
    
        if (config == null)
            throw new System.ArgumentNullException(nameof(config));
        
        GameObject swordObject = Object.Instantiate(templatePrefab, position, rotation);
        SwordSkillController controller = AddControllerByType(type, swordObject);
        
        controller.SetupSword(config.Direction, config.Gravity, config.Player, 
            config.FreezeTimeDuration, config.ReturnSpeed);
        
        SetupControllerByType(controller, config);
        
        return controller;
    }
    
    private static SwordSkillController AddControllerByType(SwordType type, GameObject swordObject)
    {
        return type switch
        {
            SwordType.Bounce => swordObject.AddComponent<BounceSwordSkillController>(),
            SwordType.Pierce => swordObject.AddComponent<PierceSwordSkillController>(),
            SwordType.Spin => swordObject.AddComponent<SpinSwordSkillController>(),
            _ => swordObject.AddComponent<RegularSwordSkillController>()
        };
    }
    
    private static void SetupControllerByType(SwordSkillController controller, SwordConfig config)
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