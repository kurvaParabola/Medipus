using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistem_Informasi_Puskesmas
{
    public class Pasien
    {
        public string Nik { get; set; }
        public string Nama { get; set; }
        public DateTime TglLahir { get; set; }
        public string JenisKelamin { get; set; }
        public string NoHp { get; set; }
        public string Alamat { get; set; }
        public string Poli { get; set; }
        public string Keluhan { get; set; }
    }
}
