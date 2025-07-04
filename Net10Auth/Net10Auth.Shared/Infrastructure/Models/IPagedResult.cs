namespace Net10Auth.Shared.Infrastructure.Models;

public interface IPagedResult<T> : IListResult<T>, IHasTotalCount
{
}