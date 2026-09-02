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
           
            using(var c = new SimuladorAGVEntities())
            {
                var ciclor = c.ControleTurno.FirstOrDefault();
                cliclo.Text = $"Ciclo: {ciclor.CicloAtual}";
            }

            CarregarPaletes();
            CarregarMapa();

            AtualizarMapa();

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
                dataGridView1.DataSource = palete;
                Esconder("Linha");
                Esconder("Coluna");
                Esconder("RecebidoEm");
                Paletes = palete;
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
        }
    }
}
