//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Net.Http.Json;

//namespace Sistem_Informasi_Puskesmas
//{
//    public partial class test : Form
//    {
//        private static readonly HttpClient client = new HttpClient();
//        public test()
//        {
//            InitializeComponent();
//        }

//        private async void button1_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                string url = "http://localhost:8000/api/obats";

//                // Ambil data menggunakan model ApiResponse
//                ApiResponse response = await client.GetFromJsonAsync<ApiResponse>(url);

//                if (response != null && response.data != null)
//                {
//                    StringBuilder sb = new StringBuilder();
//                    sb.AppendLine($"Status: {response.message}");
//                    sb.AppendLine("============================");

//                    // Melakukan looping karena 'data' berbentuk List/Array
//                    foreach (var item in response.data)
//                    {
//                        sb.AppendLine($"ID      : {item.id}");
//                        sb.AppendLine($"Kode    : {item.kode_obat}");
//                        sb.AppendLine($"Nama    : {item.nama_obat}");
//                        sb.AppendLine($"Stok    : {item.stok}");
//                        sb.AppendLine($"Harga   : Rp{item.harga_satuan}");
//                        sb.AppendLine($"Lokasi  : {item.lokasi_penyimpanan}");
//                        sb.AppendLine("----------------------------");
//                    }

//                    textBox1.Text = sb.ToString();
//                }
//                else
//                {
//                    textBox1.Text = "No response received or failed to deserialize";
//                }
//            }
//            catch (HttpRequestException httpEx)
//            {
//                textBox1.Text = $"Request error : {httpEx.Message}";
//            }
//            catch (Exception ex)
//            {
//                textBox1.Text = $"General error : {ex.Message}";
//            }
//        }

//        private void button2_Click(object sender, EventArgs e)
//        {

//        }

//        private void Test_Load(object sender, EventArgs e)
//        {
//            client.DefaultRequestHeaders.Clear();
//            client.DefaultRequestHeaders.Add("Accept-Language", "en-US");
//        }

//    }
//}
