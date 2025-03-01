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
    /// Interaction logic for FrmRacun.xaml
    /// </summary>
    public partial class FrmRacun : Window
    {
        Konekcija kon = new Konekcija();
        SqlConnection konekcija = new SqlConnection();
        DataRowView pomocniRed;
        bool azuriraj;
        public FrmRacun()
        {
            InitializeComponent();
            PopuniPadajuceListe();
            txtStanjeNaRacunu.Focus();
        }
        public FrmRacun(bool azuriraj, DataRowView pomocniRed)
        {
            InitializeComponent();
            txtBrojRacuna.Focus();
            PopuniPadajuceListe();
            this.azuriraj = azuriraj;
            this.pomocniRed = pomocniRed;
       
        }
        private void PopuniPadajuceListe()
        {
            try
            {
                konekcija = kon.KreirajKonekciju();
                konekcija.Open();

                string vratiKlijente = @"select KlijentID, Ime+' '+Prezime+' '+JMBG as Klijent from tblKlijent";

                SqlDataAdapter daKlijent = new SqlDataAdapter(vratiKlijente, konekcija);
                DataTable dtKlijent = new DataTable();
                daKlijent.Fill(dtKlijent);
                cbKlijent.ItemsSource = dtKlijent.DefaultView;
                daKlijent.Dispose();
                dtKlijent.Dispose();


                string vratiTipoveRacuna = @"select TipRačunaID, NazivTipaRačuna from tblTipRačuna";

                SqlDataAdapter daTipRacuna = new SqlDataAdapter(vratiTipoveRacuna, konekcija);
                DataTable dtTipRacuna = new DataTable();
                daTipRacuna.Fill(dtTipRacuna);
                cbTipRacuna.ItemsSource = dtTipRacuna.DefaultView;
                daTipRacuna.Dispose();
                dtTipRacuna.Dispose();
            }
            catch(SqlException)
            {
                MessageBox.Show("Podaci nisu učitani!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if(konekcija!=null)
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
                if(txtBrojRacuna.Text.Length==0 || txtStanjeNaRacunu.Text.Length==0 || cbKlijent.SelectedItem==null || cbTipRacuna.SelectedItem == null)
                {
                    MessageBox.Show("Morate popuniti sva polja!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                cmd.Parameters.Add("@StanjeNaRacunu", SqlDbType.Real).Value = txtStanjeNaRacunu.Text;
                cmd.Parameters.Add("@BrojRacuna", SqlDbType.NVarChar).Value = txtBrojRacuna.Text;
                cmd.Parameters.Add("@Klijent", SqlDbType.Int).Value = cbKlijent.SelectedValue;
                cmd.Parameters.Add("@TipRacuna", SqlDbType.Int).Value = cbTipRacuna.SelectedValue;
                if(this.azuriraj)
                {
                    DataRowView red = this.pomocniRed;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = red["ID"];
                    cmd.CommandText = @"update tblRačun
                         set StanjeNaRačunu=@StanjeNaRacunu, BrojRačuna=@BrojRacuna, KlijentID=@Klijent, TipRačunaID=@TipRacuna
                          where RačunID=@id";

                    this.pomocniRed = null;
                }
                else
                {
                    cmd.CommandText = @"insert into tblRačun (StanjeNaRačunu,BrojRačuna,KlijentID,TipRačunaID)
                                   values(@StanjeNaRacunu,@BrojRacuna,@Klijent,@TipRacuna)";
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
