using Microsoft.AspNetCore.Mvc;
using server.Models;
using server.Repositories;

namespace server.Controllers;

[ApiController]
[Route("players")]
public class PlayersController : ControllerBase
{
    private readonly PlayerRepository _repo;

    public PlayersController(PlayerRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public List<Player> GetAll() => _repo.Players;
}