using System.Data;
using Microsoft.Data.SqlClient;
using Orders.Dal.Context.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Orders.Dal.Context
{
    public class DapperContext : IDapperContext
    {
        private readonly string _connectionString;
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        public DapperContext(IConfiguration configuration)
{
        _connectionString = configuration.GetConnectionString("OrdersDb");
}

        public IDbConnection Connection
            => _connection ??= new SqlConnection(_connectionString);

        public IDbTransaction Transaction => _transaction;

        public void BeginTransaction()
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();
            _transaction = Connection.BeginTransaction();
        }

        public void Commit()
        {
            _transaction?.Commit();
            _transaction = null;
        }


        public void CloseConnection()
        {
            if (Transaction != null)
            {
                Transaction.Dispose();
                _transaction = null;
            }

            if (Connection != null && Connection.State != ConnectionState.Closed)
            {
                Connection.Close();
            }
        }
    }
}