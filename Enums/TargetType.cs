namespace J_RPG.Enums
{
    /// <summary>
    /// Enum representing the different types of targets that actions or abilities can affect in the game.
    /// </summary>
    public enum TargetType
    {
        /// <summary>
        /// The action or ability targets the caster (self).
        /// </summary>
        Self,

        /// <summary>
        /// The action or ability targets an ally on the same team.
        /// </summary>
        Ally,

        /// <summary>
        /// The action or ability targets an enemy on the opposing team.
        /// </summary>
        Enemy,

        /// <summary>
        /// The action or ability targets all enemies on the opposing team.
        /// </summary>
        AllEnemies,

        /// <summary>
        /// The action or ability targets all allies on the same team.
        /// </summary>
        AllAllies
    }
}
