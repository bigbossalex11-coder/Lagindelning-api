using server.Models;
using System.Text.Json;

namespace server.Repositories;

public class PlayerRepository
{
	public List<Player> Players { get; private set; } = new List<Player>
	{
		new Player(1, "adam" , "grön"),
		new Player(2, "eva" ,"gul"),
		new Player(3, "oskar" ,"röd")
	};

	public PlayerRepository()
	{
		if (File.Exists("players.json"))
		{
			var json = File.ReadAllText("players.json");
			Players = JsonSerializer.Deserialize<List<Player>>(json) ?? Players;
		}
	}

	public void Save()
	{
		var json = JsonSerializer.Serialize(Players);
		File.WriteAllText("players.json", json);
	}
}