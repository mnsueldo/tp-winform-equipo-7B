using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using dominio;
using Negocio;

namespace TP2
{
    public partial class frmAgregarArticulo : Form
    {
        private Articulo articulo;                                    // null = alta; con valor = modificación
        private readonly List<string> _imagenes = new List<string>();  // urls/rutas a persistir
        private ContextMenuStrip _menuImagenes;

        public frmAgregarArticulo()
        {
            InitializeComponent();
            Text = "Agregar Artículo";
            WireEvents();
        }

        public frmAgregarArticulo(Articulo existente) : this()
        {
            articulo = existente;
            Text = "Modificar Artículo";
        }

        // =========================================================
        // Eventos / wiring
        // =========================================================
        private void WireEvents()
        {
            // --- Evitar doble registro (Designer + código) ---
            btnAgregarImagen.Click -= btnAgregarImagen_Click_1;
            btnAgregarImagen.Click += btnAgregarImagen_Click_1;

            btnQuitarImagen.Click -= btnQuitarImagen_Click_1;
            btnQuitarImagen.Click += btnQuitarImagen_Click_1;

            txtUrlImagen.Leave -= txtUrlImagen_Leave;
            txtUrlImagen.Leave += txtUrlImagen_Leave;

            lstImagenes.SelectedIndexChanged -= lstImagenes_SelectedIndexChanged;
            lstImagenes.SelectedIndexChanged += lstImagenes_SelectedIndexChanged;

            lstImagenes.MouseDown -= lstImagenes_MouseDown;
            lstImagenes.MouseDown += lstImagenes_MouseDown;

            lstImagenes.KeyDown -= lstImagenes_KeyDown;
            lstImagenes.KeyDown += lstImagenes_KeyDown;

            // Desactivar cualquier DoubleClick que pueda haber quedado
            lstImagenes.DoubleClick -= lstImagenes_DoubleClick_NoOp;

            // Propiedades de lista
            lstImagenes.SelectionMode = SelectionMode.MultiExtended;
            lstImagenes.HorizontalScrollbar = true;
            lstImagenes.IntegralHeight = false;

            // Menú contextual
            if (_menuImagenes == null)
            {
                _menuImagenes = new ContextMenuStrip();
                _menuImagenes.Items.Add(new ToolStripMenuItem("Copiar URL", null, (s, e) => CopiarSeleccion()));
                _menuImagenes.Items.Add(new ToolStripMenuItem("Copiar TODAS", null, (s, e) => CopiarTodas()));
                _menuImagenes.Items.Add(new ToolStripSeparator());
                _menuImagenes.Items.Add(new ToolStripMenuItem("Abrir en navegador", null, (s, e) => AbrirSeleccion()));
            }
            lstImagenes.ContextMenuStrip = _menuImagenes;
        }

        // =========================================================
        // Carga del formulario
        // =========================================================
        private void frmAgregarArticulo_Load(object sender, EventArgs e)
        {
            try
            {
                // 1) Cargar combos SIEMPRE primero
                CargarCombos();

                if (articulo != null)
                {
                    // 2) Mapear datos
                    txtCodigo.Text = articulo.Codigo;
                    txtNombre.Text = articulo.Nombre;
                    txtDescripcion.Text = articulo.Descripcion;
                    txtPrecio.Text = articulo.Precio.ToString("N2");

                    // 3) Seleccionar ítems en combos (ya tienen DataSource)
                    if (articulo.Marca != null) SeleccionarEnCombo(cboMarca, articulo.Marca.Id);
                    if (articulo.Categoria != null) SeleccionarEnCombo(cboCategoria, articulo.Categoria.Id);

                    // 4) Traer imágenes actuales
                    var neg = new ArticuloNegocio();
                    var actuales = neg.ObtenerImagenesPorId(articulo.Id);
                    _imagenes.Clear();
                    if (actuales != null && actuales.Count > 0)
                        _imagenes.AddRange(actuales);

                    CargarListaImagenesUI();

                    if (_imagenes.Count > 0)
                    {
                        txtUrlImagen.Text = _imagenes[0];
                        CargarImagenSeguro(_imagenes[0]);
                        lstImagenes.SelectedIndex = 0;
                    }
                    else
                    {
                        CargarImagenSeguro(null);
                    }
                }
                else
                {
                    // Alta
                    CargarImagenSeguro(null);
                    CargarListaImagenesUI();
                }
            }
            catch (ApplicationException aex)
            {
                MessageBox.Show(aex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
                MessageBox.Show("Ocurrió un error al cargar el formulario.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Carga de combos (Marca/Categoría)
        private void CargarCombos()
        {
            // Marcas
            var marcaNeg = new MarcaNegocio();
            var marcas = marcaNeg.listar(); // List<Marca>
            cboMarca.DataSource = null;
            cboMarca.DisplayMember = "Descripcion";
            cboMarca.ValueMember = "Id";
            cboMarca.DataSource = marcas;
            cboMarca.SelectedIndex = marcas.Count > 0 ? 0 : -1;

            // Categorías
            var catNeg = new CategoriaNegocio();
            var categorias = catNeg.listar(); // List<Categoria>
            cboCategoria.DataSource = null;
            cboCategoria.DisplayMember = "Descripcion";
            cboCategoria.ValueMember = "Id";
            cboCategoria.DataSource = categorias;
            cboCategoria.SelectedIndex = categorias.Count > 0 ? 0 : -1;
        }

        private void SeleccionarEnCombo(ComboBox combo, int id)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (combo.Items[i] is Categoria c && c.Id == id) { combo.SelectedIndex = i; return; }
                if (combo.Items[i] is Marca m && m.Id == id) { combo.SelectedIndex = i; return; }
            }
        }

        // =========================================================
        // Imágenes
        // =========================================================
        private void lstImagenes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstImagenes.SelectedItem is string u)
            {
                txtUrlImagen.Text = u;
                CargarImagenSeguro(u);
            }
        }

