using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace PrjFinanzas360.Data
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
            => new SqlConnection(
                _configuration.GetConnectionString("SqlServer1")
            );

        public async Task<IDbConnection> CreateConnectionAsync(string idUsuario)
        {
            var connection = new SqlConnection(
                _configuration.GetConnectionString("SqlServer1")
            );

            await connection.OpenAsync();

            // SETEAMOS LA CONEXION PARA PODER CONTROLAR EL TRIGGER DE LA DB
            await connection.ExecuteAsync(
                "EXEC sys.sp_set_session_context @key=N'ID_USUARIO', @value=@IdUsuario",
                new { IdUsuario = idUsuario }
            );

            return connection;
        }
    }
}
