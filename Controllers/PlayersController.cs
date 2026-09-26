using Microsoft.AspNetCore.Mvc;
using server.Models;
using server.Repositories;
using System.Resources;

namespace server.Controllers;

[ApiController]
[Route("players")]
public class PlayersController : ControllerBase
{
    private readonly PlayerRepository _repo;
    private static readonly string[] AllowedRanks = { "grön", "gul", "röd" };

    public PlayersController(PlayerRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public List<Player> GetAll() => _repo.Players;
    
    [HttpPost]
    public IActionResult Create(Player newPlayer)
    {
        if (string.IsNullOrWhiteSpace(newPlayer.Name))
            return BadRequest("Namn saknas");
        if (!AllowedRanks.Contains(newPlayer.Rank))
            return BadRequest("Ranken måste vara grön, gul eller röd");

        var nextId = _repo.Players.Count == 0 ? 1 : _repo.Players.Max(p => p.Id) + 1;
        var created = newPlayer with { Id = nextId };
        _repo.Players.Add(created);
        _repo.Save();
        return Created($"/players/{created.Id}", created);
    }
    
    [HttpPut("{id}")]
    public IActionResult Update(int id, Player updated)
    {
        var existing = _repo.Players.FirstOrDefault(p => p.Id == id);
        if (existing is null)
            return NotFound();
        if (!AllowedRanks.Contains(updated.Rank))
            return BadRequest("Ranken måste vara grön, gul eller röd");

        var changed = existing with { Rank = updated.Rank };

        _repo.Players[_repo.Players.IndexOf(existing)] = changed;
        _repo.Save();
        return Ok(changed);
    }
    
    [HttpDelete("{id}")]
    public IActionResult Delete(int id) 
    {
        var existing = _repo.Players.FirstOrDefault(p => p.Id == id);
        if (existing is null)
            return NotFound();

        _repo.Players.Remove(existing);
        _repo.Save();
        return NoContent();
    }
    [HttpPost("{id}/file")]
    public IActionResult UploadFile(int id, IFormFile file)
    {
        var existing = _repo.Players.FirstOrDefault(p => p.Id == id);
        if (existing is null)
            return NotFound();

        Directory.CreateDirectory("uploads");
        var savedName = $"{id}_{file.FileName}";
        var path = Path.Combine("uploads", savedName);
        using var stream = System.IO.File.Create(path);
        file.CopyTo(stream);
        var changed = existing with { FileName = savedName };
        _repo.Players[_repo.Players.IndexOf(existing)] = changed;
        _repo.Save();
        return Ok(changed);
    }
}