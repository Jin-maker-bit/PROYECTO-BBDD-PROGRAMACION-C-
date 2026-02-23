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
    public partial class InformeCuatro : Form
    {
        public InformeCuatro()
        {
            InitializeComponent();
        }

        private void InformeCuatro_Load(object sender, EventArgs e)
        {
            Conexion.CargarGridInforme4(dataGridView2);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
