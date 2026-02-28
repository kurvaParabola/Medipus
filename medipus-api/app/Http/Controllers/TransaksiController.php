<?php

namespace App\Http\Controllers;

use App\Models\Transaksi;
use App\Models\Pemeriksaan;
use App\Models\Resep;
use App\Models\Staff;
use App\Models\DetailResep;
use Illuminate\Http\Request;

class TransaksiController extends Controller
{
    /**
     * Display a listing of the resource.
     */

        public function index(Request $request)
        {
            try {
                $query = Transaksi::with(['pemeriksaan.register.pasien', 'pemeriksaan.register.poli'])
                    ->where('status_pembayaran', 'Menunggu');

                // Filter berdasarkan Nama atau Kode Pasien
                if ($request->has('search') && $request->search != '') {
                    $search = $request->search;
                    $query->whereHas('pemeriksaan.register.pasien', function($q) use ($search) {
                        $q->where('nama_pasien', 'like', "%{$search}%")
                        ->orWhere('kode_pasien', 'like', "%{$search}%");
                    });
                }

                // Filter berdasarkan Poli
                if ($request->has('poli') && $request->poli != '') {
                    $poli = $request->poli;
                    $query->whereHas('pemeriksaan.register.poli', function($q) use ($poli) {
                        $q->where('nama_poli', $poli);
                    });
                }

                $transaksis = $query->get()->map(function ($tr) {
                    return [
                        'id'              => $tr->id,
                        'no_antrian'      => $tr->pemeriksaan->register->nomor_antrian ?? '-',
                        'kode_pasien'     => $tr->pemeriksaan->register->pasien->kode_pasien ?? '-',
                        'nama_pasien'     => $tr->pemeriksaan->register->pasien->nama_pasien ?? '-',
                        'poli'            => $tr->pemeriksaan->register->poli->nama_poli ?? '-',
                        'biaya_tindakan'  => $tr->biaya_tindakan,
                        'biaya_obat'      => $tr->biaya_obat,
                        'total_biaya'     => $tr->total_biaya_transaksi,
                        'tanggal'         => $tr->created_at->format('Y-m-d'),
                        'status'          => $tr->status_pembayaran
                    ];
                });

                return response()->json([
                    'success' => true,
                    'data' => $transaksis
                ]);
            } catch (\Exception $e) {
                return response()->json(['success' => false, 'message' => $e->getMessage()], 500);
            }
        }

        public function history(Request $request)
        {
            try {
                $query = Transaksi::with(['pemeriksaan.register.pasien', 'pemeriksaan.register.poli'])
                    ->where('status_pembayaran', 'Lunas');

                if ($request->has('tanggal') && $request->tanggal != '') {
                    $query->whereDate('updated_at', $request->tanggal);
                }



                $transaksis = $query->get()->map(function ($tr) {
                    return [
                        'id'              => $tr->id,
                        'kode_pasien'     => $tr->pemeriksaan->register->pasien->kode_pasien ?? '-',
                        'nama_pasien'     => $tr->pemeriksaan->register->pasien->nama_pasien ?? '-',
                        'poli'            => $tr->pemeriksaan->register->poli->nama_poli ?? '-',
                        'biaya_tindakan'  => $tr->biaya_tindakan,
                        'biaya_obat'      => $tr->biaya_obat,
                        'total_biaya'     => $tr->total_biaya_transaksi,
                        'tanggal'         => $tr->updated_at->format('d-m-Y'),
                    ];
                });

                return response()->json([
                    'success' => true,
                    'data' => $transaksis
                ]);
            } catch (\Exception $e) {
                return response()->json(['success' => false, 'message' => $e->getMessage()], 500);
            }
        }

    /**
     * Show the form for creating a new resource.
     */
    public function create()
    {
        $pemeriksaans = Pemeriksaan::all();
        $reseps       = Resep::where('status_resep', 'Selesai')->get();
        $staff       = Staff::all();

        return view('transaksis.create', compact('pemeriksaans', 'reseps', 'staff'));
    }

    /**
     * Store a newly created resource in storage.
     */
    public function store(Request $request)
    {
        $validated = $request->validate([
            'pemeriksaan_id'    => 'required|exists:pemeriksaans,id',
            'resep_id'          => 'required|exists:reseps,id',
            'staff_id'          => 'required|exists:staff,id',
            'status_pembayaran' => 'required|in:Menunggu,Lunas,Batal',
            'tanggal_transaksi' => 'required|date',
        ]);

        // Hitung Biaya Tindakan dari relasi Pemeriksaan -> TindakanMedis
        $pemeriksaan = Pemeriksaan::with('tindakanMedis')->findOrFail($request->pemeriksaan_id);
        $biayaTindakan = $pemeriksaan->tindakanMedis->biaya_tindakan_medis ?? 0;

        // Hitung Biaya Obat dari sum subtotal di DetailResep
        $biayaObat = DetailResep::where('resep_id', $request->resep_id)->sum('subtotal_obat');

        $total = $biayaTindakan + $biayaObat;

        // Gabungkan hasil perhitungan ke data yang divalidasi
        $dataTransaksi = array_merge($validated, [
            'biaya_tindakan'         => $biayaTindakan,
            'biaya_obat'             => $biayaObat,
            'total_biaya_transaksi'  => $total,
        ]);

        $transaksi = Transaksi::create($dataTransaksi);

        return response()->json([
            'success' => true,
            'message' => 'Transaksi berhasil diproses',
            'data'    => $transaksi
        ], 201);

    }

