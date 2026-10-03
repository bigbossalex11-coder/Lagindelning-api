using System.Resources;
using Microsoft.AspNetCore.Mvc;
using server.Models;
using server.Repositories;

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
    public List<Player> GetAll() => _repo.GetAll();

    [HttpPost]
    public IActionResult Create(Player newPlayer)
    {
        if (string.IsNullOrWhiteSpace(newPlayer.Name))
            return BadRequest("Namn saknas");
        if (!AllowedRanks.Contains(newPlayer.Rank))
            return BadRequest("Ranken måste vara grön, gul eller röd");

        var created = _repo.Add(newPlayer with { Id = 0 });
        return Created($"/players/{created.Id}", created);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Player updated)
    {
        var existing = _repo.GetById(id);
        if (existing is null)
            return NotFound();
        if (!AllowedRanks.Contains(updated.Rank))
            return BadRequest("Ranken måste vara grön, gul eller röd");

        var changed = existing with { Rank = updated.Rank };

        _repo.Update(existing, changed);
        return Ok(changed);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var existing = _repo.GetById(id);
        if (existing is null)
            return NotFound();

        _repo.Delete(existing);
        return NoContent();
    }

    [HttpPost("{id}/file")]
    public IActionResult UploadFile(int id, IFormFile file)
    {
        var existing = _repo.GetById(id);
        if (existing is null)
            return NotFound();

        Directory.CreateDirectory("uploads");
        var savedName = $"{id}_{file.FileName}";
        var path = Path.Combine("uploads", savedName);
        using var stream = System.IO.File.Create(path);
        file.CopyTo(stream);
        var changed = existing with { FileName = savedName };
        _repo.Update(existing, changed);
        return Ok(changed);
    }
}
