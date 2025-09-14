using System;
using System.Collections.Generic;
using dominio;

namespace Negocio
{
    public class CategoriaNegocio
    {
        public List<Categoria> listar()
        {
            List<Categoria> lista = new List<Categoria>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("select Id, Descripcion from CATEGORIAS");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Categoria aux = new Categoria();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    lista.Add(aux);
                }

                return lista;
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        public void agregar(Categoria nuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("INSERT INTO CATEGORIAS (Descripcion) VALUES (@Descripcion)");
                datos.setearParametro("@Descripcion", nuevo.Descripcion);
                datos.ejecutarAccion();
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        public void modificar(Categoria nuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("UPDATE CATEGORIAS SET Descripcion = @Descripcion WHERE Id = @id");
                datos.setearParametro("@Descripcion", nuevo.Descripcion);
                datos.setearParametro("@id", nuevo.Id);
                datos.ejecutarAccion();
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        // Eliminación física (y bloqueo si está en uso por algún artículo)
        public void eliminarFisico(int id)
        {
            // 1) verificar uso
            int cantidad = 0;
            AccesoDatos verifica = new AccesoDatos();
            try
            {
                verifica.setearConsulta("SELECT COUNT(*) FROM ARTICULOS WHERE IdCategoria = @id");
                verifica.setearParametro("@id", id);
                verifica.ejecutarLectura();
                if (verifica.Lector.Read())
                    cantidad = verifica.Lector.GetInt32(0);
            }
            catch (Exception ex) { throw ex; }
            finally { verifica.cerrarConexion(); }

            if (cantidad > 0)
                throw new Exception("No se puede eliminar la categoría porque tiene artículos asociados.");

            // 2) borrar
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("DELETE FROM CATEGORIAS WHERE Id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex) { throw ex; }
            finally { datos.cerrarConexion(); }
        }

        // Alias opcional si en algún lado llaman a "eliminar"
        public void eliminar(int id)
        {
            eliminarFisico(id);
        }
    }
}
