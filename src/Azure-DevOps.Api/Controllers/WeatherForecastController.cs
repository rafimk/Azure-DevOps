using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Azure_DevOps.Api.Models;
using Azure_DevOps.Api.Data;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

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
    private readonly DataContext _context;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
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
    public async Task<ActionResult> ReadTFS()
    {
        var fileName = "./Excel/TFS_Export.xlsx";
        List<WorkItem> workItems = new();
        string pattern = "^[0-9]+$";

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
                            var id = reader.GetValue(0).ToString();
                            var type = reader.GetValue(1).ToString();
                            var title = "";
                            var description = "";
                            var assignedTo = "";
                            var state = "";
                            var tags = "";
                            var iterationPath = "";
                            var code = "";

                            Console.WriteLine($"{id} - {type}");
                            if (Regex.IsMatch(id, pattern))
                            {
                                title = type switch    
                                {    
                                    "Epic" => reader.GetValue(2) is not null ? reader.GetValue(2).ToString() : "",
                                    "Feature" => reader.GetValue(3) is not null ? reader.GetValue(3).ToString() : "",
                                    "Product Backlog Item" => reader.GetValue(4) is not null ? reader.GetValue(4).ToString() : "",
                                    "Acceptance Criteria" => reader.GetValue(5) is not null ? reader.GetValue(5).ToString() : "",
                                    "UI Acceptance Criteria" => reader.GetValue(5) is not null ? reader.GetValue(5).ToString() : "",
                                    _ => ""
                                };

                                if (reader.GetValue(9) is not null)
                                {
                                    description = reader.GetValue(9).ToString();
                                } 

                                if (reader.GetValue(6) is not null)
                                {
                                    assignedTo = reader.GetValue(6).ToString();
                                } 

                                if (reader.GetValue(7) is not null)
                                {
                                    state = reader.GetValue(7).ToString();
                                }  

                                if (reader.GetValue(8) is not null)
                                {
                                    tags = reader.GetValue(8).ToString();
                                } 

                                if (reader.GetValue(10) is not null)
                                {
                                    iterationPath = reader.GetValue(10).ToString();
                                } 

                                if (reader.GetValue(11) is not null)
                                {
                                    code = reader.GetValue(11).ToString();
                                } 

                                workItems.Add(new WorkItem {
                                    Id = Convert.ToInt32(reader.GetValue(0).ToString()),
                                    Type = reader.GetValue(1).ToString(),
                                    Title = title,
                                    Description = description,
                                    AssignedTo = assignedTo,
                                    State = state,
                                    Tags = tags,
                                    IterationPath = iterationPath,
                                    Code = code,
                                    ParentId  = 0,
                                    SiteId = 0,
                                    SiteParentId = 0,
                                    Updated  = 0
                                });
                            }
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

        if (workItems.Any())
        {
            foreach(var workItem in workItems)
            {
                var existingWorkItem = await _context.WorkItems.SingleOrDefaultAsync(x => x.Id == workItem.Id);

                if (existingWorkItem is not null)
                {
                    existingWorkItem.Type = workItem.Type;
                    existingWorkItem.Title = workItem.Title;
                    existingWorkItem.Description = workItem.Description;
                    existingWorkItem.AssignedTo = workItem.AssignedTo;
                    existingWorkItem.State = workItem.State;
                    existingWorkItem.Tags = workItem.Tags;
                    existingWorkItem.IterationPath = workItem.IterationPath;
                    existingWorkItem.Code = workItem.Code;
                }
                else
                {
                    await _context.WorkItems.AddAsync(workItem);
                }
            }
            
            await _context.SaveChangesAsync();
        }
        
        return Ok();
    }
}
