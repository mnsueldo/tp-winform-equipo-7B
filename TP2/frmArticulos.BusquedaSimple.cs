using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using dominio;

namespace TP2
{
    public partial class frmArticulos
    {
        private Panel srchPanel;
        private TextBox srchTxt;
        private ComboBox srchCboMarca;
        private ComboBox srchCboCategoria;
        private NumericUpDown srchMin;
        private NumericUpDown srchMax;
        private CheckBox srchChkConImagen;
        private Button srchBtnBuscar;
        private Button srchBtnLimpiar;
        private Label srchLblCount;

        private bool srchLayoutAjustado = false; // para no ajustar dos veces

        private List<Articulo> _all = new List<Articulo>();
        private List<Articulo> _view = new List<Articulo>();

        public void InitBusquedaSimple()
        {
            CapturarListaDeGrilla();
            CrearPanelSiHaceFalta();
            PoblarCombosDesdeLista();
            SrchMostrar(_all);
        }

        private void CapturarListaDeGrilla()
        {
            _all = new List<Articulo>();
            if (dgvArticulos == null) return;

            var ds = dgvArticulos.DataSource;
            if (ds is List<Articulo> la) _all = la.ToList();
            else if (ds is BindingList<Articulo> bl) _all = bl.ToList();
            else if (ds is BindingSource bs && bs.List is IEnumerable list)
            {
                foreach (var x in list) if (x is Articulo a) _all.Add(a);
            }
            else if (ds == null)
            {
                foreach (DataGridViewRow row in dgvArticulos.Rows)
                    if (row?.DataBoundItem is Articulo a) _all.Add(a);
            }
            else if (ds is IEnumerable en)
            {
                foreach (var x in en) if (x is Articulo a) _all.Add(a);
            }
        }

        private void CrearPanelSiHaceFalta()
        {
            if (srchPanel != null && !srchPanel.IsDisposed) return;

            srchPanel = new Panel { Dock = DockStyle.Top, Height = 86 };
            Controls.Add(srchPanel);
            Controls.SetChildIndex(srchPanel, 0);

            var lblTexto = new Label { Text = "Texto:", AutoSize = true, Left = 10, Top = 12 };
            srchTxt = new TextBox { Left = 60, Top = 9, Width = 200 };
            // Enter -> buscar
            srchTxt.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; SrchFiltrar(); } };
            // En vivo (>=2 chars o vacío)
            srchTxt.TextChanged += (s, e) =>
            {
                var t = (srchTxt.Text ?? "").Trim();
                if (t.Length == 0 || t.Length >= 2) SrchFiltrar();
            };

            var lblMarca = new Label { Text = "Marca:", AutoSize = true, Left = 280, Top = 12 };
            srchCboMarca = new ComboBox { Left = 330, Top = 9, Width = 160, DropDownStyle = ComboBoxStyle.DropDownList };

            var lblCategoria = new Label { Text = "Categoría:", AutoSize = true, Left = 510, Top = 12 };
            srchCboCategoria = new ComboBox { Left = 580, Top = 9, Width = 160, DropDownStyle = ComboBoxStyle.DropDownList };

            var lblMin = new Label { Text = "Precio ≥", AutoSize = true, Left = 10, Top = 48 };
            srchMin = new NumericUpDown { Left = 70, Top = 45, Width = 90, DecimalPlaces = 2, Maximum = 100000000, Minimum = 0 };

            var lblMax = new Label { Text = "Precio ≤", AutoSize = true, Left = 170, Top = 48 };
            srchMax = new NumericUpDown { Left = 230, Top = 45, Width = 90, DecimalPlaces = 2, Maximum = 100000000, Minimum = 0 };

            srchChkConImagen = new CheckBox { Text = "Con imagen", Left = 340, Top = 47, AutoSize = true };

            srchBtnBuscar = new Button { Text = "Buscar", Left = 460, Top = 43, Width = 90, Height = 28 };
            srchBtnBuscar.Click += (s, e) => SrchFiltrar();

            srchBtnLimpiar = new Button { Text = "Limpiar", Left = 560, Top = 43, Width = 90, Height = 28 };
            srchBtnLimpiar.Click += (s, e) => SrchLimpiar();

            srchLblCount = new Label { Text = "", Left = 660, Top = 48, AutoSize = true };

            srchPanel.Controls.AddRange(new Control[] {
                lblTexto, srchTxt, lblMarca, srchCboMarca, lblCategoria, srchCboCategoria,
                lblMin, srchMin, lblMax, srchMax, srchChkConImagen, srchBtnBuscar, srchBtnLimpiar, srchLblCount
            });

