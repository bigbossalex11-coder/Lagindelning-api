using server.Data;
using server.Models;

namespace server.Repositories;

public class PlayerRepository
{
    private readonly AppDbContext _db;

    public PlayerRepository(AppDbContext db)
    {
        _db = db;
    }

    public List<Player> GetAll() => _db.Players.ToList();

    public Player? GetById(int id) => _db.Players.FirstOrDefault(p => p.Id == id);

    public Player Add(Player player)
    {
        _db.Players.Add(player);
        _db.SaveChanges();
        return player;
    }

    public void Delete(Player player)
    {
        _db.Players.Remove(player);
        _db.SaveChanges();
    }

    public Player Update(Player existing, Player changed)
    {
        _db.Entry(existing).CurrentValues.SetValues(changed);
        _db.SaveChanges();
        return existing;
    }
}
