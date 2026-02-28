# 🏥 MEDIPUS - Sistem Informasi Kesehatan Puskesmas

**MEDIPUS** adalah proyek tugas akhir untuk mata kuliah Pemrograman Lanjut. Aplikasi ini mengintegrasikan backend berbasis web (API) dengan aplikasi desktop untuk sistem informasi kesehatan di Puskesmas.

---

## 🚀 Fitur Utama
* **Manajemen Pasien:** Pendaftaran dan riwayat rekam medis.
* **Manajemen Dokter & Staff:** Pengaturan jadwal dan data pegawai.
* **Integrasi API:** Sinkronisasi data real-time antara Desktop dan Database Cloud/Local.
* **Laporan Kesehatan:** Visualisasi data kunjungan pasien.

---

## 🛠️ Teknologi yang Digunakan
| **Frontend Desktop** | C# Windows Forms (.NET Framework) |
| **Backend API** | Laravel (PHP) |
| **Database** | MySQL |
| **Library C#** | Newtonsoft.Json, HttpClient |

---

## 📖 Cara Instalasi & Menjalankan

### 1. Backend (Laravel API)
Masuk ke folder `medipus-api`:
1. Jalankan `composer install`.
2. Duplikat file `.env.example` menjadi `.env` dan sesuaikan konfigurasi database Anda.
3. Jalankan `php artisan key:generate`.
4. Jalankan migrasi database: `php artisan migrate`.
5. Jalankan server: `php artisan serve`.
   > API akan berjalan di `http://127.0.0.1:8000`

### 2. Frontend (WinForms)
Masuk ke folder `medipus-app`:
1. Buka file solusi `.sln` menggunakan **Visual Studio 2022** atau versi terbaru.
2. Pastikan koneksi URL API di dalam kode C# sudah mengarah ke alamat Laravel (default: `http://127.0.0.1:8000/api`).
3. Tekan **F5** atau klik **Start** untuk menjalankan aplikasi.

---

## 👨‍💻 Kontributor
* **Wahyu Damar Wiguna** - [kurvaParabola](https://github.com/kurvaParabola)
* Proyek ini dikembangkan untuk Tugas Akhir Pemrograman Lanjut 2025-2026.
