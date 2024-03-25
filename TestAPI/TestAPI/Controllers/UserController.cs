using Microsoft.AspNetCore.Mvc;
using TestAPI.Service;


namespace TestAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IGenService<User> _userService;
    private readonly IMetricsService _metricsService;
    public UserController(IGenService<User> userService,IMetricsService metricsService)
    {
        _userService = userService;
        _metricsService = metricsService;
    }


    [HttpGet("{id:int}")]
    public IActionResult GettingUserById(int id)
    {
        return Ok(_userService.Get(id));
    }

    [HttpDelete]
    public IActionResult GetError()
    {
        return BadRequest();
    }

    [HttpPost]
    public IActionResult CreatingNewUser([FromBody] User user)
    {
        try
        {
            _userService.Set(user.Id,user);
            _metricsService.Inc();
            return Ok();
        }
        catch(Exception e){
            return BadRequest(e.Message);
        }
    }
}

