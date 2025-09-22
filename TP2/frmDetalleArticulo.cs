using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using dominio;
using Negocio;

namespace TP2
{
    public partial class frmDetalleArticulo : Form
    {
        private readonly Articulo _articulo;
        private List<string> _imgs = new List<string>(); 
        private int _idx = 0;

        public frmDetalleArticulo(Articulo art)
        {
            InitializeComponent();
            _articulo = art;
        }

        private void frmDetalleArticulo_Load(object sender, EventArgs e)
        {
            
            lblCodigoValor.Text = _articulo.Codigo;
            lblNombreValor.Text = _articulo.Nombre;
            lblDescripcionValor.Text = _articulo.Descripcion;
            lblMarcaValor.Text = _articulo.Marca != null ? _articulo.Marca.Descripcion : "-";
            lblCategoriaValor.Text = _articulo.Categoria != null ? _articulo.Categoria.Descripcion : "-";
            lblPrecioValor.Text = _articulo.Precio.ToString("C2");

            
            _imgs = (_articulo.Imagenes ?? new List<string>()).Where(u => !string.IsNullOrWhiteSpace(u)).ToList();
            if (_imgs.Count == 0)
            {
                var neg = new ArticuloNegocio();
                var desdeBd = neg.ObtenerImagenesPorId(_articulo.Id);
                if (desdeBd != null) _imgs = desdeBd;
            }

            _idx = 0;
            MostrarImagenActual();
        }

        private void MostrarImagenActual()
        {
            if (_imgs == null || _imgs.Count == 0)
            {
                try
                {
                    pictureBox.LoadAsync("https://efectocolibri.com/wp-content/uploads/2021/01/placeholder.png");
                }
                catch { /* ignore */ }
                lblPaginador.Text = "0 / 0";
                btnAnterior.Enabled = btnSiguiente.Enabled = false;
                return;
            }

            if (_idx < 0) _idx = 0;
            if (_idx >= _imgs.Count) _idx = _imgs.Count - 1;

            try
            {
                pictureBox.Image = null;
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox.LoadAsync(_imgs[_idx].Trim());
            }
            catch
            {
                pictureBox.LoadAsync("https://efectocolibri.com/wp-content/uploads/2021/01/placeholder.png");
            }

            lblPaginador.Text = string.Format("{0} / {1}", _idx + 1, _imgs.Count);
            bool habilita = _imgs.Count > 1;
            btnAnterior.Enabled = habilita;
            btnSiguiente.Enabled = habilita;
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (_imgs == null || _imgs.Count == 0) return;
            _idx = (_idx - 1 + _imgs.Count) % _imgs.Count; 
            MostrarImagenActual();
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (_imgs == null || _imgs.Count == 0) return;
            _idx = (_idx + 1) % _imgs.Count; 
            MostrarImagenActual();
        }

        private void btnCerrar_Click(object sender, EventArgs e) => Close();
    }
}
