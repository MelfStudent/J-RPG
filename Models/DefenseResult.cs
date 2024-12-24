namespace J_RPG.Models;

public struct DefenseResult
{
    public bool IsDodged { get; set; }
    public bool IsParried { get; set; }
    public int DamageTaken { get; set; }
}