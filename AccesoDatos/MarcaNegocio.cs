using dominio;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Negocio
{
    public class MarcaNegocio
    {
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
                // Mensaje técnico encapsulado (la UI muestra algo amable)
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
                datos.setearConsulta("INSERT INTO MARCAS (Descripcion) VALUES (@Descripcion)");
                datos.setearParametro("@Descripcion", nueva.Descripcion);
                datos.ejecutarAccion();
            }
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
                datos.setearConsulta("UPDATE MARCAS SET Descripcion = @Descripcion WHERE Id = @Id");
                datos.setearParametro("@Descripcion", marca.Descripcion);
                datos.setearParametro("@Id", marca.Id);
                datos.ejecutarAccion();
            }
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

        // --- NUEVO: chequeo previo sin tocar la BD (no requiere FK) ---
        private bool TieneArticulosAsociados(int idMarca)
        {
            var datos = new AccesoDatos();
            try
            {
                // Si tu tabla Articulos no tiene 'Eliminado', quitá esa parte
                datos.setearConsulta(
                    "SELECT TOP 1 1 FROM ARTICULOS WHERE IdMarca = @IdMarca AND (Eliminado = 0 OR Eliminado IS NULL)"
                );
                datos.setearParametro("@IdMarca", idMarca);
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
                // Regla de negocio: NO permitir borrar si hay artículos asociados
                if (TieneArticulosAsociados(id))
                    throw new BusinessRuleException("No se puede eliminar la Marca: tiene artículos asociados.");

                datos.setearConsulta("DELETE FROM MARCAS WHERE Id = @Id");
                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();
            }
            catch (BusinessRuleException) // ya viene con mensaje claro para la UI
            {
                throw;
            }
            catch (SqlException ex) when (ex.Number == 547) // por si existen FKs aunque no podamos tocarlos
            {
                // traducimos a mensaje entendible
                throw new BusinessRuleException("No se puede eliminar la Marca: tiene artículos asociados.");
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
