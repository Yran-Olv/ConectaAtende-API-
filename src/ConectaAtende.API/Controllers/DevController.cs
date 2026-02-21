using ConectaAtende.Application.DTOs;
using ConectaAtende.Application.Services;
using ConectaAtende.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ConectaAtende.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevController : ControllerBase
{
    private readonly IContactRepository _contactRepository;
    private readonly ContactService _contactService;

    public DevController(IContactRepository contactRepository, ContactService contactService)
    {
        _contactRepository = contactRepository;
        _contactService = contactService;
    }

    [HttpGet("seed")]
    public async Task<IActionResult> Seed([FromQuery] int count = 100)
    {
        var random = new Random();
        var firstNames = new[] { "João", "Maria", "Pedro", "Ana", "Carlos", "Julia", "Lucas", "Fernanda", "Rafael", "Mariana" };
        var lastNames = new[] { "Silva", "Santos", "Oliveira", "Souza", "Costa", "Pereira", "Rodrigues", "Almeida", "Nascimento", "Lima" };

        for (int i = 0; i < count; i++)
        {
            var firstName = firstNames[random.Next(firstNames.Length)];
            var lastName = lastNames[random.Next(lastNames.Length)];
            var name = $"{firstName} {lastName}";
            
            var phoneCount = random.Next(1, 4);
            var phones = new List<string>();
            for (int j = 0; j < phoneCount; j++)
            {
                var phone = $"{random.Next(10, 99)}{random.Next(100000000, 999999999)}";
                phones.Add(phone);
            }

            var dto = new CreateContactDto
            {
                Name = name,
                Phones = phones
            };

            await _contactService.CreateAsync(dto);
        }

        return Ok(new { message = $"{count} contatos criados com sucesso" });
    }
}
