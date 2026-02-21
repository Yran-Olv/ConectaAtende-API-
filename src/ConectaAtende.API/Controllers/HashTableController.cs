using ConectaAtende.Infrastructure.DataStructures;
using Microsoft.AspNetCore.Mvc;

namespace ConectaAtende.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HashTableController : ControllerBase
{
    [HttpPost("compare")]
    public IActionResult Compare([FromQuery] int itemCount = 1000)
    {
        if (itemCount < 1 || itemCount > 100000)
        {
            return BadRequest("itemCount deve estar entre 1 e 100000");
        }

        var result = HashTableComparison.ComparePerformance(itemCount);
        return Ok(result);
    }

    [HttpPost("demo")]
    public IActionResult Demo()
    {
        var hashTable = new CustomHashTable<string, string>();

        // Demonstração de operações
        hashTable.Insert("chave1", "valor1");
        hashTable.Insert("chave2", "valor2");
        hashTable.Insert("chave3", "valor3");

        var results = new
        {
            Count = hashTable.Count,
            Capacity = hashTable.Capacity,
            ContainsKey1 = hashTable.ContainsKey("chave1"),
            ContainsKey4 = hashTable.ContainsKey("chave4"),
            GetValue1 = hashTable.TryGetValue("chave1", out var value1) ? value1 : null,
            GetValue4 = hashTable.TryGetValue("chave4", out var value4) ? value4 : null,
            AllItems = hashTable.ToList()
        };

        hashTable.Remove("chave2");

        return Ok(new
        {
            Initial = results,
            AfterRemove = new
            {
                Count = hashTable.Count,
                ContainsKey2 = hashTable.ContainsKey("chave2")
            }
        });
    }
}
