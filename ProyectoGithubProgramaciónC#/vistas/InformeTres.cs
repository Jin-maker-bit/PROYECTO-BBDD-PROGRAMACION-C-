using ProyectoGithubProgramaciónC_.bbdd;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoGithubProgramaciónC_
{
    public partial class InformeTres : Form
    {
        private bool cargando = true;

        public InformeTres()
        {
            InitializeComponent();

            
            comboSeccion.SelectedIndexChanged += comboSeccion_SelectedIndexChanged;
            this.Load += InformeTres_Load;
        }

        private void InformeTres_Load(object sender, EventArgs e)
        {
            cargando = true;

            comboSeccion.Items.Clear();
            comboSeccion.DropDownStyle = ComboBoxStyle.DropDownList;

            comboSeccion.Items.Add("Seleccione...");
            for (int i = 1; i <= 9; i++)
                comboSeccion.Items.Add(i);

            comboSeccion.SelectedIndex = 0;

            
            dataGridView2.DataSource = null;

            cargando = false;
        }

        private void comboSeccion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cargando) return;

            
            if (comboSeccion.SelectedIndex == 0)
            {
                dataGridView2.DataSource = null;
                return;
            }

            int seccion = Convert.ToInt32(comboSeccion.SelectedItem);
            Conexion.CargarGridInforme3(dataGridView2, seccion);
        }

        
    }
}
