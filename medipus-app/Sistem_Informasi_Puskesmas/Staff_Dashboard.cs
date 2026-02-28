using MySqlX.XDevAPI;
using Newtonsoft.Json;
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

namespace Sistem_Informasi_Puskesmas
{
    public partial class Staff_Dashboard : Form
    {
        public static readonly HttpClient client = new HttpClient();
        public Staff_Dashboard()
        {
            InitializeComponent();
        }

        public async void Staff_Dashboard_Load(object sender, EventArgs e)
        {
            if (UserSession.CurrentUser != null)
            {
                lblPengguna.Text = "Staff - " + UserSession.CurrentUser.nama_user;
            }

            lblDate.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
            lblDate2.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
            lblDate3.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));

            InisialisasiListPasien();
            InisialisasiListLaporan();
            InisialisasiJadwalDokter();

            await LoadPoliToComboBox();
            await LoadDokterToComboBox();
            LoadHariToComboBox();

            await LoadDataKunjungan();
            await LoadLaporanKunjungan();
            await LoadJadwalDokter();
        }

        private void LoadHariToComboBox()
        {
            cboHari.Items.Clear();
            cboHari.Items.Add("Semua Hari");
            cboHari.Items.Add("Senin");
            cboHari.Items.Add("Selasa");
            cboHari.Items.Add("Rabu");
            cboHari.Items.Add("Kamis");
            cboHari.Items.Add("Jumat");
            cboHari.Items.Add("Sabtu");
            cboHari.Items.Add("Minggu");

            cboHari.SelectedIndex = 0;
        }

        private async Task LoadPoliToComboBox()
        {
            try
            {
                var response = await client.GetAsync("http://localhost:8000/api/polis");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<PoliData>>>();

                    cboPoli.Items.Clear(); cboPoli2.Items.Clear();

                    cboPoli.Items.Add("Semua Poli"); cboPoli2.Items.Add("Semua Poli");

                    foreach (var poli in result.data)
                    {
                        cboPoli.Items.Add(poli.nama_poli);
                        cboPoli2.Items.Add(poli.nama_poli);
                    }

                    cboPoli.SelectedIndex = 0;
                    cboPoli2.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data poli: " + ex.Message);
            }
        }

        private void InisialisasiListPasien()
        {
            lvwListPasien.View = View.Details;
            lvwListPasien.FullRowSelect = true;
            lvwListPasien.GridLines = true;

            lvwListPasien.Columns.Clear();
            lvwListPasien.Columns.Add("No", 40);
            lvwListPasien.Columns.Add("No. Antrian", 131);
            lvwListPasien.Columns.Add("Kode Pasien", 131);
            lvwListPasien.Columns.Add("Nama Pasien", 349);
            lvwListPasien.Columns.Add("Poli", 148);
            lvwListPasien.Columns.Add("Ruangan", 171);
            lvwListPasien.Columns.Add("Jadwal", 91);
            lvwListPasien.Columns.Add("Tanggal", 137);

            lvwListPasien.OwnerDraw = true;

            lvwListPasien.DrawColumnHeader += (s, e) =>
            {
                using (SolidBrush brush = new SolidBrush(Color.DarkCyan))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }

                TextRenderer.DrawText(e.Graphics, e.Header.Text, e.Font,
                    e.Bounds, Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            };

            lvwListPasien.DrawItem += (s, e) => e.DrawDefault = true;
            lvwListPasien.DrawSubItem += (s, e) => e.DrawDefault = true;
        }

        private void InisialisasiJadwalDokter()
        {
            lvwJadwalPraktik.View = View.Details;
            lvwJadwalPraktik.FullRowSelect = true;
            lvwJadwalPraktik.GridLines = true;

            lvwJadwalPraktik.Columns.Clear();
            lvwJadwalPraktik.Columns.Add("Nama Dokter", 274);
            lvwJadwalPraktik.Columns.Add("Poli", 160);
            lvwJadwalPraktik.Columns.Add("Ruangan", 280);
            lvwJadwalPraktik.Columns.Add("Jadwal", 171);
            lvwJadwalPraktik.Columns.Add("Hari", 131);

            lvwJadwalPraktik.OwnerDraw = true;

            lvwJadwalPraktik.DrawColumnHeader += (s, e) =>
            {
                using (SolidBrush brush = new SolidBrush(Color.DarkCyan))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }

                TextRenderer.DrawText(e.Graphics, e.Header.Text, e.Font,
                    e.Bounds, Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            };

            lvwJadwalPraktik.DrawItem += (s, e) => e.DrawDefault = true;
            lvwJadwalPraktik.DrawSubItem += (s, e) => e.DrawDefault = true;
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
            lsvListLaporan.Columns.Add("Poli", 148);
            lsvListLaporan.Columns.Add("Jadwal", 91);
            lsvListLaporan.Columns.Add("Tanggal", 137);

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

        private async Task LoadDokterToComboBox()
        {
            try
            {
                string url = "http://localhost:8000/api/dokters"; 
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResponse<List<DataDokter>>>(json);

                    cboDokter.Items.Clear();
                    cboDokter.Items.Add("Semua Dokter");

                    foreach (var d in result.data)
                    {
                        cboDokter.Items.Add(d.user.nama_user);
                    }
                    cboDokter.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat daftar dokter: " + ex.Message);
            }
        }

        private async Task LoadDataKunjungan()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                lvwListPasien.Items.Clear();

                string keyword = txtSearch.Text.Trim();
                string poli = cboPoli.SelectedItem?.ToString() ?? "Semua Poli";

                string url = $"http://localhost:8000/api/registers/semua?search={Uri.EscapeDataString(keyword)}&poli={Uri.EscapeDataString(poli)}";

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResponse<List<DataPasienResponse>>>(jsonResponse);

                if (result?.data != null && result.data.Count > 0)
                {
                        int no = 1;
                    foreach (var item in result.data)
                    {
                        ListViewItem lvi = new ListViewItem(no.ToString());
                        lvi.SubItems.Add(item.nomor_antrian ?? "-");
                        lvi.SubItems.Add(item.pasien?.kode_pasien ?? "-");
                        lvi.SubItems.Add(item.pasien?.nama_pasien ?? "-");
                        lvi.SubItems.Add(item.poli?.nama_poli ?? "-");
                        lvi.SubItems.Add(item.jadwal_dokter?.ruangan?.nama_ruangan ?? "-");
                        lvi.SubItems.Add(item.jadwal?.Substring(0, 5) ?? "-");

                        if (DateTime.TryParse(item.tanggal_register, out DateTime tgl))
                            lvi.SubItems.Add(tgl.ToString("dd-MM-yyyy"));
                        else
                            lvi.SubItems.Add(item.tanggal_register ?? "-");

                        lvi.Tag = item.id;
                        lvwListPasien.Items.Add(lvi);
                        no++;
                    }
                }
                else
                {
                    lvwListPasien.Items.Clear();

                    ListViewItem itemKosong = new ListViewItem("");
                    itemKosong.SubItems.Add("-");
                    itemKosong.SubItems.Add("-");
                    itemKosong.SubItems.Add("Data Pasien Masih Kosong");
                    itemKosong.SubItems.Add("-");
                    itemKosong.SubItems.Add("-");
                    itemKosong.SubItems.Add("-");
                    itemKosong.SubItems.Add("-");

                    itemKosong.ForeColor = Color.Red;

                    lvwListPasien.Items.Add(itemKosong);
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

        private async Task LoadLaporanKunjungan()
        {
            try
            {
                lsvListLaporan.Items.Clear();

                string day = dtpKunjungan.Value.ToString("yyyy-MM-dd");
                string url = $"http://localhost:8000/api/registers/laporan?tanggal={day}";

                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResponse<List<DataPasienResponse>>>(jsonResponse);

                    int no = 1;
                    foreach (var item in result.data)
                    {
                        ListViewItem lvi = new ListViewItem(no.ToString());
                        lvi.SubItems.Add(item.nomor_antrian ?? "-");
                        lvi.SubItems.Add(item.pasien?.kode_pasien ?? "-");
                        lvi.SubItems.Add(item.pasien?.nama_pasien ?? "-");
                        lvi.SubItems.Add(item.poli?.nama_poli ?? "-");
                        lvi.SubItems.Add(item.jadwal?.Substring(0, 5) ?? "-");

                        if (DateTime.TryParse(item.tanggal_register, out DateTime tgl))
                            lvi.SubItems.Add(tgl.ToString("dd-MM-yyyy"));
                        else
                            lvi.SubItems.Add(item.tanggal_register ?? "-");

                        lsvListLaporan.Items.Add(lvi);
                        no++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Laporan: " + ex.Message);
            }
        }

        private async Task LoadJadwalDokter()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                lvwJadwalPraktik.Items.Clear();

                string dokterTerpilih = cboDokter.SelectedItem?.ToString() ?? "Semua Dokter";
                string hari = cboHari.SelectedItem?.ToString() ?? "Semua Hari";
                string poli = cboPoli2.SelectedItem?.ToString() ?? "Semua Poli";

                string url = $"http://localhost:8000/api/jadwal_dokter?hari={Uri.EscapeDataString(hari)}&poli={Uri.EscapeDataString(poli)}";

                if (dokterTerpilih != "Semua Dokter")
                {
                    url += $"&search={Uri.EscapeDataString(dokterTerpilih)}";
                }

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResponse<List<JadwalResponse>>>(json);

                    int no = 1;
                    foreach (var item in result.data)
                    {
                        ListViewItem lvi = new ListViewItem(item.dokter?.user?.nama_user ?? "-");

                        lvi.SubItems.Add(item.dokter?.poli?.nama_poli ?? "-");
                        lvi.SubItems.Add(item.ruangan?.nama_ruangan ?? "-");
                        string jamTampil = "-";
                        if (!string.IsNullOrEmpty(item.jam_mulai) && !string.IsNullOrEmpty(item.jam_selesai))
                        {
                            try
                            {
                                DateTime mulai = DateTime.Parse(item.jam_mulai);
                                DateTime selesai = DateTime.Parse(item.jam_selesai);
                                jamTampil = $"{mulai:HH:mm} - {selesai:HH:mm}";
                            }
                            catch
                            {
                                jamTampil = $"{item.jam_mulai} - {item.jam_selesai}";
                            }
                        }
                        lvi.SubItems.Add(jamTampil);
                        lvi.SubItems.Add(item.hari ?? "-");

                        lvwJadwalPraktik.Items.Add(lvi);
                        no++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat jadwal: " + ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
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

        private void btnDaftarPasien_Click(object sender, EventArgs e)
        {
            pnlDaftarPasien.Visible = true;
            pnlJadwal.Visible = false;
            pnlLaporan.Visible = false;

            pnlDaftarPasien.BringToFront();
        }

        private void btnJadwalPraktik_Click(object sender, EventArgs e)
        {
            pnlDaftarPasien.Visible = false;
            pnlJadwal.Visible = true;
            pnlLaporan.Visible = false;

            pnlJadwal.BringToFront();
        }

        private void btnLaporan_Click(object sender, EventArgs e)
        {
            pnlDaftarPasien.Visible = false;
            pnlJadwal.Visible = false;
            pnlLaporan.Visible = true;

            pnlLaporan.BringToFront();
        }

        private void btnRegistrasi_Click(object sender, EventArgs e)
        {
            new Register_Pasien_Form().Show();
            this.Hide();
        }

        private async void btnCariDaftarPasien_Click(object sender, EventArgs e)
        {
            await LoadDataKunjungan();
        }

        private async void btnResetDaftarPasien_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            cboPoli.SelectedIndex = 0;

            await LoadDataKunjungan();
        }

        private async void btnCariLaporan_Click(object sender, EventArgs e)
        {
            await LoadLaporanKunjungan();

        }

        private async void btnResetLaporan_Click(object sender, EventArgs e)
        {
            dtpKunjungan.Value = DateTime.Now;

            await LoadLaporanKunjungan();
        }

        private async void btnCariJadwalPraktik_Click(object sender, EventArgs e)
        {
            await LoadJadwalDokter();
        }

        private async void btnResetJadwalPraktik_Click(object sender, EventArgs e)
        {
            cboDokter.SelectedIndex = 0; 
            cboPoli2.SelectedIndex = 0;
            cboHari.SelectedIndex = 0;

            await LoadJadwalDokter();
        }

        private void btnCetak_Click(object sender, EventArgs e)
        {
            if (lsvListLaporan.Items.Count == 0)
            {
                MessageBox.Show("Tidak ada data kunjungan untuk diekspor!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Workbook|*.xlsx";
                sfd.FileName = $"Laporan_Kunjungan_{dtpKunjungan.Value:yyyyMMdd}.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        OfficeOpenXml.ExcelPackage.License.SetNonCommercialPersonal("Admin Puskesmas");

                        using (var package = new OfficeOpenXml.ExcelPackage())
                        {
                            var worksheet = package.Workbook.Worksheets.Add("Laporan Kunjungan");
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
                            row3.Value = "LAPORAN KUNJUNGAN PASIEN HARIAN";
                            ApplyHeaderStyle(row3, 12, true);

                            var row4 = worksheet.Cells[4, 1, 4, totalCols];
                            row4.Merge = true;
                            row4.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            row4.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 112, 192));

                            worksheet.Cells[5, 1].Value = "Atas Nama : " + (UserSession.CurrentUser.nama_user ?? "Staff");
                            worksheet.Cells[6, 1].Value = "Tanggal   : " + dtpKunjungan.Value.ToString("dd MMMM yyyy");

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

                                worksheet.Cells[currentRow, 1].Value = lsvListLaporan.Items[i].Text;
                                worksheet.Cells[currentRow, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                                for (int j = 1; j < totalCols; j++)
                                {
                                    worksheet.Cells[currentRow, j + 1].Value = lsvListLaporan.Items[i].SubItems[j].Text;
                                    worksheet.Cells[currentRow, j + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                }
                            }

                            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                            package.SaveAs(new System.IO.FileInfo(sfd.FileName));
                        }
                        MessageBox.Show("Laporan Kunjungan Berhasil Diekspor!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
    }
}
