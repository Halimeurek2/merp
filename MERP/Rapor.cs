﻿using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.IO;
using System.Globalization;
using System.Collections;
using System.ComponentModel;

namespace MERP
{
    public partial class Rapor : Form
    {
        public MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        string connectionString;
        string komut;
        MySqlCommand myCommand;
        MySqlDataAdapter da;
        MySqlConnection myConnection;
        MySqlDataReader myReader;
        DataTable dt = new DataTable();
        HelperFunctions hf;

        public decimal TOPLAM=0;
        public string BIRIM;

        string el_mal;
        string mek_mal;
        string genel_mal;

        string el_mal2;
        string mek_mal2;
        string genel_mal2;

        string el_ongorulen;
        string el_harcanan;
        string el_kalan;

        public string mek_ongorulen;
        string mek_harcanan;
        string mek_kalan;

        string genel_ongorulen;
        string genel_harcanan;
        string genel_kalan;

        StringFormat strFormat;
        ArrayList arrColumnLefts = new ArrayList();
        ArrayList arrColumnWidths = new ArrayList();
        int iCellHeight = 0;
        int iTotalWidth = 0;
        int iRow = 0;
        bool bFirstPage = false;
        bool bNewPage = false;
        int iHeaderHeight = 0;

        public static float[] month_sumG = new float[12];
        public static DateTime[] monthG = new DateTime[12];

        public static float[] month_sumK = new float[12];
        public static DateTime[] monthK = new DateTime[12];
        int index = 0;

        private System.IO.Stream streamToPrint;

        string streamType;

        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern bool BitBlt
        (
            IntPtr hdcDest, // handle to destination DC
            int nXDest, // x-coord of destination upper-left corner
            int nYDest, // y-coord of destination upper-left corner
            int nWidth, // width of destination rectangle
            int nHeight, // height of destination rectangle
            IntPtr hdcSrc, // handle to source DC
            int nXSrc, // x-coordinate of source upper-left corner
            int nYSrc, // y-coordinate of source upper-left corner
            System.Int32 dwRop // raster operation code
        );

        public Rapor()
        {
            InitializeComponent();
            hf = new HelperFunctions();
        }

        private void Rapor_Load(object sender, EventArgs e)
        {
            server = "localhost";
            database = "uretimplanlama_2";
            uid = "root";
            password = "root";
            //string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            myConnection = new MySqlConnection(connectionString);
            myConnection.Open();


            komut = "SELECT DISTINCT proje_no FROM db_projeler";
            da = new MySqlDataAdapter(komut, connection);
            myCommand = new MySqlCommand(komut, myConnection);
            MySqlDataReader myReader;
            myReader = myCommand.ExecuteReader();
            while (myReader.Read())
            {
                cmb_projeler.Items.Add(myReader["proje_no"]);
            }

            chart2.Titles.Add("ELEKTRONİK");
            chart3.Titles.Add("MEKANİK");
            chart4.Titles.Add("GENEL GİDERLER");

            myReader.Close();
            myConnection.Close();
        }

