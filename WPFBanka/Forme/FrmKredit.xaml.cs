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
    /// Interaction logic for FrmKredit.xaml
    /// </summary>
    public partial class FrmKredit : Window
    {
        Konekcija kon = new Konekcija();
        SqlConnection konekcija = new SqlConnection();
        DataRowView pomocniRed;
        bool azuriraj;
        public FrmKredit()
        {
            InitializeComponent();
            PopuniPadajuceListe();
            txtKamata.Focus();
        }
        public FrmKredit(bool azuriraj, DataRowView pomocniRed)
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


                SqlDataAdapter daRačun = new SqlDataAdapter(vratiBrojRacuna, konekcija);
                DataTable dtRacun = new DataTable();
                daRačun.Fill(dtRacun);
                cbRacun.ItemsSource = dtRacun.DefaultView;

                daRačun.Dispose();
                dtRacun.Dispose();
            }
            catch
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

                if(txtIznos.Text.Length==0 || txtKamata.Text.Length==0 || txtMesecnaRata.Text.Length==0 || cbRacun.SelectedItem==null || dpPocetakOtplacivanja.SelectedDate==null || dpZavrsetakOtplacivanja.SelectedDate == null)
                {
                    MessageBox.Show("Morate popuniti sva polja!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                DateTime date =(DateTime) dpPocetakOtplacivanja.SelectedDate;
                string datumPO = date.ToString("dd.MM.yyyy");

                DateTime date1 = (DateTime)dpZavrsetakOtplacivanja.SelectedDate;
                string datumZO = date1.ToString("dd.MM.yyyy");

                SqlCommand cmd = new SqlCommand { Connection = konekcija };
                cmd.Parameters.Add("@Iznos", SqlDbType.Real).Value = txtIznos.Text;
                cmd.Parameters.Add("@Kamata", SqlDbType.NVarChar).Value = txtKamata.Text;
                cmd.Parameters.Add("@MesecnaRata", SqlDbType.Real).Value = txtMesecnaRata.Text;
                cmd.Parameters.Add("@PocetakOtplacivanja", SqlDbType.Date).Value = datumPO;
                cmd.Parameters.Add("@ZavrsetakOtplacivanja", SqlDbType.Date).Value = datumZO;
                cmd.Parameters.Add("@RacunID", SqlDbType.Int).Value =cbRacun.SelectedValue;
                if(this.azuriraj)
                {
                    DataRowView red = this.pomocniRed;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = red["ID"];
                    cmd.CommandText = @"update tblKredit
                          set IznosKredita=@Iznos, KamataKredita=@Kamata, MesečnaRata=@MesecnaRata, PočetakOtplaćivanja=@PocetakOtplacivanja, ZavršetakOtplaćivanja=@ZavrsetakOtplacivanja, RačunID=@RacunID where KreditID=@id";
                    this.pomocniRed = null;
                }
                else
                {
                    cmd.CommandText = @"insert into tblKredit (IznosKredita, KamataKredita, MesečnaRata, PočetakOtplaćivanja, ZavršetakOtplaćivanja, RačunID)
                             values(@Iznos,@Kamata,@MesecnaRata,@PocetakOtplacivanja,@ZavrsetakOtplacivanja,@RacunID)";
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
                MessageBox.Show("Greška prilikom konverzije podataka!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
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
