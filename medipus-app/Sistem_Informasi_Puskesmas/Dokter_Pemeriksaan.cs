using MySqlX.XDevAPI;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace Sistem_Informasi_Puskesmas
{
    public partial class Dokter_Pemeriksaan : Form
    {
        private int pemeriksaanId = 0;

        public static readonly HttpClient client = new HttpClient();

        private DataPasienResponse _data;

        public Dokter_Pemeriksaan(DataPasienResponse data)
        {
            InitializeComponent();

            this._data = data;

            TampilkanDetailPasien();
            LoadTindakan();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async void Dokter_Pemeriksaan_Load (object sender, EventArgs e)
        {
            await LoadObatToComboBox();

            ListViewDetailObat();
        }

        private void TampilkanDetailPasien()
        {
            lblNoAntrian.Text = _data.nomor_antrian;
            lblNoPasien.Text = _data.pasien?.kode_pasien;

            lblNama.Text = _data.pasien?.nama_pasien;
            lblNIK.Text = _data.pasien?.nik_pasien.ToString();
            lblJenisKelamin.Text = _data.pasien?.jenis_kelamin_pasien;
            lblNoHP.Text = _data.pasien?.nomor_hp_pasien.ToString();
            lblAlamat.Text = _data.pasien?.alamat_pasien.ToString();

            lblPoliTujuan.Text = _data.poli.nama_poli;
            lblKeluhan.Text = _data.keluhan_pasien?? "Keluhan Kosong";

            lblTanggalLahir.Text = _data.pasien?.tanggal_lahir_pasien.ToString("dd - MM - yyyy");

            lblNoAntrianDetail.Text = _data.nomor_antrian;
            lblNoPasienDetail.Text = _data.pasien?.kode_pasien;

            lblTanggalPemeriksaan.Text = DateTime.Now.ToString("dd-MM-yyyy");
            lblDokter.Text = UserSession.CurrentUser.nama_user;
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            new Dokter_Dashboard().Show();
            this.Hide();
        }

        private async void btnProsesPemeriksaan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTekananDarah.Text))
            {
                MessageBox.Show("Tekanan Darah (Blood Pressure) wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTekananDarah.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtHeartrate.Text))
            {
                MessageBox.Show("Denyut Nadi (Heart Rate) wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHeartrate.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSuhu.Text))
            {
                MessageBox.Show("Suhu Badan (Temperature) wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSuhu.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDiagnosa.Text))
            {
                MessageBox.Show("Diagnosa Dokter wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiagnosa.Focus();
                return;
            }

            if (cboTindakan.SelectedIndex == -1)
            {
                MessageBox.Show("Silakan pilih Tindakan Medis!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboTindakan.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtWeight.Text))
            {
                MessageBox.Show("Berat Badan wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtWeight.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtHeight.Text))
            {
                MessageBox.Show("Tinggi Badan wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHeight.Focus();
                return;
            }

            try
            {
                var dataPemeriksaan = new
                {
                    diagnosa_dokter = txtDiagnosa.Text,
                    catatan_dokter = txtCatatan.Text,
                    tanggal_pemeriksaan = DateTime.Now.ToString("yyyy-MM-dd"),

                    tekanan_darah = txtTekananDarah.Text,
                    suhu_badan = txtSuhu.Text,
                    denyut_nadi = txtHeartrate.Text,
                    berat_badan = txtWeight.Text,
                    tinggi_badan = txtHeight.Text,

                    register_id = _data.id,
                    
                    dokter_id = UserSession.CurrentUser.id,

                    tindakan_medis_id = cboTindakan.SelectedValue,

                    punya_resep = chkKeterangan.Checked
                };

                string json = JsonConvert.SerializeObject(dataPemeriksaan);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("http://localhost:8000/api/pemeriksaans", content);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResponse<PemeriksaanData>>(jsonResponse);

                    this.pemeriksaanId = result.data.id;

                    MessageBox.Show("Pemeriksaan berhasil disimpan dan status pasien diperbarui!", "Sukses");

                    if (chkKeterangan.Checked)
                    {
                        pnlDetailResep.Visible = true; 
                        pnlDetailResep.BringToFront();
                    }
                    else
                    {
                        new Dokter_Dashboard().Show();
                        this.Close();
                    }
                    
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Gagal simpan: " + error);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
        }

        private async void LoadTindakan()
        {
            try
            {
                int poli_id = _data.poli.id;

                string url = $"http://localhost:8000/api/tindakan-by-poli/{poli_id}";
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<TindakanResponse>(content);

                    if (result.data != null && result.data.Count > 0)
                    {

                        cboTindakan.DataSource = result.data;
                        cboTindakan.DisplayMember = "nama_tindakan";
                        cboTindakan.ValueMember = "id";

                        cboTindakan.SelectedIndex = -1;
                    }
                    else
                    {
                        MessageBox.Show("Data sukses tapi List Kosong. Cek isi tabel poli_tindakans!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading tindakan: " + ex.Message);
            }
        }

        private void btnBatalResep_Click(object sender, EventArgs e)
        {
            pnlDetailResep.Visible = false;
        }

        private void btnSimpanInputObat_Click(object sender, EventArgs e)
        {
            if (cboObat.SelectedValue == null)
            {
                MessageBox.Show("Lengkapi data obat terlebih dahulu!");
                return;
            }

            int idObat = (int)cboObat.SelectedValue;
            string namaObat = cboObat.Text;

            ListViewItem item = new ListViewItem(namaObat);
            item.SubItems.Add(txtDosis.Text);
            item.SubItems.Add(txtFrekuensi.Text);
            item.SubItems.Add(txtDurasi.Text);
            item.SubItems.Add(txtCatatanObat.Text);

            item.Tag = idObat;
            lvwDaftarObat.Items.Add(item);

            cboObat.SelectedIndex = -1;
            txtDosis.Clear();
            txtFrekuensi.Clear();
            txtDurasi.Clear();
            txtCatatanObat.Clear();

            cboObat.Focus();
        }

        private void btnResetInputObat_Click(object sender, EventArgs e)
        {
            cboObat.SelectedIndex = -1;
            txtDosis.Clear();
            txtFrekuensi.Clear();
            txtDurasi.Clear();
            txtCatatanObat.Clear();

            cboObat.Focus();
        }

        private async void btnKirimResep_Click(object sender, EventArgs e)
        {
            if (lvwDaftarObat.Items.Count == 0)
            {
                MessageBox.Show("Belum ada obat di daftar rincian!");
                return;
            }

            try
            {
                if (this.pemeriksaanId == 0)
                {
                    MessageBox.Show("ID Pemeriksaan tidak ditemukan. Simpan pemeriksaan ulang!");
                    return;
                }

                var resepData = new
                {
                    status_resep = "Menunggu",
                    tanggal_resep = DateTime.Now.ToString("yyyy-MM-dd"),
                    pemeriksaan_id = this.pemeriksaanId
                };

                string jsonResep = JsonConvert.SerializeObject(resepData);
                var contentResep = new StringContent(jsonResep, Encoding.UTF8, "application/json");

                var responseResep = await client.PostAsync("http://localhost:8000/api/reseps", contentResep);

                if (responseResep.IsSuccessStatusCode)
                {
                    string jsonResponse = await responseResep.Content.ReadAsStringAsync();
                    var resultResep = JsonConvert.DeserializeObject<ApiResponse<ResepResult>>(jsonResponse); 

                    int newResepId = resultResep.data.id;
                    var listObat = new List<object>();

                    foreach (ListViewItem row in lvwDaftarObat.Items)
                    {
                        listObat.Add(new
                        {
                            obat_id = Convert.ToInt32(row.Tag),
                            dosis_obat = row.SubItems[1].Text,
                            frekuensi_obat = row.SubItems[2].Text,
                            durasi_obat = row.SubItems[3].Text,
                            jumlah_obat = 1,
                            catatan_obat = row.SubItems[4].Text
                        });
                    }

                    var detailRequest = new
                    {
                        resep_id = newResepId,
                        items = listObat
                    };

                    string jsonDetail = JsonConvert.SerializeObject(detailRequest);
                    var contentDetail = new StringContent(jsonDetail, Encoding.UTF8, "application/json");

                    var responseDetail = await client.PostAsync("http://localhost:8000/api/detail_resep", contentDetail);

                    if (responseDetail.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Resep dan Detail Berhasil Terkirim!");
                        new Dokter_Dashboard().Show();
                        this.Close();
                    }
                    else
                    {
                        string errorDetail = await responseDetail.Content.ReadAsStringAsync();
                        MessageBox.Show("Gagal simpan detail: " + errorDetail);
                    }
                }
                else
                {
                    string errorResep = await responseResep.Content.ReadAsStringAsync();
                    MessageBox.Show("Gagal simpan header resep: " + errorResep);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mengirim resep: " + ex.Message);
            }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (lvwDaftarObat.SelectedItems.Count > 0)
            {
                DialogResult result = MessageBox.Show("Hapus obat yang dipilih dari daftar?",
                    "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    foreach (ListViewItem item in lvwDaftarObat.SelectedItems)
                    {
                        lvwDaftarObat.Items.Remove(item);
                    }
                }
            }
            else
            {
                MessageBox.Show("Silakan pilih obat di tabel rincian yang ingin dihapus!",
                    "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async Task LoadObatToComboBox()
        {
            try
            {
                var response = await client.GetAsync("http://localhost:8000/api/obats");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ObatData>>>();

                    cboObat.DataSource = result.data;
                    cboObat.DisplayMember = "nama_obat"; 
                    cboObat.ValueMember = "id";

                    cboObat.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mengambil data obat: " + ex.Message);
            }
        }

        private void btnResetPemeriksaan_Click(object sender, EventArgs e)
        {
            txtTekananDarah.Clear();
            txtHeartrate.Clear();
            txtSuhu.Clear();
            txtHeight.Clear();
            txtWeight.Clear();

            txtDiagnosa.Clear();

            cboTindakan.SelectedIndex = -1;

            txtCatatanObat.Clear();
        }

        private void ListViewDetailObat()
        {
            lvwDaftarObat.View = View.Details;
            lvwDaftarObat.FullRowSelect = true;
            lvwDaftarObat.GridLines = true;

            lvwDaftarObat.Columns.Clear();
            lvwDaftarObat.Columns.Add("Nama Obat", 171);
            lvwDaftarObat.Columns.Add("Dosis", 86);
            lvwDaftarObat.Columns.Add("Frekuensi", 114);
            lvwDaftarObat.Columns.Add("Durasi", 80);
            lvwDaftarObat.Columns.Add("Catatan", 194);

            lvwDaftarObat.OwnerDraw = true;

            lvwDaftarObat.DrawColumnHeader += (s, e) =>
            {
                using (SolidBrush brush = new SolidBrush(Color.DarkCyan))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }

                TextRenderer.DrawText(e.Graphics, e.Header.Text, e.Font,
                    e.Bounds, Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            };

            lvwDaftarObat.DrawItem += (s, e) => e.DrawDefault = true;
            lvwDaftarObat.DrawSubItem += (s, e) => e.DrawDefault = true;

        }
    }
}
