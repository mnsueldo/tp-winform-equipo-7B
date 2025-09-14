<<<<<<< HEAD
using Negocio;
using System;
using System.Collections.Generic;
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
    public partial class frmCategorias : Form
    {
<<<<<<< HEAD
        private List<Categoria> lista;

=======
        private List<Categoria> listaCategoria;
>>>>>>> 792ff78601678c4d514f59504e6504e536469e3b
        public frmCategorias()
        {
            InitializeComponent();
        }
<<<<<<< HEAD

=======
       
>>>>>>> 792ff78601678c4d514f59504e6504e536469e3b
        private void frmCategorias_Load(object sender, EventArgs e)
        {
            cargar();
        }

        private void cargar()
        {
<<<<<<< HEAD
            CategoriaNegocio negocio = new CategoriaNegocio();
            try
            {
                lista = negocio.listar();
                dgvCategorias.DataSource = lista;
                dgvCategorias.Columns["Id"].Visible = false;
=======
            try
            {
                CategoriaNegocio negocio = new CategoriaNegocio();
                listaCategoria = negocio.listar();
                dgvCategorias.DataSource = listaCategoria;
                dgvCategorias.Columns["Id"].Visible = false;


>>>>>>> 792ff78601678c4d514f59504e6504e536469e3b
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
<<<<<<< HEAD
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAgregarCategoria f = new frmAgregarCategoria();
            f.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvCategorias.CurrentRow == null) return;

            Categoria seleccionada = (Categoria)dgvCategorias.CurrentRow.DataBoundItem;
            frmAgregarCategoria f = new frmAgregarCategoria(seleccionada);
            f.ShowDialog();
            cargar();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
=======

            }

        }

        private void btnAgregarCategoria_Click(object sender, EventArgs e)
        {
            frmAgregarCategoria agregar = new frmAgregarCategoria();
            agregar.ShowDialog();
            cargar();
        }

        private void btnModificarCategoria_Click(object sender, EventArgs e)
        {
            Categoria seleccionado;
            seleccionado = (Categoria)dgvCategorias.CurrentRow.DataBoundItem;

            frmAgregarCategoria modificar = new frmAgregarCategoria(seleccionado);
            modificar.ShowDialog();
            cargar();
        }

        private void btnEliminarCategoria_Click(object sender, EventArgs e)
>>>>>>> 792ff78601678c4d514f59504e6504e536469e3b
        {
            try
            {
                if (dgvCategorias.CurrentRow == null)
                    return;

<<<<<<< HEAD
                Categoria seleccionada = (Categoria)dgvCategorias.CurrentRow.DataBoundItem;

                var resp = MessageBox.Show(
                    "¿Eliminar físicamente la categoría seleccionada?",
=======
                Categoria seleccionado = (Categoria)dgvCategorias.CurrentRow.DataBoundItem;

                var resp = MessageBox.Show(
                    "¿Eliminar físicamente la categoria seleccionado?",
>>>>>>> 792ff78601678c4d514f59504e6504e536469e3b
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (resp == DialogResult.Yes)
                {
                    CategoriaNegocio negocio = new CategoriaNegocio();
<<<<<<< HEAD
                    negocio.eliminarFisico(seleccionada.Id);
=======
                    negocio.eliminar(seleccionado.Id);
>>>>>>> 792ff78601678c4d514f59504e6504e536469e3b
                    cargar();
                }
            }
            catch (Exception ex)
            {
<<<<<<< HEAD
                // Si la categoría está en uso, acá verás el mensaje del throw
                MessageBox.Show(ex.Message);
=======
                MessageBox.Show(ex.ToString());
>>>>>>> 792ff78601678c4d514f59504e6504e536469e3b
            }
        }
    }
}
