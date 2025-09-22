using Negocio;
using System;
using System.Text.RegularExpressions;
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
            Text = "Agregar Categoría";
        }

        public frmAgregarCategoria(Categoria categoria) : this()
        {
            this.categoria = categoria;
            Text = "Modificar Categoría";
        }

        private void frmAgregarCategoria_Load(object sender, EventArgs e)
        {
            if (categoria != null)
                txtDescripcionCategoria.Text = categoria.Descripcion;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            var negocio = new CategoriaNegocio();

            try
            {
                //Normalizar entrada: trim + colapsar espacios internos
                string input = (txtDescripcionCategoria.Text ?? string.Empty).Trim();
                input = Regex.Replace(input, @"\s{2,}", " "); 

                //Validaciones de negocio (UI)
                if (string.IsNullOrWhiteSpace(input))
                {
                    MessageBox.Show("La descripción no puede estar vacía.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDescripcionCategoria.Focus();
                    return;
                }

                //validar longitud mínima
                if (input.Length < 2)
                {
                    MessageBox.Show("La descripción debe tener al menos 2 caracteres.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDescripcionCategoria.Focus();
                    return;
                }

                //Preparar entidad
                if (categoria == null)
                    categoria = new Categoria();

                //Chequear duplicados (case-insensitive) excluyendo el propio Id si es edición
                int idExcluir = categoria.Id;
                bool existe = negocio.ExisteDescripcion(input, idExcluir);
                if (existe)
                {
                    MessageBox.Show("Ya existe una categoría con esa descripción.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDescripcionCategoria.SelectAll();
                    txtDescripcionCategoria.Focus();
                    return;
                }

                
                categoria.Descripcion = input;

                if (categoria.Id != 0)
                {
                    negocio.modificar(categoria);
                    MessageBox.Show("Categoría modificada correctamente", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    negocio.agregar(categoria);
                    MessageBox.Show("Categoría agregada correctamente", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
