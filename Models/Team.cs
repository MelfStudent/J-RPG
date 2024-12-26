namespace J_RPG.Models;

using Services;

/// <summary>
/// Represents a team of characters in the game. The team can be either attacking or defending.
/// This class encapsulates the team name, the list of team members, and provides methods to add members and check the team status.
/// </summary>
public class Team
{
    /// <summary>
    /// Gets or sets the name of the team.
    /// </summary>
    /// <value>The name of the team.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets the list of characters that are part of the team.
    /// </summary>
    /// <value>A list of characters that belong to this team.</value>
    public List<Character> Members { get; private set; } = new List<Character>();

    /// <summary>
    /// Initializes a new instance of the <see cref="Team"/> class with the specified name.
    /// </summary>
    /// <param name="name">The name of the team.</param>
    public Team(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Adds a new member to the team.
    /// </summary>
    /// <param name="character">The character to be added to the team.</param>
    public void AddMember(Character character)
    {
        Members.Add(character);
    }
    
    /// <summary>
    /// Switches the attacking and defending teams in the game. 
    /// This method swaps the teams in the <see cref="Menu.TeamThatDefends"/> and <see cref="Menu.TeamThatAttacks"/> static references.
    /// </summary>
    public static void SwitchPlayers()
    {
        (Menu.TeamThatDefends, Menu.TeamThatAttacks) = (Menu.TeamThatAttacks, Menu.TeamThatDefends);
    }
    
    /// <summary>
    /// Counts the number of living members in the team.
    /// </summary>
    /// <returns>The number of living members in the team.</returns>
    public int NumberPeopleAlive()
    {
        var members = Members.Where(character => !character.IsDead);
        
        return members.Count();
    }
}
