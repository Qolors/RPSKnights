using LeagueStatusBot.RPGEngine.Core.Engine.Beings;
using LeagueStatusBot.RPGEngine.Messaging;

namespace LeagueStatusBot.RPGEngine.Core.Controllers;
/// <summary>
/// TurnManager handles the processing of the gif generated after each turn is complete
/// </summary>
public class TurnManager
{
    private AnimationManager animationManager;
    public TurnManager(AnimationManager animationManager)
    {
        this.animationManager = animationManager;
    }
    public async Task<TurnMessage> ProcessTurn(List<string> player1actions, List<string> player2actions, Player player1, Player player2, string basePath, AnimationManager animationManager)
    {
        this.animationManager = animationManager;
        return await this.animationManager.CreateBattleAnimation(player1, player2, player1actions, player2actions, basePath);
    }
}