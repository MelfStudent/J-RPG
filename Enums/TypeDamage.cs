namespace J_RPG.Enums
{
    /// <summary>
    /// Enum representing the types of damage that can be dealt in the game.
    /// </summary>
    public enum TypeDamage
    {
        /// <summary>
        /// Represents physical damage, typically based on the attacker's physical stats and the target's physical defense.
        /// </summary>
        Physical,

        /// <summary>
        /// Represents magic damage, typically based on the attacker's magic stats and the target's magic resistance.
        /// </summary>
        Magic,

        /// <summary>
        /// Represents a type of damage that has no effect, often used for special cases or non-damaging actions.
        /// </summary>
        Null
    }
}
