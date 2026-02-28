using Newtonsoft.Json;
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
using System.Xml.Linq;

namespace Sistem_Informasi_Puskesmas
{
    public partial class Dokter_Dashboard : Form
    {
        public static readonly HttpClient client = new HttpClient();

        private DataPasienResponse _data;

        public Dokter_Dashboard()
        {
            InitializeComponent();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async void Dokter_Dashboard_Load(object sender, EventArgs e)
        {
            if (UserSession.CurrentUser != null)
            {
                lblPenguna.Text = "Dokter - " + UserSession.CurrentUser.nama_user;
            }

            lblDate.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
            lblDate2.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));

            InisialisasiListPasien();
            InisialisasiListPemeriksaan();

            await TampilkanDataPasien();
            await LoadHistoryPemeriksaan();
        }

        private void InisialisasiListPemeriksaan()
        {
            lvwListLaporan.View = View.Details;
            lvwListLaporan.FullRowSelect = true;
            lvwListLaporan.GridLines = true;

            lvwListLaporan.Columns.Clear();
            lvwListLaporan.Columns.Add("No", 40);
            lvwListLaporan.Columns.Add("No. Antrian", 91);
            lvwListLaporan.Columns.Add("Kode Pasien", 103);
            lvwListLaporan.Columns.Add("Nama Pasien", 285);
            lvwListLaporan.Columns.Add("Diagnosa", 263);
            lvwListLaporan.Columns.Add("Tindakan", 246);
            lvwListLaporan.Columns.Add("Tanggal", 137);

            lvwListLaporan.OwnerDraw = true;

            lvwListLaporan.DrawColumnHeader += (s, e) =>
            {
                using (SolidBrush brush = new SolidBrush(Color.DarkCyan))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }

                TextRenderer.DrawText(e.Graphics, e.Header.Text, e.Font,
                    e.Bounds, Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            };

            lvwListLaporan.DrawItem += (s, e) => e.DrawDefault = true;
            lvwListLaporan.DrawSubItem += (s, e) => e.DrawDefault = true;

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
            lvwListPasien.Columns.Add("Jadwal", 91);
            lvwListPasien.Columns.Add("Tanggal", 137);
            lvwListPasien.Columns.Add("Status", 131);

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

        private async Task TampilkanDataPasien()
        {
            try
            {
                string keyword = txtSearch.Text.Trim();
                string day = lblDate.Text;

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", UserSession.Token);

                var response = await client.GetAsync($"http://localhost:8000/api/registers/dokter?tanggal={day}&search={keyword}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<DataPasienResponse>>>();

                    lvwListPasien.Items.Clear();

                    if (result?.data != null && result.data.Count > 0)
                    {
                        int no = 1;


                        foreach (var item in result.data)
                        {
                            ListViewItem row = new ListViewItem(no.ToString());

                            row.SubItems.Add(item.nomor_antrian ?? "-");
                            row.SubItems.Add(item.pasien?.kode_pasien ?? "-");
                            row.SubItems.Add(item.pasien?.nama_pasien ?? "-");
                            row.SubItems.Add(item.jadwal?.Substring(0, 5) ?? "-");

                            if (DateTime.TryParse(item.tanggal_register, out DateTime tgl))
                                row.SubItems.Add(tgl.ToString("dd-MM-yyyy"));
                            else
                                row.SubItems.Add("-");

                            string status = item.status_register ?? "Menunggu";
                            row.SubItems.Add(status);

                            row.Tag = item;

                            lvwListPasien.Items.Add(row);
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
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Gagal! Kode: {response.StatusCode}\nSebab: {errorContent}");
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show("Error saat mengambil data: " + ex.Message, "Kesalahan", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnKeluarAkun_Click(object sender, EventArgs e)
        {
            new Login_Form().Show();
            this.Hide();
        }

        private void btnProfilAkun_Click(object sender, EventArgs e)
        {
            new User_Profile_Form().Show();
            this.Hide();
        }

        private void btnDaftarPasien_Click(object sender, EventArgs e)
        {
            pnlRiwayat.Visible = false;
            pnlDaftarPasien.Visible = true;

            pnlDaftarPasien.BringToFront();
        }

        private void btnPemeriksaan_Click(object sender, EventArgs e)
        {
            pnlDaftarPasien.Visible = false;
            pnlRiwayat.Visible = true;

            pnlRiwayat.BringToFront();
        }

        private void btnPeriksaPasien_Click(object sender, EventArgs e)
        {

            if (lvwListPasien.SelectedItems.Count > 0)
            {
                var dataTerpilih = (DataPasienResponse)lvwListPasien.SelectedItems[0].Tag;

                Dokter_Pemeriksaan frmPemeriksaan = new Dokter_Pemeriksaan(dataTerpilih);
                frmPemeriksaan.ShowDialog();

                this.Hide();
            }
            else
            {
                MessageBox.Show("Silakan pilih pasien dari daftar terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private async void btnCariDaftarPasien_Click(object sender, EventArgs e)
        {
            await TampilkanDataPasien();
        }

        private async void btnReetDaftarPasien_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();

            await TampilkanDataPasien();

            txtSearch.Focus();
        }

        private async Task LoadHistoryPemeriksaan()
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserSession.Token);
                string tanggal = dtpPemeriksaan.Value.ToString("yyyy-MM-dd");

                string url = $"http://localhost:8000/api/pemeriksaan/history?tanggal={tanggal}";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<HistoryResponse>(content);

                    lvwListLaporan.Items.Clear();

                    if (result?.data != null && result.data.Count > 0)
                    {
                        int no = 1;

                        foreach (var item in result.data)
                        {
                            ListViewItem lvi = new ListViewItem(no.ToString());

                            lvi.SubItems.Add(item.no_antrian ?? "-");
                            lvi.SubItems.Add(item.kode_pasien ?? "-");
                            lvi.SubItems.Add(item.nama_pasien ?? "-");
                            lvi.SubItems.Add(item.diagnosa ?? "-");
                            lvi.SubItems.Add(item.tindakan ?? "Tanpa Tindakan");
                            lvi.SubItems.Add(item.tanggal ?? "-");

                            lvwListLaporan.Items.Add(lvi);

                            no++;
                        }
                    }
                    else
                    {
                        lvwListLaporan.Items.Clear();

                        ListViewItem itemKosong = new ListViewItem("");
                        itemKosong.SubItems.Add("-");
                        itemKosong.SubItems.Add("-");
                        itemKosong.SubItems.Add("Data Pemeriksaan Masih Kosong");
                        itemKosong.SubItems.Add("-");
                        itemKosong.SubItems.Add("-");
                        itemKosong.SubItems.Add("-");
                        itemKosong.SubItems.Add("-");

                        itemKosong.ForeColor = Color.Red;

                        lvwListLaporan.Items.Add(itemKosong);
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat history: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnResetRiwayat_Click(object sender, EventArgs e)
        {
            dtpPemeriksaan.Value = DateTime.Now;

            await LoadHistoryPemeriksaan();
        }

        private async void btnCariRiwayat_Click(object sender, EventArgs e)
        {

            await LoadHistoryPemeriksaan();
        }

        private void btnCetakLaporan_Click(object sender, EventArgs e)
        {
            if (lvwListLaporan.Items.Count == 0 || lvwListLaporan.Items[0].SubItems[3].Text == "Data Pemeriksaan Masih Kosong")
            {
                MessageBox.Show("Tidak ada data pemeriksaan untuk diekspor!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Workbook|*.xlsx";
                sfd.FileName = $"Laporan_Pemeriksaan_{dtpPemeriksaan.Value:yyyyMMdd}.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        OfficeOpenXml.ExcelPackage.License.SetNonCommercialPersonal("Admin Puskesmas");

                        using (var package = new OfficeOpenXml.ExcelPackage())
                        {
                            var worksheet = package.Workbook.Worksheets.Add("Laporan Pemeriksaan");
                            int totalCols = lvwListLaporan.Columns.Count;

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
                            row3.Value = "LAPORAN PEMERIKSAAN PASIEN HARIAN";
                            ApplyHeaderStyle(row3, 12, true);

                            var row4 = worksheet.Cells[4, 1, 4, totalCols];
                            row4.Merge = true;
                            row4.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            row4.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 112, 192));

                            worksheet.Cells[5, 1].Value = "Atas Nama : " + (UserSession.CurrentUser.nama_user ?? "Dokter");
                            worksheet.Cells[6, 1].Value = "Tanggal   : " + dtpPemeriksaan.Value.ToString("dd MMMM yyyy");

                            int startRow = 8;
                            for (int i = 0; i < totalCols; i++)
                            {
                                var cell = worksheet.Cells[startRow, i + 1];
                                cell.Value = lvwListLaporan.Columns[i].Text;

                                cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 112, 192));
                                cell.Style.Font.Color.SetColor(System.Drawing.Color.White);
                                cell.Style.Font.Bold = true;
                                cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            }

                            for (int i = 0; i < lvwListLaporan.Items.Count; i++)
                            {
                                int currentRow = startRow + 1 + i;

                                for (int j = 0; j < totalCols; j++)
                                {
                                    worksheet.Cells[currentRow, j + 1].Value = lvwListLaporan.Items[i].SubItems[j].Text;

                                    worksheet.Cells[currentRow, j + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                                }
                            }

                            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                            package.SaveAs(new System.IO.FileInfo(sfd.FileName));
                        }
                        MessageBox.Show("Laporan Pemeriksaan Berhasil Diekspor!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
