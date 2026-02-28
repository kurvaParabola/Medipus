using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Sistem_Informasi_Puskesmas
{
    public partial class Login_Form : Form
    {
        public static readonly HttpClient client = new HttpClient();
        public Login_Form()
        {
            InitializeComponent();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async void btnMasuk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Username dan Password tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var payload = new
            {
                username = txtUsername.Text,
                password = txtPassword.Text
            };

            try
            {
                var response = await client.PostAsJsonAsync("http://localhost:8000/api/login", payload);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResult>>();

                    if (result != null && result.success)
                    {
                        UserSession.Token = result.data.token;
                        UserSession.CurrentUser = result.data.user;

                        NavigateByRole(result.data.user.role);
                    }
                }
                else
                {
                    MessageBox.Show("Username atau Password salah!", "Gagal Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            catch (Exception ex) 
            {
                MessageBox.Show("Tidak dapat terhubung ke server: " + ex.Message, "Error Koneksi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NavigateByRole(string role)
        {
            switch (role)
            {
                case "Staff Register":
                    new Staff_Dashboard().Show();
                    break;
                case "Dokter":
                    new Dokter_Dashboard().Show();
                    break;
                case "Apoteker":
                    new Apoteker_Dashboard().Show();
                    break;
                case "Staff Transaksi":
                    new Staff_Transaksi().Show();
                    break;
                default:
                    MessageBox.Show("Role pengguna tidak terdaftar di sistem!", "Akses Ditolak", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
            }

            this.Hide(); 
        }

        private void btnSembunyi_MouseDown(object sender, MouseEventArgs e)
        {
            txtPassword.UseSystemPasswordChar = false;
        }

        private void btnSembunyi_MouseUp(object sender, MouseEventArgs e)
        {
            txtPassword.UseSystemPasswordChar = true;
        }

        private void btnKeluar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Apakah Anda yakin akan keluar?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
