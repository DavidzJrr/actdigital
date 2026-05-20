using System.Diagnostics;
using Desafio.Application.Requests;
using Desafio.Application.Services;
using Desafio.Models;
using Microsoft.AspNetCore.Mvc;

namespace Desafio.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IAnalystService _analystService;

    public HomeController(ILogger<HomeController> logger, IAnalystService analystService)
    {
        _logger = logger;
        _analystService = analystService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost("api/analysts/register")]
    public async Task<IActionResult> RegisterAnalystAsync([FromBody] RegisterAnalystRequest request)
    {
        await _analystService.RegisterAnalystAsync(request);
        return Created(string.Empty, null);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
