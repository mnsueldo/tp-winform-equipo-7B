using Negocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using dominio;

namespace TP2
{
    public partial class frmArticulos : Form
    {
        private List<Articulo> listaArticulo;
        private int indiceImagenActual = 0;
        private Articulo articuloActual = null;
        private bool eventosInicializados = false;

        // Placeholder (podés cambiarlo por uno local en Resources)
        private const string PLACEHOLDER_URL = "https://efectocolibri.com/wp-content/uploads/2021/01/placeholder.png";

        public frmArticulos()
        {
            InitializeComponent();
        }

        private void frmArticulos_Load(object sender, EventArgs e)
        {
            ConfigurarPictureBox();
            cargar();

            if (dgvArticulos != null)
                dgvArticulos.CellFormatting += dgvArticulos_CellFormatting;

            SuscribirEventosDetalleUnaVez();
            // InitBusquedaSimple(); // si lo usás
        }

        private void ConfigurarPictureBox()
        {
            if (pbxArticulo == null) return;

            pbxArticulo.SizeMode = PictureBoxSizeMode.Zoom;   // <<< evita deformación
            pbxArticulo.WaitOnLoad = false;                   // no bloquear UI
            pbxArticulo.InitialImage = null;                  // opcional: imagen de "cargando"
            pbxArticulo.ErrorImage = null;                    // manejamos nosotros el error
            pbxArticulo.BorderStyle = BorderStyle.None;
        }

        private void SuscribirEventosDetalleUnaVez()
        {
            if (eventosInicializados) return;
            if (btnVerDetalle != null) btnVerDetalle.Click += btnVerDetalle_Click;
            if (dgvArticulos != null) dgvArticulos.CellDoubleClick += (s, ev) => btnVerDetalle_Click(s, EventArgs.Empty);
            eventosInicializados = true;
        }

        private void ocultarColumnas()
        {
            if (dgvArticulos.Columns["Id"] != null)
                dgvArticulos.Columns["Id"].Visible = false;
        }

