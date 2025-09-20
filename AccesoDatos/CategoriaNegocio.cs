using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using dominio;

namespace Negocio
{
    public class CategoriaNegocio
    {
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
                datos.setearConsulta("INSERT INTO CATEGORIAS (Descripcion) VALUES (@Descripcion)");
                datos.setearParametro("@Descripcion", nuevo.Descripcion);
                datos.ejecutarAccion();
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
                datos.setearConsulta("UPDATE CATEGORIAS SET Descripcion = @Descripcion WHERE Id = @Id");
                datos.setearParametro("@Descripcion", nuevo.Descripcion);
                datos.setearParametro("@Id", nuevo.Id);
                datos.ejecutarAccion();
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

        // --- Chequeo previo (sin tocar la BD): ¿hay artículos que usan esta categoría? ---
        private bool TieneArticulosAsociados(int idCategoria)
        {
            var datos = new AccesoDatos();
            try
            {
                // Si tu tabla ARTICULOS NO tiene la columna 'Eliminado', quitá "AND (Eliminado = 0 OR Eliminado IS NULL)".
                datos.setearConsulta(
                    "SELECT TOP 1 1 FROM ARTICULOS WHERE IdCategoria = @IdCategoria AND (Eliminado = 0 OR Eliminado IS NULL)"
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
                // 1) Reglas de negocio (sin tocar la estructura de la BD)
                if (!ExisteCategoria(id))
                    throw new BusinessRuleException("La categoría no existe o ya fue eliminada.");

                if (TieneArticulosAsociados(id))
                    throw new BusinessRuleException("No se puede eliminar la Categoría: tiene artículos asociados.");

                // 2) Borrado físico
                datos.setearConsulta("DELETE FROM CATEGORIAS WHERE Id = @Id");
                datos.setearParametro("@Id", id);
                datos.ejecutarAccion(); // ejecutarAccion() es void en tu AccesoDatos
            }
            catch (BusinessRuleException)
            {
                // Mensaje “amigable” para la UI
                throw;
            }
            catch (SqlException ex) when (ex.Number == 547) // por si hay FK en la BD
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
