using dominio;
using Negocio;
using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TP2
{
    public partial class frmAgregarMarca : Form
    {
        private Marca marca = null;

        public frmAgregarMarca()
        {
            InitializeComponent();
            Text = "Agregar Marca";
        }

        public frmAgregarMarca(Marca marca) : this()
        {
            this.marca = marca;
            Text = "Modificar Marca";
        }

        private void frmAgregarMarca_Load(object sender, EventArgs e)
        {
            if (marca != null)
                txtDescripcionMarca.Text = marca.Descripcion;
        }

        private void btnAceptarMarca_Click(object sender, EventArgs e)
        {
            var negocio = new MarcaNegocio();

            try
            {
                
                string input = (txtDescripcionMarca.Text ?? string.Empty).Trim();
                input = Regex.Replace(input, @"\s{2,}", " ");

                // Validaciones de UI
                if (string.IsNullOrWhiteSpace(input))
                {
                    MessageBox.Show("La descripción de la marca es obligatoria.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDescripcionMarca.Focus();
                    return;
                }

                if (input.Length < 2)
                {
                    MessageBox.Show("La descripción debe tener al menos 2 caracteres.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDescripcionMarca.Focus();
                    return;
                }

                if (marca == null)
                    marca = new Marca();

                // Evitar duplicados (case-insensitive), excluyendo el propio Id si es edición
                int idExcluir = marca.Id;
                if (negocio.ExisteDescripcion(input, idExcluir))
                {
                    MessageBox.Show("Ya existe una marca con esa descripción.",
                        "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDescripcionMarca.SelectAll();
                    txtDescripcionMarca.Focus();
                    return;
                }

                // Persistencia
                marca.Descripcion = input;

                if (marca.Id != 0)
                {
                    negocio.modificar(marca);
                    MessageBox.Show("Marca modificada correctamente", "OK",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    negocio.agregar(marca);
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException?.Message ?? ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelarMarca_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
