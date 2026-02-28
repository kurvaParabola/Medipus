<?php

namespace App\Http\Controllers;

use App\Models\Pemeriksaan;
use App\Models\Register;
use App\Models\Dokter;
use App\Models\TindakanMedis;
use Illuminate\Support\Facades\DB;

use Illuminate\Http\Request;

class PemeriksaanController extends Controller
{
    /**
     * Display a listing of the resource.
     */
    public function index()
    {
        $pemeriksaans = Pemeriksaan::with([
            'register.pasien', 
            'dokter', 
            'tindakanMedis'
        ])->get();

        return response()->json([
            'success' => true,
            'data'    => $pemeriksaans
        ], 200);
    }

    /**
     * Show the form for creating a new resource.
     */
    public function create()
    {
        $registers = Register::where('status_register', 'Menunggu')->get();
        $dokters   = Dokter::all();
        $tindakans = TindakanMedis::all();

        return view('pemeriksaans.create', compact('registers', 'dokters', 'tindakans'));
    }

    /**
     * Store a newly created resource in storage.
     */
    public function store(Request $request)
    {
        try {
        return DB::transaction(function () use ($request) {
            // 1. Cari ID Dokter yang valid berdasarkan User ID login
            $dokter = \App\Models\Dokter::where('user_id', $request->dokter_id)->first();
            
            if (!$dokter) {
                return response()->json(['success' => false, 'message' => 'ID Dokter tidak ditemukan untuk user ini'], 404);
            }

            // 2. Simpan Pemeriksaan
            $pemeriksaan = \App\Models\Pemeriksaan::create([
                'register_id'         => $request->register_id,
                'dokter_id'           => $dokter->id, // Menggunakan ID Dokter hasil pencarian
                'tindakan_medis_id'   => $request->tindakan_medis_id,
                'diagnosa_dokter'     => $request->diagnosa_dokter,
                'catatan_dokter'      => $request->catatan_dokter,
                'tanggal_pemeriksaan' => $request->tanggal_pemeriksaan,
                'tekanan_darah'       => $request->tekanan_darah,
                'suhu_badan'          => $request->suhu_badan,
                'denyut_nadi'         => $request->denyut_nadi,
                'berat_badan'         => $request->berat_badan,
                'tinggi_badan'        => $request->tinggi_badan,
            ]);

            // 3. Update Status Register Pasien
            \App\Models\Register::where('id', $request->register_id)->update(['status_register' => 'Selesai']);

            // 4. PEMISAHAN LOGIKA BERDASARKAN CHECKBOX
            if ($request->punya_resep == true) {
                // JALUR A: Pakai Resep (Hanya buat header resep)
                $resep = \App\Models\Resep::create([
                    'pemeriksaan_id' => $pemeriksaan->id,
                    'tanggal_resep'  => now(),
                    'status_resep'   => 'Menunggu'
                ]);

                return response()->json([
                    'success' => true,
                    'message' => 'Pemeriksaan disimpan, lanjut input obat.',
                    'data'    => ['id' => $pemeriksaan->id, 'resep_id' => $resep->id]
                ]);
            } else {
                // JALUR B: Tanpa Resep (Langsung buat Transaksi)
                $resep = \App\Models\Resep::create([
                    'pemeriksaan_id' => $pemeriksaan->id,
                    'tanggal_resep'  => now(),
                    'status_resep'   => 'Selesai' // Status langsung selesai
                ]);

                $tindakan = \App\Models\TindakanMedis::find($request->tindakan_medis_id);
                
                \App\Models\Transaksi::create([
                    'biaya_tindakan'        => $tindakan->biaya_tindakan_medis ?? 0,
                    'biaya_obat'            => 0,
                    'total_biaya_transaksi' => $tindakan->biaya_tindakan_medis ?? 0,
                    'tanggal_transaksi'     => now(),
                    'status_pembayaran'     => 'Menunggu',
                    'pemeriksaan_id'        => $pemeriksaan->id,
                    'resep_id'              => $resep->id,
                    'staff_id'              => 1,
                ]);

                return response()->json([
                    'success' => true,
                    'message' => 'Pemeriksaan selesai, langsung ke kasir.',
                    'data'    => ['id' => $pemeriksaan->id]
                ]);
            }
        });
    } catch (\Exception $e) {
        return response()->json(['success' => false, 'message' => $e->getMessage()], 500);
    }
    }

