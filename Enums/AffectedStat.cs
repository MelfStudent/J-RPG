namespace J_RPG.Enums
{
    /// <summary>
    /// Enum representing the different stats that can be affected by actions or abilities in the game.
    /// </summary>
    public enum AffectedStat
    {
        /// <summary>
        /// Represents the physical attack stat, which affects the character's damage output in physical attacks.
        /// </summary>
        PhysicalAttack,

        /// <summary>
        /// Represents the magic attack stat, which affects the character's damage output in magic-based attacks.
        /// </summary>
        MagicAttack,

        /// <summary>
        /// Represents a stat that does not affect any specific stat. Used as a neutral state.
        /// </summary>
        Null
    }
}
