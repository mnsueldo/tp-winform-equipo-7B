using System;
using System.Drawing;
using System.Windows.Forms;
using dominio;

namespace TP2
{
    public partial class frmDetalleArticulo : Form
    {
        private readonly Articulo articulo;
        private int idxImagen = 0;

        public frmDetalleArticulo(Articulo articulo)
        {
            if (articulo == null) throw new ArgumentNullException(nameof(articulo));
            InitializeComponent();
            this.articulo = articulo;
            this.Text = "Detalle del artículo";
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void frmDetalleArticulo_Load(object sender, EventArgs e)
        {
            // Datos
            lblCodigoValor.Text = articulo.Codigo ?? "-";
            lblNombreValor.Text = articulo.Nombre ?? "-";
            lblDescripcionValor.Text = articulo.Descripcion ?? "-";
            lblMarcaValor.Text = articulo.Marca != null ? articulo.Marca.Descripcion : "-";
            lblCategoriaValor.Text = articulo.Categoria != null ? articulo.Categoria.Descripcion : "-";
            lblPrecioValor.Text = articulo.Precio.ToString("C");

            // Imágenes
            if (articulo.Imagenes != null && articulo.Imagenes.Count > 0)
            {
                idxImagen = 0;
                CargarImagenActual();
            }
            else
            {
                lblPaginador.Text = "Sin imágenes";
                pictureBox.Image = null;
                btnAnterior.Enabled = btnSiguiente.Enabled = false;
            }
        }

        private void CargarImagenActual()
        {
            if (articulo.Imagenes == null || articulo.Imagenes.Count == 0)
            {
                pictureBox.Image = null;
                lblPaginador.Text = "Sin imágenes";
                btnAnterior.Enabled = btnSiguiente.Enabled = false;
                return;
            }

            if (idxImagen < 0) idxImagen = articulo.Imagenes.Count - 1;
            if (idxImagen >= articulo.Imagenes.Count) idxImagen = 0;

            var url = articulo.Imagenes[idxImagen].ImagenUrl;

            try
            {
                pictureBox.ImageLocation = null; // reset
                pictureBox.Load(url);
            }
            catch
            {
                pictureBox.Image = null;
            }

            lblPaginador.Text = $"{idxImagen + 1} / {articulo.Imagenes.Count}";
            btnAnterior.Enabled = articulo.Imagenes.Count > 1;
            btnSiguiente.Enabled = articulo.Imagenes.Count > 1;
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            idxImagen--;
            CargarImagenActual();
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            idxImagen++;
            CargarImagenActual();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
