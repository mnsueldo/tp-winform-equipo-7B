using System;
using System.Data;
using System.Data.SqlClient;
using dominio;

namespace Negocio
{
    public class AccesoDatos
    {
        private SqlConnection conexion;
        private SqlCommand comando;
        private SqlDataReader lector;

        public SqlDataReader Lector => lector;

        public AccesoDatos()
        {
            conexion = new SqlConnection("server=localhost; database=CATALOGO_P3_DB; integrated security=true");
            comando = new SqlCommand();
        }

        public void setearConsulta(string consulta)
        {
            comando.CommandType = CommandType.Text;
            comando.CommandText = consulta;
            comando.Parameters.Clear();
        }

        public void ejecutarLectura()
        {
            comando.Connection = conexion;
            try
            {
                conexion.Open();
                lector = comando.ExecuteReader();
            }
            catch (SqlException ex)
            {
                if (conexion.State == ConnectionState.Open) conexion.Close();
                throw new ApplicationException("Error de base al ejecutar la lectura.", ex);
            }
            catch (Exception ex)
            {
                if (conexion.State == ConnectionState.Open) conexion.Close();
                throw new ApplicationException("Error inesperado al ejecutar la lectura.", ex);
            }
        }

        // Nota: el parámetro 'id' no se usa; se mantiene para no romper llamadas existentes.
        public void ejecutarExcalar(int id)
        {
            comando.Connection = conexion;
            try
            {
                conexion.Open();
                var _ = comando.ExecuteScalar(); // si alguna llamada espera el valor, cambiar a retornar object/int
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Error de base al ejecutar el escalar.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al ejecutar el escalar.", ex);
            }
            finally
            {
                if (conexion.State == ConnectionState.Open) conexion.Close();
            }
        }

        public void ejecutarAccion()
        {
            comando.Connection = conexion;
            try
            {
                conexion.Open();
                comando.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Error de base al ejecutar la acción.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al ejecutar la acción.", ex);
            }
            finally
            {
                if (conexion.State == ConnectionState.Open) conexion.Close();
            }
        }

        public void setearParametro(string nombre, object valor)
        {
            comando.Parameters.AddWithValue(nombre, valor ?? DBNull.Value);
        }

        public void cerrarConexion()
        {
            try
            {
                if (lector != null && !lector.IsClosed)
                    lector.Close();
            }
            finally
            {
                if (conexion.State == ConnectionState.Open)
                    conexion.Close();
                comando.Parameters.Clear();
            }
        }
    }
}
