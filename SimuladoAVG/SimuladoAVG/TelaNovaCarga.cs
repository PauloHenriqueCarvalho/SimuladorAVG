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
    public partial class TelaNovaCarga : Form
    {
        public TelaNovaCarga()
        {
            InitializeComponent();
            this.BackColor = ColorTranslator.FromHtml("#F0F4FF");
            this.StartPosition = FormStartPosition.CenterScreen;
            button1.BackColor = ColorTranslator.FromHtml("#1B3A6B");
            button2.BackColor = ColorTranslator.FromHtml("#1B3A6B");
        }

        private void TelaNovaCarga_Load(object sender, EventArgs e)
        {
            using (var c = new SimuladorAGVEntities())
            {
                var lista = c.NiveisPeso.ToList();
                comboBox1.DataSource = lista;
                comboBox1.DisplayMember = "NomeNivel";
                comboBox1.ValueMember = "IdNivelPeso";
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || comboBox1.SelectedIndex < 0 || numericUpDown1.Value == 0)
            {
                MessageBox.Show("Preencha todos os campos corretamente!");
                return;
            }

            var nivel = comboBox1.SelectedItem as NiveisPeso;



            var random = new Random(200);
            string cod = "PT" + random.Next(999);
            while (!ValidarCodigo(cod))
            {
                cod = "PT" + random.Next(999);
                ValidarCodigo(cod);
            }
        

            using(var c = new SimuladorAGVEntities())
            {
               

                var palete = new Paletes
                {
                    IdNivelPeso = nivel.IdNivelPeso,
                    Carga = textBox1.Text,
                    Status = "Doca",
                    RecebidoEm = DateTime.Now,
                    CodigoPalete = cod,

                };
                c.Paletes.Add(palete);
                c.SaveChanges();
            }
            MessageBox.Show("Salvo com sucesso!");
            this.Close();

        }

        private bool ValidarCodigo(string cod)
        {
            using(var c = new SimuladorAGVEntities())
            {
                if (c.Paletes.FirstOrDefault(x => x.CodigoPalete == cod) != null)
                {
                    return false;
                }
                return true;

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            comboBox1.SelectedIndex = 0;
            numericUpDown1.Value = 0;
        }
    }
}
