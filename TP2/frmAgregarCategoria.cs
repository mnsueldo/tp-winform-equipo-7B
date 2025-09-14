<<<<<<< HEAD
using Negocio;
using System;
using System.Windows.Forms;
using dominio;
=======
﻿using dominio;
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
>>>>>>> 792ff78601678c4d514f59504e6504e536469e3b

namespace TP2
{
    public partial class frmAgregarCategoria : Form
    {
        private Categoria categoria = null;
<<<<<<< HEAD

=======
>>>>>>> 792ff78601678c4d514f59504e6504e536469e3b
        public frmAgregarCategoria()
        {
            InitializeComponent();
        }
<<<<<<< HEAD

=======
>>>>>>> 792ff78601678c4d514f59504e6504e536469e3b
        public frmAgregarCategoria(Categoria categoria)
        {
            this.categoria = categoria;
            InitializeComponent();
<<<<<<< HEAD
            Text = "Modificar Categoría";
        }

        private void frmAgregarCategoria_Load(object sender, EventArgs e)
        {
            if (categoria != null)
                txtDescripcionCategoria.Text = categoria.Descripcion;   // <- acá
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            CategoriaNegocio negocio = new CategoriaNegocio();
=======
            Text = "Modificar Categoria";
        }

        private void btnCancelarCategoria_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAceptarCategoria_Click(object sender, EventArgs e)
        {
            CategoriaNegocio categoriaNegocio = new CategoriaNegocio();

>>>>>>> 792ff78601678c4d514f59504e6504e536469e3b
            try
            {
                if (categoria == null)
                    categoria = new Categoria();

<<<<<<< HEAD
                categoria.Descripcion = txtDescripcionCategoria.Text;   // <- y acá

                if (categoria.Id != 0)
                {
                    negocio.modificar(categoria);
                    MessageBox.Show("Categoría modificada correctamente");
                }
                else
                {
                    negocio.agregar(categoria);
                    MessageBox.Show("Categoría agregada correctamente");
=======
                               
                categoria.Descripcion = txtDescripcionCategoria.Text;                

                if (categoria.Id != 0)
                {
                    categoriaNegocio.modificar(categoria);
                    MessageBox.Show("Categoria modificada correctamente");
                }
                else
                {
                    categoriaNegocio.agregar(categoria);
                    MessageBox.Show("Categoria agregada correctamente");
>>>>>>> 792ff78601678c4d514f59504e6504e536469e3b
                }

                Close();
            }
            catch (Exception ex)
            {
<<<<<<< HEAD
                MessageBox.Show(ex.ToString());
            }
        }


        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
=======

                throw ex;
            }
        }

        private void frmAgregarCategoria_Load(object sender, EventArgs e)
        {
            CategoriaNegocio categoriaNegocio = new CategoriaNegocio();
            
            try
            {                
                if (categoria != null)
                {
                    
                    txtDescripcionCategoria.Text = categoria.Descripcion;
                    
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
>>>>>>> 792ff78601678c4d514f59504e6504e536469e3b
        }
    }
}
