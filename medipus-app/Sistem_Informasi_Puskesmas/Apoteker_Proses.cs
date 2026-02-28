using Microsoft.Office.Interop.Excel;
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
    public partial class Apoteker_Proses : Form
    {
        public static readonly HttpClient client = new HttpClient();

        private List<DetailResepResponse> _obatTersedia = new List<DetailResepResponse>();

        private ResepRequest _resep;
        public Apoteker_Proses(ResepRequest data)
        {
            InitializeComponent();

            this._resep = data;
        }

        private async void Apoteker_Dashboard_Load(object sender, EventArgs e)
        {
            TampilkanHeaderPasien();
            ListViewDetailObat();
            ListViewDetailPenyerahanObat();

            await LoadDetailResepDokter();

        }

        private void btnKembali_Click_1(object sender, EventArgs e)
        {
            new Apoteker_Dashboard().Show();
            this.Hide();
        }

        private void TampilkanHeaderPasien()
        {
            var reg = _resep.pemeriksaan?.register;
            var pas = reg?.pasien;

            lblNoPasien.Text = pas?.kode_pasien ?? "-";
            lblNoAntrian.Text = reg?.nomor_antrian ?? "-";
            lblNama.Text = pas?.nama_pasien ?? "-";
            lblNIK.Text = pas?.nik_pasien.ToString() ?? "-";
            lblPoli.Text = reg?.poli?.nama_poli ?? "-";
            lblDokter.Text = _resep.pemeriksaan?.dokter?.user?.nama_user ?? "-";
            lblDiagnosa.Text = _resep.pemeriksaan?.diagnosa_dokter ?? "-";
        }

        private void ListViewDetailObat()
        {
            lvwDetailObat.View = View.Details;
            lvwDetailObat.FullRowSelect = true;
            lvwDetailObat.GridLines = true;

            lvwDetailObat.Columns.Clear();
            lvwDetailObat.Columns.Add("Nama Obat", 171);
            lvwDetailObat.Columns.Add("Dosis", 86);
            lvwDetailObat.Columns.Add("Frekuensi", 114);
            lvwDetailObat.Columns.Add("Durasi", 80);
            lvwDetailObat.Columns.Add("Catatan", 194);

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

        private void ListViewDetailPenyerahanObat()
        {
            lvwDetailPenyerahanObat.View = View.Details;
            lvwDetailPenyerahanObat.FullRowSelect = true;
            lvwDetailPenyerahanObat.GridLines = true;

            lvwDetailPenyerahanObat.Columns.Clear();
            lvwDetailPenyerahanObat.Columns.Add("Nama Obat", 171);
            lvwDetailPenyerahanObat.Columns.Add("Harga Obat", 103);
            lvwDetailPenyerahanObat.Columns.Add("Jumlah", 86);
            lvwDetailPenyerahanObat.Columns.Add("SubTotal", 114);
            lvwDetailPenyerahanObat.OwnerDraw = true;

            lvwDetailPenyerahanObat.DrawColumnHeader += (s, e) =>
            {
                using (SolidBrush brush = new SolidBrush(Color.DarkCyan))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }

                TextRenderer.DrawText(e.Graphics, e.Header.Text, e.Font,
                    e.Bounds, Color.White, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            };

            lvwDetailPenyerahanObat.DrawItem += (s, e) => e.DrawDefault = true;
            lvwDetailPenyerahanObat.DrawSubItem += (s, e) => e.DrawDefault = true;

        }

        private async Task LoadDetailResepDokter()
        {
            try
            {
                var response = await client.GetAsync($"http://localhost:8000/api/detail_resep/{_resep.id}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<DetailResepResponse>>>();
                    _obatTersedia = result.data;
                    RefreshComboBoxObat();

                    lvwDetailObat.Items.Clear();

                    foreach (var item in result.data)
                    {
                        ListViewItem row = new ListViewItem(item.obat?.nama_obat);
                        row.SubItems.Add(item.dosis_obat);
                        row.SubItems.Add(item.frekuensi_obat);
                        row.SubItems.Add(item.durasi_obat);
                        row.SubItems.Add(item.catatan_obat);

                        lvwDetailObat.Items.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat detail obat: " + ex.Message);
            }
        }

        private void HitungTotalAkhir()
        {
            long total = 0;
            foreach (ListViewItem item in lvwDetailPenyerahanObat.Items)
            {
                if (item.SubItems.Count > 3)
                {
                    string subtotalClean = item.SubItems[3].Text.Replace(".", "").Replace(",", "");
                    total += long.Parse(subtotalClean);
                }
            }
            lblTotal.Text = total.ToString("N0"); 
        }

        private void RefreshComboBoxObat()
        {
            cboPilihObat.Items.Clear();
            foreach (var item in _obatTersedia)
            {
                cboPilihObat.Items.Add(item.obat?.nama_obat ?? "Tanpa Nama");
            }
        }

        private async void btnKirimResep_Click(object sender, EventArgs e)
        {
            if (lvwDetailPenyerahanObat.Items.Count == 0)
            {
                MessageBox.Show("Daftar penyerahan masih kosong!", "Peringatan");
                return;
            }

            this.Cursor = Cursors.WaitCursor;

            try
            {
                var listObatDiproses = new List<object>();

                foreach (ListViewItem item in lvwDetailPenyerahanObat.Items)
                {
                    string qtyStr = item.SubItems[2].Text.Trim();
                    string subtotalStr = item.SubItems[3].Text.Replace(".", "").Replace(",", "").Trim();

                    listObatDiproses.Add(new
                    {
                        obat_id = Convert.ToInt32(item.Tag),
                        jumlah = int.TryParse(qtyStr, out int q) ? q : 0,
                        subtotal = long.TryParse(subtotalStr, out long s) ? s : 0
                    });
                }

                var updatePayload = new
                {
                    resep_id = _resep.id,
                    pemeriksaan_id = _resep.pemeriksaan.id,
                    items = listObatDiproses
                };

                string json = JsonConvert.SerializeObject(updatePayload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"http://localhost:8000/api/detail_resep/update_after_process", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Data detail resep berhasil diperbarui di database!", "Sukses");

                    new Apoteker_Dashboard().Show();
                    this.Close();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Gagal update data: " + error, "Error API");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan teknis: " + ex.Message);
            }
        }

        private void btnResetObat_Click(object sender, EventArgs e)
        {
            txtJumlahObat.Clear();
            cboPilihObat.SelectedIndex = -1;

            cboPilihObat.Focus();
        }

        private void btnSimpanObat_Click(object sender, EventArgs e)
        {
            if (cboPilihObat.SelectedIndex == -1)
            {
                MessageBox.Show("Pilih obat terlebih dahulu!");
                return;
            }

            if (!int.TryParse(txtJumlahObat.Text, out int jumlahInput) || jumlahInput <= 0)
            {
                MessageBox.Show("Masukkan jumlah obat yang valid (angka lebih dari 0)!", "Input Tidak Valid");
                return;
            }

            var obatDipilih = _obatTersedia[cboPilihObat.SelectedIndex];
            long harga = obatDipilih.obat?.harga_satuan ?? 0;
            long subtotal = harga * jumlahInput;

            ListViewItem item = new ListViewItem(obatDipilih.obat?.nama_obat ?? "-"); 
            item.SubItems.Add(harga.ToString("N0")); 
            item.SubItems.Add(jumlahInput.ToString());    
            item.SubItems.Add(subtotal.ToString("N0"));

            item.Tag = obatDipilih.obat_id;

            ListViewItem itm = new ListViewItem(obatDipilih.obat?.nama_obat);
            itm.SubItems.Add(txtJumlahObat.Text);
            lvwDetailPenyerahanObat.Items.Add(item);
            _obatTersedia.RemoveAt(cboPilihObat.SelectedIndex);

            RefreshComboBoxObat();

            cboPilihObat.SelectedIndex = -1;
            txtJumlahObat.Clear();

            HitungTotalAkhir();
        }
    }
}
