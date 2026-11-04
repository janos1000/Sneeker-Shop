using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthUserAPI.Models.DTOs
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewRoleDtos : ControllerBase
    {
        public string? Name { get; set; }
    }
}
