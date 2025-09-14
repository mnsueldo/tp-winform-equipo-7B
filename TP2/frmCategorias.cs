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
    public partial class frmCategorias : Form
    {
        private List<Categoria> listaCategoria;
        public frmCategorias()
        {
            InitializeComponent();
        }
       
        private void frmCategorias_Load(object sender, EventArgs e)
        {
            cargar();
        }

        private void cargar()
        {
            try
            {
                CategoriaNegocio negocio = new CategoriaNegocio();
                listaCategoria = negocio.listar();
                dgvCategorias.DataSource = listaCategoria;
                dgvCategorias.Columns["Id"].Visible = false;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }

        }

        private void btnAgregarCategoria_Click(object sender, EventArgs e)
        {
            frmAgregarCategoria agregar = new frmAgregarCategoria();
            agregar.ShowDialog();
            cargar();
        }

        private void btnModificarCategoria_Click(object sender, EventArgs e)
        {
            Categoria seleccionado;
            seleccionado = (Categoria)dgvCategorias.CurrentRow.DataBoundItem;

            frmAgregarCategoria modificar = new frmAgregarCategoria(seleccionado);
            modificar.ShowDialog();
            cargar();
        }

        private void btnEliminarCategoria_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvCategorias.CurrentRow == null)
                    return;

                Categoria seleccionado = (Categoria)dgvCategorias.CurrentRow.DataBoundItem;

                var resp = MessageBox.Show(
                    "¿Eliminar físicamente la categoria seleccionado?",
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (resp == DialogResult.Yes)
                {
                    CategoriaNegocio negocio = new CategoriaNegocio();
                    negocio.eliminar(seleccionado.Id);
                    cargar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
