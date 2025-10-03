using Enemies.Base;
using UnityEngine;

namespace Controllers.Skill_Controllers.SwordSkill
{
    public class PierceSwordSkillController: SwordSkillController
    {
        private int _pierceAmount;
        
        public void SetupPierce(int pierceAmount)
        {
            _pierceAmount = pierceAmount;
        }

        protected override void StuckInto(Collider2D collision)
        {
            if (_pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
            {
                _pierceAmount--;
                return;
            }
            
            base.StuckInto(collision);
        }
    }
}