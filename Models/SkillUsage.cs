namespace J_RPG.Models
{
    /// <summary>
    /// Represents the usage of a skill by a character in combat.
    /// This class encapsulates the user of the skill, the skill itself, and the target of the skill.
    /// </summary>
    public class SkillUsage
    {
        /// <summary>
        /// Gets or sets the character who is using the skill.
        /// </summary>
        /// <value>The character who is using the skill.</value>
        public Character User { get; set; }

        /// <summary>
        /// Gets or sets the skill that is being used.
        /// </summary>
        /// <value>The skill being used by the character.</value>
        public Skill ChosenSkill { get; set; }

        /// <summary>
        /// Gets or sets the target of the skill.
        /// This is the character who is affected by the skill. It can be the user themselves, an enemy, or an ally.
        /// </summary>
        /// <value>The target of the skill.</value>
        public Character Target { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillUsage"/> class.
        /// This constructor assigns the user, the chosen skill, and the target of the skill.
        /// </summary>
        /// <param name="user">The character who is using the skill.</param>
        /// <param name="skill">The skill that the user is using.</param>
        /// <param name="target">The target of the skill. If null, the skill may target the user or be used on all allies/enemies, depending on the skill type.</param>
        public SkillUsage(Character user, Skill skill, Character target = null!)
        {
            User = user;
            ChosenSkill = skill;
            Target = target;
        }
    }
}