        private void cargar()
        {
            try
            {
                var negocio = new ArticuloNegocio();
                listaArticulo = negocio.listar();

                dgvArticulos.DataSource = null;
                dgvArticulos.DataSource = listaArticulo;
                ocultarColumnas();
                FormatearGrilla();
                MostrarImagenSeleccionActual();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void FormatearGrilla()
        {
            if (dgvArticulos == null || dgvArticulos.Columns.Count == 0) return;

            dgvArticulos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvArticulos.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgvArticulos.DefaultCellStyle.WrapMode = DataGridViewTriState.False;

            SetFill("Codigo", 70, "Código");
            SetFill("Nombre", 120, "Nombre");
            SetFill("Descripcion", 260, "Descripción");
            SetFill("Marca", 100, "Marca");
            SetFill("Categoria", 110, "Categoría");
            SetFill("Precio", 90, "Precio");

            var colPrecio = dgvArticulos.Columns["Precio"];
            if (colPrecio != null)
            {
                colPrecio.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                colPrecio.DefaultCellStyle.Format = "N2";
            }

            // Si tenés una columna de imagen en el DGV, asegurá Zoom
            foreach (DataGridViewColumn c in dgvArticulos.Columns)
            {
                if (c is DataGridViewImageColumn imgCol)
                {
                    imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
                    dgvArticulos.RowTemplate.Height = Math.Max(dgvArticulos.RowTemplate.Height, 64);
                }
            }

            dgvArticulos.ShowCellToolTips = true;
        }

        private void SetFill(string name, float weight, string headerText = null)
        {
            var col = dgvArticulos.Columns[name];
            if (col == null) return;
            col.FillWeight = weight;
            col.MinimumWidth = (int)(weight * 0.4);
            if (!string.IsNullOrEmpty(headerText)) col.HeaderText = headerText;
        }

        private void dgvArticulos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var grid = (DataGridView)sender;
            var cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (e.Value != null)
                cell.ToolTipText = e.Value.ToString();
        }

        private void MostrarImagenSeleccionActual()
        {
            if (dgvArticulos?.CurrentRow == null) { MostrarImagen(null); return; }
            var seleccionado = dgvArticulos.CurrentRow.DataBoundItem as Articulo;
            if (seleccionado == null) { MostrarImagen(null); return; }

            articuloActual = seleccionado;
            indiceImagenActual = 0;

            // 1) Si hay lista de imágenes, usa la actual
            if (articuloActual.Imagenes != null && articuloActual.Imagenes.Count > 0)
            {
                MostrarImagen(articuloActual.Imagenes[indiceImagenActual]);
                return;
            }

            // 2) Si no hay lista, pero hay UrlImagen única, úsala
            if (!string.IsNullOrWhiteSpace(articuloActual.UrlImagen))
            {
                MostrarImagen(articuloActual.UrlImagen);
                return;
            }

            // 3) Placeholder
            MostrarImagen(null);
        }

        private void MostrarImagen(string url)
        {
            if (pbxArticulo == null) return;

            var destino = string.IsNullOrWhiteSpace(url) ? PLACEHOLDER_URL : url;

            try
            {
                // LoadAsync evita bloquear el hilo UI.
                pbxArticulo.Image = null;             // libera imagen previa
                pbxArticulo.ImageLocation = null;
                pbxArticulo.LoadAsync(destino);
            }
            catch
            {
                // fallback por si falla el Async/URL
                try { pbxArticulo.Load(PLACEHOLDER_URL); } catch { /* swallow */ }
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            var agregar = new frmAgregarArticulo();
            agregar.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvArticulos.CurrentRow != null)
            {
                var seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                var modificar = new frmAgregarArticulo(seleccionado);
                modificar.ShowDialog();
                cargar();
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un artículo para modificar.");
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvArticulos.CurrentRow != null)
                {
                    var seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;

                    var resp = MessageBox.Show(
                        "¿Eliminar físicamente el artículo seleccionado?",
                        "Confirmar eliminación",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (resp == DialogResult.Yes)
                    {
                        var negocio = new ArticuloNegocio();
                        negocio.eliminarFisico(seleccionado.Id);
                        cargar();
                    }
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione un artículo para eliminar.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnMenuCategorias_Click(object sender, EventArgs e)
        {
            using (var f = new frmCategorias()) f.ShowDialog();
            cargar();
        }

        private void btnMenuMarcas_Click(object sender, EventArgs e)
        {
            using (var f = new frmMarcas()) f.ShowDialog();
            cargar();
        }

        private void btnVerDetalle_Click(object sender, EventArgs e)
        {
            if (dgvArticulos?.CurrentRow == null) { MessageBox.Show("Seleccioná un artículo primero."); return; }
            var seleccionado = dgvArticulos.CurrentRow.DataBoundItem as Articulo;
            if (seleccionado == null) { MessageBox.Show("No se pudo obtener el artículo seleccionado."); return; }

            using (var frm = new frmDetalleArticulo(seleccionado)) frm.ShowDialog(this);
        }

        private void dgvArticulos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvArticulos.CurrentRow != null)
            {
                articuloActual = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                indiceImagenActual = 0;
                // Reutilizamos la lógica centralizada
                MostrarImagenSeleccionActual();
            }
        }

        // Si más adelante sumás botones "Imagen anterior / siguiente":
        private void MostrarImagenActual()
        {
            if (articuloActual == null)
            {
                MostrarImagen(null);
                return;
            }

            if (articuloActual.Imagenes != null && articuloActual.Imagenes.Count > 0)
            {
                // Clamp de índice por las dudas
                if (indiceImagenActual < 0) indiceImagenActual = 0;
                if (indiceImagenActual >= articuloActual.Imagenes.Count)
                    indiceImagenActual = articuloActual.Imagenes.Count - 1;

                MostrarImagen(articuloActual.Imagenes[indiceImagenActual]);
            }
            else if (!string.IsNullOrWhiteSpace(articuloActual.UrlImagen))
            {
                MostrarImagen(articuloActual.UrlImagen);
            }
            else
            {
                MostrarImagen(null);
            }
        }
    }
}
