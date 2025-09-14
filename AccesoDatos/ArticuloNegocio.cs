using System;
using System.Collections.Generic;
using dominio;

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
                datos.setearConsulta("Select A.Id,Codigo, Nombre, A.Descripcion, Precio, C.Descripcion Categoria, M.Descripcion Marca, A.IdCategoria, A.IdMarca, I.ImagenUrl From ARTICULOS as A, CATEGORIAS as C, MARCAS as M, IMAGENES AS I WHERE C.Id = A.IdCategoria AND M.Id = A.IdMarca AND A.Id = I.IdArticulo");
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

                    if (!(datos.Lector.IsDBNull(datos.Lector.GetOrdinal("ImagenUrl"))))
                    {
                        Imagen img = new Imagen
                        {
                            ImagenUrl = (string)datos.Lector["ImagenUrl"]
                        };
                        aux.Imagenes.Add(img);
                    }


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
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("INSERT into ARTICULOS (Codigo, Nombre, Descripcion,IdCategoria,IdMarca,Precio)values(@Codigo, @Nombre, @Descripcion,@IdCategoria,@IdMarca,@Precio)");
                datos.setearParametro("@Codigo", nuevo.Codigo);
                datos.setearParametro("@Nombre", nuevo.Nombre);
                datos.setearParametro("@Descripcion", nuevo.Descripcion);
                datos.setearParametro("@IdCategoria", nuevo.Categoria.Id);
                datos.setearParametro("@IdMarca", nuevo.Marca.Id);
                datos.setearParametro("@Precio", nuevo.Precio);

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
                // Base igual a tu listar, incluyendo la unión con IMAGENES
                string consulta =
                    "SELECT A.Id, Codigo, Nombre, A.Descripcion, Precio, " +
                    "       C.Descripcion Categoria, M.Descripcion Marca, " +
                    "       A.IdCategoria, A.IdMarca, I.ImagenUrl " +
                    "FROM ARTICULOS AS A, CATEGORIAS AS C, MARCAS AS M, IMAGENES AS I " +
                    "WHERE C.Id = A.IdCategoria AND M.Id = A.IdMarca AND A.Id = I.IdArticulo ";

                // Agrego el filtro según el campo/criterio
                if (campo == "Precio")
                {
                    // Para precio uso comparadores numéricos
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
                        default: // "Contiene"
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
                // Si el campo no coincide con ninguno, no agrego filtro extra.

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

                    if (!datos.Lector.IsDBNull(datos.Lector.GetOrdinal("ImagenUrl")))
                    {
                        Imagen img = new Imagen { ImagenUrl = (string)datos.Lector["ImagenUrl"] };
                        aux.Imagenes.Add(img);
                    }

                    lista.Add(aux);
                }

                return lista; // <-- asegura que siempre devuelve lista
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
  

