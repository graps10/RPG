using UnityEngine;

public class Dash_Skill : Skill
{
    public override void UseSkill()
    {
        base.UseSkill();

        Debug.Log("Created clone");
    }
}
