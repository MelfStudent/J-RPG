namespace J_RPG;

public class Character
{
    public string Name { get; set; }
    public int CurrentHitPoints { get; set; }
    public int MaxHitPoints { get; set; }
    public int PhysicalAttackPower  { get; set; }
    public int MagicAttackPower  { get; set; }
    public enum TypeOfArmor { Fabric, Leather, Mesh, Plates }
    public int DodgeChance { get; set; }
    public int ParadeChance { get; set; }
    public int ChanceSpellResistance { get; set; }

    public void Tackle(Character target)
    {
        
    }

    public void Defend()
    {
        
    }

    public void Heal()
    {
        
    }
    
}