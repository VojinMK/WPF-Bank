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
    /// Interaction logic for FrmKartica.xaml
    /// </summary>
    public partial class FrmKartica : Window
    {
        Konekcija kon = new Konekcija();
        SqlConnection konekcija = new SqlConnection();
        DataRowView pomocniRed;
        bool azuriraj;
        public FrmKartica()
        {                                                       
            InitializeComponent();
            PopuniPadajuceListe();
            cbRacun.Focus();
           
        }
        public FrmKartica(bool azuriraj, DataRowView pomocniRed)
        {
            InitializeComponent();
            PopuniPadajuceListe();
            cbRacun.Focus();
            this.azuriraj = azuriraj;
            this.pomocniRed = pomocniRed;

        }
        private void PopuniPadajuceListe()
        {
            try
            {
                konekcija = kon.KreirajKonekciju();
                konekcija.Open();


                string vratiBrojRacuna = @"select RačunID, BrojRačuna+' '+Ime+' '+Prezime as 'BrojRačuna'
                                           from tblRačun join tblKlijent on tblRačun.KlijentID=tblKlijent.KlijentID;";


                SqlDataAdapter dataAdapter = new SqlDataAdapter(vratiBrojRacuna, konekcija);
                DataTable dtRacun = new DataTable();
                dataAdapter.Fill(dtRacun);
                cbRacun.ItemsSource = dtRacun.DefaultView;

                dataAdapter.Dispose();
                dtRacun.Dispose();

                string vratiTipKartice = @"select TipKarticeID, NazivTipaKartice from tblTipKartice";

                SqlDataAdapter daTipKartice = new SqlDataAdapter(vratiTipKartice, konekcija);
                DataTable dtTipKartice = new DataTable();
                daTipKartice.Fill(dtTipKartice);
                cbTipKartice.ItemsSource = dtTipKartice.DefaultView;

                daTipKartice.Dispose();
                dtTipKartice.Dispose();


            }
            catch (SqlException)
            {
                MessageBox.Show("Podaci nisu učitani!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (konekcija != null)
                {
                    konekcija.Close();
                }
            }
        }
        private void btnSacuvaj_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                konekcija = kon.KreirajKonekciju();
                konekcija.Open();
                SqlCommand cmd = new SqlCommand { Connection = konekcija };
                if(cbRacun.SelectedItem==null || cbTipKartice.SelectedItem == null)
                {
                    MessageBox.Show("Morate popuniti sva polja!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                cmd.Parameters.Add("@Racun", SqlDbType.Int).Value = cbRacun.SelectedValue;
                cmd.Parameters.Add("@TipKartice", SqlDbType.Int).Value = cbTipKartice.SelectedValue;
                if (this.azuriraj)
                {
                    DataRowView red = this.pomocniRed;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = red["ID"];
                    cmd.CommandText = @"update tblKartica 
                    set RačunID=@Racun, TipKarticeID=@TipKartice where KarticaID=@id";
                    this.pomocniRed = null;
                }
                else
                {
                    cmd.CommandText = @"insert into tblKartica (RačunID,TipKarticeID)
                  values(@Racun,@TipKartice)";
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

        private void btnOtkazi_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
