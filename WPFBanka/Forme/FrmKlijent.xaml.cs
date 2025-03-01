using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFBanka.Forme
{
    /// <summary>
    /// Interaction logic for FrmKlijent.xaml
    /// </summary>
    public partial class FrmKlijent : Window
    {
        Konekcija kon = new Konekcija();
        SqlConnection konekcija = new SqlConnection();
        DataRowView pomocniRed;
        bool azuriraj;
        public FrmKlijent()
        {
            InitializeComponent();
            txtJmbg.Focus();
            konekcija = kon.KreirajKonekciju();
        }
        public FrmKlijent(bool azuriraj, DataRowView pomocniRed)
        {
            InitializeComponent();
            txtIme.Focus();
            this.azuriraj = azuriraj;
            this.pomocniRed = pomocniRed;
            konekcija = kon.KreirajKonekciju();
        }


        private void btnOtkazi_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSacuvaj_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                konekcija.Open();
                SqlCommand cmd = new SqlCommand { Connection = konekcija };
                if (txtIme.Text.Length==0 || txtPrezime.Text.Length==0 || txtJmbg.Text.Length==0 || txtGrad.Text.Length==0 || txtAdresa.Text.Length==0 || txtKontakt.Text.Length == 0)
                {
                    MessageBox.Show("Morate popuniti sva polja!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                cmd.Parameters.Add("@Ime", System.Data.SqlDbType.NVarChar).Value = txtIme.Text;
                cmd.Parameters.Add("@Prezime", System.Data.SqlDbType.NVarChar).Value = txtPrezime.Text;
                cmd.Parameters.Add("@JMBG", System.Data.SqlDbType.NVarChar).Value = txtJmbg.Text;
                cmd.Parameters.Add("@Grad", System.Data.SqlDbType.NVarChar).Value = txtGrad.Text;
                cmd.Parameters.Add("@Adresa", System.Data.SqlDbType.NVarChar).Value = txtAdresa.Text;
                cmd.Parameters.Add("@Kontakt", System.Data.SqlDbType.NVarChar).Value = txtKontakt.Text;


                if (this.azuriraj)
                {
                    DataRowView red = this.pomocniRed;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = red["ID"];
                    cmd.CommandText = @"update tblKlijent
                           set Ime=@ime, Prezime=@Prezime, JMBG=@JMBG, Grad=@Grad, Adresa=@Adresa, Kontakt=@Kontakt
                          where KlijentID=@id";
                    this.pomocniRed = null;
                }
                else
                {

                    cmd.CommandText = @"insert into tblKlijent (Ime, Prezime, JMBG, Grad, Adresa, Kontakt)
                      values(@Ime,@Prezime,@JMBG,@Grad,@Adresa,@Kontakt)";
                }
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                this.Close();
            }
            catch (SqlException)
            {

                MessageBox.Show("Greška pri čuvanju podataka!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (konekcija != null)
                {
                    konekcija.Close();
                }
            }
        }
    }
}
