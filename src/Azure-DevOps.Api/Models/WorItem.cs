namespace Azure_DevOps.Api.Models;

public class WorkItem
{
    public int Id { get; set;} = 0;
    public string? Type { get; set;}
    public string? Title { get; set;}
    public string? Description { get; set; }
    public string? AssignedTo { get; set; }
    public string? State { get; set; } 
    public string? Tags { get; set; }
    public string? IterationPath { get; set; }
    public string? Code { get; set; }
    public int ParentId { get; set; } = 0;
    public int SiteId { get; set; } = 0;
    public int SiteParentId { get; set; } = 0;
    public int Updated { get; set;} = 0;
}
