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
    public partial class FormUlov : Form
    {
        SqlConnection konekcija = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\A10.mdf;Integrated Security=True");
        public FormUlov()
        {
            InitializeComponent();
        }

        private void FormUlov_Load(object sender, EventArgs e)
        {
            PopuniComboPecarosa();
        }
        private void PopuniComboPecarosa()
        {
            string upit = "SELECT PecarosID," +
                "CONCAT(PecarosID,' - ',Ime,' ',Prezime) AS Pec " +
                "FROM Pecaros";
            SqlCommand komanda= new SqlCommand(upit,konekcija);
            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(komanda);
            try
            {
                adapter.Fill(dt);
                comboBoxPecaros.DataSource = dt;
                comboBoxPecaros.DisplayMember = "Pec";
                comboBoxPecaros.ValueMember = "PecarosID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska: " + ex.Message);
            }

        }

        private void buttonIzadji_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonPrikazi_Click(object sender, EventArgs e)
        {
            string upit = "SELECT v.Naziv AS Vrsta, " +
                "COUNT(u.VrstaID) AS Broj " +
                "FROM Ulov AS u, Pecaros AS p, Vrsta_Ribe AS v " +
                "WHERE p.PecarosID=u.PecarosID " +
                "AND u.VrstaID=v.VrstaID " +
                "AND p.PecarosID=@pecarosID " +
                "AND u.Datum BETWEEN @datumOd AND @datumDo " +
                "GROUP BY v.Naziv";
            SqlCommand komanda = new SqlCommand(upit, konekcija);
            komanda.Parameters.AddWithValue("@pecarosID", comboBoxPecaros.SelectedValue);
            komanda.Parameters.AddWithValue("@datumOd", dateTimePickerOd.Value.Date);
            komanda.Parameters.AddWithValue("@datumDo", dateTimePickerDo.Value.Date);
            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(komanda);
            try
            {
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska: " + ex.Message);
            }
            finally
            {
                konekcija.Close();
            }
            dataGridView1.DataSource = dt;
            chart1.DataSource = dt;
            chart1.Series[0].XValueMember="Vrsta";
            chart1.Series[0].YValueMembers="Broj";
            chart1.Series[0].IsValueShownAsLabel=true;


        }
    }
}
