namespace Net10Auth.Shared.Infrastructure.Models;

public interface IListResult<T>
{
    IReadOnlyList<T> Items { get; set; }
}