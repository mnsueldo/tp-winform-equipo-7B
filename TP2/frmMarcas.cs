using dominio;
using Negocio;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TP2
{
    public partial class frmMarcas : Form
    {
        private List<Marca> listaMarca;

        public frmMarcas()
        {
            InitializeComponent();
        }

        private void frmMarcas_Load(object sender, EventArgs e)
        {
            cargar();
        }

        private void cargar()
        {
            try
            {
                var negocio = new MarcaNegocio();
                listaMarca = negocio.listar();
                dgvMarcas.DataSource = listaMarca;
                if (dgvMarcas.Columns["Id"] != null)
                    dgvMarcas.Columns["Id"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnAgregarMarca_Click(object sender, EventArgs e)
        {
            var agregar = new frmAgregarMarca();
            agregar.ShowDialog();
            cargar();
        }

        private void btnModificarMarca_Click(object sender, EventArgs e)
        {
            if (dgvMarcas.CurrentRow == null) return;

            var seleccionado = (Marca)dgvMarcas.CurrentRow.DataBoundItem;
            var modificar = new frmAgregarMarca(seleccionado);
            modificar.ShowDialog();
            cargar();
        }

        private void btnEliminarMarca_Click(object sender, EventArgs e)
        {
            if (dgvMarcas.CurrentRow == null) return;

            var seleccionado = (Marca)dgvMarcas.CurrentRow.DataBoundItem;

            var resp = MessageBox.Show(
                $"¿Eliminar la marca '{seleccionado.Descripcion}'?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (resp != DialogResult.Yes) return;

            try
            {
                var negocio = new MarcaNegocio();
                negocio.eliminar(seleccionado.Id);
                cargar();
                MessageBox.Show("Marca eliminada.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (BusinessRuleException brex)
            {
                MessageBox.Show(brex.Message, "No permitido", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException?.Message ?? ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
