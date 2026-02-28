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

using OfficeOpenXml;
using System.IO;

namespace Sistem_Informasi_Puskesmas
{
    public partial class Staff_Transaksi : Form
    {
        private static readonly HttpClient client = new HttpClient();

        public int idTerpilih = 0;
        public Staff_Transaksi()
        {
            InitializeComponent();
        }

        private void ListViewDetailPenyerahanObat()
        {
            lvwDetailObat.View = View.Details;
            lvwDetailObat.FullRowSelect = true;
            lvwDetailObat.GridLines = true;

            lvwDetailObat.Columns.Clear();
            lvwDetailObat.Columns.Add("Nama Obat", 171);
            lvwDetailObat.Columns.Add("Harga Obat", 103);
            lvwDetailObat.Columns.Add("Jumlah", 86);
            lvwDetailObat.Columns.Add("SubTotal", 114);
            lvwDetailObat.OwnerDraw = true;

            lvwDetailObat.DrawColumnHeader += (s, e) =>
            {
                using (SolidBrush brush = new SolidBrush(Color.DarkCyan))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }

                TextRenderer.DrawText(e.Graphics, e.Header.Text, e.Font,
                    e.Bounds, Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            };

            lvwDetailObat.DrawItem += (s, e) => e.DrawDefault = true;
            lvwDetailObat.DrawSubItem += (s, e) => e.DrawDefault = true;

        }