            // >>> Ajustar layout para que la grilla NO quede debajo del panel
            AjustarLayoutDebajoDelPanel();
        }

        // Mueve la grilla (y opcionalmente otros controles) por debajo del panel superior
        private void AjustarLayoutDebajoDelPanel()
        {
            if (srchLayoutAjustado || srchPanel == null || dgvArticulos == null) return;

            int nuevoTop = srchPanel.Bottom + 6;     // margen de 6px
            if (dgvArticulos.Top < nuevoTop)
            {
                int delta = nuevoTop - dgvArticulos.Top;
                dgvArticulos.Top = nuevoTop;
                // reducir altura para que siga entrando en el formulario
                if (dgvArticulos.Height > delta + 40)
                    dgvArticulos.Height -= delta;
            }

            // Si tenés un PictureBox en la misma línea, también lo bajamos si hiciera falta
            if (pbxArticulo != null && pbxArticulo.Top < nuevoTop)
            {
                int delta = nuevoTop - pbxArticulo.Top;
                pbxArticulo.Top = nuevoTop;
                if (pbxArticulo.Height > delta + 40)
                    pbxArticulo.Height -= delta;
            }

            srchLayoutAjustado = true;
        }

        private void PoblarCombosDesdeLista()
        {
            var marcas = _all.Where(a => a?.Marca != null).Select(a => a.Marca)
                             .GroupBy(m => m.Id).Select(g => g.First())
                             .OrderBy(m => m.Descripcion).ToList();
            marcas.Insert(0, new Marca { Id = 0, Descripcion = "(Todas)" });
            srchCboMarca.DataSource = marcas;
            srchCboMarca.DisplayMember = "Descripcion";
            srchCboMarca.ValueMember = "Id";

            var categorias = _all.Where(a => a?.Categoria != null).Select(a => a.Categoria)
                                 .GroupBy(c => c.Id).Select(g => g.First())
                                 .OrderBy(c => c.Descripcion).ToList();
            categorias.Insert(0, new Categoria { Id = 0, Descripcion = "(Todas)" });
            srchCboCategoria.DataSource = categorias;
            srchCboCategoria.DisplayMember = "Descripcion";
            srchCboCategoria.ValueMember = "Id";
        }

        private void SrchFiltrar()
        {
            var texto = (srchTxt?.Text ?? "").Trim().ToLowerInvariant();
            int idMarca = (srchCboMarca?.SelectedItem is Marca m) ? m.Id : 0;
            int idCategoria = (srchCboCategoria?.SelectedItem is Categoria c) ? c.Id : 0;
            decimal min = srchMin?.Value ?? 0m;
            decimal max = srchMax?.Value ?? 0m;
            bool conImg = srchChkConImagen?.Checked ?? false;

            IEnumerable<Articulo> q = _all;

            // Palabra clave en Código/Nombre/Descripción
            if (!string.IsNullOrEmpty(texto))
                q = q.Where(a =>
                    (!string.IsNullOrEmpty(a.Codigo) && a.Codigo.ToLowerInvariant().Contains(texto)) ||
                    (!string.IsNullOrEmpty(a.Nombre) && a.Nombre.ToLowerInvariant().Contains(texto)) ||
                    (!string.IsNullOrEmpty(a.Descripcion) && a.Descripcion.ToLowerInvariant().Contains(texto)));

            // Marca / Categoría
            if (idMarca > 0) q = q.Where(a => a.Marca != null && a.Marca.Id == idMarca);
            if (idCategoria > 0) q = q.Where(a => a.Categoria != null && a.Categoria.Id == idCategoria);

            // Precio entre
            if (min > 0) q = q.Where(a => a.Precio >= min);
            if (max > 0) q = q.Where(a => a.Precio <= max);

            // Con imagen
            if (conImg) q = q.Where(a => a.Imagenes != null && a.Imagenes.Count > 0);

            _view = q.ToList();
            SrchMostrar(_view);
        }

        private void SrchLimpiar()
        {
            if (srchTxt != null) srchTxt.Text = "";
            if (srchCboMarca != null && srchCboMarca.Items.Count > 0) srchCboMarca.SelectedIndex = 0;
            if (srchCboCategoria != null && srchCboCategoria.Items.Count > 0) srchCboCategoria.SelectedIndex = 0;
            if (srchMin != null) srchMin.Value = 0;
            if (srchMax != null) srchMax.Value = 0;
            if (srchChkConImagen != null) srchChkConImagen.Checked = false;

            _view = _all.ToList();
            SrchMostrar(_view);
        }

        private void SrchMostrar(List<Articulo> lista)
        {
            var bs = new BindingSource();
            bs.DataSource = lista;
            dgvArticulos.DataSource = bs;

            // asegura formato/anchos y que no se tape la cabecera
            FormatearGrilla();
            AjustarLayoutDebajoDelPanel();

            if (srchLblCount != null) srchLblCount.Text = $"{lista.Count} resultado(s)";
        }
    }
}
