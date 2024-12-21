namespace J_RPG.Models;

using Services;

public class Team
{
    public string Name { get; set; }
    public List<Character> Members { get; private set; } = new List<Character>();
    
    public Team(string name)
    {
        Name = name;
    }

    public void AddMember(Character character)
    {
        Members.Add(character);
    }
    
    public static void SwitchPlayers()
    {
        (Menu.TeamThatDefends, Menu.TeamThatAttacks) = (Menu.TeamThatAttacks, Menu.TeamThatDefends);
    }
    
    public int NumberPeopleAlive()
    {
        var members = Members.Where(character => !character.IsDead);
        
        return members.Count();
    }
}
