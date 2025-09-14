using dominio;
using Negocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            Text = "Modificar Categoria";
        }

        private void btnCancelarCategoria_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAceptarCategoria_Click(object sender, EventArgs e)
        {
            CategoriaNegocio categoriaNegocio = new CategoriaNegocio();

            try
            {
                if (categoria == null)
                    categoria = new Categoria();

                               
                categoria.Descripcion = txtDescripcionCategoria.Text;                

                if (categoria.Id != 0)
                {
                    categoriaNegocio.modificar(categoria);
                    MessageBox.Show("Categoria modificada correctamente");
                }
                else
                {
                    categoriaNegocio.agregar(categoria);
                    MessageBox.Show("Categoria agregada correctamente");
                }

                Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void frmAgregarCategoria_Load(object sender, EventArgs e)
        {
            CategoriaNegocio categoriaNegocio = new CategoriaNegocio();
            
            try
            {                
                if (categoria != null)
                {
                    
                    txtDescripcionCategoria.Text = categoria.Descripcion;
                    
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
