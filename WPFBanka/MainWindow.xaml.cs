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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFBanka.Forme;

namespace WPFBanka
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string ucitanaTabela;
        bool azuriraj;
        Konekcija kon = new Konekcija();
        SqlConnection konekcija = new SqlConnection();
        #region Select upiti
        static string klijentiSelect = @"select KlijentID as ID, Ime, Prezime, JMBG as 'Matični broj', Grad, Adresa, Kontakt from tblKlijent";
        static string zaposleniSelect = @"select ZaposleniID as ID, Ime,Prezime, SektorRada as 'Sektor rada', JMBG as 'Matični broj', Adresa, Kontakt from tblZaposleni";
        static string racuniSelect = @"select RačunID as ID, StanjeNaRačunu as 'Stanje na računu', BrojRačuna as 'Broj računa', Ime+' '+Prezime+' '+JMBG as 'Klijent', NazivTipaRačuna as 'Tip računa'
                                       from tblRačun join tblKlijent on tblRačun.KlijentID=tblKlijent.KlijentID
                                                     join tblTipRačuna on tblRačun.TipRačunaID=tblTipRačuna.TipRačunaID;";
        static string tipoviRacunaSelect = @"select TipRačunaID as ID, NazivTipaRačuna as 'Naziv tipa računa' from tblTipRačuna";
        static string transakcijeSelect = @"select TransakcijaID as ID, Datum, TipTransakcije as 'Tip transakcije', IznosTransakcije as 'Iznos transakcije', BrojRačuna as 'Račun', Ime+' '+Prezime as 'Zaposleni-Izvršilac'
                                 from tblTransakcija join tblRačun on tblTransakcija.RačunID=tblRačun.RačunID
                                 join tblZaposleni on tblTransakcija.ZaposleniID=tblZaposleni.ZaposleniID;";
        static string kreditiSelect = @"select KreditID as ID, IznosKredita as 'Iznos kredita', KamataKredita as 'Kamata kredita', MesečnaRata as 'Mesečna rata', PočetakOtplaćivanja as 'Početak otplaćivanja', ZavršetakOtplaćivanja as 'Završetak otplaćivanja', BrojRačuna as 'Račun'
                                        from tblKredit join tblRačun on  tblKredit.RačunID=tblRačun.RačunID;";
        static string karticeSelect = @"Select KarticaID as ID, BrojRačuna as 'Broj računa', NazivTipaKartice as 'Tip kartice'
 from tblKartica join tblRačun on tblKartica.RačunID=tblRačun.RačunID
                 join tblTipKartice on tblKartica.TipKarticeID=tblTipKartice.TipKarticeID;";
        static string tipoviKarticeSelect = @"select TipKarticeID as ID, NazivTipaKartice as 'Tip kartice' from tblTipKartice";
        #endregion
        #region Select sa uslovima
        string selectUslovKlijenti = @"select * from tblKlijent where KlijentID=";
        string selectUslovZaposleni = @"select * from tblZaposleni where ZaposleniID=";
        string selectUslovRacuni = @"select * from tblRačun where RačunID=";
        string selectUslovTipoviRacuna = @"select * from tblTipRačuna where TipRačunaID=";
        string selectUslovTransakcije = @"select * from tblTransakcija where TransakcijaID=";
        string selectUslovKrediti = @"select * from tblKredit where KreditID=";
        string selectUslovKartice = @"select * from tblKartica where KarticaID=";
        string selectUslovTipoviKartice = @"select * from tblTipKartice where TipKarticeID=";
        #endregion
        #region Delete upiti
        static string deleteKlijenti = @"delete from tblKlijent where KlijentID=";
        static string deleteZaposleni = @"delete from tblZaposleni where ZaposleniID=";
        static string deleteRacuni = @"delete from tblRačun where RačunID=";
        static string deleteTipoviRacuna = @"delete from tblTipRačuna where TipRačunaID=";
        static string deleteTransakcije = @"delete from tblTransakcija where TransakcijaID=";
        static string deleteKrediti = @"delete from tblKredit where KreditID=";
        static string deleteKartice = @"delete from tblKartica where KarticaID=";
        static string deleteTipoviKartice = @"delete from tblTipKartice where TipKarticeID=";
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            konekcija = kon.KreirajKonekciju();
            btnKartice.Focus();
            UcitajPodatke(dataGridCentralni, klijentiSelect);
        }
        private void UcitajPodatke(DataGrid grid, string selectUpit)
        {
            try
            {
                konekcija.Open();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(selectUpit, konekcija);
                DataTable dt = new DataTable();
                dataAdapter.Fill(dt);
                if (grid != null)
                {
                    grid.ItemsSource = dt.DefaultView;
                }
                ucitanaTabela = selectUpit;
                dataAdapter.Dispose();
                dt.Dispose();
            }
            catch
            {
                MessageBox.Show("Podaci ne mogu da se očitaju!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if(konekcija!=null)
                {
                    konekcija.Close();
                }
            }

        }

        private void btnKlijenti_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentralni, klijentiSelect);
        }

        private void btnZaposleni_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentralni, zaposleniSelect);
        }

        private void btnRacuni_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentralni, racuniSelect);
        }

        private void btnTipoviRacuna_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentralni, tipoviRacunaSelect);
        }

        private void btnTransakcije_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentralni, transakcijeSelect);
        }

        private void btnKrediti_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentralni, kreditiSelect);
        }

        private void btnKartice_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentralni, karticeSelect);
        }

        private void btnTipoviKartice_Click(object sender, RoutedEventArgs e)
        {
            UcitajPodatke(dataGridCentralni, tipoviKarticeSelect);
        }

        private void btnDodaj_Click(object sender, RoutedEventArgs e)
        {
            Window prozor;
            if(ucitanaTabela.Equals(klijentiSelect))
            {
                prozor = new FrmKlijent();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentralni, klijentiSelect);
            }
            else if (ucitanaTabela.Equals(zaposleniSelect))
            {
                prozor = new FrmZaposleni();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentralni, zaposleniSelect);
            }
            else if(ucitanaTabela.Equals(racuniSelect))
            {
                prozor = new FrmRacun();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentralni, racuniSelect);
            }
            else if (ucitanaTabela.Equals(tipoviRacunaSelect))
            {
                prozor = new FrmTipRacuna();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentralni, tipoviRacunaSelect);
            }
            else if (ucitanaTabela.Equals(transakcijeSelect))
            {
                prozor = new FrmTransakcija();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentralni, transakcijeSelect);
            }
            else if (ucitanaTabela.Equals(kreditiSelect))
            {
                prozor = new FrmKredit();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentralni, kreditiSelect);
            }
            else if (ucitanaTabela.Equals(karticeSelect))
            {
                prozor = new FrmKartica();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentralni, karticeSelect);
            }
            else if (ucitanaTabela.Equals(tipoviKarticeSelect))
            {
                prozor = new FrmTipKartice();
                prozor.ShowDialog();
                UcitajPodatke(dataGridCentralni, tipoviKarticeSelect);
            }
        }

        void PopuniFormu(DataGrid grid, string selectUslov)
        {
            try
            {
                konekcija.Open();
                azuriraj = true;
                DataRowView red = (DataRowView)grid.SelectedItems[0];
                SqlCommand cmd = new SqlCommand { Connection = konekcija };
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = red["ID"];
                cmd.CommandText = selectUslov + "@id";
                SqlDataReader citac = cmd.ExecuteReader();
                cmd.Dispose();

                if (citac.Read())
                {
                    if (ucitanaTabela.Equals(klijentiSelect))
                    {
                        FrmKlijent prozor = new FrmKlijent(azuriraj, red);
                        prozor.txtIme.Text = citac["Ime"].ToString();
                        prozor.txtPrezime.Text = citac["Prezime"].ToString();
                        prozor.txtJmbg.Text = citac["JMBG"].ToString();
                        prozor.txtGrad.Text = citac["Grad"].ToString();
                        prozor.txtAdresa.Text = citac["Adresa"].ToString();
                        prozor.txtKontakt.Text = citac["Kontakt"].ToString();
                        prozor.ShowDialog();
                    }
                    else if(ucitanaTabela.Equals(zaposleniSelect))
                    {
                        FrmZaposleni prozor = new FrmZaposleni(azuriraj, red);
                        prozor.txtIme.Text = citac["Ime"].ToString();
                        prozor.txtPrezime.Text = citac["Prezime"].ToString();
                        prozor.txtSektorRada.Text = citac["SektorRada"].ToString();
                        prozor.txtJmbg.Text = citac["JMBG"].ToString();
                        prozor.txtAdresa.Text = citac["Adresa"].ToString();
                        prozor.txtKontakt.Text = citac["Kontakt"].ToString();
                        prozor.ShowDialog();
                    }
                    else if (ucitanaTabela.Equals(racuniSelect))
                    {
                        FrmRacun prozor = new FrmRacun(azuriraj, red);
                        prozor.txtStanjeNaRacunu.Text = citac["StanjeNaRačunu"].ToString();
                        prozor.txtBrojRacuna.Text = citac["BrojRačuna"].ToString();
                        prozor.cbKlijent.SelectedValue = citac["KlijentID"].ToString();
                        prozor.cbTipRacuna.SelectedValue = citac["TipRačunaID"].ToString();
                        prozor.ShowDialog();
                    }
                    else if (ucitanaTabela.Equals(tipoviRacunaSelect))
                    {
                        FrmTipRacuna prozor = new FrmTipRacuna(azuriraj, red);
                        prozor.txtTipRacuna.Text = citac["NazivTipaRačuna"].ToString();
                        prozor.ShowDialog();
                    }
                    else if (ucitanaTabela.Equals(transakcijeSelect))
                    {
                       
                        FrmTransakcija prozor = new FrmTransakcija(azuriraj, red);
                        prozor.dpDatum.SelectedDate = (DateTime)citac["Datum"];
                        prozor.txtTip.Text = citac["TipTransakcije"].ToString();
                        prozor.txtIznos.Text = citac["IznosTransakcije"].ToString();
                        prozor.cbRacun.SelectedValue = citac["RačunID"].ToString();
                        prozor.cbZaposleni.SelectedValue = citac["ZaposleniID"].ToString();
                        prozor.ShowDialog();
                    }
                    else if (ucitanaTabela.Equals(kreditiSelect))
                    {
                        FrmKredit prozor = new FrmKredit(azuriraj, red);
                        prozor.txtIznos.Text = citac["IznosKredita"].ToString();
                        prozor.txtKamata.Text = citac["KamataKredita"].ToString();
                        prozor.txtMesecnaRata.Text = citac["MesečnaRata"].ToString();
                        prozor.dpPocetakOtplacivanja.SelectedDate = (DateTime)citac["PočetakOtplaćivanja"];
                        prozor.dpZavrsetakOtplacivanja.SelectedDate = (DateTime)citac["ZavršetakOtplaćivanja"];
                        prozor.cbRacun.SelectedValue = citac["RačunID"].ToString();
                        prozor.ShowDialog();
                    }
                    else if (ucitanaTabela.Equals(karticeSelect))
                    {
                        FrmKartica prozor = new FrmKartica(azuriraj, red);
                        prozor.cbRacun.SelectedValue = citac["RačunID"].ToString();
                        prozor.cbTipKartice.SelectedValue = citac["TipKarticeID"].ToString();
                        prozor.ShowDialog();
                    }
                    else if (ucitanaTabela.Equals(tipoviKarticeSelect))
                    {
                        FrmTipKartice prozor = new FrmTipKartice(azuriraj, red);
                        prozor.txtTipKartice.Text = citac["NazivTipaKartice"].ToString();
                        prozor.ShowDialog();
                    }
                }

            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Niste selektovali red!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            finally
            {
                if (konekcija != null)
                    konekcija.Close();

                azuriraj = false;

            }
        }

        private void btnIzmeni_Click(object sender, RoutedEventArgs e)
        {
            if (ucitanaTabela.Equals(klijentiSelect,StringComparison.Ordinal))
            {
                PopuniFormu(dataGridCentralni, selectUslovKlijenti);
                UcitajPodatke(dataGridCentralni, klijentiSelect);
            }
            else if (ucitanaTabela.Equals(zaposleniSelect, StringComparison.Ordinal))
            {
                PopuniFormu(dataGridCentralni, selectUslovZaposleni);
                UcitajPodatke(dataGridCentralni, zaposleniSelect);
            }
            else if(ucitanaTabela.Equals(racuniSelect,StringComparison.Ordinal))
            {
                PopuniFormu(dataGridCentralni, selectUslovRacuni);
                UcitajPodatke(dataGridCentralni, racuniSelect);
            }
            else if (ucitanaTabela.Equals(tipoviRacunaSelect, StringComparison.Ordinal))
            {
                PopuniFormu(dataGridCentralni, selectUslovTipoviRacuna);
                UcitajPodatke(dataGridCentralni, tipoviRacunaSelect);
            }
            else if (ucitanaTabela.Equals(transakcijeSelect, StringComparison.Ordinal))
            {
                PopuniFormu(dataGridCentralni, selectUslovTransakcije);
                UcitajPodatke(dataGridCentralni, transakcijeSelect);
            }
            else if (ucitanaTabela.Equals(kreditiSelect, StringComparison.Ordinal))
            {
                PopuniFormu(dataGridCentralni, selectUslovKrediti);
                UcitajPodatke(dataGridCentralni, kreditiSelect);
            }
            else if (ucitanaTabela.Equals(karticeSelect, StringComparison.Ordinal))
            {
                PopuniFormu(dataGridCentralni, selectUslovKartice);
                UcitajPodatke(dataGridCentralni, karticeSelect);
            }
            else if (ucitanaTabela.Equals(tipoviKarticeSelect, StringComparison.Ordinal))
            {
                PopuniFormu(dataGridCentralni, selectUslovTipoviKartice);
                UcitajPodatke(dataGridCentralni, tipoviKarticeSelect);
            }
        }

        void ObrisiZapis(DataGrid grid, string deleteUpit)
        {
            try
            {
                konekcija.Open();
                DataRowView red = (DataRowView)grid.SelectedItems[0];
                MessageBoxResult rezultat = MessageBox.Show("Da li ste sigurni da želite da izbrišete selektovani red?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (rezultat == MessageBoxResult.Yes)
                {
                    SqlCommand cmd = new SqlCommand { Connection = konekcija };
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = red["ID"];
                    cmd.CommandText = deleteUpit + "@id";
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Niste selektovali red!", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            catch (SqlException)
            {
                MessageBox.Show("Postoje povezani podaci u drugim tabelama.", "Obaveštenje", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (konekcija != null)
                    konekcija.Close();
            }
        }
        private void btnObrisi_Click(object sender, RoutedEventArgs e)
        {
            if (ucitanaTabela.Equals(karticeSelect, StringComparison.Ordinal))
            {
                ObrisiZapis(dataGridCentralni, deleteKartice);
                UcitajPodatke(dataGridCentralni, karticeSelect);
            }
            else if (ucitanaTabela.Equals(klijentiSelect, StringComparison.Ordinal))
            {
                ObrisiZapis(dataGridCentralni, deleteKlijenti);
                UcitajPodatke(dataGridCentralni, klijentiSelect);
            }
            else if (ucitanaTabela.Equals(kreditiSelect, StringComparison.Ordinal))
            {
                ObrisiZapis(dataGridCentralni, deleteKrediti);
                UcitajPodatke(dataGridCentralni, kreditiSelect);
            }
            else if(ucitanaTabela.Equals(racuniSelect,StringComparison.Ordinal))
            {
                ObrisiZapis(dataGridCentralni, deleteRacuni);
                UcitajPodatke(dataGridCentralni, racuniSelect);
            }
            else if (ucitanaTabela.Equals(tipoviKarticeSelect, StringComparison.Ordinal))
            {
                ObrisiZapis(dataGridCentralni, deleteTipoviKartice);
                UcitajPodatke(dataGridCentralni, tipoviKarticeSelect);
            }
            else if (ucitanaTabela.Equals(tipoviRacunaSelect, StringComparison.Ordinal))
            {
                ObrisiZapis(dataGridCentralni, deleteTipoviRacuna);
                UcitajPodatke(dataGridCentralni, tipoviRacunaSelect);
            }
            else if (ucitanaTabela.Equals(transakcijeSelect, StringComparison.Ordinal))
            {
                ObrisiZapis(dataGridCentralni, deleteTransakcije);
                UcitajPodatke(dataGridCentralni, transakcijeSelect);
            }
            else if (ucitanaTabela.Equals(zaposleniSelect, StringComparison.Ordinal))
            {
                ObrisiZapis(dataGridCentralni, deleteZaposleni);
                UcitajPodatke(dataGridCentralni, zaposleniSelect);
            }
        }
    }

}
