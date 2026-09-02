using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimuladoAVG
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            panel1.BackColor = ColorTranslator.FromHtml("#1B3A6B");
            panel2.BackColor = ColorTranslator.FromHtml("#1B3A6B");
            button1.BackColor = ColorTranslator.FromHtml("#F59E0B");
            button2.BackColor = ColorTranslator.FromHtml("#F59E0B");
            button3.BackColor = ColorTranslator.FromHtml("#F59E0B");

            this.BackColor = ColorTranslator.FromHtml("#F0F4FF");
            this.StartPosition = FormStartPosition.CenterScreen;
            //Tema(this);
        }

        //private void Tema(Control c)
        //{
        //   if(c is Label l)
        //    {
        //        l.ForeColor = ColorTranslator.FromHtml("#0F172A");
        //    }
        //    foreach (Control item in c.Controls)
        //    {
        //        Tema(item);
        //    }

        //}

        private void Form1_Load(object sender, EventArgs e)
        {
            var table = new TableLayoutPanel();
            table.ColumnCount = 12;
            table.RowCount = 4;
            table.Dock = DockStyle.Fill;
            table.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            panel8.Controls.Add(table);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
