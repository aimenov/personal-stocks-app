namespace StockScreener.Application.Interfaces;

/// <summary>
/// Factory for creating new UnitOfWork instances with scoped DbContext
/// Used for concurrent database operations to avoid threading issues
/// </summary>
public interface IUnitOfWorkFactory
{
    IUnitOfWork CreateUnitOfWork();
}