        private void InisialisasiListTransaksi()
        {
            lsvListLaporan.View = View.Details;
            lsvListLaporan.FullRowSelect = true;
            lsvListLaporan.GridLines = true;

            lsvListLaporan.Columns.Clear();
            lsvListLaporan.Columns.Add("No", 40);
            lsvListLaporan.Columns.Add("Kode Pasien", 131);
            lsvListLaporan.Columns.Add("Nama Pasien", 349);
            lsvListLaporan.Columns.Add("Poli", 137);
            lsvListLaporan.Columns.Add("Biaya Tindakan", 114);
            lsvListLaporan.Columns.Add("Biaya Obat", 114);
            lsvListLaporan.Columns.Add("Biaya Transaksi", 124);
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

        private void InisialisasiListResep()
        {
            lvwListPasien.View = View.Details;
            lvwListPasien.FullRowSelect = true;
            lvwListPasien.GridLines = true;

            lvwListPasien.Columns.Clear();
            lvwListPasien.Columns.Add("No", 40);
            lvwListPasien.Columns.Add("No. Antrian", 131);
            lvwListPasien.Columns.Add("Kode Pasien", 131);
            lvwListPasien.Columns.Add("Nama Pasien", 349);
            lvwListPasien.Columns.Add("Poli", 137);
            lvwListPasien.Columns.Add("Biaya Tindakan", 114);
            lvwListPasien.Columns.Add("Biaya Obat", 114);
            lvwListPasien.Columns.Add("Biaya Transaksi", 124);
            lvwListPasien.Columns.Add("Tanggal", 131);

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

        public async void Staff_Transaksi_Load(object sender, EventArgs e)
        {
            if (UserSession.CurrentUser != null)
            {
                lblPengguna.Text = "Staff - " + UserSession.CurrentUser.nama_user;
            }

            lblDate.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
            lblDate3.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));

            InisialisasiListResep();
            InisialisasiListTransaksi();
            ListViewDetailPenyerahanObat();

            await LoadDataTransaksi();
            await LoadHistoryTransaksi();

            await LoadPoliToComboBox();
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

        private async Task LoadDataTransaksi()
        {
            try
            {
                lvwListPasien.Items.Clear();
                this.Cursor = Cursors.WaitCursor;

                string keyword = txtSearch.Text.Trim();
                string poliTerpilih = cboPoli.SelectedItem?.ToString() ?? "Semua Poli";

                string url = $"http://localhost:8000/api/transaksis?search={Uri.EscapeDataString(keyword)}";

                if (poliTerpilih != "Semua Poli")
                {
                    url += $"&poli={Uri.EscapeDataString(poliTerpilih)}";
                }

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResponse<List<TransaksiDetail>>>(jsonResponse);

                    int no = 1;

                    foreach (var item in result.data)
                    {
                        ListViewItem lvi = new ListViewItem(no.ToString());
                        lvi.SubItems.Add(item.no_antrian.ToString());
                        lvi.SubItems.Add(item.kode_pasien);
                        lvi.SubItems.Add(item.nama_pasien);
                        lvi.SubItems.Add(item.poli);

                        lvi.SubItems.Add(string.Format("{0:N0}", item.biaya_tindakan));
                        lvi.SubItems.Add(string.Format("{0:N0}", item.biaya_obat));
                        lvi.SubItems.Add(string.Format("{0:N0}", item.total_biaya));
                        lvi.SubItems.Add(item.tanggal);

                        lvi.Tag = item.id;

                        lvwListPasien.Items.Add(lvi);

                        no++;
                    }

                }
                else
                {
                    MessageBox.Show("Gagal mengambil data transaksi.");
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

        private void btnDaftarStruk_Click(object sender, EventArgs e)
        {
            pnlDaftarStruk.Visible = true;
            pnlLaporanTransaksi.Visible = false;

            pnlDaftarStruk.BringToFront();
        }

        private void btnRiwayatTransaksi_Click(object sender, EventArgs e)
        {
            pnlDaftarStruk.Visible = false;
            pnlLaporanTransaksi.Visible = true;

            pnlLaporanTransaksi.BringToFront();
        }

        private void btnProfilAkun_Click(object sender, EventArgs e)
        {
            new User_Profile_Form().Show();
            this.Hide();    
        }

        private void btnKeluarAkun_Click(object sender, EventArgs e)
        {
            new Login_Form().Show();
            this.Close();
        }

        private async void btnProsesStruk_Click(object sender, EventArgs e)
        {

            if (lvwListPasien.SelectedItems.Count == 0)
            {
                MessageBox.Show("Silakan pilih data pasien dari daftar terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

             idTerpilih = (int)lvwListPasien.SelectedItems[0].Tag;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                lvwDetailObat.Items.Clear();

                HttpResponseMessage response = await client.GetAsync($"http://localhost:8000/api/transaksis/detail/{idTerpilih}");

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResponse<TransaksiStrukResponse>>(jsonResponse);
                    var tr = result.data;

                    // --- MAPPING DATA KE PANEL ---
                    // Header
                    lblNoPasien2.Text = tr.kode_pasien;
                    lblNoAntrian2.Text = tr.nomor_antrian;
                    lblTanggal.Text = DateTime.Now.ToString("dd-MM-yyyy");

                    // Biodata Pasien
                    lblNama.Text = tr.nama_pasien;
                    lblNIK.Text = tr.nik_pasien;
                    lblNoHP.Text = tr.nomor_hp;
                    lblAlamat.Text = tr.alamat_pasien;
                    if (!string.IsNullOrEmpty(tr.tanggal_lahir))
                        lblTanggalLahir.Text = DateTime.Parse(tr.tanggal_lahir).ToString("dd-MM-yyyy");

                    // Info Pemeriksaan
                    lblPoli1.Text = tr.poli;
                    lblDokter.Text = tr.dokter;
                    lblTindakan.Text = tr.tindakan;
                    lblDiagnosa.Text = tr.diagnosa;
                    lblCatatanDokter.Text = tr.catatan;

                    lblBloodPreasure.Text = tr.fisik.tensi;
                    lblTemperatureBody.Text = tr.fisik.suhu.ToString();
                    lblHeartRate.Text = tr.fisik.nadi.ToString();
                    lblWeight.Text = tr.fisik.berat.ToString();
                    lblHeight.Text = tr.fisik.tinggi.ToString();

                    lblTBT.Text = string.Format("{0:N0}", tr.biaya_tindakan);
                    lblTBO.Text = string.Format("{0:N0}", tr.biaya_obat);
                    lblTotalTransaksi.Text = string.Format("{0:N0}", tr.total_transaksi);

                    //Tabel
                    lvwDetailObat.Items.Clear();
                    if (tr.daftar_obat != null)
                    {
                        foreach (var item in tr.daftar_obat)
                        {
                            ListViewItem lvi = new ListViewItem(item.nama_obat.ToString());
                            lvi.SubItems.Add(string.Format("{0:N0}", item.harga_satuan));
                            lvi.SubItems.Add(item.jumlah.ToString());
                            lvi.SubItems.Add(string.Format("{0:N0}", item.subtotal));
                            lvwDetailObat.Items.Add(lvi);
                        }
                    }

                    pnlDetailResep.Visible = true;
                    pnlDetailResep.BringToFront();
                }
                else
                {
                    MessageBox.Show("Gagal mengambil detail transaksi dari server.");
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

        private void btnBatalStruk_Click(object sender, EventArgs e)
        {
            pnlDetailResep.Visible = false;
        }

        private async void btnCetakStruk_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.Filter = "Text File (*.txt)|*.txt";
                saveFile.FileName = $"Struk_Transaksi_{lblNoPasien2.Text}_{DateTime.Now:yyyyMMdd}.txt";

                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    StringBuilder struk = new StringBuilder();

                    struk.AppendLine("============================================================");
                    struk.AppendLine("                LAPORAN PEMERIKSAAN DAN TRANSAKSI            ");
                    struk.AppendLine("                      PUSKESMAS CITRA KASIH                 ");
                    struk.AppendLine("============================================================");
                    struk.AppendLine($"Tanggal Cetak: {DateTime.Now:dd-MM-yyyy HH:mm}");
                    struk.AppendLine($"ID Pasien    : {lblNoPasien2.Text}");
                    struk.AppendLine($"No. Antrian  : {lblNoAntrian2.Text}");
                    struk.AppendLine("------------------------------------------------------------");

                    struk.AppendLine("DATA PASIEN");
                    struk.AppendLine($"Nama          : {lblNama.Text}");
                    struk.AppendLine($"NIK           : {lblNIK.Text}");
                    struk.AppendLine($"Tanggal Lahir : {lblTanggalLahir.Text}");
                    struk.AppendLine($"Jenis Kelamin : {lblGender.Text}");
                    struk.AppendLine($"No. HP        : {lblNoHP.Text}");
                    struk.AppendLine($"Alamat        : {lblAlamat.Text}");
                    struk.AppendLine("------------------------------------------------------------");

                    struk.AppendLine("HASIL PEMERIKSAAN");
                    struk.AppendLine($"Poli          : {lblPoli1.Text}");
                    struk.AppendLine($"Dokter        : {lblDokter.Text}");
                    struk.AppendLine($"Tindakan      : {lblTindakan.Text}");
                    struk.AppendLine($"Diagnosa      : {lblDiagnosa.Text}");
                    struk.AppendLine($"Catatan       : {lblCatatanDokter.Text}");
                    struk.AppendLine("");
                    struk.AppendLine("FISIK:");
                    struk.AppendLine($"- Tensi  : {lblBloodPreasure.Text}");
                    struk.AppendLine($"- Suhu   : {lblTemperatureBody.Text}");
                    struk.AppendLine($"- Nadi   : {lblHeartRate.Text}");
                    struk.AppendLine($"- Berat  : {lblWeight.Text}");
                    struk.AppendLine($"- Tinggi : {lblHeight.Text}");
                    struk.AppendLine("------------------------------------------------------------");

                    struk.AppendLine(string.Format("{0,-25} {1,-12} {2,-8} {3,-12}", "Nama Obat", "Harga", "Jml", "Subtotal"));
                    struk.AppendLine("------------------------------------------------------------");
                    foreach (ListViewItem item in lvwDetailObat.Items)
                    {
                        struk.AppendLine(string.Format("{0,-25} {1,-12} {2,-8} {3,-12}",
                            item.Text,
                            item.SubItems[1].Text,
                            item.SubItems[2].Text,
                            item.SubItems[3].Text));
                    }
                    struk.AppendLine("------------------------------------------------------------");

                    struk.AppendLine($"Total Biaya Tindakan : Rp {lblTBT.Text}");
                    struk.AppendLine($"Total Biaya Obat     : Rp {lblTBO.Text}");
                    struk.AppendLine($"GRAND TOTAL          : Rp {lblTotalTransaksi.Text}");
                    struk.AppendLine("------------------------------------------------------------");
                    struk.AppendLine("          TERIMA KASIH - SEMOGA LEKAS SEMBUH          ");
                    struk.AppendLine("============================================================");

                    System.IO.File.WriteAllText(saveFile.FileName, struk.ToString());

                    try
                    {
                        var response = await client.PutAsync($"http://localhost:8000/api/transaksis/lunas/{idTerpilih}", null);

                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Struk berhasil disimpan dan status transaksi diperbarui menjadi LUNAS!", "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            await LoadDataTransaksi();

                            pnlDetailResep.Visible = false;
                        }
                        else
                        {
                            MessageBox.Show("Struk tersimpan, namun gagal memperbarui status di server.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error saat update status: " + ex.Message);
                    }

                    System.Diagnostics.Process.Start(saveFile.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mencetak struk: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnCariTransaksi_Click(object sender, EventArgs e)
        {
            await LoadDataTransaksi();

        }

        private async void btnResetTransaksi_Click(object sender, EventArgs e)
        {
            cboPoli.SelectedIndex = 0;
            txtSearch.Clear();

            await LoadDataTransaksi();
        }

        private async Task LoadHistoryTransaksi()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                lsvListLaporan.Items.Clear(); 

                string tgl = dtpTransaksi.Value.ToString("yyyy-MM-dd");
                string url = $"http://localhost:8000/api/transaksis/history?tanggal={tgl}";

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResponse<List<TransaksiDetail>>>(jsonResponse);

                    int no = 1;
                    foreach (var item in result.data)
                    {
                        ListViewItem lvi = new ListViewItem(no.ToString());
                        lvi.SubItems.Add(item.kode_pasien);
                        lvi.SubItems.Add(item.nama_pasien);
                        lvi.SubItems.Add(item.poli);
                        lvi.SubItems.Add(item.biaya_tindakan.ToString("N0"));
                        lvi.SubItems.Add(item.biaya_obat.ToString("N0"));
                        lvi.SubItems.Add(item.total_biaya.ToString("N0"));
                        lvi.SubItems.Add(item.tanggal);

                        lsvListLaporan.Items.Add(lvi);
                        no++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat history: " + ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private async void btnCari2_Click(object sender, EventArgs e)
        {
            await LoadHistoryTransaksi();
        }

        private async void btnReset2_Click(object sender, EventArgs e)
        {
            dtpTransaksi.Value = DateTime.Now;

            await LoadHistoryTransaksi();
        }

        private void btnCetak_Click(object sender, EventArgs e)
        {
            if (lsvListLaporan.Items.Count == 0)
            {
                MessageBox.Show("Tidak ada data transaksi untuk diekspor!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Workbook|*.xlsx";
                sfd.FileName = $"Laporan_Transaksi_{dtpTransaksi.Value:yyyyMMdd}.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        OfficeOpenXml.ExcelPackage.License.SetNonCommercialPersonal("Nama Kamu");
                        using (var package = new OfficeOpenXml.ExcelPackage())
                        {
                            var worksheet = package.Workbook.Worksheets.Add("Laporan Transaksi");
                            
                            worksheet.Cells.Style.Font.Name = "Times New Roman";

                            int totalColumns = lsvListLaporan.Columns.Count;

                            worksheet.Cells[1, 1, 1, totalColumns].Merge = true;
                            worksheet.Cells[1, 1].Value = "PUSKESMAS CITRA KASIH";
                            StyleHeaderBlue(worksheet.Cells[1, 1], 16, true);

                            worksheet.Cells[2, 1, 2, totalColumns].Merge = true;
                            worksheet.Cells[2, 1].Value = "Jl. Kenanga Indah No.16, Desa Sekar Sari, Kecamatan Harapan Jaya, Kabupaten Permai Hulu, Provinsi Jayakarta 35643";
                            StyleHeaderBlue(worksheet.Cells[2, 1], 10, false);

                            worksheet.Cells[3, 1, 3, totalColumns].Merge = true;
                            worksheet.Cells[3, 1].Value = "LAPORAN TRANSAKSI HARIAN";
                            StyleHeaderBlue(worksheet.Cells[3, 1], 12, true);

                            worksheet.Cells[4, 1, 4, totalColumns].Merge = true;
                            StyleHeaderBlue(worksheet.Cells[4, 1], 2, false);

                            worksheet.Cells[5, 1].Value = "Atas Nama : " + UserSession.CurrentUser.nama_user;
                            worksheet.Cells[6, 1].Value = "Tanggal   : " + dtpTransaksi.Value.ToString("dd MMMM yyyy");

                            int startRow = 8;
                            for (int i = 0; i < totalColumns; i++)
                            {
                                var cell = worksheet.Cells[startRow, i + 1];
                                cell.Value = lsvListLaporan.Columns[i].Text; 

                                cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 112, 192));
                                cell.Style.Font.Color.SetColor(System.Drawing.Color.White);
                                cell.Style.Font.Bold = true;
                                cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            }

                            // --- ISI DATA DARI LISTVIEW ---
                            for (int i = 0; i < lsvListLaporan.Items.Count; i++)
                            {
                                int currentRow = startRow + 1 + i;

                                // Kolom 1 (No)
                                worksheet.Cells[currentRow, 1].Value = lsvListLaporan.Items[i].Text;

                                // Kolom SubItems (Kode Pasien, Nama, Poli, Biaya, dll)
                                for (int j = 1; j < totalColumns; j++)
                                {
                                    string value = lsvListLaporan.Items[i].SubItems[j].Text;

                                    // Cek jika kolom biaya, masukkan sebagai angka (numeric) agar bisa di-SUM di excel
                                    if (j >= 4 && j <= 6) // Index kolom biaya_tindakan, biaya_obat, total_biaya
                                    {
                                        if (double.TryParse(value.Replace(".", ""), out double num))
                                            worksheet.Cells[currentRow, j + 1].Value = num;
                                        else
                                            worksheet.Cells[currentRow, j + 1].Value = value;
                                    }
                                    else
                                    {
                                        worksheet.Cells[currentRow, j + 1].Value = value;
                                    }
                                }

                                worksheet.Cells[currentRow, 1, currentRow, totalColumns].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                worksheet.Cells[currentRow, 1, currentRow, totalColumns].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                worksheet.Cells[currentRow, 1, currentRow, totalColumns].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                                worksheet.Cells[currentRow, 1, currentRow, totalColumns].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            }

                            // Format kolom biaya menjadi Accounting/Number agar ada ribuan
                            worksheet.Cells[startRow + 1, 5, startRow + lsvListLaporan.Items.Count, 7].Style.Numberformat.Format = "#,##0";

                            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                            package.SaveAs(new FileInfo(sfd.FileName));
                        }
                        MessageBox.Show("Laporan Transaksi Berhasil Diekspor!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal ekspor: " + ex.Message);
                    }
                }
            }
        }

        private void StyleHeaderBlue(ExcelRange cell, float fontSize, bool isBold)
        {
            cell.Style.Font.Size = fontSize;
            cell.Style.Font.Bold = isBold;
            cell.Style.Font.Color.SetColor(System.Drawing.Color.White);
            cell.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(0, 112, 192));
        }
    }
}
