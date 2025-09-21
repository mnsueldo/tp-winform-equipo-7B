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
            var lista = new List<Articulo>();
            var datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(@"
SELECT  A.Id, A.Codigo, A.Nombre, A.Descripcion, 
        A.IdMarca, A.IdCategoria, A.Precio,
        M.Descripcion AS Marca,
        C.Descripcion AS Categoria
FROM ARTICULOS A
INNER JOIN MARCAS M     ON A.IdMarca = M.Id
INNER JOIN CATEGORIAS C ON A.IdCategoria = C.Id
");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    var aux = new Articulo
                    {
                        Id = (int)datos.Lector["Id"],
                        Codigo = (string)datos.Lector["Codigo"],
                        Nombre = (string)datos.Lector["Nombre"],
                        Descripcion = (string)datos.Lector["Descripcion"],
                        Marca = new Marca
                        {
                            Id = (int)datos.Lector["IdMarca"],
                            Descripcion = (string)datos.Lector["Marca"]
                        },
                        Categoria = new Categoria
                        {
                            Id = (int)datos.Lector["IdCategoria"],
                            Descripcion = (string)datos.Lector["Categoria"]
                        }
                    };

                    if (!datos.Lector.IsDBNull(datos.Lector.GetOrdinal("Precio")))
                        aux.Precio = (decimal)datos.Lector["Precio"];

                    // Carga de imágenes (si existen)
                    aux.UrlImagen = ObtenerPrimeraImagenValida(aux.Id);
                    aux.Imagenes = ObtenerImagenesPorId(aux.Id);

                    lista.Add(aux);
                }

                return lista;
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Error de base al listar artículos.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al listar artículos.", ex);
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public int agregar(Articulo nuevo)
        {
            var datos = new AccesoDatos();
            int idGenerado = 0;

            try
            {
                // Obtenemos el Id con OUTPUT INSERTED.Id y lo leemos con ejecutarLectura()
                datos.setearConsulta(@"
INSERT INTO ARTICULOS (Codigo, Nombre, Descripcion, IdCategoria, IdMarca, Precio)
OUTPUT INSERTED.Id
VALUES (@Codigo, @Nombre, @Descripcion, @IdCategoria, @IdMarca, @Precio);
");
                datos.setearParametro("@Codigo", nuevo.Codigo);
                datos.setearParametro("@Nombre", nuevo.Nombre);
                datos.setearParametro("@Descripcion", nuevo.Descripcion);
                datos.setearParametro("@IdCategoria", nuevo.Categoria.Id);
                datos.setearParametro("@IdMarca", nuevo.Marca.Id);
                datos.setearParametro("@Precio", nuevo.Precio);

                datos.ejecutarLectura();
                if (datos.Lector.Read())
                    idGenerado = Convert.ToInt32(datos.Lector[0]);

                return idGenerado;
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Error de base al agregar el artículo.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al agregar el artículo.", ex);
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void modificar(Articulo nuevo)
        {
            var datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"
UPDATE ARTICULOS
SET Codigo = @Codigo,
    Nombre = @Nombre,
    Descripcion = @Descripcion,
    IdCategoria = @IdCategoria,
    IdMarca = @IdMarca,
    Precio = @Precio
WHERE Id = @Id;
");
                datos.setearParametro("@Codigo", nuevo.Codigo);
                datos.setearParametro("@Nombre", nuevo.Nombre);
                datos.setearParametro("@Descripcion", nuevo.Descripcion);
                datos.setearParametro("@IdCategoria", nuevo.Categoria.Id);
                datos.setearParametro("@IdMarca", nuevo.Marca.Id);
                datos.setearParametro("@Precio", nuevo.Precio);
                datos.setearParametro("@Id", nuevo.Id);

                datos.ejecutarAccion();
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Error de base al modificar el artículo.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al modificar el artículo.", ex);
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void eliminarFisico(int id)
        {
            var datos = new AccesoDatos();
            try
            {
                // Borramos primero imágenes relacionadas y luego el artículo
                datos.setearConsulta(@"
DELETE FROM IMAGENES WHERE IdArticulo = @Id;
DELETE FROM ARTICULOS WHERE Id = @Id;
");
                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Error de base al eliminar el artículo.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al eliminar el artículo.", ex);
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public List<Articulo> filtrar(string campo, string criterio, string filtro)
        {
            var lista = new List<Articulo>();
            var datos = new AccesoDatos();

            try
            {
                // WHERE 1=1 para poder concatenar ANDs
                string consulta = @"
SELECT A.Id, A.Codigo, A.Nombre, A.Descripcion, A.Precio,
       C.Descripcion AS Categoria, M.Descripcion AS Marca,
       A.IdCategoria, A.IdMarca
FROM ARTICULOS AS A
INNER JOIN CATEGORIAS AS C ON C.Id = A.IdCategoria
INNER JOIN MARCAS     AS M ON M.Id = A.IdMarca
LEFT JOIN  IMAGENES   AS I ON A.Id = I.IdArticulo
WHERE 1=1 ";

                if (campo == "Precio")
                {
                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += "AND A.Precio > @filtroPrecio ";
                            datos.setearParametro("@filtroPrecio", Convert.ToDecimal(filtro));
                            break;
                        case "Menor a":
                            consulta += "AND A.Precio < @filtroPrecio ";
                            datos.setearParametro("@filtroPrecio", Convert.ToDecimal(filtro));
                            break;
                        default: // "Igual a"
                            consulta += "AND A.Precio = @filtroPrecio ";
                            datos.setearParametro("@filtroPrecio", Convert.ToDecimal(filtro));
                            break;
                    }
                }
                else if (campo == "Nombre")
                {
                    consulta += "AND A.Nombre LIKE @filtro ";
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
                    consulta += "AND A.Codigo LIKE @filtro ";
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
                    var aux = new Articulo
                    {
                        Id = (int)datos.Lector["Id"],
                        Codigo = (string)datos.Lector["Codigo"],
                        Nombre = (string)datos.Lector["Nombre"],
                        Descripcion = (string)datos.Lector["Descripcion"],
                        Marca = new Marca
                        {
                            Id = (int)datos.Lector["IdMarca"],
                            Descripcion = (string)datos.Lector["Marca"]
                        },
                        Categoria = new Categoria
                        {
                            Id = (int)datos.Lector["IdCategoria"],
                            Descripcion = (string)datos.Lector["Categoria"]
                        }
                    };

                    if (!datos.Lector.IsDBNull(datos.Lector.GetOrdinal("Precio")))
                        aux.Precio = (decimal)datos.Lector["Precio"];

                    lista.Add(aux);
                }

                return lista;
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Error de base al filtrar artículos.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al filtrar artículos.", ex);
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public string ObtenerPrimeraImagenValida(int idArticulo)
        {
            var datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT TOP 1 ImagenUrl FROM IMAGENES WHERE IdArticulo = @idArticulo");
                datos.setearParametro("@idArticulo", idArticulo);
                datos.ejecutarLectura();

                if (datos.Lector.Read())
                    return (string)datos.Lector["ImagenUrl"];

                return null;
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Error de base al obtener la imagen del artículo.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al obtener la imagen del artículo.", ex);
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public List<string> ObtenerImagenesPorId(int idArticulo)
        {
            var imagenes = new List<string>();
            var datos = new AccesoDatos();
            try
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
            catch (SqlException ex)
            {
                throw new ApplicationException("Error de base al listar imágenes del artículo.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al listar imágenes del artículo.", ex);
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public static bool EsImagenValida(string url)
        {
            try
            {
                var request = System.Net.WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 3000; // 3s
                using (var response = request.GetResponse())
                {
                    return ((System.Net.HttpWebResponse)response).StatusCode
                           == System.Net.HttpStatusCode.OK;
                }
            }
            catch
            {
                return false;
            }
        }

        public void guardarImagenes(int idArticulo, List<string> imagenes)
        {
            var datos = new AccesoDatos();
            try
            {
                if (imagenes == null)
                    return;

                // Borrar existentes
                datos.setearConsulta("DELETE FROM IMAGENES WHERE IdArticulo = @idArticulo");
                datos.setearParametro("@idArticulo", idArticulo);
                datos.ejecutarAccion();

                // Insertar nuevas
                foreach (var url in imagenes)
                {
                    datos.setearConsulta("INSERT INTO IMAGENES (IdArticulo, ImagenUrl) VALUES (@idArticulo, @url)");
                    datos.setearParametro("@idArticulo", idArticulo);
                    datos.setearParametro("@url", url);
                    datos.ejecutarAccion();
                }
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Error de base al guardar imágenes del artículo.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al guardar imágenes del artículo.", ex);
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}
