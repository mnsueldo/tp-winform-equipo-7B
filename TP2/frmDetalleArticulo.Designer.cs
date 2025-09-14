namespace TP2
{
    partial class frmDetalleArticulo
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TableLayoutPanel table;
        private System.Windows.Forms.Label lblCodigo;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.Label lblDescripcion;
        private System.Windows.Forms.Label lblMarca;
        private System.Windows.Forms.Label lblCategoria;
        private System.Windows.Forms.Label lblPrecio;
        private System.Windows.Forms.Label lblCodigoValor;
        private System.Windows.Forms.Label lblNombreValor;
        private System.Windows.Forms.Label lblDescripcionValor;
        private System.Windows.Forms.Label lblMarcaValor;
        private System.Windows.Forms.Label lblCategoriaValor;
        private System.Windows.Forms.Label lblPrecioValor;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button btnAnterior;
        private System.Windows.Forms.Button btnSiguiente;
        private System.Windows.Forms.Label lblPaginador;
        private System.Windows.Forms.Button btnCerrar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.table = new System.Windows.Forms.TableLayoutPanel();
            this.lblCodigo = new System.Windows.Forms.Label();
            this.lblNombre = new System.Windows.Forms.Label();
            this.lblDescripcion = new System.Windows.Forms.Label();
            this.lblMarca = new System.Windows.Forms.Label();
            this.lblCategoria = new System.Windows.Forms.Label();
            this.lblPrecio = new System.Windows.Forms.Label();
            this.lblCodigoValor = new System.Windows.Forms.Label();
            this.lblNombreValor = new System.Windows.Forms.Label();
            this.lblDescripcionValor = new System.Windows.Forms.Label();
            this.lblMarcaValor = new System.Windows.Forms.Label();
            this.lblCategoriaValor = new System.Windows.Forms.Label();
            this.lblPrecioValor = new System.Windows.Forms.Label();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.btnAnterior = new System.Windows.Forms.Button();
            this.btnSiguiente = new System.Windows.Forms.Button();
            this.lblPaginador = new System.Windows.Forms.Label();
            this.btnCerrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // table
            // 
            this.table.ColumnCount = 2;
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.table.Location = new System.Drawing.Point(12, 12);
            this.table.Name = "table";
            this.table.RowCount = 6;
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.table.Size = new System.Drawing.Size(440, 144);
            this.table.TabIndex = 0;
            // 
            // Labels
            // 
            this.lblCodigo.AutoSize = true;
            this.lblCodigo.Text = "Código:";
            this.lblNombre.AutoSize = true;
            this.lblNombre.Text = "Nombre:";
            this.lblDescripcion.AutoSize = true;
            this.lblDescripcion.Text = "Descripción:";
            this.lblMarca.AutoSize = true;
            this.lblMarca.Text = "Marca:";
            this.lblCategoria.AutoSize = true;
            this.lblCategoria.Text = "Categoría:";
            this.lblPrecio.AutoSize = true;
            this.lblPrecio.Text = "Precio:";
            // 
            // Valores
            // 
            this.lblCodigoValor.AutoSize = true;
            this.lblNombreValor.AutoSize = true;
            this.lblDescripcionValor.AutoSize = true;
            this.lblMarcaValor.AutoSize = true;
            this.lblCategoriaValor.AutoSize = true;
            this.lblPrecioValor.AutoSize = true;
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(12, 170);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(440, 280);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 1;
            this.pictureBox.TabStop = false;
            // 
            // btnAnterior
            // 
            this.btnAnterior.Location = new System.Drawing.Point(12, 460);
            this.btnAnterior.Name = "btnAnterior";
            this.btnAnterior.Size = new System.Drawing.Size(75, 23);
            this.btnAnterior.TabIndex = 2;
            this.btnAnterior.Text = "< Anterior";
            this.btnAnterior.UseVisualStyleBackColor = true;
            this.btnAnterior.Click += new System.EventHandler(this.btnAnterior_Click);
            // 
            // btnSiguiente
            // 
            this.btnSiguiente.Location = new System.Drawing.Point(377, 460);
            this.btnSiguiente.Name = "btnSiguiente";
            this.btnSiguiente.Size = new System.Drawing.Size(75, 23);
            this.btnSiguiente.TabIndex = 3;
            this.btnSiguiente.Text = "Siguiente >";
            this.btnSiguiente.UseVisualStyleBackColor = true;
            this.btnSiguiente.Click += new System.EventHandler(this.btnSiguiente_Click);
            // 
            // lblPaginador
            // 
            this.lblPaginador.AutoSize = true;
            this.lblPaginador.Location = new System.Drawing.Point(210, 465);
            this.lblPaginador.Name = "lblPaginador";
            this.lblPaginador.Size = new System.Drawing.Size(35, 13);
            this.lblPaginador.TabIndex = 4;
            this.lblPaginador.Text = "0 / 0";
            this.lblPaginador.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(190, 500);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(90, 28);
            this.btnCerrar.TabIndex = 5;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // frmDetalleArticulo
            // 
            this.ClientSize = new System.Drawing.Size(464, 540);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.lblPaginador);
            this.Controls.Add(this.btnSiguiente);
            this.Controls.Add(this.btnAnterior);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.table);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDetalleArticulo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Detalle del artículo";
            this.Load += new System.EventHandler(this.frmDetalleArticulo_Load);
            // Agregar las filas a la tabla
            this.table.Controls.Add(this.lblCodigo, 0, 0);
            this.table.Controls.Add(this.lblCodigoValor, 1, 0);
            this.table.Controls.Add(this.lblNombre, 0, 1);
            this.table.Controls.Add(this.lblNombreValor, 1, 1);
            this.table.Controls.Add(this.lblDescripcion, 0, 2);
            this.table.Controls.Add(this.lblDescripcionValor, 1, 2);
            this.table.Controls.Add(this.lblMarca, 0, 3);
            this.table.Controls.Add(this.lblMarcaValor, 1, 3);
            this.table.Controls.Add(this.lblCategoria, 0, 4);
            this.table.Controls.Add(this.lblCategoriaValor, 1, 4);
            this.table.Controls.Add(this.lblPrecio, 0, 5);
            this.table.Controls.Add(this.lblPrecioValor, 1, 5);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
