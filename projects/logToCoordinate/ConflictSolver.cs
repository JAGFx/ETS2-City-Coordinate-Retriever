using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ETS2_Log_to_Coordinates
{
    public partial class ConflictSolver : Form
    {
        public List<string> uncheckedCities = new List<string>();

        public ConflictSolver()
        {
            InitializeComponent();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listCities.Items)
            {
                if (item.Checked == false)
                {
                    uncheckedCities.Add(item.Text);
                }
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ConflictSolver_Load(object sender, EventArgs e)
        {

        }
        private void ConflictSolver_FormClosing(object sender, EventArgs e)
        {
            if (DialogResult != DialogResult.OK)
            {
                DialogResult = DialogResult.Abort;
            }
            Application.Exit();
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            Application.Exit();
        }
    }
}
