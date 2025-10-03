using Core;
using UnityEngine;

namespace Controllers.Skill_Controllers.SwordSkill
{
    public class RegularSwordSkillController : SwordSkillController
    {
        public override void SetupSword(Vector2 dir, float gravityScale,
            Player.Player player, float freezeTimeDuration, float returnSpeed)
        {
            base.SetupSword(dir, gravityScale, player, freezeTimeDuration, returnSpeed);

            anim.SetBool(AnimatorHashes.Rotation, true);
        }
    }
}