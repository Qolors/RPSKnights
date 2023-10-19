# Welcome to RPS Knights

![RPS Knights](https://github.com/Qolors/RPSKnights/blob/master/docs/Images/RPSKnights.gif)

## Patch Notes 10/19/2023
**The following changes have been made to the codebase**:

### Bug Fixes
- Fixed an issue where players could get stuck in an infinite match having only 1 energy each
- Fixed a bug where if a match never finished the player could not join or start any other matches

### UI
- Added more info on the end game message showing Elo adjustments
- Slightly changed the perform action message to be better spaced

### Combat Changes
- Attack now costs 1 Energy
- Defend now costs 0 Energy
- Players regenerate 1 Energy per turn
- Players have 1 minute to make their action, or will result in a forfeit

## What is RPS Knights?
RPS Knights is a unique game engine and simulator that uses Discord as its user interface. It brings a competitive edge to the timeless game of Rock, Paper, Scissors.

- It features GIF rendering of actions for engaging visual feedback.
- It utilizes local database storage for maintaining leaderboards.

## Interested in Self Hosting?
RPS Knights is deployed using Docker. If you're interested in hosting your own RPS Knights Bot, you can find all the necessary files and a comprehensive guide [here](https://github.com/Qolors/RPSKnights/blob/master/docs/setup.md) to get it up and running on your machine.

## Looking to try out?
I have a temporary instance of the bot deployed. You can invite the bot with this url [here](https://discord.com/api/oauth2/authorize?client_id=950229954468126790&permissions=0&scope=applications.commands%20bot). I am not sure how long I will keep this instance up.

## Libraries & Asset Packs Utilized

### Libraries
- [Discord.Net](https://github.com/discord-net/Discord.Net)
- [ImageSharp](https://github.com/SixLabors/ImageSharp)
- [Entity Framework Core](https://github.com/dotnet/efcore)
- [Fergun Interactive](https://github.com/d4n3436/Fergun.Interactive)

### Asset Packs
- [Character Sprites](https://brullov.itch.io/generic-char-asset)