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
    public partial class InformeCinco : Form
    {
        public InformeCinco()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void InformeCinco_Load(object sender, EventArgs e)
        {
            Conexion.CargarGridInforme5(dataGridView2);
        }
    }
}
