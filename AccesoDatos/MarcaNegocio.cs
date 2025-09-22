using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using dominio;

namespace Negocio
{
    public class MarcaNegocio
    {
        // -------------------- Helpers --------------------

        private static string NormalizarDescripcion(string s)
        {
            var input = (s ?? string.Empty).Trim();
            // Colapsa espacios internos: "Sony   Corp" -> "Sony Corp"
            input = Regex.Replace(input, @"\s{2,}", " ");
            return input;
        }

        /// <summary>
        /// Devuelve true si existe otra marca con la misma descripción (case-insensitive).
        /// Excluye el Id indicado (útil en edición).
        /// </summary>
        public bool ExisteDescripcion(string descripcionNormalizada, int idExcluir = 0)
        {
            var datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(
                    "SELECT COUNT(1) AS Cnt " +
                    "FROM MARCAS " +
                    "WHERE UPPER(LTRIM(RTRIM(Descripcion))) = UPPER(@desc) AND Id <> @idExcl"
                );
                datos.setearParametro("@desc", descripcionNormalizada);
                datos.setearParametro("@idExcl", idExcluir);
                datos.ejecutarLectura();

                if (datos.Lector.Read())
                    return Convert.ToInt32(datos.Lector["Cnt"]) > 0;

                return false;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        // -------------------- CRUD --------------------

        public List<Marca> listar()
        {
            var lista = new List<Marca>();
            var datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("SELECT Id, Descripcion FROM MARCAS");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    var aux = new Marca
                    {
                        Id = (int)datos.Lector["Id"],
                        Descripcion = (string)datos.Lector["Descripcion"]
                    };
                    lista.Add(aux);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error listando marcas.", ex);
            }
            finally
            {
                datos.cerrarConexion();
            }

            return lista;
        }

        public void agregar(Marca nueva)
        {
            var datos = new AccesoDatos();
            try
            {
                // Validaciones de negocio
                string desc = NormalizarDescripcion(nueva?.Descripcion);

                if (string.IsNullOrWhiteSpace(desc))
                    throw new BusinessRuleException("La descripción no puede estar vacía.");

                if (desc.Length < 2)
                    throw new BusinessRuleException("La descripción debe tener al menos 2 caracteres.");

                if (ExisteDescripcion(desc))
                    throw new BusinessRuleException("Ya existe una marca con esa descripción.");

                datos.setearConsulta("INSERT INTO MARCAS (Descripcion) VALUES (@Descripcion)");
                datos.setearParametro("@Descripcion", desc);
                datos.ejecutarAccion();
            }
            catch (BusinessRuleException) { throw; }
            catch (SqlException ex)
            {
                throw new ApplicationException("Error de base al agregar la marca.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al agregar la marca.", ex);
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void modificar(Marca marca)
        {
            var datos = new AccesoDatos();
            try
            {
                if (marca == null || marca.Id == 0)
                    throw new BusinessRuleException("Marca inválida para modificar.");

                string desc = NormalizarDescripcion(marca.Descripcion);

                if (string.IsNullOrWhiteSpace(desc))
                    throw new BusinessRuleException("La descripción no puede estar vacía.");

                if (desc.Length < 2)
                    throw new BusinessRuleException("La descripción debe tener al menos 2 caracteres.");

                if (ExisteDescripcion(desc, marca.Id))
                    throw new BusinessRuleException("Ya existe otra marca con esa descripción.");

                datos.setearConsulta("UPDATE MARCAS SET Descripcion = @Descripcion WHERE Id = @Id");
                datos.setearParametro("@Descripcion", desc);
                datos.setearParametro("@Id", marca.Id);
                datos.ejecutarAccion();
            }
            catch (BusinessRuleException) { throw; }
            catch (SqlException ex)
            {
                throw new ApplicationException("Error de base al modificar la marca.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al modificar la marca.", ex);
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        private bool TieneArticulosAsociados(int idMarca)
        {
            var datos = new AccesoDatos();
            try
            {
                // NO usamos columna 'Eliminado' (no existe en tu modelo)
                datos.setearConsulta("SELECT TOP 1 1 FROM ARTICULOS WHERE IdMarca = @IdMarca");
                datos.setearParametro("@IdMarca", idMarca);
                datos.ejecutarLectura();
                return datos.Lector.Read();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        private bool ExisteMarca(int id)
        {
            var datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT TOP 1 1 FROM MARCAS WHERE Id = @Id");
                datos.setearParametro("@Id", id);
                datos.ejecutarLectura();
                return datos.Lector.Read();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void eliminar(int id)
        {
            var datos = new AccesoDatos();
            try
            {
                if (!ExisteMarca(id))
                    throw new BusinessRuleException("La marca no existe o ya fue eliminada.");

                if (TieneArticulosAsociados(id))
                    throw new BusinessRuleException("No se puede eliminar la marca: tiene artículos asociados.");

                datos.setearConsulta("DELETE FROM MARCAS WHERE Id = @Id");
                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();
            }
            catch (BusinessRuleException) { throw; }
            catch (SqlException ex) when (ex.Number == 547)
            {
                // Por si hay FK en la BD
                throw new BusinessRuleException("No se puede eliminar la marca: tiene artículos asociados.");
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Error de base al eliminar la marca.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al eliminar la marca.", ex);
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}
