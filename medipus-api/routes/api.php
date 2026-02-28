<?php

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;
use App\Http\Controllers\Api\AuthController;

use App\Http\Controllers\UserController;
use App\Http\Controllers\ObatController;
use App\Http\Controllers\TindakanMedisController;
use App\Http\Controllers\PoliController;
use App\Http\Controllers\RuanganController;
use App\Http\Controllers\PasienController;

use App\Http\Controllers\BiodataUserController;

use App\Http\Controllers\DokterController;
use App\Http\Controllers\ApotekerController;
use App\Http\Controllers\StaffController;

use App\Http\Controllers\PoliTindakanController;

use App\Http\Controllers\JadwalDokterController;
use App\Http\Controllers\RegisterController;
use App\Http\Controllers\PemeriksaanController;
use App\Http\Controllers\ResepController;
use App\Http\Controllers\DetailResepController;
use App\Http\Controllers\TransaksiController;


Route::get('/user', function (Request $request) {
    return $request->user();
})->middleware('auth:sanctum');

Route::get('/users', [AuthController::class, 'index']);
Route::post('/register', [AuthController::class, 'register']);
Route::post('/login', [AuthController::class, 'login']);

//Tindakan Medis
Route::get('/tindakan_medis', [TindakanMedisController::class, 'index']);
Route::post('/tindakan_medis', [TindakanMedisController::class, 'store']);
Route::put('/tindakan_medis/{tindakanMedis}', [TindakanMedisController::class, 'update']);
Route::delete('/tindakan_medis/{tindakanMedis}', [TindakanMedisController::class, 'destroy']);

Route::get('/tindakan-by-poli/{poli_id}', [TindakanMedisController::class, 'getTindakanByPoli']);

//Poli
Route::get('/polis', [PoliController::class, 'index']);
Route::post('/polis', [PoliController::class, 'store']);
Route::put('/polis/{poli}', [PoliController::class, 'update']);
Route::delete('/polis/{poli}', [PoliController::class, 'destroy']);

//ruangan
Route::get('/ruangans', [RuanganController::class, 'index']);
Route::post('/ruangans', [RuanganController::class, 'store']);
Route::put('/ruangans/{ruangan}', [RuanganController::class, 'update']);
Route::delete('/ruangans/{ruangan}', [RuanganController::class, 'destroy']);

//Obat
Route::post('/obats', [ObatController::class, 'store']);
Route::get('/obats', [ObatController::class, 'index']);
Route::put('/obats/{obat}', [ObatController::class, 'update']);
Route::delete('/obats/{obat}', [ObatController::class, 'destroy']);

Route::get('/obat/{id}', [ObatController::class, 'show']);

//Pasien
Route::get('/pasiens', [PasienController::class, 'index']);
Route::post('/pasiens', [PasienController::class, 'store']);
Route::put('/pasiens/{pasien}', [PasienController::class, 'update']);
Route::delete('/pasiens/{pasien}', [PasienController::class, 'destroy']);

//Dokter
Route::get('/dokters', [DokterController::class, 'index']);
Route::post('/dokters', [DokterController::class, 'store']);
Route::put('/dokters/{dokter}', [DokterController::class, 'update']);
Route::delete('/dokters/{dokter}', [DokterController::class, 'destroy']);

//Apoteker
Route::get('/apotekers', [ApotekerController::class, 'index']);
Route::post('/apotekers', [ApotekerController::class, 'store']);
Route::put('/apotekers/{apoteker}', [ApotekerController::class, 'update']);
Route::delete('/apotekers/{apoteker}', [ApotekerController::class, 'destroy']);

//Staff
Route::get('/staff', [StaffController::class, 'index']);
Route::post('/staff', [StaffController::class, 'store']);
Route::put('/staff/{staff}', [StaffController::class, 'update']);
Route::delete('/staff/{staff}', [StaffController::class, 'destroy']);

//Poli Tindakan
Route::get('/poli_tindakan', [PoliTindakanController::class, 'index']);
Route::post('/poli_tindakan', [PoliTindakanController::class, 'store']);
Route::put('/poli_tindakan/{poliTindakan}', [PoliTindakanController::class, 'update']);
Route::delete('/poli_tindakan/{poliTindakan}', [PoliTindakanController::class, 'destroy']);

//Jadwal Dokter
Route::get('/jadwal_dokter', [JadwalDokterController::class, 'index']);
Route::post('/jadwal_dokter', [JadwalDokterController::class, 'store']);
Route::put('/jadwal_dokter/{jadwalDokter}', [JadwalDokterController::class, 'update']);
Route::delete('/jadwal_dokter/{jadwalDokter}', [JadwalDokterController::class, 'destroy']);

//Register
Route::get('/registers', [RegisterController::class, 'index']);
Route::post('/registers', [RegisterController::class, 'store']);
Route::put('/registers/{register}', [RegisterController::class, 'update']);
Route::delete('/registers/{register}', [RegisterController::class, 'destroy']);

Route::get('/registers/semua', [RegisterController::class, 'indexSemuaPoli']);
Route::get('/registers/laporan', [RegisterController::class, 'laporanKunjungan']);

Route::middleware('auth:sanctum')->group(function () {
    Route::get('/registers/staff', [RegisterController::class, 'indexStaff']);
    Route::get('/registers/dokter', [RegisterController::class, 'indexDokter']);
    Route::get('/pemeriksaan/history', [PemeriksaanController::class, 'history']);
});

//Pemeriksaan
Route::get('/pemeriksaans', [PemeriksaanController::class, 'index']);
Route::post('/pemeriksaans', [PemeriksaanController::class, 'store']);
Route::put('/pemeriksaans/{pemeriksaan}', [PemeriksaanController::class, 'update']);
Route::delete('/pemeriksaans/{pemeriksaan}', [PemeriksaanController::class, 'destroy']);


//Resep
Route::get('/reseps', [ResepController::class, 'index']);
Route::post('/reseps', [ResepController::class, 'store']);
Route::put('/reseps/{resep}', [ResepController::class, 'update']);
Route::delete('/reseps/{resep}', [ResepController::class, 'destroy']);

Route::get('/reseps/selesai', [ResepController::class, 'indexSelesai']);

//Detail Resep
Route::get('/detail_resep', [DetailResepController::class, 'index']);
Route::post('/detail_resep', [DetailResepController::class, 'store']);
Route::put('/detail_resep/{resepId}', [DetailResepController::class, 'update']);
Route::delete('/detail_resep/{detailResep}', [DetailResepController::class, 'destroy']);

Route::get('/detail_resep/{resep_id}', [DetailResepController::class, 'showByResep']);
Route::post('/detail_resep/update_after_process', [DetailResepController::class, 'updateAfterProcess']);

//Transaksi
Route::get('/transaksis', [TransaksiController::class, 'index']);
Route::get('/transaksis/history', [TransaksiController::class, 'history']);

Route::post('/transaksis', [TransaksiController::class, 'store']);
Route::put('/transaksis/{transaksi}', [TransaksiController::class, 'update']);
Route::delete('/transaksis/{transaksi}', [TransaksiController::class, 'destroy']);

Route::get('/transaksis/detail/{id}', [TransaksiController::class, 'showDetail']);
Route::put('/transaksis/lunas/{id}', [TransaksiController::class, 'updateStatusLunas']);

// Protected routes (requires a valid Sanctum token)
Route::middleware('auth:sanctum')->group(function () {
    Route::post('logout', [AuthController::class, 'logout']);
    // You can add more protected routes here, e.g., to get user profile
    Route::get('user', function (Request $request) {
        return $request->user();
    });
});