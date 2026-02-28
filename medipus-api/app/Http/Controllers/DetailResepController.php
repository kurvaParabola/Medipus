<?php

namespace App\Http\Controllers;

use App\Models\DetailResep;
use App\Models\Resep;
use App\Models\Obat;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class DetailResepController extends Controller
{
    /**
     * Display a listing of the resource.
     */
    public function index()
    {
        $details = DetailResep::with(['resep', 'obat'])->get();

        return response()->json([
            'success' => true,
            'data'    => $details
        ], 200);
    }

    public function showByResep($resep_id)
    {
        $details = DetailResep::with('obat') 
                    ->where('resep_id', $resep_id)
                    ->get();

        return response()->json([
            'success' => true,
            'data' => $details
        ]);
    }

    /**
     * Show the form for creating a new resource.
     */
    public function create()
    {
        $reseps = Resep::all();
        $obats  = Obat::all();

        return view('detail_reseps.create', compact('reseps', 'obats'));
    }

    /**
     * Store a newly created resource in storage.
     */
    public function store(Request $request)
    {
        $request->validate([
            'resep_id'               => 'required|exists:reseps,id',
            'items'                  => 'required|array',
            'items.*.obat_id'        => 'required',
            'items.*.dosis_obat'     => 'required|string',
            'items.*.frekuensi_obat' => 'required|string',
            'items.*.durasi_obat'    => 'required|string',
            'items.*.jumlah_obat'    => 'required|integer',
            'items.*.catatan_obat'   => 'nullable|string',
        ]);

        return DB::transaction(function () use ($request) {
            $createdItems = [];
            foreach ($request->items as $item) {
                $obat = Obat::find($item['obat_id']);

                if (!$obat) {
                    throw new \Exception("Obat dengan ID " . $item['obat_id'] . " tidak ditemukan.");
                }
                
                $detail = DetailResep::create([
                    'resep_id'       => $request->resep_id,
                    'obat_id'        => $item['obat_id'],
                    'dosis_obat'     => $item['dosis_obat'],
                    'frekuensi_obat' => $item['frekuensi_obat'],
                    'durasi_obat'    => $item['durasi_obat'],
                    'jumlah_obat'    => $item['jumlah_obat'],
                    'catatan_obat'   => $item['catatan_obat'] ?? null,
                    'subtotal_obat'  => $obat->harga_satuan * $item['jumlah_obat'],
                ]);
                $createdItems[] = $detail;
            }

            return response()->json([
                'success' => true,
                'message' => count($createdItems) . ' obat berhasil ditambahkan ke resep',
                'data'    => $createdItems
            ], 201);
        });
    }

    /**
     * Display the specified resource.
     */
    public function show(DetailResep $detailResep)
    {
        $detailResep->load(['obat', 'resep']);
        return view('detail_reseps.show', compact('detailResep'));
    }

    /**
     * Show the form for editing the specified resource.
     */
    public function edit(DetailResep $detailResep)
    {
        $detailResep->load(['obat', 'resep']);
        return view('detail_reseps.show', compact('detailResep'));
    }

    /**
     * Update the specified resource in storage.
     */
    public function update(Request $request,  $resepId)
    {
        $request->validate([
            'items'                  => 'required|array|min:1',
            'items.*.obat_id'        => 'required|exists:obats,id',
            'items.*.dosis_obat'     => 'required|string',
            'items.*.frekuensi_obat' => 'required|string',
            'items.*.durasi_obat'    => 'required|string',
            'items.*.jumlah_obat'    => 'required|integer|min:1',
            'items.*.catatan_obat'   => 'nullable|string',
        ]);

        return DB::transaction(function () use ($request, $resepId) {

            DetailResep::where('resep_id', $resepId)->delete();

            $updatedItems = [];
            foreach ($request->items as $item) {
                $obat = Obat::findOrFail($item['obat_id']);
                $detail = DetailResep::create([
                    'resep_id'       => $resepId,
                    'obat_id'        => $item['obat_id'],
                    'dosis_obat'     => $item['dosis_obat'],
                    'frekuensi_obat' => $item['frekuensi_obat'],
                    'durasi_obat'    => $item['durasi_obat'],
                    'jumlah_obat'    => $item['jumlah_obat'],
                    'catatan_obat'   => $item['catatan_obat'] ?? null,
                    'subtotal_obat'  => $obat->harga_satuan * $item['jumlah_obat'],
                ]);
                $updatedItems[] = $detail;
            }

            return response()->json([
                'success' => true,
                'message' => 'Daftar obat pada resep berhasil diperbarui',
                'data'    => $updatedItems
            ], 200);
        });
    }

    /**
     * Remove the specified resource from storage.
     */
    public function destroy(DetailResep $detailResep)
    {
        $deleted = $detailResep->delete();

        return response()->json([
            'success' => $deleted,
            'message' => $deleted ? 'Detail resep berhasil dihapus' : 'Gagal menghapus data'
        ], 200);
    }

    public function updateAfterProcess(Request $request)
    {
        try {
        return \DB::transaction(function () use ($request) {
            
            foreach ($request->items as $item) {
                \App\Models\DetailResep::where('resep_id', $request->resep_id)
                    ->where('obat_id', $item['obat_id'])
                    ->update([
                        'jumlah_obat'   => $item['jumlah'],
                        'subtotal_obat' => $item['subtotal']
                    ]);
            }

            $pemeriksaan = \App\Models\Pemeriksaan::find($request->pemeriksaan_id);
            $tindakan = \App\Models\TindakanMedis::find($pemeriksaan->tindakan_medis_id);
            $biayaTindakan = $tindakan->biaya_tindakan_medis ?? 0;

            $totalBiayaObat = \App\Models\DetailResep::where('resep_id', $request->resep_id)->sum('subtotal_obat');

            \App\Models\Resep::where('id', $request->resep_id)->update(['status_resep' => 'Selesai']);

            \App\Models\Transaksi::create([
                'biaya_tindakan'        => $biayaTindakan,
                'biaya_obat'            => $totalBiayaObat,
                'total_biaya_transaksi' => $biayaTindakan + $totalBiayaObat,
                'tanggal_transaksi'     => now(),
                'status_pembayaran'     => 'Menunggu',
                'pemeriksaan_id'        => $request->pemeriksaan_id,
                'resep_id'              => $request->resep_id,
                'staff_id'              => 1, 
            ]);

            return response()->json(['success' => true, 'message' => 'Sukses!']);
        });
    } catch (\Exception $e) {
        return response()->json(['success' => false, 'message' => $e->getMessage()], 500);
    }
        
    }
}
