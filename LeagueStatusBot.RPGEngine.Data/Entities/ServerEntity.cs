namespace LeagueStatusBot.RPGEngine.Data.Entities;

public class ServerEntity
{
    public ulong ServerId { get; set; }
    public ICollection<BeingEntity> Beings { get; set; } = new List<BeingEntity>();
}