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
using dominio;
using AccesoDatos;


namespace TP2
{
    public partial class frmArticulos : Form
    {
        private List<Articulo> listaArticulo;
        private List<Categoria> listaCategoria;
        private List<Marca> listaMarca;
        public frmArticulos()
        {
            InitializeComponent();
        }

        private void frmArticulos_Load(object sender, EventArgs e)
        {
            cargar();
        }
        
        private void cargar()
        {
            ArticuloNegocio negocio = new ArticuloNegocio();
            try
            {
                listaArticulo = negocio.listar();
                dgvArticulos.DataSource = listaArticulo;
                dgvArticulos.Columns["Id"].Visible = false;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

            CategoriaNegocio negocio2 = new CategoriaNegocio();
            listaCategoria = negocio2.listar();
           

            MarcaNegocio negocio3 = new MarcaNegocio();
            listaMarca = negocio3.listar();
            
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAgregarArticulo agregar = new frmAgregarArticulo();
            agregar.ShowDialog();
        }
                
    }
}
