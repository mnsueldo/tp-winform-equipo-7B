using Negocio;
using System;
using System.Windows.Forms;
using dominio;

namespace TP2
{
    public partial class frmAgregarCategoria : Form
    {
        private Categoria categoria = null;

        public frmAgregarCategoria()
        {
            InitializeComponent();
        }

        public frmAgregarCategoria(Categoria categoria)
        {
            this.categoria = categoria;
            InitializeComponent();
            Text = "Modificar Categoría";
        }

        private void frmAgregarCategoria_Load(object sender, EventArgs e)
        {
            if (categoria != null)
                txtDescripcionCategoria.Text = categoria.Descripcion;   // <- acá
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            CategoriaNegocio negocio = new CategoriaNegocio();
            try
            {
                if (categoria == null)
                    categoria = new Categoria();

                categoria.Descripcion = txtDescripcionCategoria.Text;   // <- y acá

                if (categoria.Id != 0)
                {
                    negocio.modificar(categoria);
                    MessageBox.Show("Categoría modificada correctamente");
                }
                else
                {
                    negocio.agregar(categoria);
                    MessageBox.Show("Categoría agregada correctamente");
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
