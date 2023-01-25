using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Azure_DevOps.Api.Models;

namespace Azure_DevOps.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet("ReadTFS")]
    public ActionResult ReadTFS()
    {
        var fileName = "./Excel/TFS_Export.xlsx";
        List<WorkItem> workItems = new();

        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                        
                while (reader.Read()) //Each row of the file
                {

                    if (reader.GetValue(0) is not null && reader.GetValue(1) is not null)
                    {
                        if (reader.GetValue(0)?.ToString()?.Length < 10)
                        {
                            Console.WriteLine($"{reader.GetValue(0).ToString()} - {reader.GetValue(1).ToString()}");
                        }
                    }
                   
                    // workItems.Add(new WorkItem
                    // {
                    //     Description = reader.GetValue(0).ToString(),
                    //     Type = reader.GetValue(1).ToString(),
                    //     Title = reader.GetValue(2).ToString()
                    // });
                }
            }
        }


        return Ok();
    }
}
