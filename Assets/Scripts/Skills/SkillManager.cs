using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    public Dash_Skill dash { get; private set; }
    public Clone_Skill clone { get; private set; }
    public Sword_Skill sword { get; private set; }
    public BlackHole_Skill blackHole { get; private set; }
    public Crystal_Skill crystal { get; private set; }
    public Parry_Skill parry { get; private set; }
    public Dodge_Skill dodge { get; private set; }

    void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    void Start()
    {
        dash = GetComponent<Dash_Skill>();
        clone = GetComponent<Clone_Skill>();
        sword = GetComponent<Sword_Skill>();
        blackHole = GetComponent<BlackHole_Skill>();
        crystal = GetComponent<Crystal_Skill>();
        parry = GetComponent<Parry_Skill>();
        dodge = GetComponent<Dodge_Skill>();
    }
}