        private void lstImagenes_MouseDown(object sender, MouseEventArgs ev)
        {
            if (ev.Button == MouseButtons.Right)
            {
                int idx = lstImagenes.IndexFromPoint(ev.Location);
                if (idx >= 0) lstImagenes.SelectedIndex = idx;
            }
        }

        private void lstImagenes_KeyDown(object sender, KeyEventArgs ev)
        {
            if (ev.Control && ev.KeyCode == Keys.C)
            {
                CopiarSeleccion();
                ev.SuppressKeyPress = true;
            }
        }

        private void lstImagenes_DoubleClick_NoOp(object sender, EventArgs e) { /* intencionalmente vacío */ }

        private void txtUrlImagen_Leave(object sender, EventArgs e)
        {
            string url = txtUrlImagen.Text?.Trim();
            if (string.IsNullOrWhiteSpace(url)) { CargarImagenSeguro(null); return; }

            if (!EsRutaValidaDeImagen(url))
            {
                MessageBox.Show("La imagen debe ser una URL http/https válida o un archivo local existente.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CargarImagenSeguro(url);
        }

        private void btnAgregarImagen_Click_1(object sender, EventArgs e)
        {
            string url = txtUrlImagen.Text?.Trim();
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("Ingresá una URL o ruta de imagen.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUrlImagen.Focus();
                return;
            }

            if (!EsRutaValidaDeImagen(url))
            {
                MessageBox.Show("La imagen debe ser una URL http/https válida o un archivo local existente.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool existe = _imagenes.Exists(u => string.Equals(u?.Trim(), url, StringComparison.OrdinalIgnoreCase));
            if (!existe) _imagenes.Add(url);

            CargarListaImagenesUI();

            int idx = lstImagenes.Items.IndexOf(url);
            if (idx >= 0) lstImagenes.SelectedIndex = idx;
            CargarImagenSeguro(url);
        }

        private void btnQuitarImagen_Click_1(object sender, EventArgs e)
        {
            if (lstImagenes.SelectedItems.Count == 0) return;

            // snapshot para no mutar mientras iteramos
            var aQuitar = new List<string>();
            foreach (object it in lstImagenes.SelectedItems)
            {
                if (it is string u && !string.IsNullOrEmpty(u))
                    aQuitar.Add(u);
            }

            foreach (string u in aQuitar)
                _imagenes.Remove(u);

            CargarListaImagenesUI();

            if (_imagenes.Count > 0)
            {
                txtUrlImagen.Text = _imagenes[0];
                CargarImagenSeguro(_imagenes[0]);
                lstImagenes.SelectedIndex = 0;
            }
            else
            {
                txtUrlImagen.Clear();
                CargarImagenSeguro(null);
            }
        }

        private void CargarListaImagenesUI()
        {
            lstImagenes.BeginUpdate();
            try
            {
                lstImagenes.Items.Clear();
                foreach (string u in _imagenes)
                    if (!string.IsNullOrWhiteSpace(u))
                        lstImagenes.Items.Add(u);
            }
            finally { lstImagenes.EndUpdate(); }
        }

        private static bool EsRutaValidaDeImagen(string ruta)
        {
            if (string.IsNullOrWhiteSpace(ruta)) return false;
            if (File.Exists(ruta)) return true; // local

            if (Uri.TryCreate(ruta, UriKind.Absolute, out var uri))
                return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;

            return false;
        }

        private void CargarImagenSeguro(string url)
        {
            try
            {
                pbxImagenArticulo.Image = null;
                pbxImagenArticulo.SizeMode = PictureBoxSizeMode.Zoom;

                if (string.IsNullOrWhiteSpace(url))
                {
                    pbxImagenArticulo.LoadAsync("https://efectocolibri.com/wp-content/uploads/2021/01/placeholder.png");
                    return;
                }

                url = url.Trim();
                if (File.Exists(url))
                    pbxImagenArticulo.Image = System.Drawing.Image.FromFile(url);
                else
                    pbxImagenArticulo.LoadAsync(url);
            }
            catch
            {
                pbxImagenArticulo.LoadAsync("https://efectocolibri.com/wp-content/uploads/2021/01/placeholder.png");
            }
        }

        // =========================================================
        // Guardar / Validar
        // =========================================================
        private void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarCampos()) return;

                if (articulo == null) articulo = new Articulo();

                articulo.Codigo = txtCodigo.Text.Trim();
                articulo.Nombre = txtNombre.Text.Trim();
                articulo.Descripcion = txtDescripcion.Text.Trim();
                articulo.Marca = cboMarca.SelectedItem as Marca;
                articulo.Categoria = cboCategoria.SelectedItem as Categoria;
                articulo.Precio = decimal.Parse(txtPrecio.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture);

                var neg = new ArticuloNegocio();

                if (articulo.Id == 0)
                    articulo.Id = neg.agregar(articulo);
                else
                    neg.modificar(articulo);

                neg.guardarImagenes(articulo.Id, _imagenes);

                MessageBox.Show("Artículo guardado correctamente.", "OK",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (ApplicationException aex)
            {
                MessageBox.Show(aex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
                MessageBox.Show("Ocurrió un error inesperado al guardar el artículo.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("El código es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCodigo.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }
            if (cboCategoria.SelectedItem == null)
            {
                MessageBox.Show("Seleccioná una categoría.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboCategoria.DroppedDown = true;
                return false;
            }
            if (cboMarca.SelectedItem == null)
            {
                MessageBox.Show("Seleccioná una marca.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboMarca.DroppedDown = true;
                return false;
            }

            if (!decimal.TryParse(txtPrecio.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out var precio) || precio < 0)
            {
                MessageBox.Show("El precio debe ser un número válido y no negativo.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecio.Focus();
                return false;
            }

            string url = txtUrlImagen.Text?.Trim();
            if (!string.IsNullOrWhiteSpace(url) && !EsRutaValidaDeImagen(url))
            {
                MessageBox.Show("La imagen debe ser una URL http/https válida o un archivo local existente.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUrlImagen.Focus();
                return false;
            }

            return true;
        }

        private void btnCancelar_Click(object sender, EventArgs e) => Close();

        // =========================================================
        // Helpers menú
        // =========================================================
        private void CopiarSeleccion()
        {
            if (lstImagenes.SelectedItems.Count == 0) return;
            var urls = new List<string>();
            foreach (object it in lstImagenes.SelectedItems)
                if (it is string u && !string.IsNullOrWhiteSpace(u)) urls.Add(u);

            if (urls.Count > 0) Clipboard.SetText(string.Join(Environment.NewLine, urls));
        }

        private void CopiarTodas()
        {
            if (lstImagenes.Items.Count == 0) return;
            var urls = new List<string>();
            foreach (object it in lstImagenes.Items)
                if (it is string u && !string.IsNullOrWhiteSpace(u)) urls.Add(u);

            if (urls.Count > 0) Clipboard.SetText(string.Join(Environment.NewLine, urls));
        }

        private void AbrirSeleccion()
        {
            if (lstImagenes.SelectedItem is string url && !string.IsNullOrWhiteSpace(url))
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
                }
                catch
                {
                    MessageBox.Show("No se pudo abrir la URL en el navegador.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