    /**
     * Display the specified resource.
     */
    public function show(Transaksi $transaksi)
    {
        $transaksi->load([
            'pemeriksaan.register.pasien',
            'pemeriksaan.dokter',
            'resep.detailReseps',
            'staff'
        ]);

        return view('transaksis.show', compact('transaksi'));
    }

    /**
     * Show the form for editing the specified resource.
     */
    public function edit(Transaksi $transaksi)
    {
        return view('transaksis.edit', compact('transaksi'));
    }

    /**
     * Update the specified resource in storage.
     */
    public function update(Request $request, Transaksi $transaksi)
    {
        $validated = $request->validate([
            'status_pembayaran' => 'required|in:Menunggu,Lunas,Batal',
            'staff_id'          => 'required|exists:staff,id',
        ]);

        $transaksi->update($validated);

        return response()->json([
            'success' => true,
            'message' => 'Status pembayaran berhasil diperbarui',
            'data'    => $transaksi->fresh()
        ], 200);
    }

    /**
     * Remove the specified resource from storage.
     */
    public function destroy(Transaksi $transaksi)
    {
        $deleted = $transaksi->delete();
        
        return response()->json([
            'success' => $deleted,
            'message' => $deleted ? 'Transaksi berhasil dihapus' : 'Gagal menghapus transaksi'
        ], 200);

    }

    public function showDetail($id)
    {
        try {
            // Mengambil data transaksi dengan relasi yang sangat lengkap
            $transaksi = Transaksi::with([
                'pemeriksaan.register.pasien',
                'pemeriksaan.register.poli',
                'pemeriksaan.tindakanMedis',
                'pemeriksaan.dokter.user',
                'resep.detail_reseps.obat' 
            ])->find($id);

            if (!$transaksi) {
                return response()->json([
                    'success' => false,
                    'message' => 'Data transaksi tidak ditemukan.'
                ], 404);
            }

            // Memetakan data agar lebih mudah dibaca oleh C#
            return response()->json([
                'success' => true,
                'data' => [
                    'id'                => $transaksi->id,
                    'kode_pasien'       => $transaksi->pemeriksaan->register->pasien->kode_pasien,
                    'nomor_antrian'     => $transaksi->pemeriksaan->register->nomor_antrian,
                    'nama_pasien'       => $transaksi->pemeriksaan->register->pasien->nama_pasien,
                    'nik_pasien'        => $transaksi->pemeriksaan->register->pasien->nik_pasien,
                    'alamat_pasien'     => $transaksi->pemeriksaan->register->pasien->alamat_pasien,

                    'tanggal_lahir'     => $transaksi->pemeriksaan->register->pasien->tanggal_lahir_pasien,
                    'nomor_hp'          => $transaksi->pemeriksaan->register->pasien->nomor_hp_pasien,
                    'tindakan'          => $transaksi->pemeriksaan->tindakanMedis->nama_tindakan_medis ?? '-',

                    'poli'              => $transaksi->pemeriksaan->register->poli->nama_poli,
                    'dokter'            => $transaksi->pemeriksaan->dokter->user->nama_user,
                    'diagnosa'          => $transaksi->pemeriksaan->diagnosa_dokter,
                    'catatan'           => $transaksi->pemeriksaan->catatan_dokter,
                    'biaya_tindakan'    => $transaksi->biaya_tindakan,
                    'biaya_obat'        => $transaksi->biaya_obat,
                    'total_transaksi'   => $transaksi->total_biaya_transaksi,
                    // Rincian fisik dasar
                    'fisik' => [
                        'tensi' => $transaksi->pemeriksaan->tekanan_darah,
                        'suhu'  => $transaksi->pemeriksaan->suhu_badan,
                        'nadi'  => $transaksi->pemeriksaan->denyut_nadi,
                        'berat' => $transaksi->pemeriksaan->berat_badan,
                        'tinggi'=> $transaksi->pemeriksaan->tinggi_badan,
                    ],
                    // Rincian daftar obat
                    'daftar_obat' => $transaksi->resep->detail_reseps->map(function ($det) {
                        return [
                            'nama_obat'    => $det->obat->nama_obat,
                            'harga_satuan' => $det->obat->harga_satuan,
                            'jumlah'       => $det->jumlah_obat,
                            'subtotal'     => $det->obat->harga_satuan * $det->jumlah_obat
                        ];
                    })
                ]
            ]);
        } catch (\Exception $e) {
            return response()->json([
                'success' => false,
                'message' => 'Terjadi kesalahan: ' . $e->getMessage()
            ], 500);
        }
    }
    
    public function updateStatusLunas($id)
    {
        try {
            $transaksi = Transaksi::find($id);
            
            if (!$transaksi) {
                return response()->json(['success' => false, 'message' => 'Transaksi tidak ditemukan'], 404);
            }

            $transaksi->status_pembayaran = 'Lunas'; 
            $transaksi->save();

            return response()->json([
                'success' => true,
                'message' => 'Status berhasil diperbarui menjadi Lunas'
            ]);
        } catch (\Exception $e) {
            return response()->json(['success' => false, 'message' => $e->getMessage()], 500);
        }
    }


}
