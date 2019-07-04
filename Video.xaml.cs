using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Media;
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
using System.Windows.Threading;
using WMPLib;
namespace MusicCollection
{
    /// <summary>
    /// Interaction logic for Video.xaml
    /// </summary>
    public partial class Video : Window
    {
        private string connstring = String.Format("Server={0};Port={1};" +
"User Id={2};Password={3};Database={4};", "localhost", 5432, "postgres", "95zusanu", "MusicPlayer");
        private NpgsqlConnection conn;
        private string sql; //доступ до команд
        private NpgsqlCommand cmd;  // для команд
        private DataTable dt;   //для дататейбл
        string scriptVideo = File.ReadAllText(@"D:\MusicCollection\video.sql");
        string scriptLearn = File.ReadAllText(@"D:\MusicCollection\learn.sql");
        string scriptKaraoke = File.ReadAllText(@"D:\MusicCollection\karaoke.sql");
        string scriptDevices = File.ReadAllText(@"D:\MusicCollection\devices.sql");
        public Video()
        {
            InitializeComponent();
            conn = new NpgsqlConnection(connstring);
            conn.Open();
            cmd = new NpgsqlCommand(scriptVideo, conn);
            cmd = new NpgsqlCommand(scriptLearn, conn);
            cmd = new NpgsqlCommand(scriptKaraoke, conn);
            cmd = new NpgsqlCommand(scriptDevices, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }
        void timer_Tick(object sender, EventArgs e)
        {
            if (mePlayer.Source != null)
            {
                if (mePlayer.NaturalDuration.HasTimeSpan)
                    lblStatus.Content = String.Format("{0} / {1}", mePlayer.Position.ToString(@"mm\:ss"), mePlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            }
            else
                lblStatus.Content = "No file selected...";
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Play();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Pause();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Stop();
        }
        private void Select1()
        {
            try
            {
                conn.Open();
                sql = @"select * from video_select()";
                cmd = new NpgsqlCommand(sql, conn);
                dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                dgvVideo.DataContext = dt.DefaultView;   //конекшн до дата тайбл
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
            DataGridCellInfo cellInfo = dgvVideo.SelectedCells[i];
            if (cellInfo == null) return null;

            DataGridBoundColumn column = cellInfo.Column as DataGridBoundColumn;
            if (column == null) return null;

            FrameworkElement element = new FrameworkElement() { DataContext = cellInfo.Item };
            BindingOperations.SetBinding(element, TagProperty, column.Binding);

            return element.Tag.ToString();
        }

        private void SelectVideo_Click(object sender, RoutedEventArgs e)
        {
            Select1();
        }

        private void SelectKaraoke_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                conn.Open();
                sql = @"select * from Karaoke";
                cmd = new NpgsqlCommand(sql, conn);
                dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                dgvKaraoke.DataContext = dt.DefaultView;   //конекшн до дата тайбл
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show("Error " + ex.Message);
            }
        }

        private void SelectDevises_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                conn.Open();
                sql = @"select * from Devises";
                cmd = new NpgsqlCommand(sql, conn);
                dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                dgvDevises.DataContext = dt.DefaultView;   //конекшн до дата тайбл
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show("Error " + ex.Message);
            }
        }

        private void SelectLearn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                conn.Open();
                sql = @"select * from Learn";
                cmd = new NpgsqlCommand(sql, conn);
                dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                dgvLearn.DataContext = dt.DefaultView;   //конекшн до дата тайбл
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show("Error " + ex.Message);
            }
        }
        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            int result = 0;
            try
            {
                conn.Open();
                sql = @"select * from video_insert(_ArtistID,_VideoName,_VideoDuration,_VideoPath)";
                cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("_artistid", Int64.Parse(txtArtistID.Text));
                cmd.Parameters.AddWithValue("_VideoName", txtVideoName.Text);
                cmd.Parameters.AddWithValue("_VideoDuration", txtVideoDuration.Text);
                cmd.Parameters.AddWithValue("_VideoPath", txtVideoPath.Text);
                result = (int)cmd.ExecuteScalar();
                conn.Close();
                if (result == 1)
                {
                    MessageBox.Show("Inserted new Video successfully");
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

        private void NumerateKaraoke_Click(object sender, RoutedEventArgs e)
        {
            {
                try
                {
                    conn.Open();
                    sql = @"SELECT KaraokeID,VideoID,ArtistID,VideoName,
row_number() OVER ()  AS Accountnumber FROM Karaoke;";
                    cmd = new NpgsqlCommand(sql, conn);
                    dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    dgvKaraoke.DataContext = dt.DefaultView;   //конекшн до дата тайбл
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    MessageBox.Show("Error " + ex.Message);
                }
            }
        }

        private void CountVideo_Click(object sender, RoutedEventArgs e)
        {
            {
                try
                {
                    conn.Open();
                    sql = @"SELECT COUNT(VideoID),VideoName FROM Video GROUP BY VideoName;";
                    cmd = new NpgsqlCommand(sql, conn);
                    dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    dgvVideo.DataContext = dt.DefaultView;   //конекшн до дата тайбл
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    MessageBox.Show("Error " + ex.Message);
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow a = new MainWindow();
            a.Show();
            this.Close();
        }
    }
}