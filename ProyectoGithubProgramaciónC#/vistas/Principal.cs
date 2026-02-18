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
    public partial class Principal : Form
    {
        public Principal()
        {
            InitializeComponent();
        }

        private void informe1_Click(object sender, EventArgs e)
        {
            InformeUno i = new InformeUno();
            i.ShowDialog();
        }

        private void informe2_Click(object sender, EventArgs e)
        {
            InformeDos i = new InformeDos();
            i.ShowDialog();
        }

        private void informe3_Click(object sender, EventArgs e)
        {
            InformeTres i = new InformeTres();
            i.ShowDialog();
        }

        private void informe4_Click(object sender, EventArgs e)
        {
            InformeCuatro i = new InformeCuatro();
            i.ShowDialog();
        }

        private void informe5_Click(object sender, EventArgs e)
        {
            InformeCinco i = new InformeCinco();
            i.ShowDialog();
        }
    }
}
