using MySqlX.XDevAPI;
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

namespace Sistem_Informasi_Puskesmas
{
    public partial class User_Profile_Form : Form
    {
        public static readonly HttpClient client = new HttpClient();
        public User_Profile_Form()
        {
            InitializeComponent();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async void User_Profile_Form_Load(object sender, EventArgs e)
        {
            if (UserSession.CurrentUser != null)
            {
                var user = UserSession.CurrentUser;
                string role = UserSession.CurrentUser.role;

                lblNamaPro.Text = user.nama_user;

                lblEmail.Text = user.email;
                lblNoHP.Text = "+62" + user.nomor_hp_user.ToString();
                lblStatus.Text = user.status_user;

                lblNIK.Text = user.nik_user.ToString();
                lblNama.Text = user.nama_user;
                lblBirthdate.Text = user.tanggal_lahir_user.ToString();
                lblJenisKelamin.Text = user.jenis_kelamin_user;
                lblAlamat.Text = user.alamat_user;

                if (role == "Dokter")
                {
                    lblRole.Text = "Dokter Puskesmas Citra Kasih";
                    lblOption.Text = "Poli";
                    lblNI.Text = "Nomor Induk Dokter";

                    await LoadDataDokter();
                }
                else if (role == "Staff Register")
                {
                    lblRole.Text = "Staff Registrasi Puskesmas Citra Kasih";

                    await LoadDataStaff();
                }
                else if (role == "Staff Transaksi")
                {
                    lblRole.Text = "Staff Transaksi Puskesmas Citra Kasih";

                    await LoadDataStaff();
                }
                else if (role == "Apoteker")
                {
                    lblRole.Text = "Apoteker Puskesmas Citra Kasih";
                    lblNI.Text = "Nomor Induk Apoteker";

                    await LoadDataApoteker();
                }
            }
        }

        private async Task LoadDataStaff()
        {
            try
            {
                client.DefaultRequestHeaders.Authorization =new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", UserSession.Token);

                var response = await client.GetAsync("http://localhost:8000/api/staff");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<DataStaff>>>();

                    if (result != null && result.success)
                    {
                        var myDataStaff = result.data.FirstOrDefault(s => s.user_id == UserSession.CurrentUser.id);

                        if (myDataStaff != null)
                        {
                            lblNoInduk.Text = myDataStaff.nomor_induk_staff.ToString();
                        }
                        else
                        {
                            lblNoInduk.Text = "-";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Gagal mengambil data staff. Status: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tidak dapat terhubung ke server: " + ex.Message);
            }
        }

        private async Task LoadDataDokter()
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", UserSession.Token);

                var response = await client.GetAsync("http://localhost:8000/api/dokters");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<DataDokter>>>();

                    if (result != null && result.success)
                    {
                        var myDataDokter = result.data.FirstOrDefault(s => s.user_id == UserSession.CurrentUser.id);

                        if (myDataDokter != null)
                        {
                            lblNoInduk.Text = myDataDokter.nomor_induk_dokter.ToString();
                            lblOptionData.Text = myDataDokter.poli.nama_poli;
                        }
                        else
                        {
                            lblNoInduk.Text = "-";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Gagal mengambil data dokter. Status: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tidak dapat terhubung ke server: " + ex.Message);
            }
        }

        private async Task LoadDataApoteker()
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", UserSession.Token);

                var response = await client.GetAsync("http://localhost:8000/api/apotekers");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<DataApoteker>>>();

                    if (result != null && result.success)
                    {
                        var myDataStaff = result.data.FirstOrDefault(s => s.user_id == UserSession.CurrentUser.id);

                        if (myDataStaff != null)
                        {
                            lblNoInduk.Text = myDataStaff.nomor_induk_apoteker.ToString();
                        }
                        else
                        {
                            lblNoInduk.Text = "-";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Gagal mengambil data staff. Status: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tidak dapat terhubung ke server: " + ex.Message);
            }
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            string role = UserSession.CurrentUser.role;

            if (role == "Dokter")
            {
                new Dokter_Dashboard().Show();
            }
            else if (role == "Staff Register")
            {
                new Staff_Dashboard().Show();
            }
            else if (role == "Staff Transaksi")
            {
                new Staff_Transaksi().Show();
            }
            else if (role == "Apoteker")
            {
                new Apoteker_Dashboard().Show();
            }


            this.Close();
        }

    }
}