        public void DrawChart1()
        {
            myConnection.Open();
            try
            {
                komut = "select sum(fatura_euro) as EURO from db_faturalar where fatura_cinsi='Elektronik' AND fatura_proje_no ='" + cmb_projeler.Text + "' AND fatura_tipi='G'";
                da = new MySqlDataAdapter(komut, connection);

                myCommand = new MySqlCommand(komut, myConnection);
                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    el_mal = Convert.ToString(myReader.GetString(0));
                    el_mal2= Convert.ToString(myReader.GetString(0));
                }
                myReader.Close();
            }
            catch
            {
                el_mal = "0";
                el_mal2 = "0";
                myReader.Close();
            }
            try
            {
                komut = "select sum(fatura_euro) as EURO from db_faturalar where fatura_cinsi='Mekanik' AND fatura_proje_no ='" + cmb_projeler.Text + "' AND fatura_tipi='G'";
                da = new MySqlDataAdapter(komut, connection);

                myCommand = new MySqlCommand(komut, myConnection);
                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    mek_mal = Convert.ToString(myReader.GetString(0));
                    mek_mal2 = Convert.ToString(myReader.GetString(0));
                }
                myReader.Close();
            }
            catch
            {
                mek_mal = "0";
                mek_mal2 = "0";
                myReader.Close();
            }
            try
            {
                komut = "select sum(fatura_euro) as EURO from db_faturalar WHERE fatura_cinsi='Genel Giderler' AND fatura_proje_no ='" + cmb_projeler.Text + "' AND fatura_tipi='G'";
                da = new MySqlDataAdapter(komut, connection);

                myCommand = new MySqlCommand(komut, myConnection);
                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    genel_mal = Convert.ToString(myReader.GetString(0));
                    genel_mal2 = Convert.ToString(myReader.GetString(0));
                }
                myReader.Close();
            }
            catch
            {
                genel_mal = "0";
                genel_mal2 = "0";
                myReader.Close();
            }
            chart1.Series["pieChart"].Points.Clear();
            
            //// el_mal = Convert.ToString(Convert.ToDouble(el_mal, CultureInfo.GetCultureInfo("en-US").NumberFormat));


            chart1.Series["pieChart"].Points.Add(Convert.ToDouble(el_mal));
            chart1.Series["pieChart"].Points.Add(Convert.ToDouble(mek_mal));
            chart1.Series["pieChart"].Points.Add(Convert.ToDouble(genel_mal));

            el_mal2 = hf.DecimalToCurrency(Convert.ToDecimal(el_mal2), el_mal2);
            mek_mal2 = hf.DecimalToCurrency(Convert.ToDecimal(mek_mal2), mek_mal2);
            genel_mal2 = hf.DecimalToCurrency(Convert.ToDecimal(genel_mal2), genel_mal2);

            var p1 = chart1.Series["pieChart"].Points[0];
            p1.AxisLabel = Convert.ToString(el_mal2);
            p1.LegendText = "Elektronik " + "#PERCENT";

            var p2 = chart1.Series["pieChart"].Points[1];
            p2.AxisLabel = Convert.ToString(mek_mal2);
            p2.LegendText = "Mekanik " + "#PERCENT";

            var p3 = chart1.Series["pieChart"].Points[2];
            p3.AxisLabel = Convert.ToString(genel_mal2);
            p3.LegendText = "Genel Giderler " + "#PERCENT";

            chart1.Series[0]["PieLabelStyle"] = "Outside";

            chart1.Series[0].BorderWidth = 1;
            chart1.Series[0].BorderColor = Color.FromArgb(26, 59, 105);

            myConnection.Close();

        }
        public void DrawChart2()
        {
            myConnection.Open();

            try
            {
                komut = "select sum(harcama_el_mlz) from db_projeler where proje_no in(select fatura_proje_no from db_faturalar where fatura_cinsi='Elektronik' and fatura_proje_no='"+cmb_projeler.Text+"' AND fatura_tipi='G')";
                da = new MySqlDataAdapter(komut, connection);

                myCommand = new MySqlCommand(komut, myConnection);
                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    el_ongorulen = Convert.ToString(myReader.GetString(0));
                }
                myReader.Close();
            }
            catch
            {
                el_ongorulen = "0";
                myReader.Close();
            }
            try
            {
                el_harcanan = el_mal;
            }
            catch
            {
                el_harcanan = "0"; 
            }
            try
            {
                el_kalan = Convert.ToString((Convert.ToDecimal(el_ongorulen)-Convert.ToDecimal(el_harcanan)));
            }
            catch
            {
                el_kalan = "0";
            }
            
            chart2.Series["Series1"].Points.Clear();

            el_ongorulen = hf.DecimalToCurrency(Convert.ToDecimal(el_ongorulen), el_ongorulen);
            chart2.Legends[0].Title = "Öngörülen Toplam" + " " + el_ongorulen;

            chart2.Series["Series1"].Points.Add(Convert.ToDouble(el_harcanan));
            chart2.Series["Series1"].Points.Add(Convert.ToDouble(el_kalan));

            el_harcanan = hf.DecimalToCurrency(Convert.ToDecimal(el_harcanan), el_harcanan);
            el_kalan= hf.DecimalToCurrency(Convert.ToDecimal(el_kalan), el_kalan);

            var p1 = chart2.Series["Series1"].Points[0];
            p1.AxisLabel = Convert.ToString(el_harcanan);
            p1.LegendText = "Harcanan " + "#PERCENT";

            var p2 = chart2.Series["Series1"].Points[1];
            p2.AxisLabel = Convert.ToString(el_kalan);
            p2.LegendText = "Kalan " + "#PERCENT";

            chart2.Series[0]["PieLabelStyle"] = "Outside";

            chart2.Series[0].BorderWidth = 1;
            chart2.Series[0].BorderColor = Color.FromArgb(26, 59, 105);

            myConnection.Close();
        }
        public void DrawChart3()
        {
            myConnection.Open();

            try
            {
                komut = "select (sum(harcama_m_mlz)+sum(harcama_imalat)) from db_projeler where proje_no in(select fatura_proje_no from db_faturalar where fatura_cinsi='Mekanik' and fatura_proje_no='" + cmb_projeler.Text + "' AND fatura_tipi='G')";
                da = new MySqlDataAdapter(komut, connection);

                myCommand = new MySqlCommand(komut, myConnection);
                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    mek_ongorulen = Convert.ToString(myReader.GetString(0));
                }
                myReader.Close();
            }
            catch
            {
                mek_ongorulen = "0";
                myReader.Close();
            }
            try
            {
                mek_harcanan = mek_mal;
            }
            catch
            {
                mek_harcanan = "0";
                myReader.Close();
            }
            try
            {
                mek_kalan = Convert.ToString((Convert.ToDecimal(mek_ongorulen) - Convert.ToDecimal(mek_harcanan)));
            }
            catch
            {

            }
            
            chart3.Series["Series1"].Points.Clear();
            
            mek_ongorulen =hf.DecimalToCurrency(Convert.ToDecimal(mek_ongorulen), mek_ongorulen);

            chart3.Legends[0].Title = "Öngörülen Toplam" + " " + mek_ongorulen;
            chart3.Series["Series1"].Points.Add(Convert.ToDouble(mek_harcanan));
            chart3.Series["Series1"].Points.Add(Convert.ToDouble(mek_kalan));

            mek_harcanan = hf.DecimalToCurrency(Convert.ToDecimal(mek_harcanan), mek_harcanan);
            mek_kalan = hf.DecimalToCurrency(Convert.ToDecimal(mek_kalan), mek_kalan);

            var p1 = chart3.Series["Series1"].Points[0];
            p1.AxisLabel = Convert.ToString(mek_harcanan);
            p1.LegendText = "Harcanan " + "#PERCENT";

            var p2 = chart3.Series["Series1"].Points[1];
            p2.AxisLabel = Convert.ToString(mek_kalan);
            p2.LegendText = "Kalan " + "#PERCENT";

            chart3.Series[0]["PieLabelStyle"] = "Outside";

            chart3.Series[0].BorderWidth = 1;
            chart3.Series[0].BorderColor = Color.FromArgb(26, 59, 105);

            myConnection.Close();
        }
        public void DrawChart4()
        {
            myConnection.Open();

            try
            {
                komut = "select (sum(harcama_risk)+sum(harcama_test)) from db_projeler where proje_no in(select fatura_proje_no from db_faturalar where fatura_cinsi='Genel Giderler' and fatura_proje_no='" + cmb_projeler.Text + "' AND fatura_tipi='G')";
                da = new MySqlDataAdapter(komut, connection);

                myCommand = new MySqlCommand(komut, myConnection);
                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    genel_ongorulen = Convert.ToString(myReader.GetString(0));
                }
                myReader.Close();
            }
            catch
            {
                genel_ongorulen = "0";
                myReader.Close();
            }
            try
            {
                genel_harcanan = genel_mal;
            }
            catch
            {
                genel_harcanan = "0";
                myReader.Close();
            }
            try
            {
                genel_kalan = Convert.ToString((Convert.ToDecimal(genel_ongorulen) - Convert.ToDecimal(genel_harcanan)));
            }
            catch
            {

            }
            
            chart4.Series["Series1"].Points.Clear();

            genel_ongorulen = hf.DecimalToCurrency(Convert.ToDecimal(genel_ongorulen), genel_ongorulen);

            chart4.Legends[0].Title = "Öngörülen Toplam" + " " + genel_ongorulen;
            chart4.Series["Series1"].Points.Add(Convert.ToDouble(genel_harcanan));
            chart4.Series["Series1"].Points.Add(Convert.ToDouble(genel_kalan));

            genel_harcanan = hf.DecimalToCurrency(Convert.ToDecimal(genel_harcanan), genel_harcanan);
            genel_kalan = hf.DecimalToCurrency(Convert.ToDecimal(genel_kalan), genel_kalan);

            var p1 = chart4.Series["Series1"].Points[0];
            p1.AxisLabel = Convert.ToString(genel_harcanan);
            p1.LegendText = "Harcanan " + "#PERCENT";

            var p2 = chart4.Series["Series1"].Points[1];
            p2.AxisLabel = Convert.ToString(genel_kalan);
            p2.LegendText = "Kalan " + "#PERCENT";

            chart4.Series[0]["PieLabelStyle"] = "Outside";

            chart4.Series[0].BorderWidth = 1;
            chart4.Series[0].BorderColor = Color.FromArgb(26, 59, 105);

            myConnection.Close();
        }
 
        private void cmb_projeler_SelectedIndexChanged(object sender, EventArgs e)
        {
            index = 0;
            Array.Clear(monthG, 0, 12);
            Array.Clear(month_sumG, 0, 12);
            Array.Clear(monthK, 0, 12);
            Array.Clear(month_sumK, 0, 12);

            myConnection.Open();
            try
            {
                komut = "SELECT sum(fatura_euro) FROM db_faturalar WHERE fatura_proje_no ='" + cmb_projeler.Text + "' AND fatura_tipi='G'";
                da = new MySqlDataAdapter(komut, connection);
                myCommand = new MySqlCommand(komut, myConnection);
                myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    TOPLAM = Convert.ToDecimal(myReader.GetString(0));
                    lbl_top_maliyet.Text = string.Format(new CultureInfo("de-DE"), "{0:C2}", Convert.ToDecimal(TOPLAM));
                }
                myReader.Close();
            }
            catch
            {
                lbl_top_maliyet.Text = "0";
                myReader.Close();
            }

            try
            {
                komut = "SELECT sum(fatura_euro) FROM db_faturalar WHERE fatura_durum='ÖDENDİ' AND fatura_proje_no ='" + cmb_projeler.Text + "' AND fatura_tipi='G'";
                da = new MySqlDataAdapter(komut, connection);
                myCommand = new MySqlCommand(komut, myConnection);
                myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    TOPLAM = Convert.ToDecimal(myReader.GetString(0));
                    lbl_odenmisG.Text = string.Format(new CultureInfo("de-DE"), "{0:C2}", Convert.ToDecimal(TOPLAM));
                }
                myReader.Close();
            }
            catch
            {
                lbl_odenmisG.Text = "0";
                myReader.Close();
            }

            try
            {
                komut = "SELECT proje_butce,proje_birim,proje_baslangic FROM db_projeler WHERE proje_no='" + cmb_projeler.Text + "'";
                da = new MySqlDataAdapter(komut, connection);
                myCommand = new MySqlCommand(komut, myConnection);
                myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    lbl_prjEuro.Text = hf.EuroCalculation(Convert.ToString(myReader.GetString(2)), Convert.ToString(myReader.GetString(0)), Convert.ToString(myReader.GetString(1)), lbl_prjEuro.Text);
                    lbl_prjEuro.Text = string.Format(new CultureInfo("de-DE"), "{0:C2}", Convert.ToDecimal(lbl_prjEuro.Text));
                }
                myReader.Close();
            }
            catch
            {
                lbl_prjEuro.Text = "0";
                myReader.Close();
            }

            try
            {
                komut = "SELECT sum(fatura_euro) FROM db_faturalar WHERE fatura_durum='ÖDENDİ' AND fatura_proje_no ='" + cmb_projeler.Text + "' AND fatura_tipi='K'";
                da = new MySqlDataAdapter(komut, connection);
                myCommand = new MySqlCommand(komut, myConnection);
                myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    TOPLAM = Convert.ToDecimal(myReader.GetString(0));
                    lbl_odenmisK.Text = string.Format(new CultureInfo("de-DE"), "{0:C2}", Convert.ToDecimal(TOPLAM));
                }
                myReader.Close();
            }
            catch
            {
                lbl_odenmisK.Text = "0";
                myReader.Close();
            }

            try
            {
                komut = "SELECT proje_butce,proje_birim FROM db_projeler WHERE proje_no='" + cmb_projeler.Text + "'";
                da = new MySqlDataAdapter(komut, connection);
                myCommand = new MySqlCommand(komut, myConnection);
                myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    TOPLAM = Convert.ToDecimal(myReader.GetString(0));
                    BIRIM = Convert.ToString(myReader.GetString(1));

                    if(BIRIM=="USD")
                    {
                        lbl_prj_butce.Text = string.Format(new CultureInfo("en-SG"), "{0:C2}", Convert.ToDecimal(TOPLAM));
                    }
                    else if(BIRIM=="EUR")
                    {
                        lbl_prj_butce.Text = string.Format(new CultureInfo("de-DE"), "{0:C2}", Convert.ToDecimal(TOPLAM));
                    }
                    else if(BIRIM == "TRY")
                    {
                        lbl_prj_butce.Text = string.Format("{0:C2}", Convert.ToDecimal(TOPLAM));
                    }
                    else if(BIRIM == "GBP")
                    {
                        lbl_prj_butce.Text = string.Format(new CultureInfo("en-GB"), "{0:C2}", Convert.ToDecimal(TOPLAM));
                    }
                    else
                    {
                        lbl_prj_butce.Text = string.Format(new CultureInfo("en-CH"), "{0:C2}", Convert.ToDecimal(TOPLAM));
                    }
                }
                //lbl_prjEuro.Text = hf.EuroDonusum(BIRIM, Convert.ToString(TOPLAM));
                //lbl_prjEuro.Text = string.Format(new CultureInfo("de-DE"), "{0:C2}", Convert.ToDecimal(lbl_prjEuro.Text));

                myReader.Close();
            }
            catch
            {
                lbl_prj_butce.Text = "0";
                //lbl_prjEuro.Text = "0";
                myReader.Close();
            }

            try
            {
                 komut = "SELECT DATE_FORMAT(fatura_vade_tarih,'%m-%Y') AS Month, SUM(fatura_euro) FROM db_faturalar WHERE fatura_durum='ÖDENMEDİ' and fatura_tipi='G' and fatura_proje_no ='" + cmb_projeler.Text + "' GROUP BY DATE_FORMAT(fatura_vade_tarih, '%m-%Y')";
                 da = new MySqlDataAdapter(komut, connection);
                 myCommand = new MySqlCommand(komut, myConnection);
                 myReader = myCommand.ExecuteReader();
                 while (myReader.Read())
                 {
                    if(Convert.ToDateTime(myReader.GetString(0)).Year == DateTime.Now.Year)
                    {
                        monthG[index] = Convert.ToDateTime(myReader.GetString(0));
                        month_sumG[index] = (float)Convert.ToDouble(myReader.GetString(1));
                        index++;
                    }
                 }
                myReader.Close();
            }
            catch
            {
                //MessageBox.Show(Convert.ToString(DateTime.Now.AddMonths(1)));
                myReader.Close();
            }

            try
            {
                index = 0;
                komut = "SELECT DATE_FORMAT(fatura_vade_tarih,'%m-%Y') AS Month, SUM(fatura_euro) FROM db_faturalar WHERE fatura_durum='ÖDENMEDİ' and fatura_tipi='K' and fatura_proje_no ='" + cmb_projeler.Text + "' GROUP BY DATE_FORMAT(fatura_vade_tarih, '%m-%Y')";
                da = new MySqlDataAdapter(komut, connection);
                myCommand = new MySqlCommand(komut, myConnection);
                myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    if (Convert.ToDateTime(myReader.GetString(0)).Year == DateTime.Now.Year)
                    {
                        monthK[index] = Convert.ToDateTime(myReader.GetString(0));
                        month_sumK[index] = (float)Convert.ToDouble(myReader.GetString(1));
                        index++;
                    }
                }
                myReader.Close();
            }
            catch
            {
                //MessageBox.Show(Convert.ToString(DateTime.Now.AddMonths(1)));
                myReader.Close();
            }

            myConnection.Close();

            FillDGW();
            DGWToplam();
            DrawChart1();
            DrawChart2();
            DrawChart3();
            DrawChart4();
        }

        public void FillDGW()
        {
            

            myConnection.Open();

            komut = "SELECT fatura_no as FaturaNo," +
                    "fatura_firma as Firma," +
                    "fatura_vade as Vade," +
                    "fatura_vade_tarih as VadeTarih," +
                    "fatura_aciklama as Açıklama," +
                    "fatura_euro as Euro FROM db_faturalar WHERE fatura_durum='ÖDENMEDİ' AND fatura_proje_no ='" + cmb_projeler.Text + "' AND fatura_tipi='G'";
            myCommand = new MySqlCommand(komut, myConnection);
            da = new MySqlDataAdapter(myCommand);
            dt = new DataTable();
            // myReader = myCommand.ExecuteReader();

            da.Fill(dt);
            dgw_odenmemisG.DataSource = dt;

            dgw_odenmemisG.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgw_odenmemisG.AutoSizeColumnsMode =
                       DataGridViewAutoSizeColumnsMode.Fill;

            dgw_odenmemisG.Columns["FaturaNo"].Width = 70;
            dgw_odenmemisG.Columns["Vade"].Width = 35;
            dgw_odenmemisG.Columns["VadeTarih"].Width = 80;
            dgw_odenmemisG.Columns["Euro"].Width = 90;

            komut = "SELECT fatura_no as FaturaNo," +
                    "fatura_firma as Firma," +
                    "fatura_vade as Vade," +
                    "fatura_vade_tarih as VadeTarih," +
                    "fatura_aciklama as Açıklama," +
                    "fatura_euro as Euro FROM db_faturalar WHERE fatura_durum='ÖDENMEDİ' AND fatura_proje_no ='" + cmb_projeler.Text + "' AND fatura_tipi='K'";
            myCommand = new MySqlCommand(komut, myConnection);
            da = new MySqlDataAdapter(myCommand);
            dt = new DataTable();
            // myReader = myCommand.ExecuteReader();

            da.Fill(dt);
            dgw_odenmemisK.DataSource = dt;

            dgw_odenmemisK.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgw_odenmemisK.AutoSizeColumnsMode =
                       DataGridViewAutoSizeColumnsMode.Fill;

            dgw_odenmemisK.Columns["FaturaNo"].Width = 70;
            dgw_odenmemisK.Columns["Vade"].Width = 35;
            dgw_odenmemisK.Columns["VadeTarih"].Width = 80;
            dgw_odenmemisK.Columns["Euro"].Width = 90;

            dgw_odenmemisK.Columns[5].DefaultCellStyle.Format = "c2";
            dgw_odenmemisK.Columns[5].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("de-DE");
            dgw_odenmemisG.Columns[5].DefaultCellStyle.Format = "c2";
            dgw_odenmemisG.Columns[5].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("de-DE");
            myConnection.Close();

            try
            {
                for (int i = 0; i < dgw_odenmemisG.Rows.Count; i++)
                {
                    if (Convert.ToDateTime(dgw_odenmemisG.Rows[i].Cells[3].Value).Date < DateTime.Now)
                    {
                        dgw_odenmemisG.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    }
                    else if (Convert.ToDateTime(dgw_odenmemisG.Rows[i].Cells[3].Value).Date > DateTime.Now && Convert.ToDateTime(dgw_odenmemisG.Rows[i].Cells[3].Value).Date < DateTime.Now.AddDays(5))
                    {
                        dgw_odenmemisG.Rows[i].DefaultCellStyle.BackColor = Color.Orange;
                    }
                }
            }
            catch { }
            try
            {
                for (int i = 0; i < dgw_odenmemisK.Rows.Count; i++)
                {
                    if (Convert.ToDateTime(dgw_odenmemisK.Rows[i].Cells[3].Value).Date < DateTime.Now)
                    {
                        dgw_odenmemisK.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    }
                    else if(Convert.ToDateTime(dgw_odenmemisK.Rows[i].Cells[3].Value).Date > DateTime.Now && Convert.ToDateTime(dgw_odenmemisK.Rows[i].Cells[3].Value).Date < DateTime.Now.AddDays(5))
                    {
                        dgw_odenmemisK.Rows[i].DefaultCellStyle.BackColor = Color.Orange;
                    }
                }
            }
            catch { }
        }

        public void DGWToplam()
        {
            myConnection.Open();
            try
            {
                komut = "SELECT sum(fatura_euro) FROM db_faturalar WHERE fatura_durum='ÖDENMEDİ' AND fatura_proje_no ='" + cmb_projeler.Text + "' AND fatura_tipi='G'";
                da = new MySqlDataAdapter(komut, connection);
                myCommand = new MySqlCommand(komut, myConnection);
                myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    TOPLAM = Convert.ToDecimal(myReader.GetString(0));
                    gb_G.Text = "Toplam Gelen : " + string.Format(new CultureInfo("de-DE"), "{0:C2}", Convert.ToDecimal(TOPLAM));
                    lbl_odenmemisGelen.Text = string.Format(new CultureInfo("de-DE"), "{0:C2}", Convert.ToDecimal(TOPLAM));
                }
                myReader.Close();
            }
            catch
            {
                gb_G.Text = "0";
                lbl_odenmemisGelen.Text = "0";
                myReader.Close();
            }

            try
            {
                komut = "SELECT sum(fatura_euro) FROM db_faturalar WHERE fatura_durum='ÖDENMEDİ' AND fatura_proje_no ='" + cmb_projeler.Text + "' AND fatura_tipi='K'";
                da = new MySqlDataAdapter(komut, connection);
                myCommand = new MySqlCommand(komut, myConnection);
                myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    TOPLAM = Convert.ToDecimal(myReader.GetString(0));
                    gb_K.Text = "Toplam Kesilen : " + string.Format(new CultureInfo("de-DE"), "{0:C2}", Convert.ToDecimal(TOPLAM));
                    lbl_odenmemisKesilen.Text = string.Format(new CultureInfo("de-DE"), "{0:C2}", Convert.ToDecimal(TOPLAM));
                }
            }
            catch
            {
                gb_K.Text = "0";
                lbl_odenmemisKesilen.Text = "0";
            }
          
            myReader.Close();
            myConnection.Close();
        }

        public void CaptureScreen()
        {
            //Graphics myGraphics = this.CreateGraphics();
            //Size s = this.Size;
            //memoryImage = new Bitmap(s.Width, s.Height, myGraphics);
            //Graphics memoryGraphics = Graphics.FromImage(memoryImage);
            //memoryGraphics.CopyFromScreen(this.Location.X, this.Location.Y, 0, 0, s);
            //printPreviewDialog1.ShowDialog();
        }

        private void btn_print_Click(object sender, EventArgs e)
        {
            btn_print.Visible = false;

            string filename = Path.GetTempFileName();

            Graphics g1 = this.CreateGraphics();
            Image MyImage = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height, g1);
            Graphics g2 = Graphics.FromImage(MyImage);
            IntPtr dc1 = g1.GetHdc();
            IntPtr dc2 = g2.GetHdc();
            BitBlt(dc2, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height, dc1, 0, 0, 13369376);
            g1.ReleaseHdc(dc1);
            g2.ReleaseHdc(dc2);
            MyImage.Save(filename, ImageFormat.Png);
            FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            StartPrint(fileStream, "Image");
            fileStream.Close();
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            ////Process snippingToolProcess = new Process();
            ////snippingToolProcess.EnableRaisingEvents = true;
            ////if (!Environment.Is64BitProcess)
            ////{
            ////    snippingToolProcess.StartInfo.FileName = "C:\\Windows\\sysnative\\SnippingTool.exe";
            ////    snippingToolProcess.Start();
            ////}
            ////else
            ////{
            ////    snippingToolProcess.StartInfo.FileName = "C:\\Windows\\system32\\SnippingTool.exe";
            ////    snippingToolProcess.Start();
            ////}
            ////CaptureScreen();
            ////printDocument1.Print();
            ////printDocument1.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            Image image = Image.FromStream(streamToPrint);

            int x = e.MarginBounds.X;
            int y = e.MarginBounds.Y;

            int width = image.Width;
            int height = image.Height;
            if ((width / e.MarginBounds.Width) > (height / e.MarginBounds.Height))
            {
                width = e.MarginBounds.Width;
                height = image.Height * e.MarginBounds.Width / image.Width;
            }
            else
            {
                height = e.MarginBounds.Height;
                width = image.Width * e.MarginBounds.Height / image.Height;
            }
            Rectangle destRect = new Rectangle(x, y, width, height);
            e.Graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);

            btn_print.Visible = true;
        }

        public void StartPrint(Stream streamToPrint, string streamType)
        {
            this.printDocument1.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);

            this.streamToPrint = streamToPrint;

            this.streamType = streamType;

            PrintDialog PrintDialog1 = new PrintDialog();

            PrintDialog1.AllowSomePages = true;
            PrintDialog1.ShowHelp = true;
            PrintDialog1.Document = printDocument1;
            DialogResult result = PrintDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                printDocument1.Print();
            }
            else
            {
                btn_print.Visible = true;
            }
        }

        private void btn_print1_Click(object sender, EventArgs e)
        {
            //dgw_odenmemisG.Columns["VadeTarih"].DefaultCellStyle.Format = "dd.MM.yyyy";
            PrintPreviewDialog onizleme = new PrintPreviewDialog();
            onizleme.Document = printDocument2;
            onizleme.ShowDialog();
        }

        private void printDocument2_PrintPage(object sender, PrintPageEventArgs e)
        {
            #region
            try
            {
                int iLeftMargin = e.MarginBounds.Left;
                int iTopMargin = e.MarginBounds.Top;
                bool bMorePagesToPrint = false;
                int iTmpWidth = 0;
                bFirstPage = true;

                if (bFirstPage)
                {
                    foreach (DataGridViewColumn GridCol in dgw_odenmemisG.Columns)
                    {
                        iTmpWidth = (int)(Math.Floor((double)((double)GridCol.Width /
                                       (double)iTotalWidth * (double)iTotalWidth *
                                       ((double)e.MarginBounds.Width / (double)iTotalWidth))));

                        iHeaderHeight = (int)(e.Graphics.MeasureString(GridCol.HeaderText,
                                    GridCol.InheritedStyle.Font, iTmpWidth).Height) + 11;


                        arrColumnLefts.Add(iLeftMargin);
                        arrColumnWidths.Add(iTmpWidth);
                        iLeftMargin += iTmpWidth;
                    }
                }

                while (iRow <= dgw_odenmemisG.Rows.Count - 1)
                {
                    DataGridViewRow GridRow = dgw_odenmemisG.Rows[iRow];

                    iCellHeight = GridRow.Height + 5;
                    int iCount = 0;

                    if (iTopMargin + iCellHeight >= e.MarginBounds.Height + e.MarginBounds.Top)
                    {
                        bNewPage = true;
                        bFirstPage = false;
                        bMorePagesToPrint = true;
                        break;
                    }
                    else
                    {
                        if (bNewPage)
                        {

                            e.Graphics.DrawString("Ödenmemiş Gelen Faturalar--" + cmb_projeler.Text, new Font(dgw_odenmemisG.Font, FontStyle.Bold),
                                    Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Top -
                                    e.Graphics.MeasureString("Ödenmemiş Gelen Faturalar--" + cmb_projeler.Text, new Font(dgw_odenmemisG.Font,
                                    FontStyle.Bold), e.MarginBounds.Width).Height - 13);

                            String strDate = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToShortTimeString();

                            e.Graphics.DrawString(strDate, new Font(dgw_odenmemisG.Font, FontStyle.Bold),
                                    Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width -
                                    e.Graphics.MeasureString(strDate, new Font(dgw_odenmemisG.Font,
                                    FontStyle.Bold), e.MarginBounds.Width).Width), e.MarginBounds.Top -
                                    e.Graphics.MeasureString("Ödenmemiş Gelen Faturalar--" + cmb_projeler.Text, new Font(new Font(dgw_odenmemisG.Font,
                                    FontStyle.Bold), FontStyle.Bold), e.MarginBounds.Width).Height - 13);


                            iTopMargin = e.MarginBounds.Top;
                            foreach (DataGridViewColumn GridCol in dgw_odenmemisG.Columns)
                            {
                                e.Graphics.FillRectangle(new SolidBrush(Color.LightGray),
                                    new Rectangle((int)arrColumnLefts[iCount], iTopMargin,
                                    (int)arrColumnWidths[iCount], iHeaderHeight));

                                e.Graphics.DrawRectangle(Pens.Black,
                                    new Rectangle((int)arrColumnLefts[iCount], iTopMargin,
                                    (int)arrColumnWidths[iCount], iHeaderHeight));

                                e.Graphics.DrawString(GridCol.HeaderText, GridCol.InheritedStyle.Font,
                                    new SolidBrush(GridCol.InheritedStyle.ForeColor),
                                    new RectangleF((int)arrColumnLefts[iCount], iTopMargin,
                                    (int)arrColumnWidths[iCount], iHeaderHeight), strFormat);
                                iCount++;
                            }
                            bNewPage = false;
                            iTopMargin += iHeaderHeight;
                        }
                        iCount = 0;

                        foreach (DataGridViewCell Cel in GridRow.Cells)
                        {
                            if (Cel.Value != null)
                            {
                                e.Graphics.DrawString(Cel.Value.ToString(), Cel.InheritedStyle.Font,
                                            new SolidBrush(Cel.InheritedStyle.ForeColor),
                                            new RectangleF((int)arrColumnLefts[iCount], (float)iTopMargin,
                                            (int)arrColumnWidths[iCount], (float)iCellHeight), strFormat);
                            }

                            e.Graphics.DrawRectangle(Pens.Black, new Rectangle((int)arrColumnLefts[iCount],
                                    iTopMargin, (int)arrColumnWidths[iCount], iCellHeight));

                            iCount++;
                        }
                    }
                    iRow++;
                    iTopMargin += iCellHeight;
                }


                if (bMorePagesToPrint)
                    e.HasMorePages = true;
                else
                    e.HasMorePages = false;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion
        }

        private void printDocument2_BeginPrint(object sender, PrintEventArgs e)
        {
            try
            {
                strFormat = new StringFormat();
                strFormat.Alignment = StringAlignment.Near;
                strFormat.LineAlignment = StringAlignment.Center;
                strFormat.Trimming = StringTrimming.EllipsisCharacter;

                arrColumnLefts.Clear();
                arrColumnWidths.Clear();
                iCellHeight = 0;
                iRow = 0;
                bFirstPage = true;
                bNewPage = true;

                iTotalWidth = 0;
                foreach (DataGridViewColumn dgvGridCol in dgw_odenmemisG.Columns)
                {
                    iTotalWidth += dgvGridCol.Width;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_print2_Click(object sender, EventArgs e)
        {
            PrintPreviewDialog onizleme = new PrintPreviewDialog();
            onizleme.Document = printDocument3;
            onizleme.ShowDialog();
        }

        private void printDocument3_BeginPrint(object sender, PrintEventArgs e)
        {
            try
            {
                strFormat = new StringFormat();
                strFormat.Alignment = StringAlignment.Near;
                strFormat.LineAlignment = StringAlignment.Center;
                strFormat.Trimming = StringTrimming.EllipsisCharacter;

                arrColumnLefts.Clear();
                arrColumnWidths.Clear();
                iCellHeight = 0;
                iRow = 0;
                bFirstPage = true;
                bNewPage = true;

                iTotalWidth = 0;
                foreach (DataGridViewColumn dgvGridCol in dgw_odenmemisK.Columns)
                {
                    iTotalWidth += dgvGridCol.Width;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void printDocument3_PrintPage(object sender, PrintPageEventArgs e)
        {
            #region
            try
            {
                int iLeftMargin = e.MarginBounds.Left;
                int iTopMargin = e.MarginBounds.Top;
                bool bMorePagesToPrint = false;
                int iTmpWidth = 0;
                bFirstPage = true;

                if (bFirstPage)
                {
                    foreach (DataGridViewColumn GridCol in dgw_odenmemisK.Columns)
                    {
                        iTmpWidth = (int)(Math.Floor((double)((double)GridCol.Width /
                                       (double)iTotalWidth * (double)iTotalWidth *
                                       ((double)e.MarginBounds.Width / (double)iTotalWidth))));

                        iHeaderHeight = (int)(e.Graphics.MeasureString(GridCol.HeaderText,
                                    GridCol.InheritedStyle.Font, iTmpWidth).Height) + 11;


                        arrColumnLefts.Add(iLeftMargin);
                        arrColumnWidths.Add(iTmpWidth);
                        iLeftMargin += iTmpWidth;
                    }
                }

                while (iRow <= dgw_odenmemisK.Rows.Count - 1)
                {
                    DataGridViewRow GridRow = dgw_odenmemisK.Rows[iRow];

                    iCellHeight = GridRow.Height + 5;
                    int iCount = 0;

                    if (iTopMargin + iCellHeight >= e.MarginBounds.Height + e.MarginBounds.Top)
                    {
                        bNewPage = true;
                        bFirstPage = false;
                        bMorePagesToPrint = true;
                        break;
                    }
                    else
                    {
                        if (bNewPage)
                        {

                            e.Graphics.DrawString("Ödenmemiş Kesilen Faturalar--" + cmb_projeler.Text, new Font(dgw_odenmemisK.Font, FontStyle.Bold),
                                    Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Top -
                                    e.Graphics.MeasureString("Ödenmemiş Kesilen Faturalar--" + cmb_projeler.Text, new Font(dgw_odenmemisK.Font,
                                    FontStyle.Bold), e.MarginBounds.Width).Height - 13);

                            String strDate = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToShortTimeString();

                            e.Graphics.DrawString(strDate, new Font(dgw_odenmemisK.Font, FontStyle.Bold),
                                    Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width -
                                    e.Graphics.MeasureString(strDate, new Font(dgw_odenmemisK.Font,
                                    FontStyle.Bold), e.MarginBounds.Width).Width), e.MarginBounds.Top -
                                    e.Graphics.MeasureString("Ödenmemiş Kesilen Faturalar--" + cmb_projeler.Text, new Font(new Font(dgw_odenmemisK.Font,
                                    FontStyle.Bold), FontStyle.Bold), e.MarginBounds.Width).Height - 13);


                            iTopMargin = e.MarginBounds.Top;
                            foreach (DataGridViewColumn GridCol in dgw_odenmemisK.Columns)
                            {
                                e.Graphics.FillRectangle(new SolidBrush(Color.LightGray),
                                    new Rectangle((int)arrColumnLefts[iCount], iTopMargin,
                                    (int)arrColumnWidths[iCount], iHeaderHeight));

                                e.Graphics.DrawRectangle(Pens.Black,
                                    new Rectangle((int)arrColumnLefts[iCount], iTopMargin,
                                    (int)arrColumnWidths[iCount], iHeaderHeight));

                                e.Graphics.DrawString(GridCol.HeaderText, GridCol.InheritedStyle.Font,
                                    new SolidBrush(GridCol.InheritedStyle.ForeColor),
                                    new RectangleF((int)arrColumnLefts[iCount], iTopMargin,
                                    (int)arrColumnWidths[iCount], iHeaderHeight), strFormat);
                                iCount++;
                            }
                            bNewPage = false;
                            iTopMargin += iHeaderHeight;
                        }
                        iCount = 0;

                        foreach (DataGridViewCell Cel in GridRow.Cells)
                        {
                            if (Cel.Value != null)
                            {
                                e.Graphics.DrawString(Cel.Value.ToString(), Cel.InheritedStyle.Font,
                                            new SolidBrush(Cel.InheritedStyle.ForeColor),
                                            new RectangleF((int)arrColumnLefts[iCount], (float)iTopMargin,
                                            (int)arrColumnWidths[iCount], (float)iCellHeight), strFormat);
                            }

                            e.Graphics.DrawRectangle(Pens.Black, new Rectangle((int)arrColumnLefts[iCount],
                                    iTopMargin, (int)arrColumnWidths[iCount], iCellHeight));

                            iCount++;
                        }
                    }
                    iRow++;
                    iTopMargin += iCellHeight;
                }


                if (bMorePagesToPrint)
                    e.HasMorePages = true;
                else
                    e.HasMorePages = false;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion
        }

        private void btn_ozet_Click(object sender, EventArgs e)
        {
            OdenecekFaturalar frm1 = new OdenecekFaturalar();

            Array.Clear(frm1.monthG, 0, 12);
            Array.Clear(frm1.month_sumG, 0, 12);
            Array.Clear(frm1.monthK, 0, 12);
            Array.Clear(frm1.month_sumK, 0, 12);

            for (int i=0;i<12;i++)
            {
                frm1.monthG[i] = monthG[i];
                frm1.month_sumG[i] = month_sumG[i];
                frm1.monthK[i] = monthK[i];
                frm1.month_sumK[i] = month_sumK[i];
            }
            frm1.lbl_prjNo.Text = cmb_projeler.Text;
            frm1.Show();

        }
    }
}
