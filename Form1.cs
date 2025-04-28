using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace A10a
{
    public partial class Form1 : Form
    {
        SqlConnection konekcija = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\A10.mdf;Integrated Security=True");
        DataTable dt = new DataTable();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PopuniComboGradova();
            OsveziListuPecarosa();

        }
        private void PopuniComboGradova()
        {
            DataTable dtg = new DataTable();
            string upit = "SELECT g.GradID AS Sifra, g.Grad AS Naziv " +
                "FROM Grad AS g";
            SqlDataAdapter dag = new SqlDataAdapter(upit,konekcija);
            try
            {
                dag.Fill(dtg);
                comboBoxGrad.DataSource = dtg;
                comboBoxGrad.ValueMember = "Sifra";
                comboBoxGrad.DisplayMember = "Naziv";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška " + ex.Message);
            }
            finally
            {
                konekcija.Close();
            }
        }
        private void OsveziListuPecarosa()
        {
            string upit =
                "SELECT p.PecarosID,p.Ime,p.Prezime,p.Adresa,g.Grad,p.Telefon " +
                "FROM Pecaros AS p, Grad AS g " +
                "WHERE p.GradID=g.GradID";
            SqlDataAdapter adapter = new SqlDataAdapter(upit, konekcija);
            try
            {
                dt.Clear();
                listBox1.Items.Clear();
                adapter.Fill(dt);
                foreach (DataRow red in dt.Rows)
                {
                    string pec = String.Format(
                        "{0,-3}{1,-15}{2,-15}{3,-20}{4,-15}{5,-15}", 
                        red[0], red[1], red[2], red[3], red[4], red[5]
                        );
                    listBox1.Items.Add(pec);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška " + ex.Message);
            }
            finally
            {
                konekcija.Close();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int sel= listBox1.SelectedIndex;
            if (sel == -1) 
                IsprazniKontrole();
            else
            {
                DataRow red = dt.Rows[sel];
                textBoxSifra.Text = red[0].ToString();
                textBoxIme.Text = red[1].ToString();
                textBoxPrezime.Text = red[2].ToString();
                textBoxAdresa.Text = red[3].ToString();
                comboBoxGrad.Text = red[4].ToString();
                textBoxTelefon.Text = red[5].ToString();
            }

        }
        private void IsprazniKontrole()
        {
            textBoxSifra.Text = "";
            textBoxIme.Text = "";
            textBoxPrezime.Text = "";
            textBoxAdresa.Text = "";
            comboBoxGrad.SelectedIndex = -1;
            textBoxTelefon.Text = "";
        }

        private void toolStripButtonIzmena_Click(object sender, EventArgs e)
        {
            if(textBoxSifra.Text == "")
            {
                MessageBox.Show("Nema selektovanog pecaroša!");
                return;
            }
            if(textBoxIme.Text == "" || textBoxPrezime.Text == "" 
                || textBoxAdresa.Text == "" || 
                comboBoxGrad.SelectedIndex == -1 || 
                textBoxTelefon.Text == "")
            {
                MessageBox.Show("Nisu popunjena sva polja!");
                return;
            }
            string upit = "UPDATE Pecaros " +
                "SET Ime=@ime, Prezime=@prezime, Adresa=@adresa, GradID=@grad, Telefon=@telefon " +
                "WHERE PecarosID=@pecarosID";
            SqlCommand komanda = new SqlCommand(upit, konekcija);
            komanda.Parameters.AddWithValue("@ime", textBoxIme.Text);
            komanda.Parameters.AddWithValue("@prezime", textBoxPrezime.Text);
            komanda.Parameters.AddWithValue("@adresa", textBoxAdresa.Text);
            komanda.Parameters.AddWithValue("@grad", comboBoxGrad.SelectedValue);
            komanda.Parameters.AddWithValue("@telefon", textBoxTelefon.Text);
            komanda.Parameters.AddWithValue("@pecarosID", textBoxSifra.Text);
            try
            {
                konekcija.Open();
                komanda.ExecuteNonQuery();
                MessageBox.Show("Uspešno izmenjen pecaroš!");
                int sel = listBox1.SelectedIndex;   
                OsveziListuPecarosa();
                listBox1.SelectedIndex = sel;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška " + ex.Message);
            }
            finally
            {
                konekcija.Close();
            }
        }

        private void toolStripButtonAnaliza_Click(object sender, EventArgs e)
        {
            FormUlov frm = new FormUlov();
            frm.ShowDialog();
        }
    }
}
