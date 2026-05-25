using System.Data;

namespace Orders.Dal.Context.Interfaces
{
    public interface IDapperContext
    {
        IDbConnection Connection { get; }
        IDbTransaction? Transaction { get; }
        void BeginTransaction();
        void CloseConnection(); 
    }
}