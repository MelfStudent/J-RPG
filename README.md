# J-RPG Game

Welcome to the J-RPG Game, a turn-based role-playing game (RPG) designed for two players. This project showcases an implementation of various object-oriented programming (OOP) principles in C#, including abstraction, inheritance, and polymorphism. Players create teams of characters, each with unique abilities and stats, and battle until one team is victorious.


## Features

- Five unique character classes: Warrior, Mage, Paladin, Thief, and Priest.
- Skill-based combat: Each class has unique abilities and skills with cooldowns and mana costs.
- Turn-based gameplay: Players alternate turns choosing actions and targeting opponents.
- Configurable game settings: Adjust character stats and team size via a configuration menu.
- Detailed battle logs: See the results of each action during gameplay.


## Gameplay Overview

- Character Selection: Each player selects characters for their team, customizing their names and choosing from five predefined classes.
- Turn-Based Combat: Players take turns choosing skills and targets. Each action has consequences, such as dealing damage, buffing allies, or healing.
- Victory Conditions: The game ends when one team is entirely defeated.
## Installation

### Prerequisites
- .NET 8 SDK installed on your machine.
- A code editor such as Visual Studio or Visual Studio Code.

### Setup Steps
1. Clone the repository:
```
git clone https://github.com/MelfStudent/J-RPG.git
```
2. Navigate to the project directory:
```
cd J-RPG
```
3. Build the project:
```
dotnet build
```
4. Run the game:
```
dotnet run
```

## Gameplay Mechanics

### Character Stats
#### Characters are defined by the following attributes:

- Hit Points (HP): Health of the character.
- Physical Attack: Base damage for physical attacks.
- Magic Attack: Base damage for magical attacks.
- Armor: Damage reduction based on armor type.
- Dodge Chance: Probability of evading an attack.
- Parade Chance: Probability of parrying an attack to reduce damage.
- Spell Resistance: Probability of resisting magical attacks.
- Speed: Determines the order of actions during combat.
#### Combat System
- Skill Selection: Players choose a skill from their character’s skill set.
- Target Selection: Choose an opponent or ally as the target of the skill.
- Attack Resolution: The game calculates the effects of the chosen action, including damage, buffs, or healing.
- Turn Rotation: Players alternate turns until one team is defeated.
## Configuration

The game uses a configuration file (classes.json) to define character stats and team size. This file is located in the Resources folder.

### Example Configuration
```
  {
    "Warrior": {
        "MaxHitPoints": 100,
        "PhysicalAttackPower": 50,
        "MagicAttackPower": 0,
        "Armor": "Plates",
        "DodgeChance": 5,
        "ParadeChance": 25,
        "ChanceSpellResistance": 10,
        "Speed": 50,
        "HasMana": false,
        "ManaPoints": 0
    },
    ...
  }
```

### Editing Configuration
1. Launch the game and choose "Edit Configuration" from the main menu.
2. Select a character class and modify its attributes.
3. Save changes to apply them to future games.
## Gameplay Screenshots

### Welcome Screen
The main menu welcomes players to the J-RPG game and provides options to start a new game, edit configurations, or exit.

![Welcome Screen](/Screenshots/welcome_screen.png)

### Character Selection
Players can name their characters, select their classes, and create their teams in the character selection phase.

![Character Selection](/Screenshots/character_selection.png)

### Battle Phase
During combat, players take turns using skills to defeat the opposing team.

![Battle Phase](/Screenshots/battle_phase.png)

### Configuration Menu
Modify character stats or adjust team sizes using the in-game configuration menu.

![Configuration Menu](/Screenshots/config_menu.png)
![Configuration Menu2](/Screenshots/config_menu2.png)

### End of the Game
At the end of the game, a screen displays the victorious player, providing closure to the intense battle.

![Game End](/Screenshots/game_end.png)
## Contributing

- MelfStudent - Developer 
