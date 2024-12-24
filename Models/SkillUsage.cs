namespace J_RPG.Models;

public class SkillUsage
{
    public Character User { get; set; }
    public Skill ChosenSkill { get; set; }
    public Character Target { get; set; }

    public SkillUsage(Character user, Skill skill, Character target = null!)
    {
        User = user;
        ChosenSkill = skill;
        Target = target;
    }
}
