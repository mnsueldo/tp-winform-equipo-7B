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
            MarcaNegocio marcaNegocio = new MarcaNegocio();

            try
            {
                if (marca == null)
                    marca = new Marca();


                marca.Descripcion = txtDescripcionMarca.Text;

                if (marca.Id != 0)
                {
                    marcaNegocio.modificar(marca);
                    MessageBox.Show("Marca modificada correctamente");
                }
                else
                {
                    marcaNegocio.agregar(marca);
                    MessageBox.Show("Marca agregada correctamente");
                }

                Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void btnCancelarMarca_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmAgregarMarca_Load(object sender, EventArgs e)
        {
            MarcaNegocio marcaNegocio = new MarcaNegocio();

            try
            {
                if (marca != null)
                {

                    txtDescripcionMarca.Text = marca.Descripcion;

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
