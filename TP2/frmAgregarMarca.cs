using dominio;
using Negocio;
using System;
using System.Windows.Forms;

namespace TP2
{
    public partial class frmAgregarMarca : Form
    {
        private Marca marca = null;

        public frmAgregarMarca()
        {
            InitializeComponent();
        }

        public frmAgregarMarca(Marca marca)
        {
            this.marca = marca;
            InitializeComponent();
            Text = "Modificar Marca";
        }

        private void btnAceptarMarca_Click(object sender, EventArgs e)
        {
            var marcaNegocio = new MarcaNegocio();

            try
            {
                // Validación mínima de UI
                var desc = txtDescripcionMarca.Text?.Trim();
                if (string.IsNullOrWhiteSpace(desc))
                {
                    MessageBox.Show("La descripción de la marca es obligatoria.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDescripcionMarca.Focus();
                    return;
                }

                if (marca == null)
                    marca = new Marca();

                marca.Descripcion = desc;

                if (marca.Id != 0)
                {
                    marcaNegocio.modificar(marca);
                    MessageBox.Show("Marca modificada correctamente", "OK",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    marcaNegocio.agregar(marca);
                    MessageBox.Show("Marca agregada correctamente", "OK",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                Close();
            }
            catch (BusinessRuleException brex)
            {
                MessageBox.Show(brex.Message, "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (ApplicationException aex)
            {
                MessageBox.Show(aex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception)
            {
                MessageBox.Show("Ocurrió un error inesperado al guardar la marca.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelarMarca_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmAgregarMarca_Load(object sender, EventArgs e)
        {
            try
            {
                if (marca != null)
                    txtDescripcionMarca.Text = marca.Descripcion;
            }
            catch (ApplicationException aex)
            {
                MessageBox.Show(aex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception)
            {
                MessageBox.Show("Ocurrió un error cargando la marca.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
