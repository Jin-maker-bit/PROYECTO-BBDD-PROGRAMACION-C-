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
    public partial class InformeDos : Form
    {
        public InformeDos()
        {
            InitializeComponent();
            Conexion.CargarGridInforme2_Vendedores(dataGridView3);
            Conexion.CargarGridInforme2_LibrosPlataformas(dataGridView4);
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
