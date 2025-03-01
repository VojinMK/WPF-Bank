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
    /// Interaction logic for FrmTransakcija.xaml
    /// </summary>
    public partial class FrmTransakcija : Window
    {
        Konekcija kon = new Konekcija();
        SqlConnection konekcija = new SqlConnection();
        bool azuriraj;
        DataRowView pomocniRed;
        public FrmTransakcija()
        {
            InitializeComponent();
            PopuniPadajuceListe();
            txtIznos.Focus();
        }
        public FrmTransakcija(bool azuriraj, DataRowView pomocniRed)
        {
            InitializeComponent();
            PopuniPadajuceListe();
            txtIznos.Focus();
            this.azuriraj = azuriraj;
            this.pomocniRed = pomocniRed;
            
        }
        private void PopuniPadajuceListe()
        {
            try
            {
                konekcija = kon.KreirajKonekciju();
                konekcija.Open();

                string vratiBrojRacuna = @"select RačunID,BrojRačuna+' '+Ime+' '+Prezime as Račun 
                     from tblRačun join tblKlijent on tblRačun.KlijentID=tblKlijent.KlijentID";


                SqlDataAdapter daRacun = new SqlDataAdapter(vratiBrojRacuna, konekcija);
                DataTable dtRacun = new DataTable();
                daRacun.Fill(dtRacun);
                cbRacun.ItemsSource = dtRacun.DefaultView;
          

                daRacun.Dispose();
                dtRacun.Dispose();

                string vratiZaposlenog = @"select ZaposleniID, Ime+' '+Prezime+' '+JMBG as Zaposleni from tblZaposleni";

                SqlDataAdapter daZaposleni = new SqlDataAdapter(vratiZaposlenog, konekcija);
                DataTable dtZaposleni = new DataTable();
                daZaposleni.Fill(dtZaposleni);
                cbZaposleni.ItemsSource = dtZaposleni.DefaultView;

                daZaposleni.Dispose();
                dtZaposleni.Dispose();
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

                if(dpDatum.SelectedDate==null || txtTip.Text.Length==0 || txtIznos.Text.Length==0 || cbRacun.SelectedItem==null || cbZaposleni.SelectedItem == null)
                {
                    MessageBox.Show("Morate popuniti sva polja!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DateTime date = (DateTime)dpDatum.SelectedDate;
                string datum = date.ToString("dd.MM.yyyy");

                SqlCommand cmd = new SqlCommand { Connection = konekcija };
                cmd.Parameters.Add("@Datum", SqlDbType.Date).Value =datum;
                cmd.Parameters.Add("@Tip", SqlDbType.NVarChar).Value = txtTip.Text;
                cmd.Parameters.Add("@Iznos", SqlDbType.Real).Value =txtIznos.Text;
                cmd.Parameters.Add("@Racun", SqlDbType.Int).Value = cbRacun.SelectedValue;
                cmd.Parameters.Add("@Zaposleni", SqlDbType.Int).Value = cbZaposleni.SelectedValue;
                if (this.azuriraj)
                {
                    DataRowView red = this.pomocniRed;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = red["ID"];
                    cmd.CommandText = @"update tblTransakcija
                                set Datum=@Datum, TipTransakcije=@Tip, IznosTransakcije=@Iznos, RačunID=@Racun, ZaposleniID=@Zaposleni
                           where TransakcijaID=@id";
                    this.pomocniRed = null;
                }
                else
                {
                    cmd.CommandText = @"insert into tblTransakcija (Datum,TipTransakcije,IznosTransakcije, RačunID, ZaposleniID)
                       values (@Datum,@Tip,@Iznos,@Racun,@Zaposleni)";
                }
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                this.Close();
            }
            catch (SqlException)
            {

                MessageBox.Show("Greška pri čuvanju podataka!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (InvalidExpressionException)
            {
                MessageBox.Show("Niste izabrali datum!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (FormatException)
            {
                MessageBox.Show("Greška pri konverziji podataka!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
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