    /**
     * Display the specified resource.
     */
    public function show(Pemeriksaan $pemeriksaan)
    {
        return view('pemeriksaans.show', compact('pemeriksaan'));
    }

    /**
     * Show the form for editing the specified resource.
     */
    public function edit(Pemeriksaan $pemeriksaan)
    {
        $registers = Register::all();
        $dokters   = Dokter::all();
        $tindakans = TindakanMedis::all();

        return view('pemeriksaans.edit', compact(
            'pemeriksaan',
            'registers',
            'dokters',
            'tindakans'
        ));

    }

    /**
     * Update the specified resource in storage.
     */
    public function update(Request $request, Pemeriksaan $pemeriksaan)
    {
        $validated = $request->validate([
            'diagnosa_dokter'     => 'required|string',
            'catatan_dokter'      => 'nullable|string',
            'tanggal_pemeriksaan' => 'required|date',
            'tekanan_darah'       => 'required|string|max:10',
            'denyut_nadi'         => 'required|integer|min:30|max:200',
            'suhu_badan'          => 'required|integer|min:30|max:45',
            'berat_badan'         => 'required|integer|min:1|max:300',
            'tinggi_badan'        => 'required|integer|min:30|max:250',
            'register_id'         => 'required|exists:registers,id',
            'dokter_id'           => 'required|exists:dokters,id',
            'tindakan_medis_id'   => 'required|exists:tindakan_medis,id',
        ]);

        $pemeriksaan->update($validated);

        return response()->json([
            'success' => true,
            'message' => 'Data pemeriksaan berhasil diperbarui',
            'data'    => $pemeriksaan->fresh()
        ], 200);
    }

    /**
     * Remove the specified resource from storage.
     */
    public function destroy(Pemeriksaan $pemeriksaan)
    {
        $deleted = $pemeriksaan->delete();

        return response()->json([
            'success' => $deleted,
            'message' => $deleted ? 'Data pemeriksaan berhasil dihapus' : 'Gagal menghapus data'
        ], 200);
    }

    public function history(Request $request)
{
    try {
        $user = auth()->user();
        $dokter = \App\Models\Dokter::where('user_id', $user->id)->first();

        if (!$dokter) {
            return response()->json(['success' => false, 'message' => 'Data dokter tidak ditemukan'], 403);
        }

        // 2. Query Pemeriksaan dengan relasi yang diperlukan
        $query = Pemeriksaan::with(['register.pasien', 'register.poli', 'tindakanMedis'])
            // Filter: Hanya ambil register yang poli_id nya sama dengan poli dokter yang login
            ->whereHas('register', function($q) use ($dokter) {
                $q->where('poli_id', $dokter->poli_id);
            });

        // 3. Filter berdasarkan Tanggal (jika ada)
        if ($request->has('tanggal') && $request->tanggal != '') {
            $query->whereDate('created_at', $request->tanggal);
        }

        // 4. Eksekusi query dan mapping data
        $history = $query->orderBy('created_at', 'desc')
            ->get()
            ->map(function ($item) {
                return [
                    'no_antrian'  => $item->register->nomor_antrian ?? '-',
                    'kode_pasien' => $item->register->pasien->kode_pasien ?? '-',
                    'nama_pasien' => $item->register->pasien->nama_pasien ?? '-',
                    'diagnosa'    => $item->diagnosa_dokter,
                    'tindakan'    => $item->tindakanMedis->nama_tindakan_medis ?? 'Tanpa Tindakan',
                    'tanggal'     => $item->created_at->format('Y-m-d')
                ];
            });

        return response()->json([
            'success' => true,
            'data' => $history
        ]);

    } catch (\Exception $e) {
        return response()->json([
            'success' => false,
            'message' => 'Terjadi kesalahan: ' . $e->getMessage()
        ], 500);
    }
}

}
