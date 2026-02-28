using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;

namespace Sistem_Informasi_Puskesmas
{
    public partial class Register_Pasien_Form : Form
    {
        public static readonly HttpClient client = new HttpClient();
        private DataPasienResponse dataTerakhir;
        public Register_Pasien_Form()
        {
            InitializeComponent();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async void Register_Pasien_Form_Load(object sender, EventArgs e)
        {
            await ComboBoxDefault();
        }

        public async Task ComboBoxDefault()
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", UserSession.Token);

                var response = await client.GetAsync("http://localhost:8000/api/polis");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<PoliData>>>();

                    if (result != null && result.data != null)
                    {
                        cboPoli.DataSource = result.data;       
                        cboPoli.DisplayMember = "nama_poli";     
                        cboPoli.ValueMember = "id";              

                        cboPoli.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show("Gagal memuat daftar poli: " + ex.Message);
            }
        }

        private async void btnSimpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNik.Text))
            {
                MessageBox.Show("NIK Pasien wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNik.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNama.Text))
            {
                MessageBox.Show("Nama Pasien wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNama.Focus();
                return;
            }

           if (!rdoLaki.Checked && !rdoPerempuan.Checked)
            {
                MessageBox.Show("Silakan pilih Jenis Kelamin!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboPoli.SelectedIndex == -1)
            {
                MessageBox.Show("Silakan pilih Poli tujuan!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboPoli.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtKeluhan.Text))
            {
                MessageBox.Show("Keluhan wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtKeluhan.Focus();
                return;
            }

            var dataRegister = new DataPasienRegister
            {
                nik_pasien = int.Parse(txtNik.Text),
                nama_pasien = txtNama.Text,
                jenis_kelamin_pasien = rdoLaki.Checked ? "Laki-laki" : "Perempuan",
                tanggal_lahir_pasien = dtpTanggalLahir.Value,
                alamat_pasien = txtAlamat.Text,
                nomor_hp_pasien = int.Parse(txtNoHP.Text),

                tanggal_register = DateTime.Now,
                keluhan_pasien = txtKeluhan.Text,
                poli_id = (int)cboPoli.SelectedValue
            };

            DialogResult dialogResult = MessageBox.Show("Apakah data yang anda masukkan sudah benar?", "Konfirmasi Simpan", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.No)
            {
                return;
            }
            else
            {
                try
                {
                    var response = await client.PostAsJsonAsync("http://localhost:8000/api/registers", dataRegister);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<ApiResponse<DataPasienResponse>>();
                        MessageBox.Show("Registrasi Berhasil!  Silahkan Cetak Tiket");

                        dataTerakhir = result.data;
                        TampilkanTiket(result.data);

                        btnReset_Click(sender, e);
                    }
                    else
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show("Gagal Registrasi: " + error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Terjadi kesalahan: " + ex.Message);
                }
            }

            

        }

        private void TampilkanTiket(DataPasienResponse data)
        {
            if (data == null) return;

            lblNoAntrian.Text = data.nomor_antrian ?? "-";

            if (data.pasien != null)
            {
                lblNoPasien.Text = data.pasien.kode_pasien;
                lblNama.Text = data.pasien.nama_pasien;
                lblNIK.Text = data.pasien.nik_pasien.ToString();
                lblNoHP.Text = data.pasien.nomor_hp_pasien.ToString();
            }

            if (data.poli != null) lblPoli.Text = data.poli.nama_poli;

            if (data.jadwal_dokter?.dokter?.user != null)
            {
                lblDokter.Text = data.jadwal_dokter.dokter.user.nama_user;
                lblRuangan.Text = data.jadwal_dokter.ruangan.kode_ruangan + " - " + data.jadwal_dokter.ruangan.nama_ruangan ?? "-";
            }

            lblTanggal.Text = !string.IsNullOrEmpty(data.jadwal) ? data.jadwal.Substring(0, 5) + " WIB" : "-";

            if (DateTime.TryParse(data.tanggal_register, out DateTime tglFinal))
                lblTanggal2.Text = tglFinal.ToString("dd MMMM yyyy");

            pnlTiket.Visible = true;
            pnlTiket.BringToFront();
        }


        private void btnCetak_Click(object sender, EventArgs e)
        {

            if (dataTerakhir == null)
            {
                MessageBox.Show("Tidak ada data untuk dicetak!");
                return;
            }

            SaveFileDialog saveFile = new SaveFileDialog();

            saveFile.Filter = "Text Files (*.txt)|*.txt";
            saveFile.FileName = "Tiket_Pendaftaran_" + dataTerakhir.nomor_antrian + dataTerakhir.tanggal_register + ".txt";

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StringBuilder struk = new StringBuilder();
                    struk.AppendLine("==============================================");
                    struk.AppendLine("            PUSKESMAS CITRA KASIH            ");
                    struk.AppendLine("      TIKET ANTRIAN PENDAFTARAN DIGITAL      ");
                    struk.AppendLine("==============================================");
                    struk.AppendLine("");
                    struk.AppendLine(" NOMOR ANTRIAN : " + dataTerakhir.nomor_antrian);
                    struk.AppendLine(" NOMOR PASIEN  : " + dataTerakhir.pasien?.kode_pasien);
                    struk.AppendLine("----------------------------------------------");
                    struk.AppendLine(" NAMA    : " + dataTerakhir.pasien?.nama_pasien);
                    struk.AppendLine(" NIK     : " + dataTerakhir.pasien?.nik_pasien);
                    struk.AppendLine(" NO HP   : " + dataTerakhir.pasien?.nomor_hp_pasien);
                    struk.AppendLine("----------------------------------------------");
                    struk.AppendLine(" POLI    : " + dataTerakhir.poli?.nama_poli);
                    struk.AppendLine(" DOKTER  : " + dataTerakhir.jadwal_dokter?.dokter?.user?.nama_user);
                    struk.AppendLine(" RUANG   : " + dataTerakhir.jadwal_dokter.ruangan.kode_ruangan + " - " + dataTerakhir.jadwal_dokter?.ruangan.nama_ruangan);
                    struk.AppendLine(" JADWAL  : " + dataTerakhir.jadwal?.Substring(0, 5) + " WIB");
                    struk.AppendLine(" TANGGAL : " + DateTime.Parse(dataTerakhir.tanggal_register).ToString("dd MMMM yyyy"));
                    struk.AppendLine("----------------------------------------------");
                    struk.AppendLine("");
                    struk.AppendLine(" CATATAN PENTING:");
                    struk.AppendLine(" - Harap datang 10 menit sebelum jadwal.");
                    struk.AppendLine(" - Tunjukkan file ini kepada petugas.");
                    struk.AppendLine("");
                    struk.AppendLine("=============================================");
                    struk.AppendLine("     Terima Kasih Atas Kepercayaan Anda      ");
                    struk.AppendLine("            Semoga Cepat Sembuh              ");
                    struk.AppendLine("=============================================");

                    System.IO.File.WriteAllText(saveFile.FileName, struk.ToString());

                    MessageBox.Show("Tiket berhasil di cetak, Terimkasih ^^", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    pnlTiket.Visible = false;
                    btnReset_Click(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal mencetak file: " + ex.Message);
                }
            }

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtNik.Clear();
            txtNama.Clear();
            dtpTanggalLahir.Value = DateTime.Now;
            rdoLaki.Checked = false; rdoPerempuan.Checked = false;
            txtNoHP.Clear();
            txtAlamat.Clear();

            cboPoli.SelectedIndex = -1;
            txtKeluhan.Clear();

            txtNik.Focus();
        }

    }
}
