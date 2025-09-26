using Skills.Skills;
using UnityEngine;

namespace Skills
{
    public class SkillManager : MonoBehaviour
    {
        public static SkillManager Instance;

        public DashSkill Dash { get; private set; }
        public CloneSkill Clone { get; private set; }
        public SwordSkill Sword { get; private set; }
        public BlackHoleSkill BlackHole { get; private set; }
        public CrystalSkill Crystal { get; private set; }
        public ParrySkill Parry { get; private set; }
        public DodgeSkill Dodge { get; private set; }

        private void Awake()
        {
            if (Instance != null)
                Destroy(Instance.gameObject);
            else
                Instance = this;
        }

        private void Start()
        {
            Dash = GetComponent<DashSkill>();
            Clone = GetComponent<CloneSkill>();
            Sword = GetComponent<SwordSkill>();
            BlackHole = GetComponent<BlackHoleSkill>();
            Crystal = GetComponent<CrystalSkill>();
            Parry = GetComponent<ParrySkill>();
            Dodge = GetComponent<DodgeSkill>();
        }
    }
}
