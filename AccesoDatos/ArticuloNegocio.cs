using dominio;
using System;
using System.Collections.Generic;
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
               
                datos.setearConsulta("SELECT A.Id, A.Codigo, A.Nombre, A.Descripcion, A.IdMarca, A.IdCategoria, A.Precio, M.Descripcion AS Marca, C.Descripcion AS Categoria FROM ARTICULOS A INNER JOIN MARCAS M ON A.IdMarca = M.Id INNER JOIN CATEGORIAS C ON A.IdCategoria = C.Id");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];

                    if (!(datos.Lector.IsDBNull(datos.Lector.GetOrdinal("Precio"))))
                        aux.Precio = (decimal)datos.Lector["Precio"];

                    aux.Categoria = new Categoria();
                    aux.Categoria.Id = (int)datos.Lector["IdCategoria"];
                    aux.Categoria.Descripcion = (string)datos.Lector["Categoria"];

                    aux.Marca = new Marca();
                    aux.Marca.Id = (int)datos.Lector["IdMarca"];
                    aux.Marca.Descripcion = (string)datos.Lector["Marca"];

                    aux.UrlImagen = ObtenerPrimeraImagenValida(aux.Id);                    
                    aux.Imagenes = ObtenerImagenesPorId(aux.Id);

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
        
        public int agregar(Articulo nuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            Imagen imagen = new Imagen();
            int idGenerado = 0;

            try
            {
                datos.setearConsulta("INSERT into ARTICULOS (Codigo, Nombre, Descripcion,IdCategoria,IdMarca,Precio) OUTPUT INSERTED.Id values(@Codigo, @Nombre, @Descripcion,@IdCategoria,@IdMarca,@Precio)");
                datos.setearParametro("@Codigo", nuevo.Codigo);
                datos.setearParametro("@Nombre", nuevo.Nombre);
                datos.setearParametro("@Descripcion", nuevo.Descripcion);                
                datos.setearParametro("@IdCategoria", nuevo.Categoria.Id);
                datos.setearParametro("@IdMarca", nuevo.Marca.Id);
                datos.setearParametro("@Precio", nuevo.Precio);
                
                datos.ejecutarExcalar(idGenerado);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }

            return idGenerado;
        }
       
        public void modificar(Articulo nuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("update ARTICULOS set Codigo = @codigo, Nombre = @nombre, Descripcion = @descripcion, IdCategoria = @idCategoria, IdMarca = @idMarca, Precio = @precio Where Id = @id");
                datos.setearParametro("@Codigo", nuevo.Codigo);
                datos.setearParametro("@Nombre", nuevo.Nombre);
                datos.setearParametro("@Descripcion", nuevo.Descripcion);
                datos.setearParametro("@IdCategoria", nuevo.Categoria.Id);
                datos.setearParametro("@IdMarca", nuevo.Marca.Id);
                datos.setearParametro("@Precio", nuevo.Precio);
                datos.setearParametro("@id", nuevo.Id);

                datos.ejecutarAccion();
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

        public void eliminarFisico(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {

                datos.setearConsulta(
                    "DELETE FROM IMAGENES WHERE IdArticulo = @id; " +
                    "DELETE FROM ARTICULOS WHERE Id = @id;"
                );
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
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

        public List<Articulo> filtrar(string campo, string criterio, string filtro)
        {
            List<Articulo> lista = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();

            try
            {

                string consulta = "SELECT A.Id, Codigo, Nombre, A.Descripcion, Precio, C.Descripcion Categoria, M.Descripcion Marca, A.IdCategoria, A.IdMarca, I.Id AS IdImagen, I.ImagenUrl, M.Id, C.Id FROM ARTICULOS AS A INNER JOIN CATEGORIAS AS C ON C.Id = A.IdCategoria INNER JOIN MARCAS AS M ON M.Id = A.IdMarca LEFT JOIN IMAGENES AS I ON A.Id = I.IdArticulo";
                    
                
                if (campo == "Precio")
                {
                    
                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += "AND Precio > @filtroPrecio ";
                            datos.setearParametro("@filtroPrecio", Convert.ToDecimal(filtro));
                            break;
                        case "Menor a":
                            consulta += "AND Precio < @filtroPrecio ";
                            datos.setearParametro("@filtroPrecio", Convert.ToDecimal(filtro));
                            break;
                        default: // "Igual a"
                            consulta += "AND Precio = @filtroPrecio ";
                            datos.setearParametro("@filtroPrecio", Convert.ToDecimal(filtro));
                            break;
                    }
                }
                else if (campo == "Nombre")
                {
                    consulta += "AND Nombre LIKE @filtro ";
                    switch (criterio)
                    {
                        case "Comienza con":
                            datos.setearParametro("@filtro", filtro + "%");
                            break;
                        case "Termina con":
                            datos.setearParametro("@filtro", "%" + filtro);
                            break;
                        default: 
                            datos.setearParametro("@filtro", "%" + filtro + "%");
                            break;
                    }
                }
                else if (campo == "Descripcion" || campo == "Descripción")
                {
                    consulta += "AND A.Descripcion LIKE @filtro ";
                    switch (criterio)
                    {
                        case "Comienza con":
                            datos.setearParametro("@filtro", filtro + "%");
                            break;
                        case "Termina con":
                            datos.setearParametro("@filtro", "%" + filtro);
                            break;
                        default:
                            datos.setearParametro("@filtro", "%" + filtro + "%");
                            break;
                    }
                }
                else if (campo == "Marca")
                {
                    consulta += "AND M.Descripcion LIKE @filtro ";
                    switch (criterio)
                    {
                        case "Comienza con":
                            datos.setearParametro("@filtro", filtro + "%");
                            break;
                        case "Termina con":
                            datos.setearParametro("@filtro", "%" + filtro);
                            break;
                        default:
                            datos.setearParametro("@filtro", "%" + filtro + "%");
                            break;
                    }
                }
                else if (campo == "Categoria" || campo == "Categoría")
                {
                    consulta += "AND C.Descripcion LIKE @filtro ";
                    switch (criterio)
                    {
                        case "Comienza con":
                            datos.setearParametro("@filtro", filtro + "%");
                            break;
                        case "Termina con":
                            datos.setearParametro("@filtro", "%" + filtro);
                            break;
                        default:
                            datos.setearParametro("@filtro", "%" + filtro + "%");
                            break;
                    }
                }
                else if (campo == "Codigo" || campo == "Código")
                {
                    consulta += "AND Codigo LIKE @filtro ";
                    switch (criterio)
                    {
                        case "Comienza con":
                            datos.setearParametro("@filtro", filtro + "%");
                            break;
                        case "Termina con":
                            datos.setearParametro("@filtro", "%" + filtro);
                            break;
                        default:
                            datos.setearParametro("@filtro", "%" + filtro + "%");
                            break;
                    }
                }
                
                datos.setearConsulta(consulta);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Codigo = (string)datos.Lector["Codigo"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];

                    if (!datos.Lector.IsDBNull(datos.Lector.GetOrdinal("Precio")))
                        aux.Precio = (decimal)datos.Lector["Precio"];

                    aux.Categoria = new Categoria();
                    aux.Categoria.Id = (int)datos.Lector["IdCategoria"];
                    aux.Categoria.Descripcion = (string)datos.Lector["Categoria"];
                    aux.Marca = new Marca();
                    aux.Marca.Id = (int)datos.Lector["IdMarca"];
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

        public string ObtenerPrimeraImagenValida(int idArticulo)
        {
            
            AccesoDatos datos = new AccesoDatos();
            {
                datos.setearConsulta("SELECT TOP 1 ImagenUrl FROM IMAGENES WHERE IdArticulo = @idArticulo");
                
                datos.setearParametro("@idArticulo", idArticulo);
                datos.ejecutarLectura();
                if (datos.Lector.Read())
                {
                    return (string)datos.Lector["ImagenUrl"];
                }
            }
            return null;
        }
        public List<string> ObtenerImagenesPorId(int idArticulo)
        {
            var imagenes = new List<string>();
            AccesoDatos datos = new AccesoDatos();
            {
                datos.setearConsulta("SELECT ImagenUrl FROM IMAGENES WHERE IdArticulo = @idArticulo");
                
                datos.setearParametro("@idArticulo", idArticulo);
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    imagenes.Add((string)datos.Lector["ImagenUrl"]);
                }
            return imagenes;
            }
        }

        public static bool EsImagenValida(string url)
        {
            try
            {
                // Se crea una solicitud HTTP para la URL indicada.
                // WebRequest.Create genera un objeto que permite interactuar con el recurso remoto.
                var request = System.Net.WebRequest.Create(url);

                // Se especifica el método HTTP a usar, en este caso GET, que es el estándar para obtener recursos.
                request.Method = "GET";

                // Se establece un tiempo máximo de espera (timeout) de 3 segundos para evitar que la aplicación se bloquee si la URL no responde.
                request.Timeout = 3000; // 3 segundos

                // Se envía la solicitud y se obtiene la respuesta del servidor.
                // Si la URL es válida y el recurso está disponible, se recibe una respuesta.
                using (var response = request.GetResponse())
                {
                    // Se verifica el código de estado HTTP de la respuesta.
                    // Si el código es OK (200), significa que la imagen existe y es accesible.
                    return ((System.Net.HttpWebResponse)response).StatusCode == System.Net.HttpStatusCode.OK;
                }
            }
            catch
            {
                // Si ocurre cualquier excepción (por ejemplo, la URL no existe, no hay conexión, o el recurso no está disponible),
                // el método retorna false indicando que la imagen no es válida o no se pudo acceder.
                return false;
            }
            // este metodo lo saque de https://stackoverflow.com/questions/11082804/detecting-image-url-in-c-net por si lo tengo que volver a buscar
        }

        public void guardarImagenes(int idArticulo, List<string> imagenes)
        {
            AccesoDatos datos = new AccesoDatos();
            {
                try
                {
                    if (imagenes == null)
                    return;           
                       
                    datos.setearConsulta("DELETE FROM IMAGENES WHERE IdArticulo = @idArticulo");
                    datos.setearParametro("@idArticulo", idArticulo);
                    datos.ejecutarAccion();


                    foreach (var url in imagenes)
                    {
                        datos.setearConsulta("INSERT INTO IMAGENES (IdArticulo, ImagenUrl) VALUES (@idArticulo, @url)");
                        datos.setearParametro("@idArticulo", idArticulo);
                        datos.setearParametro("@url", url);
                        datos.ejecutarAccion(); 
                    }

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
        }
        


    }
}
  

