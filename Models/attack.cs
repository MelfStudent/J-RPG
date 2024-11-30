namespace J_RPG.Models;

public class Attack
{
    public string Name { get; set; }
    public Character AttackingCharacter { get; set; }
    public Character TargetCharacter { get; set; }
    public int Damage { get; set; }
    public TypeDamage TypeOfDamage { get; set; }
    
    public enum TypeDamage { Physical, Magic }
    
    public Attack(string name, Character attackingCharacter, Character targetCharacter, int damage, TypeDamage typeOfDamage)
    {
        Name = name;
        AttackingCharacter = attackingCharacter;
        TargetCharacter = targetCharacter;
        Damage = damage;
        TypeOfDamage = typeOfDamage;
    }
}