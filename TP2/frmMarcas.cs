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
    public partial class frmMarcas : Form
    {
        private  List<Marca> listaMarca;
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
                MarcaNegocio negocio = new MarcaNegocio();
                listaMarca = negocio.listar();
                dgvMarcas.DataSource = listaMarca;
                dgvMarcas.Columns["Id"].Visible = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }

        }

        private void btnAgregarMarca_Click(object sender, EventArgs e)
        {
            frmAgregarMarca agregar = new frmAgregarMarca();
            agregar.ShowDialog();
            cargar();
        }

        private void btnModificarMarca_Click(object sender, EventArgs e)
        {
            Marca seleccionado;
            seleccionado = (Marca)dgvMarcas.CurrentRow.DataBoundItem;

            frmAgregarMarca modificar = new frmAgregarMarca(seleccionado);
            modificar.ShowDialog();
            cargar();
        }

        private void btnEliminarMarca_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvMarcas.CurrentRow == null)
                    return;

                Marca seleccionado = (Marca)dgvMarcas.CurrentRow.DataBoundItem;

                var resp = MessageBox.Show(
                    "¿Eliminar físicamente la marca seleccionada?",
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (resp == DialogResult.Yes)
                {
                    MarcaNegocio negocio = new MarcaNegocio();
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
