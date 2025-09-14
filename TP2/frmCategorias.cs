using Negocio;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using dominio;

namespace TP2
{
    public partial class frmCategorias : Form
    {
        private List<Categoria> lista;

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
            CategoriaNegocio negocio = new CategoriaNegocio();
            try
            {
                lista = negocio.listar();
                dgvCategorias.DataSource = lista;
                dgvCategorias.Columns["Id"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAgregarCategoria f = new frmAgregarCategoria();
            f.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvCategorias.CurrentRow == null) return;

            Categoria seleccionada = (Categoria)dgvCategorias.CurrentRow.DataBoundItem;
            frmAgregarCategoria f = new frmAgregarCategoria(seleccionada);
            f.ShowDialog();
            cargar();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvCategorias.CurrentRow == null)
                    return;

                Categoria seleccionada = (Categoria)dgvCategorias.CurrentRow.DataBoundItem;

                var resp = MessageBox.Show(
                    "¿Eliminar físicamente la categoría seleccionada?",
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (resp == DialogResult.Yes)
                {
                    CategoriaNegocio negocio = new CategoriaNegocio();
                    negocio.eliminarFisico(seleccionada.Id);
                    cargar();
                }
            }
            catch (Exception ex)
            {
                // Si la categoría está en uso, acá verás el mensaje del throw
                MessageBox.Show(ex.Message);
            }
        }
    }
}
