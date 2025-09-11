using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dominio;
using System.Data.SqlClient;

namespace Negocio
{
    public class ArticuloNegocio
    {   
              
        public List<Articulo> listar()
        {
            List<Articulo> lista = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("Select Codigo, Nombre, A.Descripcion, Precio, C.Descripcion AS Categoria, M.Descripcion AS Marca From ARTICULOS as A, CATEGORIAS as C, MARCAS as M WHERE C.Id = A.IdCategoria AND M.Id = A.IdMarca");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    aux.Precio = (decimal)datos.Lector["Precio"];
                    aux.Categoria = new Categoria();
                    aux.Categoria.Descripcion = (string)datos.Lector["Categoria"];
                    aux.Marca = new Marca();
                    aux.Marca.Descripcion = (string)datos.Lector["Marca"];

                    lista.Add(aux);

                }

                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
        public void agregar(Articulo nuevo)
        {

        }
               
    }
         

     
}
