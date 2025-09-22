using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using dominio;

namespace Negocio
{
    public class CategoriaNegocio
    {
        // -------------------- Helpers de normalización/validación --------------------

        /// <summary>
        /// Recorta extremos y colapsa múltiples espacios internos a uno solo.
        /// </summary>
        private static string NormalizarDescripcion(string s)
        {
            var input = (s ?? string.Empty).Trim();
            // "Nombre   con   muchos" -> "Nombre con muchos"
            input = Regex.Replace(input, @"\s{2,}", " ");
            return input;
        }

        /// <summary>
        /// Devuelve true si YA existe otra categoría con esa descripción (case-insensitive).
        /// Excluye el Id indicado (útil en edición).
        /// </summary>
        public bool ExisteDescripcion(string descripcionNormalizada, int idExcluir = 0)
        {
            var datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(
                    "SELECT COUNT(1) AS Cnt " +
                    "FROM CATEGORIAS " +
                    "WHERE UPPER(LTRIM(RTRIM(Descripcion))) = UPPER(@desc) AND Id <> @idExcl"
                );
                datos.setearParametro("@desc", descripcionNormalizada);
                datos.setearParametro("@idExcl", idExcluir);

                datos.ejecutarLectura();
                if (datos.Lector.Read())
                {
                    int count = Convert.ToInt32(datos.Lector["Cnt"]);
                    return count > 0;
                }
                return false;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        // -------------------- CRUD --------------------

        public List<Categoria> listar()
        {
            var lista = new List<Categoria>();
            var datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("SELECT Id, Descripcion FROM CATEGORIAS");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    var aux = new Categoria
                    {
                        Id = (int)datos.Lector["Id"],
                        Descripcion = (string)datos.Lector["Descripcion"]
                    };
                    lista.Add(aux);
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error listando categorías.", ex);
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void agregar(Categoria nuevo)
        {
            var datos = new AccesoDatos();
            try
            {
                // ---- Validaciones de negocio ----
                string desc = NormalizarDescripcion(nuevo?.Descripcion);

                if (string.IsNullOrWhiteSpace(desc))
                    throw new BusinessRuleException("La descripción no puede estar vacía.");

                if (desc.Length < 2)
                    throw new BusinessRuleException("La descripción debe tener al menos 2 caracteres.");

                if (ExisteDescripcion(desc))
                    throw new BusinessRuleException("Ya existe una categoría con esa descripción.");

                // ---- Insert ----
                datos.setearConsulta("INSERT INTO CATEGORIAS (Descripcion) VALUES (@Descripcion)");
                datos.setearParametro("@Descripcion", desc);
                datos.ejecutarAccion();
            }
            catch (BusinessRuleException)
            {
                throw;
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Error de base al agregar la categoría.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al agregar la categoría.", ex);
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void modificar(Categoria nuevo)
        {
            var datos = new AccesoDatos();
            try
            {
                if (nuevo == null || nuevo.Id == 0)
                    throw new BusinessRuleException("Categoría inválida para modificar.");

                // ---- Validaciones de negocio ----
                string desc = NormalizarDescripcion(nuevo.Descripcion);

                if (string.IsNullOrWhiteSpace(desc))
                    throw new BusinessRuleException("La descripción no puede estar vacía.");

                if (desc.Length < 2)
                    throw new BusinessRuleException("La descripción debe tener al menos 2 caracteres.");

                if (ExisteDescripcion(desc, nuevo.Id))
                    throw new BusinessRuleException("Ya existe otra categoría con esa descripción.");

                // ---- Update ----
                datos.setearConsulta("UPDATE CATEGORIAS SET Descripcion = @Descripcion WHERE Id = @Id");
                datos.setearParametro("@Descripcion", desc);
                datos.setearParametro("@Id", nuevo.Id);
                datos.ejecutarAccion();
            }
            catch (BusinessRuleException)
            {
                throw;
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Error de base al modificar la categoría.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al modificar la categoría.", ex);
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        // --- Chequeo previo: ¿hay artículos que usan esta categoría? ---
        // Versión que NO depende de columna 'Eliminado'.
        private bool TieneArticulosAsociados(int idCategoria)
        {
            var datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(
                    "SELECT TOP 1 1 FROM ARTICULOS WHERE IdCategoria = @IdCategoria"
                );
                datos.setearParametro("@IdCategoria", idCategoria);
                datos.ejecutarLectura();
                return datos.Lector.Read();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        // --- Chequeo de existencia (para dar buen mensaje si ya no existe) ---
        private bool ExisteCategoria(int id)
        {
            var datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT TOP 1 1 FROM CATEGORIAS WHERE Id = @Id");
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
                // 1) Reglas de negocio
                if (!ExisteCategoria(id))
                    throw new BusinessRuleException("La categoría no existe o ya fue eliminada.");

                if (TieneArticulosAsociados(id))
                    throw new BusinessRuleException("No se puede eliminar la Categoría: tiene artículos asociados.");

                // 2) Borrado físico
                datos.setearConsulta("DELETE FROM CATEGORIAS WHERE Id = @Id");
                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();
            }
            catch (BusinessRuleException)
            {
                throw;
            }
            catch (SqlException ex) when (ex.Number == 547) // FK en BD
            {
                throw new BusinessRuleException("No se puede eliminar la Categoría: tiene artículos asociados.");
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Error de base al eliminar la categoría.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al eliminar la categoría.", ex);
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}
