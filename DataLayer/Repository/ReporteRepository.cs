using EntityLayer.Responses;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DataLayer.Repository
{
    public class ReporteRepository : IReporteRepository
    {
        private readonly string _connectionString;
        private readonly Response _response = new();

        public ReporteRepository(IConfiguration configuration) 
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }

        public async Task<Response> Prueba() 
        {
            try
            {
                using SqlConnection conn = new(_connectionString);
                await conn.OpenAsync();

                using SqlCommand comand = new("TU_NOMBRE_DE_SP", conn);
                comand.Parameters.AddWithValue("@Paramentro", "Valor"); // Por si tienes que enviarle algo al SP
                comand.CommandType = CommandType.StoredProcedure;
                using SqlDataReader reader = await comand.ExecuteReaderAsync();

                //List<objet> lista = new();
                //while (await reader.ReadAsync()) 
                //{
                //  object miobjeto = new()
                //  {
                //    nombre = reader["nombre"].ToString(),
                //    apellido = reader["apellido"].ToString()
                //  };
                //
                //  lista.Add(miobjeto);
                //}

                _response.Code = ResponseType.Success;
                _response.Message = "Ok";
                _response.Data = ""; // Aqui seria _response.Data = lista;
            }
            catch (Exception ex) 
            {
                _response.Code = ResponseType.Error;
                _response.Message = ex.Message;
                _response.Data = ex.StackTrace;
            }

            return _response;
        }
    }
}
