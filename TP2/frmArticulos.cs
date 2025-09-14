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
using dominio;

namespace TP2
{
    public partial class frmArticulos : Form
    {
        private List<Articulo> listaArticulo;        
       
        public frmArticulos()
        {
            InitializeComponent();
        }

        private void frmArticulos_Load(object sender, EventArgs e)
        {
            cargar();
        }

        private void cargar()
        {
            try
            {
                ArticuloNegocio negocio = new ArticuloNegocio();
                listaArticulo = negocio.listar();           
               
                dgvArticulos.DataSource = listaArticulo;
                dgvArticulos.Columns["Id"].Visible = false;
                
                if (dgvArticulos.CurrentRow != null)
                {
                    Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                    if (seleccionado.Imagenes != null)
                        cargarImagen(seleccionado.Imagenes[0].ImagenUrl);
                    else
                        pbxArticulo.Load("https://efectocolibri.com/wp-content/uploads/2021/01/placeholder.png");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                
            }

                       
        }

        private void dgvArticulos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvArticulos.CurrentRow != null)
            {
                Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                if (seleccionado.Imagenes != null && seleccionado.Imagenes.Count > 0)
                    cargarImagen(seleccionado.Imagenes[0].ImagenUrl);
                else
                    pbxArticulo.Load("https://efectocolibri.com/wp-content/uploads/2021/01/placeholder.png");
            }
        }
        private void cargarImagen(string imagen)
        {
            try
            {
                pbxArticulo.Load(imagen);
            }
            catch (Exception ex)
            {
                pbxArticulo.Load("https://efectocolibri.com/wp-content/uploads/2021/01/placeholder.png");
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAgregarArticulo agregar = new frmAgregarArticulo();
            agregar.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Articulo seleccionado;
            seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;

            frmAgregarArticulo modificar = new frmAgregarArticulo(seleccionado);
            modificar.ShowDialog();
            cargar();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvArticulos.CurrentRow == null)
                    return;
                                
                Articulo seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;

                var resp = MessageBox.Show(
                    "¿Eliminar físicamente el artículo seleccionado?",
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (resp == DialogResult.Yes)
                {
                    ArticuloNegocio negocio = new ArticuloNegocio();
                    negocio.eliminarFisico(seleccionado.Id);
                    cargar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnCategorias_Click(object sender, EventArgs e)
        {
            frmCategorias agregar = new frmCategorias();
            agregar.ShowDialog();
            cargar();
        }

        private void btnMarcas_Click(object sender, EventArgs e)
        {
            frmMarcas agregar = new frmMarcas();
            agregar.ShowDialog();
            cargar();
        }
    }
}
