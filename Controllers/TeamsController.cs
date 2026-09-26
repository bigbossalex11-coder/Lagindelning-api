using Microsoft.AspNetCore.Mvc;
using server.Models;
using server.Repositories;

namespace server.Controllers;

[ApiController]
[Route("teams")]
public class TeamsController : ControllerBase
{
    private readonly PlayerRepository _repo;
    
    public TeamsController(PlayerRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public IActionResult ShuffleTeams(int teamCount, string mode)
    {
        if (teamCount <= 0)
            return BadRequest("0 och negativa tal är ej tillåtna");
        var shuffled = _repo.Players.OrderBy(p => Random.Shared.Next()).ToList();
        if (mode == "level")
        {
            shuffled = shuffled.OrderBy(p => p.Rank).ToList();
        }
        var teams = new List<List<Player>>();
        for (int i = 0; i < teamCount; i++)
        {
            teams.Add(new List<Player>());
        }

        for (int i = 0; i < shuffled.Count; i++)
        {
            teams[i % teamCount].Add(shuffled[i]);
        }
        return Ok(teams);
    }
}