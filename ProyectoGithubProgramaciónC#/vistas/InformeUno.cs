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
    public partial class InformeUno : Form
    {
        public InformeUno()
        {
            InitializeComponent();
        }

        private void InformeUno_Load(object sender, EventArgs e)
        {
            Conexion.CargarGridInforme1(dataGridView1);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
