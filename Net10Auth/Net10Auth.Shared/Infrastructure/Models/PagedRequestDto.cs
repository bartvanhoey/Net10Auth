namespace Net10Auth.Shared.Infrastructure.Models;

public class PagedRequestDto : IPagedRequestDto
{
    public int SkipCount { get; set; } 
    public int MaxResultCount { get; set; } = 1000;
    public string? Sorting { get; set; }
}