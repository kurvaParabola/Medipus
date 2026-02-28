using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistem_Informasi_Puskesmas
{
    public class ApiResponse<T>
    {
        public bool success { get; set; }
        public string message { get; set; }
        public T data { get; set; } 
    }

    public class LoginResult
    {
        public UserData user { get; set; }
        public string token { get; set; }
    }

    public static class UserSession
    {
        public static string Token { get; set; }
        public static UserData CurrentUser { get; set; }
        public static int? NomorIndukStaff { get; set; }
    }

    public class UserData
    {
        public int id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public string status_user { get; set; }

        public long nik_user { get; set; }
        public string nama_user { get; set; }
        public DateTime tanggal_lahir_user { get; set; }
        public string jenis_kelamin_user { get; set; }
        public string alamat_user { get; set; }
        public int nomor_hp_user { get; set; }
    }

    public class PoliData
    {
        public int id { get; set; }
        public string nama_poli { get; set; }
    }

    public class DataStaff
    {
        public int id { get; set; }
        public int nomor_induk_staff { get; set; }
        public int user_id { get; set; }
        public UserData user { get; set; }
    }

    public class DataDokter
    {
        public int id { get; set; }
        public int nomor_induk_dokter { get; set; }
        public int user_id { get; set; }
        public int poli_id { get; set; }
        public UserData user { get; set; }
        public PoliData poli { get; set; }
    }

    public class DataApoteker
    {
        public int id { get; set; }
        public int nomor_induk_apoteker { get; set; }
        public int user_id { get; set; }
        public UserData user { get; set; }
    }

    public class DataRuangan
    {
        public int id { get; set; }
        public string kode_ruangan { get; set; }
        public string nama_ruangan { get; set; }
    }

    public class JadwalResponse
    {
        public int id { get; set; }
        public string hari {  get; set; }
        public string jam_mulai { get; set; } 
        public string jam_selesai { get; set; }
        public DataRuangan ruangan {  get; set; }
        public DataDokter dokter { get; set; }
    }

    public class DataPasienRegister
    {
        public int id { get; set; }
        public string kode_pasien {  get; set; }
        public int nik_pasien { get; set; }
        public string nama_pasien { get; set; }
        public DateTime tanggal_lahir_pasien { get; set; }
        public string jenis_kelamin_pasien { get; set; }
        public int nomor_hp_pasien { get; set; }
        public string alamat_pasien { get; set; }

        public int poli_id { get; set; }
        public string keluhan_pasien { get; set; }
        public DateTime tanggal_register {  get; set; }
    }

    public class DataPasienResponse
    {
        public int id { get; set; }
        public string nomor_antrian { get; set; }
        public string tanggal_register { get; set; }
        public string jadwal { get; set; }
        public string status_register { get; set; }

        public string keluhan_pasien { get; set; }

        public DataPasienRegister pasien { get; set; }
        public PoliData poli { get; set; }
        public JadwalResponse jadwal_dokter { get; set; }
    }

    public class TindakanMedis
    {
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("nama_tindakan")]
        public string nama_tindakan { get; set; }
        public long harga { get; set; }
    }

    public class TindakanResponse
    {
        public bool success { get; set; }
        public List<TindakanMedis> data { get; set; }
    }

    public class PemeriksaanRequest
    {
        public int register_id { get; set; }
        public int dokter_id { get; set; }
        public int tindakan_medis_id { get; set; }
        public string diagnosa_dokter { get; set; }
        public string catatan_dokter { get; set; }
        public string tanggal_pemeriksaan { get; set; }
        public string tekanan_darah { get; set; }
        public int denyut_nadi { get; set; }
        public int suhu_badan { get; set; }
        public int berat_badan { get; set; }
        public int tinggi_badan { get; set; }

        public List<ResepItem> resep_obat { get; set; }
    }

    public class PemeriksaanData
    {
        public int id { get; set; } 
        public string diagnosa_dokter { get; set; }
        public string catatan_dokter { get; set; }
        public int register_id { get; set; }
        public int dokter_id { get; set; }
        public int tindakan_medis_id { get; set; }
    }
    public class ResepItem
    {
        public int obat_id { get; set; }
        public int jumlah { get; set; }
        public string aturan_pakai { get; set; }
    }

    public class PemeriksaanResultResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public object data { get; set; } 
    }

    public class HistoryData
    {
        public string no_antrian { get; set; }
        public string kode_pasien { get; set; }
        public string nama_pasien { get; set; }
        public string diagnosa { get; set; }
        public string tindakan { get; set; }
        public string tanggal { get; set; }
    }

    public class HistoryResponse
    {
        public bool success { get; set; }
        public List<HistoryData> data { get; set; }
    }

    public class ResepRequest
    {
        public int id { get; set; }
        public string status_resep { get; set; } = "Menunggu";
        public string tanggal_resep { get; set; }
        public int pemeriksaan_id { get; set; }
        public PemeriksaanDetail pemeriksaan {  get; set; }
    }

    public class ResepData
    {
        public int id { get; set; }
        public string status_resep { get; set; }
        public string updated_at { get; set; } 
        public string created_at { get; set; }
        public PemeriksaanDetail pemeriksaan { get; set; }
    }

    public class PemeriksaanDetail
    {
        public int id { get; set; }
        public string diagnosa_dokter { get; set; } 
        public string catatan_dokter { get; set; }
        public DataPasienResponse register { get; set; }
        public DataDokter dokter { get; set; }
    }

    public class DetailResepItem
    {
        public int obat_id { get; set; }
        public string dosis_obat { get; set; }
        public string frekuensi_obat { get; set; }
        public string durasi_obat { get; set; }
        public int jumlah_obat { get; set; }
        public string catatan_obat { get; set; }
    }

    public class DetailResepRequest
    {
        public int resep_id { get; set; }
        public List<DetailResepItem> items { get; set; }
    }

    public class ObatData
    {
        public int id { get; set; }
        public string kode_obat { get; set; }
        public string nama_obat { get; set; }
        public long harga_satuan { get; set; }
        public int stok { get; set; }
        public string kategori { get; set; } 
        public string created_at { get; set; }
        public string satuan_obat { get; set; }
        public string lokasi_penyimpanan { get; set; }
        public string kadaluarsa { get; set; }
    }

    public class ResepResult
    {
        public int id { get; set; }
    }

    public class DetailResepResponse
    {
        public int id { get; set; }
        public int resep_id { get; set; }
        public int obat_id { get; set; }
        public string dosis_obat { get; set; }
        public string frekuensi_obat { get; set; }
        public string durasi_obat { get; set; }
        public string catatan_obat { get; set; }
        public ObatData obat { get; set; } 
    }

    public class TransaksiRequest
    {
        public int pemeriksaan_id { get; set; }
        public int resep_id { get; set; }
        public int staff_id { get; set; }
        public string status_pembayaran { get; set; }
        public string tanggal_transaksi { get; set; }
    }

    public class TransaksiDetail
    {
        public int id { get; set; }
        public string no_antrian { get; set; }
        public string kode_pasien { get; set; }
        public string nama_pasien { get; set; }
        public string poli { get; set; }
        public long biaya_tindakan { get; set; }
        public long biaya_obat { get; set; }
        public long total_biaya{ get; set; }
        public string tanggal { get; set; }
        public string status { get; set; }
    }

    public class TransaksiStrukResponse
    {
        public int id { get; set; }
        public string kode_pasien { get; set; }
        public string nomor_antrian { get; set; }
        public string nama_pasien { get; set; }
        public string nik_pasien { get; set; }
        public string alamat_pasien { get; set; }
        public string tanggal_lahir { get; set; } 
        public string nomor_hp { get; set; }

        public string poli { get; set; }
        public string dokter { get; set; }
        public string diagnosa { get; set; }
        public string catatan { get; set; }
        public string tindakan { get; set; }

        public long biaya_tindakan { get; set; }
        public long biaya_obat { get; set; }
        public long total_transaksi { get; set; }

        public FisikData fisik { get; set; }
        public List<ObatItem> daftar_obat { get; set; } 
    }

    public class ObatItem
    {
        public string nama_obat { get; set; }
        public long harga_satuan { get; set; }
        public int jumlah { get; set; }
        public long subtotal { get; set; }
    }

    public class FisikData
    {
        public string tensi { get; set; }
        public int suhu { get; set; }
        public int nadi { get; set; }
        public int berat { get; set; }
        public int tinggi { get; set; }
    }

    public class CRUDDataObat
    {
        public string nama_obat { get; set; }
        public int stok { get; set; }
        public string satuan_obat { get; set; }
        public string kadaluarsa { get; set; }
        public string kategori { get; set; }
        public int harga_satuan { get; set; }
        public string lokasi_penyimpanan { get; set; }
    }
}
