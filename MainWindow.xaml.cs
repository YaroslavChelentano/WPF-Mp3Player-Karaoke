using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using WMPLib;
namespace MusicCollection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string connstring = String.Format("Server={0};Port={1};" +
"User Id={2};Password={3};Database={4};", "localhost", 5432, "postgres", "95zusanu", "MusicPlayer");
        private NpgsqlConnection conn;
        private string sql; //доступ до команд
        private NpgsqlCommand cmd;  // для команд
        private DataTable dt;   //для дататейбл
        string scriptAudio = File.ReadAllText(@"D:\MusicCollection\audio.sql");
        string scriptAlbum = File.ReadAllText(@"D:\MusicCollection\album.sql");
        string scriptArtist = File.ReadAllText(@"D:\MusicCollection\artist.sql");
        WindowsMediaPlayer player = new WindowsMediaPlayer();

        public MainWindow()
        {
            InitializeComponent();
            conn = new NpgsqlConnection(connstring);
            conn.Open();
            cmd = new NpgsqlCommand(scriptAudio, conn);
            cmd = new NpgsqlCommand(scriptAlbum, conn);
            cmd = new NpgsqlCommand(scriptArtist, conn);
            cmd.ExecuteNonQuery();
            conn.Close();

        }
        private void Select1()
        {
            try
            {
                conn.Open();
                sql = @"select * from audio_select()";
                cmd = new NpgsqlCommand(sql, conn);
                dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                dgvAudio.DataContext = dt.DefaultView;   //конекшн до дата тайбл
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show("Error " + ex.Message);
            }
        }
        public string GetSelectedCellValue(int i)
        {
            DataGridCellInfo cellInfo = dgvAudio.SelectedCells[i];
            if (cellInfo == null) return null;

            DataGridBoundColumn column = cellInfo.Column as DataGridBoundColumn;
            if (column == null) return null;

            FrameworkElement element = new FrameworkElement() { DataContext = cellInfo.Item };
            BindingOperations.SetBinding(element, TagProperty, column.Binding);

            return element.Tag.ToString();
        }
        private void Select_Click(object sender, RoutedEventArgs e)
        {
            Select1();
        }

        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            int result = 0;
            try
            {
                conn.Open();
                sql = @"select * from audio_insert(:_albumid,:_artistid,:_audioname,:_audioduration,:_audiopath)";
                cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("_albumid", Int64.Parse(txtAlbumID.Text));
                cmd.Parameters.AddWithValue("_artistid", Int64.Parse(txtArtistID.Text));
                cmd.Parameters.AddWithValue("_audioname", txtName.Text);
                cmd.Parameters.AddWithValue("_audioduration", txtDuration.Text);
                cmd.Parameters.AddWithValue("_audiopath", txtPath.Text);
                result = (int)cmd.ExecuteScalar();
                conn.Close();
                if (result == 1)
                {
                    MessageBox.Show("Inserted new Song successfully");
                    Select1();
                }
                else
                {
                    MessageBox.Show("Inserted failed");
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show("Inserted failed . Error: " + ex.Message);
            }
        }


        private void Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                conn.Open();
                sql = @"select * from agreement_update(:_AudioID,:_AlbumID,:_ArtistID,:_AudioName,:_AudioDuration,:_AudioPath)";
                cmd = new NpgsqlCommand(sql, conn);
                string cellValue = GetSelectedCellValue(0);
                cmd.Parameters.AddWithValue("_AudioID", Int64.Parse(cellValue));
                cmd.Parameters.AddWithValue("_AlbumID", Int64.Parse(txtAlbumID.Text));
                cmd.Parameters.AddWithValue("_ArtistID", Int64.Parse(txtArtistID.Text));
                cmd.Parameters.AddWithValue("_AudioName", txtName.Text);
                cmd.Parameters.AddWithValue("_AudioDuration", txtDuration.Text);
                cmd.Parameters.AddWithValue("_AudioPath", txtPath.Text);
                if ((int)cmd.ExecuteScalar() == 1)
                {
                    MessageBox.Show("Updated new Agreement successfully");
                    conn.Close();
                    Select1();
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message);
            }
        }



        private void Video_Click(object sender, RoutedEventArgs e)
        {
            Video a = new Video();
            a.Show();
            this.Close();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                conn.Open();
                sql = @"Select * from audio_delete(:_AudioID)";
                cmd = new NpgsqlCommand(sql, conn);
                string cellValue = GetSelectedCellValue(0);
                cmd.Parameters.AddWithValue("_AudioID", Int64.Parse(cellValue));
                if ((int)cmd.ExecuteScalar() == 1)
                {
                    MessageBox.Show("Delete Agreement successfully");
                    conn.Close();
                    Select1();
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message);
            }

        }

        private void SelectAlbum_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                conn.Open();
                sql = @"select * from Album";
                cmd = new NpgsqlCommand(sql, conn);
                dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                dgvAlbum.DataContext = dt.DefaultView;   //конекшн до дата тайбл
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show("Error " + ex.Message);
            }
        }

        private void SelectArtist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                conn.Open();
                sql = @"select * from Artist";
                cmd = new NpgsqlCommand(sql, conn);
                dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                dgvArtist.DataContext = dt.DefaultView;   //конекшн до дата тайбл
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show("Error " + ex.Message);
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            string path = GetSelectedCellValue(5);
            player.URL = $@"{path}";
            player.controls.play();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            string path = GetSelectedCellValue(5);
            player.URL = $@"{path}";
            player.controls.stop();
        }

        private void Transaction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                conn.Open();
                sql = @"BEGIN;
                UPDATE audio SET audioduration = '0.00' WHERE audioduration = '';
                Select * from audio_select();
                COMMIT;";
                //-- Trasaction
                cmd = new NpgsqlCommand(sql, conn);
                dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                dgvAudio.DataContext = dt.DefaultView;   //конекшн до дата тайбл
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show("Error " + ex.Message);
            }
        }

        private void Transaction_Copy_Click(object sender, RoutedEventArgs e)
        {
            {
                try
                {
                    conn.Open();
                    sql = @"BEGIN;
                UPDATE audio SET audioduration = '0.00' WHERE audioduration = '';
                Select * from audio_select();
                COMMIT;";
                    //-- Trasaction
                    cmd = new NpgsqlCommand(sql, conn);
                    dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    dgvAudio.DataContext = dt.DefaultView;   //конекшн до дата тайбл

                    if ((int)cmd.ExecuteScalar() == 1)
                    {
                        MessageBox.Show("Updated new Audio successfully");
                        conn.Close();
                        Select1();
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    MessageBox.Show("Error " + ex.Message);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                conn.Open();
                sql = @"  SELECT * FROM Audio ORDER BY AudioID  LIMIT 5"; //-- Return next 10 books starting from 11th (pagination, show results 11-20)
                cmd = new NpgsqlCommand(sql, conn);
                dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                dgvAudio.DataContext = dt.DefaultView;   //конекшн до дата тайбл
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show("Error " + ex.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                conn.Open();
                sql = @"  SELECT * FROM Audio ORDER BY AudioID  OFFSET 5 LIMIT 5"; //-- Return next 10 books starting from 11th (pagination, show results 11-20)
                cmd = new NpgsqlCommand(sql, conn);
                dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                dgvAudio.DataContext = dt.DefaultView;   //конекшн до дата тайбл
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show("Error " + ex.Message);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                conn.Open();
                sql = @"  SELECT * FROM Audio ORDER BY AudioID  OFFSET 10 LIMIT 5"; //-- Return next 10 books starting from 11th (pagination, show results 11-20)
                cmd = new NpgsqlCommand(sql, conn);
                dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                dgvAudio.DataContext = dt.DefaultView;   //конекшн до дата тайбл
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show("Error " + ex.Message);
            }
        }
    }
}