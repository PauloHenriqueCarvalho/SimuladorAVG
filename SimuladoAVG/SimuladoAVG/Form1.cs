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

            Filtros();
            CarregarPaletes();
            CarregarMapa();
            PainelTelemetria();
            AtualizarMapa();


        }

        public class RobosDTO
        {
            public int Id {  get; set; }
            public string Tipo {  get; set; }
            public decimal Bateria {  get; set; }
        }
        private void PainelTelemetria()
        {
            using(var c = new SimuladorAGVEntities())
            {
                var robos = c.Robos.Select(x => new RobosDTO
                {
                    Bateria = x.PorcentagemBateria,
                    Id = x.IdRobo,
                    Tipo = x.TiposRobo.NomeTipo
                }).ToList();

                dataGridView2.DataSource = robos;
            }
        }

        private void Filtros()
        {
            using (var c = new SimuladorAGVEntities())
            {
                var listaStatus = c.Paletes.Select(x => x.Status).Distinct().ToList();
                comboBox1.DataSource = listaStatus;

                var listaNivel = c.NiveisPeso.ToList();
                comboBox2.DataSource = listaNivel;
                comboBox2.DisplayMember = "NomeNivel";
                comboBox2.ValueMember = "IdNivelPeso";

                comboBox2.SelectedIndex = -1;
                comboBox1.SelectedIndex = -1;
            }
        }

        private void CarregarMetricas()
        {
            txtTotal.Text = Paletes.Count.ToString();
            txtPaletesDoca.Text = Paletes.Where(x => x.Status.Equals("Doca")).Count().ToString();
            using (var c = new SimuladorAGVEntities())
            {
                var ciclor = c.ControleTurno.FirstOrDefault();
                cliclo.Text = $"Ciclo: {ciclor.CicloAtual}";
                txtCiclo.Text = ciclor.CicloAtual.ToString();

                int robos = c.Robos.Where(x => x.PorcentagemBateria <= 10).Count();
                txtRobos.Text = robos.ToString();   
            }

        }

        TableLayoutPanel table = new TableLayoutPanel();
        List<PaletesDTO> Paletes  = new List<PaletesDTO>();
        private void CarregarMapa()
        {
            table.ColumnCount = 12;
            table.RowCount = 4;
            table.Dock = DockStyle.Fill;
            table.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            panel8.Controls.Add(table);
            table.Controls.Clear();

            for (int linha = 0; linha < 4; linha++)
            {
                for (int coluna = 0; coluna < 12; coluna++)
                {
                    Button celula = new Button();
                    celula.Height = table.Height / 5;
                    celula.Dock = DockStyle.Fill;
                    celula.Text = "0";
                    celula.Click += Celula_Click;
                    celula.Tag = new Posicao {linha= linha + 1, coluna= coluna +1 };

                    table.Controls.Add(celula, coluna, linha);
                }
            }




        }

        private void Celula_Click(object sender, EventArgs e)
        {
            var botao = sender as Button;
            Alocar(botao);
            Realocar(botao);
            Liberar(botao);

        }

        private void Liberar(Button botao)
        {
            if (botao.Text.Equals("0")) return;
            var posicao = botao.Tag as Posicao;
            if (MessageBox.Show("Deseja liberar o palete?", "Atenção", MessageBoxButtons.YesNo) == DialogResult.No) return;
            int id = int.Parse(botao.Text);
            using (var c = new SimuladorAGVEntities())
            {
                var palete = c.Paletes.FirstOrDefault(x => x.IdPalete == id);
                palete.Linha = null;
                palete.Coluna = null;
                palete.Status = "Alocado";
                c.SaveChanges();
                MessageBox.Show($"Carga {palete.Carga} de nível {palete.NiveisPeso.NomeNivel} em rota para entrega com Robô Autônomo!");
            }

            CarregarPaletes();
            CarregarMapa();
            AtualizarMapa();

        }

        private void Realocar(Button botao)
        {
            if (dataGridView1.SelectedRows.Count <= 0) return;
            var item = dataGridView1.SelectedRows[0].DataBoundItem as PaletesDTO;
            if (item == null) return;
            if (!item.Status.Equals("Alocado")) return;
            var posicao = botao.Tag as Posicao;


            if (botao.Text.Equals("0"))
            {
                using (var c = new SimuladorAGVEntities())
                {
                    var palete = c.Paletes.FirstOrDefault(x => x.IdPalete == item.Id);
                    palete.Linha = byte.Parse(posicao.linha.ToString());
                    palete.Coluna = byte.Parse(posicao.coluna.ToString());
                    palete.Status = "Alocado";
                    c.SaveChanges();
                }

                CarregarPaletes();
                CarregarMapa();
                AtualizarMapa();
            }
            else
            {
                MessageBox.Show("Posicao ja Alocada.");
            }

        }

        private void Alocar(Button botao)
        {
            if (dataGridView1.SelectedRows.Count <= 0) return;
            var item = dataGridView1.SelectedRows[0].DataBoundItem as PaletesDTO;
            if(item == null) return;
            if (!item.Status.Equals("Doca")) return;
            var posicao = botao.Tag as Posicao;

            if (botao.Text.Equals("0"))
            {
                using(var c = new SimuladorAGVEntities())
                {
                    var palete = c.Paletes.FirstOrDefault( x=> x.IdPalete ==item.Id);
                    palete.Linha = byte.Parse(posicao.linha.ToString()) ;
                    palete.Coluna = byte.Parse(posicao.coluna.ToString());
                    palete.Status = "Alocado";
                    c.SaveChanges();
                }

                CarregarPaletes();
                CarregarMapa();
                AtualizarMapa();
            }
            else
            {
                MessageBox.Show("Posicao ja Alocada.");
            }

        }

        private void AtualizarMapa()
        {
            CarregarMetricas();
            foreach (Control controle in table.Controls)
            {
                Button celula = (Button)controle;

                Posicao posicao = (Posicao)celula.Tag;

                var palete = Paletes.FirstOrDefault(p =>
                    p.Posicao != null &&
                    p.Linha == posicao.linha &&
                    p.Coluna == posicao.coluna);

                if (palete == null)
                {
                    celula.Text = "0";
                }
                else
                {
                    celula.Text = palete.Id.ToString();
                }
            }
        }
        private void CarregarPaletes()
        {
            using(var c = new SimuladorAGVEntities())
            {
                var palete = c.Paletes.Select(x => new PaletesDTO
                {
                    Posicao = (x.Linha == null || x.Coluna == null) ? "Não alocado"
                    : "L"+x.Linha+"C"+x.Coluna,
                    Id = x.IdPalete,
                    Descricao = x.Carga,
                    NivelPeso = x.NiveisPeso.NomeNivel,
                    Status = x.Status,
                    RecebidoEm = x.RecebidoEm,
                    Coluna = x.Coluna,
                    Linha = x.Linha
                   
                }).ToList();
                palete = palete.OrderByDescending(x => x.Status.Equals("Doca"))
                    .ThenByDescending(x => x.NivelPeso.Equals("Pesada"))
                    .ThenByDescending(x => x.NivelPeso.Equals("Media"))
                    .ThenByDescending(x => x.NivelPeso.Equals("Leve"))
                    .ThenByDescending(x => x.RecebidoEm)
                    .ToList();

                if(!string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    var newli = palete.Where(x => x.Descricao.ToLower().Contains(textBox1.Text.ToLower())).ToList();
                    if (newli.Count == 0)
                    {
                        if (int.TryParse(textBox1.Text, out int n))
                        {
                            newli = palete.Where(x => x.Id == n).ToList();
                        }
                        if (newli.Count == 0) palete = newli;
                    }
                    else
                    {
                        palete = newli;

                    }

                }

                if(comboBox1.SelectedIndex >= 0)
                {
                    var nivel = comboBox1.SelectedItem as string;

                    palete = palete.Where(x => x.Status.Equals(nivel)).ToList();
                }
                if (comboBox2.SelectedIndex >= 0)
                {
                    var nivel = comboBox2.SelectedItem as NiveisPeso;
                    palete = palete.Where(x => x.NivelPeso.Equals(nivel.NomeNivel)).ToList();
                }


                Paletes = palete;
                dataGridView1.DataSource = palete;
                Esconder("Linha");
                Esconder("Coluna");
                Esconder("RecebidoEm");
            }
        }

        private void Esconder(string v)
        {
            if (dataGridView1.Columns[v] != null) dataGridView1.Columns[v].Visible = false;
        }

        public class PaletesDTO
        {
            public int Id {  get; set; }
            public string Descricao {  get; set; }
            public string NivelPeso {  get; set; }
    
            public string Status { get; set; }
    
            public string Posicao { get; set; }
            public Nullable<byte> Linha {  get; set; }
            public Nullable<byte> Coluna {  get; set; }

            public System.DateTime RecebidoEm { get; set; }
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

        private void cliclo_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            using(var modal = new TelaNovaCarga())
            {
                modal.ShowDialog();
            }
            CarregarPaletes();
            CarregarMapa();

            AtualizarMapa();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using(var c = new SimuladorAGVEntities())
            {
                var ciclo = c.ControleTurno.FirstOrDefault();
                ciclo.CicloAtual++;

                var listaRobos = c.Robos.ToList();

                foreach (var item in listaRobos)
                {
                    if (item.PorcentagemBateria - item.TiposRobo.ConsumoBateriaPorCiclo <= 0) continue;
                    item.PorcentagemBateria -= item.TiposRobo.ConsumoBateriaPorCiclo;
                }

                c.SaveChanges();
            }
            CarregarMetricas();
            PainelTelemetria();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            CarregarPaletes();
        }

        private void comboBox2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            CarregarPaletes();
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            CarregarPaletes();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            comboBox2.SelectedIndex = -1;
            comboBox1.SelectedIndex = -1;
            textBox1.Text = "";
            CarregarPaletes();
        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

            if (e.RowIndex < 0) return;
            if (e.ColumnIndex < 0) return;
            var robo = dataGridView2.Rows[e.RowIndex].DataBoundItem as RobosDTO;
            if(robo.Bateria <= 10)
            {
                dataGridView2.Rows[e.RowIndex].DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#DC2626");
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count == 0)
            {
                MessageBox.Show("selecione um robo.");
                return;
            }

            var robo = dataGridView2.SelectedRows[0].DataBoundItem as RobosDTO;
            using(var c = new SimuladorAGVEntities())
            {
                var roboR = c.Robos.FirstOrDefault(x => x.IdRobo == robo.Id);
                roboR.PorcentagemBateria = 100;

                var listaRobos = c.Robos.Where(x => x.IdRobo != robo.Id).ToList();

                foreach (var item in listaRobos)
                {
                    item.PorcentagemBateria -= item.TiposRobo.ConsumoBateriaPorCiclo;
                }


                var ciclo = c.ControleTurno.FirstOrDefault();
                ciclo.CicloAtual++;
                c.SaveChanges();
            }

            CarregarMetricas();
            PainelTelemetria();
        }
    }
}
