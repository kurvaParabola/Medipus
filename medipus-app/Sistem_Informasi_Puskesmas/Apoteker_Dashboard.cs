using K4os.Hash.xxHash;
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
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistem_Informasi_Puskesmas
{
    public partial class Apoteker_Dashboard : Form
    {
        public static readonly HttpClient client = new HttpClient();

        private int selectedObatId = 0;
        public Apoteker_Dashboard()
        {
            InitializeComponent();
        }

        public async void Apoteker_Dashboard_Load(object sender, EventArgs e)
        {
            if (UserSession.CurrentUser != null)
            {
                lblPengguna.Text = "Apoteker - " + UserSession.CurrentUser.nama_user;
            }

            lblDate.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
            lblTanggal3.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
            lblDate3.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));

            InisialisasiListResep();
            InisialisasiListLaporan();
            InisialisasiListObat();

            LoadKategoriObat();
            LoadPenyimpananObat();
            
            await LoadPoliToComboBox();

            await LoadDaftarResep();
            await LoadHistoryResep();
            await LoadDataObat();
            await LoadObatToComboBox();
        }

        private void LoadKategoriObat()
        {
            cboKategoriObat.Items.Clear();
            cboKategori.Items.Clear();
            cboKategori2.Items.Clear();

            cboKategoriObat.Items.Add("Semua Kategori");
            cboKategoriObat.Items.Add("Bebas");
            cboKategoriObat.Items.Add("Bebas Terbatas");
            cboKategoriObat.Items.Add("Keras");

            cboKategori.Items.Add("Bebas");
            cboKategori.Items.Add("Bebas Terbatas");
            cboKategori.Items.Add("Keras");

            cboKategori2.Items.Add("Bebas");
            cboKategori2.Items.Add("Bebas Terbatas");
            cboKategori2.Items.Add("Keras");

            cboKategoriObat.SelectedIndex = 0;
            cboKategori.SelectedIndex = -1;
            cboKategori2.SelectedIndex = -1;
        }

        private void LoadPenyimpananObat()
        {
            cboLokasiPenyimpanan.Items.Clear();
            cboLokasiPenyimpanan2.Items.Clear();

            cboLokasiPenyimpanan2.Items.Add("Rak Obat Bebas");
            cboLokasiPenyimpanan2.Items.Add("Rak Obat Bebas Terbatas");
            cboLokasiPenyimpanan2.Items.Add("Rak Obat Keras");
            cboLokasiPenyimpanan2.Items.Add("Kulkas");

            cboLokasiPenyimpanan.Items.Add("Rak Obat Bebas");
            cboLokasiPenyimpanan.Items.Add("Rak Obat Bebas Terbatas");
            cboLokasiPenyimpanan.Items.Add("Rak Obat Keras");
            cboLokasiPenyimpanan.Items.Add("Kulkas");

            cboLokasiPenyimpanan.SelectedIndex = -1;
            cboLokasiPenyimpanan2.SelectedIndex = -1;
        }

        private async Task LoadObatToComboBox()
        {
            try
            {
                var response = await client.GetAsync("http://localhost:8000/api/obats");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ObatData>>>();

                    cboObatEdit.DataSource = result.data;
                    cboObatEdit.DisplayMember = "nama_obat";
                    cboObatEdit.ValueMember = "id";

                    cboObatHapus.DataSource = result.data;
                    cboObatHapus.DisplayMember = "nama_obat";
                    cboObatHapus.ValueMember = "id";

                    cboObatHapus.SelectedIndex = -1;
                    cboObatEdit.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mengambil data obat: " + ex.Message);
            }
        }

        private void InisialisasiListObat()
        {
            lvwDaftarObat.View = View.Details;
            lvwDaftarObat.FullRowSelect = true;
            lvwDaftarObat.GridLines = true;

            lvwDaftarObat.Columns.Clear();
            lvwDaftarObat.Columns.Add("No", 40);
            lvwDaftarObat.Columns.Add("Kode Obat", 108);
            lvwDaftarObat.Columns.Add("Nama Obat", 217);
            lvwDaftarObat.Columns.Add("kategori Obat", 217);
            lvwDaftarObat.Columns.Add("Stok", 63);
            lvwDaftarObat.Columns.Add("Satuan", 74);
            lvwDaftarObat.Columns.Add("Harga Satuan", 114);
            lvwDaftarObat.Columns.Add("Lokasi Penyimpanan", 223);
            lvwDaftarObat.Columns.Add("Kadaluarsa", 131);

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

        private void btnProfilAkun_Click(object sender, EventArgs e)
        {
            new User_Profile_Form().Show();
            this.Hide();
        }

        private void btnKeluarAkun_Click(object sender, EventArgs e)
        {
            new Login_Form().Show();
            this.Hide();
        }

        private void btnResepMasuk_Click(object sender, EventArgs e)
        {
            pnlDaftarResep.Visible = true;
            pnlLaporan.Visible = false;
            pnlInventarisObat.Visible = false;

            pnlDaftarResep.BringToFront();
        }

        private void btnRiwayatResep_Click(object sender, EventArgs e)
        {
            pnlLaporan.Visible = true;
            pnlDaftarResep.Visible = false;
            pnlInventarisObat.Visible = false;

            pnlLaporan.BringToFront();
        }

        private void btnInventaris_Click(object sender, EventArgs e)
        {
            pnlInventarisObat.Visible = true;
            pnlLaporan.Visible = false;
            pnlDaftarResep.Visible = false;

            pnlInventarisObat.BringToFront();
        }

        private void btnTambahDaftarObat_Click(object sender, EventArgs e)
        {
            pnlPengelolaanInventaris.Visible = true;
            pnlTambahObat.Visible = true;
            pnlEditObat.Visible = false;
            pnlHapusObat.Visible = false;   

            pnlPengelolaanInventaris.BringToFront();

            btnResetTambahObat_Click(sender, e);
        }

        private void btnBatalTambahObat_Click(object sender, EventArgs e)
        {
            pnlPengelolaanInventaris.Visible = false;

            btnResetTambahObat_Click(sender , e);
        }

        private void btnEditDaftarObat_Click(object sender, EventArgs e)
        {
            pnlPengelolaanInventaris.Visible = true;
            pnlEditObat.Visible = true;
            pnlTambahObat.Visible = false;
            pnlHapusObat.Visible = false;

            pnlPengelolaanInventaris.BringToFront();
        }

        private void btnBatalEditObat_Click(object sender, EventArgs e)
        {
            pnlPengelolaanInventaris.Visible = false;

            btnResetEdit_Click(sender , e);
        }

        private void btnHapusDaftarObat_Click(object sender, EventArgs e)
        {
            pnlPengelolaanInventaris.Visible = true;
            pnlHapusObat.Visible = true;
            pnlEditObat.Visible = false;
            pnlTambahObat.Visible = false;  

            pnlPengelolaanInventaris.BringToFront();
        }

        private void btnBatalHapus_Click(object sender, EventArgs e)
        {
            pnlPengelolaanInventaris.Visible = false;

            btnResetHapus_Click_1(sender , e);
        }

        private async void btnCariResep_Click(object sender, EventArgs e)
        {
            await LoadDaftarResep();
        }

        private async void btnResetResep_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            cboPoli.SelectedIndex = 0;

            await LoadDaftarResep();
        }

        private void btnProses_Click(object sender, EventArgs e)
        {
            if (lvwListResep.SelectedItems.Count == 0)
            {
                MessageBox.Show("Silakan pilih salah satu resep terlebih dahulu!",
                    "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedResep = (ResepRequest)lvwListResep.SelectedItems[0].Tag;

            Apoteker_Proses frmProses = new Apoteker_Proses(selectedResep);
            frmProses.Show();

            this.Hide();
        }

        private void InisialisasiListResep()
        {
            lvwListResep.View = View.Details;
            lvwListResep.FullRowSelect = true;
            lvwListResep.GridLines = true;

            lvwListResep.Columns.Clear();
            lvwListResep.Columns.Add("No", 40);
            lvwListResep.Columns.Add("No. Antrian", 131);
            lvwListResep.Columns.Add("Kode Pasien", 131);
            lvwListResep.Columns.Add("Nama Pasien", 349);
            lvwListResep.Columns.Add("Nama Dokter", 300);
            lvwListResep.Columns.Add("Poli", 137);
            lvwListResep.Columns.Add("Tanggal", 131);

            lvwListResep.OwnerDraw = true;

            lvwListResep.DrawColumnHeader += (s, e) =>
            {
                using (SolidBrush brush = new SolidBrush(Color.DarkCyan))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }

                TextRenderer.DrawText(e.Graphics, e.Header.Text, e.Font,
                    e.Bounds, Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            };

            lvwListResep.DrawItem += (s, e) => e.DrawDefault = true;
            lvwListResep.DrawSubItem += (s, e) => e.DrawDefault = true;
        }

        private void InisialisasiListLaporan()
        {
            lsvListLaporan.View = View.Details;
            lsvListLaporan.FullRowSelect = true;
            lsvListLaporan.GridLines = true;

            lsvListLaporan.Columns.Clear();
            lsvListLaporan.Columns.Add("No", 40);
            lsvListLaporan.Columns.Add("No. Antrian", 131);
            lsvListLaporan.Columns.Add("Kode Pasien", 131);
            lsvListLaporan.Columns.Add("Nama Pasien", 349);
            lsvListLaporan.Columns.Add("Nama Dokter", 300);
            lsvListLaporan.Columns.Add("Poli", 137);
            lsvListLaporan.Columns.Add("Tanggal", 131);

            lsvListLaporan.OwnerDraw = true;

            lsvListLaporan.DrawColumnHeader += (s, e) =>
            {
                using (SolidBrush brush = new SolidBrush(Color.DarkCyan))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }

                TextRenderer.DrawText(e.Graphics, e.Header.Text, e.Font,
                    e.Bounds, Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            };

            lsvListLaporan.DrawItem += (s, e) => e.DrawDefault = true;
            lsvListLaporan.DrawSubItem += (s, e) => e.DrawDefault = true;
        }

        private async Task LoadDaftarResep()
        {
            try
            {
                string keyword = txtSearch.Text.Trim();
                string poli = cboPoli.SelectedItem?.ToString() ?? "Semua Poli";

                var response = await client.GetAsync($"http://localhost:8000/api/reseps?search={Uri.EscapeDataString(keyword)}&poli={Uri.EscapeDataString(poli)}");
               
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ResepRequest>>>();
                    lvwListResep.Items.Clear();

                    if (result?.data != null && result.data.Count > 0)
                    {
                        int no = 1;
                        foreach (var resep in result.data)
                        {
                            ListViewItem item = new ListViewItem(no.ToString());
                            item.SubItems.Add(resep.pemeriksaan?.register?.nomor_antrian ?? "-");
                            item.SubItems.Add(resep.pemeriksaan?.register?.pasien?.kode_pasien ?? "-");
                            item.SubItems.Add(resep.pemeriksaan?.register?.pasien?.nama_pasien ?? "-");
                            item.SubItems.Add(resep.pemeriksaan?.dokter?.user.nama_user ?? "-");
                            item.SubItems.Add(resep.pemeriksaan?.register?.poli?.nama_poli ?? "-");
                            item.SubItems.Add(resep.tanggal_resep);

                            item.Tag = resep; 
                            lvwListResep.Items.Add(item);
                            no++;
                        }
                    }
                    else
                    {
                        lvwListResep.Items.Clear();

                        ListViewItem itemKosong = new ListViewItem("");
                        itemKosong.SubItems.Add("-");
                        itemKosong.SubItems.Add("-");
                        itemKosong.SubItems.Add("Data Resep Masih Kosong");
                        itemKosong.SubItems.Add("-");
                        itemKosong.SubItems.Add("-");
                        itemKosong.SubItems.Add("-");
                        itemKosong.SubItems.Add("-");

                        itemKosong.ForeColor = Color.Red;

                        lvwListResep.Items.Add(itemKosong);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat resep: " + ex.Message);
            }
        }

        private async Task LoadPoliToComboBox()
        {
            try
            {
                var response = await client.GetAsync("http://localhost:8000/api/polis");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<PoliData>>>();

                    cboPoli.Items.Clear();

                    cboPoli.Items.Add("Semua Poli");

                    foreach (var poli in result.data)
                    {
                        cboPoli.Items.Add(poli.nama_poli);
                    }

                    cboPoli.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data poli: " + ex.Message);
            }
        }

        private async Task LoadHistoryResep()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                lsvListLaporan.Items.Clear();

                string tgl = dtpRiwayat.Value.ToString("yyyy-MM-dd");
                string url = $"http://localhost:8000/api/reseps/selesai?tanggal={tgl}";

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResponse<List<ResepData>>>(jsonResponse);

                    int no = 1;
                    foreach (var item in result.data)
                    {
                        ListViewItem lvi = new ListViewItem(no.ToString());
                        lvi.SubItems.Add(item.pemeriksaan?.register?.nomor_antrian ?? "-");
                        lvi.SubItems.Add(item.pemeriksaan?.register?.pasien?.kode_pasien ?? "-");
                        lvi.SubItems.Add(item.pemeriksaan?.register?.pasien?.nama_pasien ?? "-");
                        lvi.SubItems.Add(item.pemeriksaan?.dokter?.user?.nama_user ?? "-");
                        lvi.SubItems.Add(item.pemeriksaan?.register?.poli?.nama_poli ?? "-");
                        if (DateTime.TryParse(item.updated_at, out DateTime tglSelesai))
                            lvi.SubItems.Add(tglSelesai.ToString("dd-MM-yyyy HH:mm"));
                        else
                            lvi.SubItems.Add("-");

                        lsvListLaporan.Items.Add(lvi);
                        no++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat history resep: " + ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private async void btnCariRiwayat_Click(object sender, EventArgs e)
        {
            await LoadHistoryResep();
        }

        private async void btnResetRiwayat_Click(object sender, EventArgs e)
        {
            dtpRiwayat.Value = DateTime.Now;

            await LoadHistoryResep();
        }

        private async Task LoadDataObat()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                lvwDaftarObat.Items.Clear();

                string search = txtCariObat.Text.Trim();
                string kategori = cboKategoriObat.SelectedItem?.ToString() ?? "Semua Kategori";

                string url = $"http://localhost:8000/api/obats?search={Uri.EscapeDataString(search)}&kategori={Uri.EscapeDataString(kategori)}";

                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResponse<List<ObatData>>>(json);

                    int no = 1;
                    foreach (var obat in result.data)
                    {
                        ListViewItem lvi = new ListViewItem(no.ToString());

                        lvi.SubItems.Add(obat.kode_obat ?? "-");
                        lvi.SubItems.Add(obat.nama_obat ?? "-");
                        lvi.SubItems.Add(obat.kategori ?? "-");
                        lvi.SubItems.Add(obat.stok.ToString());
                        lvi.SubItems.Add(obat.satuan_obat ?? "-");
                        lvi.SubItems.Add(obat.harga_satuan.ToString("N0"));
                        lvi.SubItems.Add(obat.lokasi_penyimpanan ?? "-");
                        if (DateTime.TryParse(obat.kadaluarsa, out DateTime tglExp))
                            lvi.SubItems.Add(tglExp.ToString("dd-MM-yyyy"));
                        else
                            lvi.SubItems.Add(obat.kadaluarsa ?? "-");

                        lvi.Tag = obat.id;

                        if (obat.stok <= 5) lvi.ForeColor = Color.Red;

                        lvwDaftarObat.Items.Add(lvi);
                        no++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private async void btnCariObat_Click(object sender, EventArgs e)
        {
            await LoadDataObat();
        }

        private async void btnResetObat_Click(object sender, EventArgs e)
        {
            cboKategoriObat.SelectedIndex = 0;
            txtCariObat.Clear();

            await LoadDataObat();
        }

        private void btnCetakLaporan_Click(object sender, EventArgs e)
        {
            if (lsvListLaporan.Items.Count == 0)
            {
                MessageBox.Show("Tidak ada data resep untuk diekspor!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Workbook|*.xlsx";
                sfd.FileName = $"Laporan_Resep_Selesai_{dtpRiwayat.Value:yyyyMMdd}.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        OfficeOpenXml.ExcelPackage.License.SetNonCommercialPersonal("Admin Puskesmas");

                        using (var package = new OfficeOpenXml.ExcelPackage())
                        {
                            var worksheet = package.Workbook.Worksheets.Add("Laporan Resep");
                            int totalCols = lsvListLaporan.Columns.Count;

                            worksheet.Cells.Style.Font.Name = "Times New Roman";
                            worksheet.Cells.Style.Font.Size = 11;

                            var row1 = worksheet.Cells[1, 1, 1, totalCols];
                            row1.Merge = true;
                            row1.Value = "PUSKESMAS CITRA KASIH";
                            ApplyHeaderStyle(row1, 16, true);

                            var row2 = worksheet.Cells[2, 1, 2, totalCols];
                            row2.Merge = true;
                            row2.Value = "Jl. Kenanga Indah No.16, Desa Sekar Sari, Kecamatan Harapan Jaya, Kabupaten Permai Hulu, Provinsi Jayakarta 35643";
                            ApplyHeaderStyle(row2, 10, false);

                            var row3 = worksheet.Cells[3, 1, 3, totalCols];
                            row3.Merge = true;
                            row3.Value = "LAPORAN PROSES RESEP HARIAN";
                            ApplyHeaderStyle(row3, 12, true);

                            var row4 = worksheet.Cells[4, 1, 4, totalCols];
                            row4.Merge = true;
                            row4.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            row4.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 112, 192));

                            worksheet.Cells[5, 1].Value = "Atas Nama : " + (UserSession.CurrentUser.nama_user ?? "Apoteker");
                            worksheet.Cells[6, 1].Value = "Tanggal   : " + dtpRiwayat.Value.ToString("dd MMMM yyyy");

                            int startRow = 8;
                            for (int i = 0; i < totalCols; i++)
                            {
                                var cell = worksheet.Cells[startRow, i + 1];
                                cell.Value = lsvListLaporan.Columns[i].Text;

                                cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 112, 192));
                                cell.Style.Font.Color.SetColor(System.Drawing.Color.White);
                                cell.Style.Font.Bold = true;
                                cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            }

                            for (int i = 0; i < lsvListLaporan.Items.Count; i++)
                            {
                                int currentRow = startRow + 1 + i;

                                for (int j = 0; j < totalCols; j++)
                                {
                                    worksheet.Cells[currentRow, j + 1].Value = lsvListLaporan.Items[i].SubItems[j].Text;

                                    worksheet.Cells[currentRow, j + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                }
                            }

                            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                            package.SaveAs(new System.IO.FileInfo(sfd.FileName));
                        }
                        MessageBox.Show("Laporan Proses Resep Berhasil Diekspor!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal ekspor: " + ex.Message);
                    }
                }
            }
        }

        private void ApplyHeaderStyle(OfficeOpenXml.ExcelRange range, int fontSize, bool isBold)
        {
            range.Style.Font.Size = fontSize;
            range.Style.Font.Bold = isBold;
            range.Style.Font.Color.SetColor(System.Drawing.Color.White);
            range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 112, 192));
        }

        private async void btnSimpanTambahObat_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNamaObat.Text) || string.IsNullOrEmpty(txtStok.Text) || string.IsNullOrEmpty(txtHargaPerSatuan.Text) || string.IsNullOrEmpty(cboKategori.Text) || string.IsNullOrEmpty(cboLokasiPenyimpanan.Text) || string.IsNullOrEmpty(txtSatuan.Text))
            {
                MessageBox.Show("Data obat wajib diisi!");
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;

                var dataObat = new CRUDDataObat
                {
                    nama_obat = txtNamaObat.Text,
                    stok = int.Parse(txtStok.Text),
                    satuan_obat = txtSatuan.Text,
                    kadaluarsa = dtpObat.Value.ToString("yyyy-MM-dd"),
                    kategori = cboKategori.SelectedItem?.ToString().Trim(),
                    harga_satuan = int.Parse(txtHargaPerSatuan.Text.Replace(".", "")),
                    lokasi_penyimpanan = cboLokasiPenyimpanan.SelectedItem?.ToString().Trim()
                };

                string json = JsonConvert.SerializeObject(dataObat, Formatting.Indented);

                MessageBox.Show("DEBUG DATA YANG DIKIRIM:\n\n" + json, "Cek Format Data", MessageBoxButtons.OK, MessageBoxIcon.Information);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("http://localhost:8000/api/obats", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Data obat berhasil ditambahkan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    await LoadDataObat();

                    pnlPengelolaanInventaris.Visible = false;
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Gagal menyimpan: " + error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnResetTambahObat_Click(object sender, EventArgs e)
        {
            txtNamaObat.Clear();
            txtStok.Clear();
            txtSatuan.Clear();
            txtHargaPerSatuan.Clear();

            cboKategori.SelectedIndex = -1;
            cboLokasiPenyimpanan.SelectedIndex = -1;

            dtpObat.Value = DateTime.Now;

            txtNamaObat.Focus();

        }

        private async void btnCariObatEdit_Click(object sender, EventArgs e)
        {
            if (cboObatEdit.SelectedValue == null || !(cboObatEdit.SelectedValue is int))
            {
                MessageBox.Show("Silahkan pilih obat dari daftar terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idObat = (int)cboObatEdit.SelectedValue;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserSession.Token);

                    var response = await client.GetAsync($"http://localhost:8000/api/obat/{idObat}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<ApiResponse<ObatData>>(content);
                        var obat = result.data;

                        selectedObatId = obat.id; 
                        txtNamaObat2.Text = obat.nama_obat;
                        txtHargaPerSatuan2.Text = obat.harga_satuan.ToString();
                        txtStok2.Text = obat.stok.ToString();
                        txtSatuan2.Text = obat.satuan_obat;
                        dtpKadaluarsaEdit.Value = DateTime.Parse(obat.kadaluarsa);
                        cboKategori2.Text = obat.kategori;
                        cboLokasiPenyimpanan2.Text = obat.lokasi_penyimpanan;

                        MessageBox.Show($"Data {obat.nama_obat} berhasil dimuat!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Gagal mengambil detail obat dari server.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan koneksi: " + ex.Message);
            }
        }

        private void btnResetEdit_Click(object sender, EventArgs e)
        {
            selectedObatId = 0; 

            cboObatEdit.SelectedIndex = -1;

            txtNamaObat2.Clear();
            txtHargaPerSatuan2.Clear();
            txtStok2.Clear();
            txtSatuan2.Clear();
            cboKategori2.SelectedIndex = -1;
            cboLokasiPenyimpanan2.SelectedIndex = -1;
            dtpKadaluarsaEdit.Value = DateTime.Now;

            cboObatEdit.Focus();
        }

        private async void btnSimpanEditObat_Click(object sender, EventArgs e)
        {
            if (selectedObatId == 0)
            {
                MessageBox.Show("Cari obat yang ingin diedit terlebih dahulu!");
                return;
            }

            var dataUpdate = new
            {
                nama_obat = txtNamaObat2.Text,
                stok = int.Parse(txtStok2.Text),
                satuan_obat = txtSatuan2.Text,
                kategori = cboKategori2.Text,
                harga_satuan = long.Parse(txtHargaPerSatuan2.Text),
                kadaluarsa = dtpKadaluarsaEdit.Value.ToString("yyyy-MM-dd"),
                lokasi_penyimpanan = cboLokasiPenyimpanan2.Text
            };

            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserSession.Token);

                var json = JsonConvert.SerializeObject(dataUpdate);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"http://localhost:8000/api/obats/{selectedObatId}", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Data obat berhasil diperbarui!");

                    btnResetEdit_Click(sender, e);
                    await LoadDataObat();
                    pnlPengelolaanInventaris.Visible = false;
                }
                else
                {
                    MessageBox.Show("Gagal memperbarui data. Cek validasi input.");
                }
            }
        }

        private async void btnCariObatHapus_Click(object sender, EventArgs e)
        {
            if (cboObatHapus.SelectedValue == null || !(cboObatHapus.SelectedValue is int))
            {
                MessageBox.Show("Silahkan pilih obat dari daftar terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idObat = (int)cboObatHapus.SelectedValue;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserSession.Token);

                    var response = await client.GetAsync($"http://localhost:8000/api/obat/{idObat}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<ApiResponse<ObatData>>(content);
                        var obat = result.data;

                        selectedObatId = obat.id;
                        txtNamaObat3.Text = obat.nama_obat;
                        txtHargaPerSatuan3.Text = obat.harga_satuan.ToString();
                        txtStok3.Text = obat.stok.ToString();
                        txtSatuan3.Text = obat.satuan_obat;
                        txtTanggalKadaluarsa3.Text =  obat.kadaluarsa;
                        txtKategori.Text = obat.kategori;
                        txtLokasiPenyimpanan.Text = obat.lokasi_penyimpanan;

                        MessageBox.Show($"Data {obat.nama_obat} berhasil dimuat!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Gagal mengambil detail obat dari server.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan koneksi: " + ex.Message);
            }
        }

        private async void btnHapusObat_Click(object sender, EventArgs e)
        {
            if (selectedObatId == 0)
            {
                MessageBox.Show("Silakan cari obat yang ingin dihapus terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var konfirmasi = MessageBox.Show($"Apakah Anda yakin ingin menghapus data obat: {txtNamaObat.Text}?",
                                             "Konfirmasi Hapus",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);

            if (konfirmasi == DialogResult.Yes)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", UserSession.Token);

                        var response = await client.DeleteAsync($"http://localhost:8000/api/obats/{selectedObatId}");

                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Data obat berhasil dihapus dari sistem.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            btnResetHapus_Click_1(sender, e);
                            await LoadDataObat();
                            pnlPengelolaanInventaris.Visible = false;
                        }
                        else
                        {
                            string errorMsg = await response.Content.ReadAsStringAsync();
                            MessageBox.Show("Gagal menghapus data. Detail: " + errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Terjadi kesalahan koneksi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnResetHapus_Click_1(object sender, EventArgs e)
        {
            selectedObatId = 0;

            cboObatHapus.SelectedIndex = -1;

            txtNamaObat3.Clear();
            txtHargaPerSatuan3.Clear();
            txtStok3.Clear();
            txtSatuan3.Clear();
            txtKategori.Clear();
            txtLokasiPenyimpanan.Clear();
            txtTanggalKadaluarsa3.Clear();

            cboObatHapus.Focus();
        }
    }
}
