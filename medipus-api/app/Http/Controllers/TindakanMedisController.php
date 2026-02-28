<?php

namespace App\Http\Controllers;

use App\Models\TindakanMedis;
use App\Models\PoliTindakan;
use Illuminate\Http\Request;

class TindakanMedisController extends Controller
{
    /**
     * Display a listing of the resource.
     */
    public function index()
    {
        $tindakan = TindakanMedis::all();
        return response()->json([
        'success' => true,
        'data'    => $tindakan
        ], 200);

    }

    public function getTindakanByPoli($poli_id)
    {
        $data = PoliTindakan::with('tindakan_medis')
            ->where('poli_id', $poli_id)
            ->get()
            ->map(function ($item) {

                return [
                    'id' => $item->tindakan_medis_id,
                    'nama_tindakan' => $item->tindakan_medis->nama_tindakan_medis ?? 'Data Kosong', 
                    'harga' => $item->tindakan_medis->biaya_tindakan_medis ?? 0,
                ];
            });

        return response()->json([
            'success' => true,
            'data' => $data
        ]);
    }

    /**
     * Show the form for creating a new resource.
     */
    public function create()
    {
        return view('tindakan_medis.create');
    }

    /**
     * Store a newly created resource in storage.
     */
    public function store(Request $request)
    {
        $validated = $request->validate([
        'nama_tindakan_medis' => 'required|string|max:255',
        'deskripsi_tindakan_medis' => 'required|string',
        'biaya_tindakan_medis' => 'required|integer',
    ]);

    $tindakan = TindakanMedis::create($validated);

    return response()->json(['success' => true, 'data' => $tindakan], 201);
    }

    /**
     * Display the specified resource.
     */
    public function show(TindakanMedis $tindakanMedis)
    {
        $tindakan = TindakanMedis::findOrFail($id);
        return view('tindakan_medis.show', compact('tindakan'));
    }

    /**
     * Show the form for editing the specified resource.
     */
    public function edit(TindakanMedis $tindakanMedis)
    {
        $tindakan = TindakanMedis::findOrFail($id);
        return view('tindakan_medis.edit', compact('tindakan'));
    }

    /**
     * Update the specified resource in storage.
     */
    public function update(Request $request, TindakanMedis $tindakanMedis)
    {
        // Validasi input sesuai kolom migration kamu
        $request->validate([
            'nama_tindakan_medis'      => 'required|string|max:255',
            'deskripsi_tindakan_medis' => 'required|string',
            'biaya_tindakan_medis'     => 'required|integer|min:0',
        ]);

        // Lakukan update
        $updated = $tindakanMedis->update($request->all());

        return response()->json([
            'success' => $updated,
            'message' => $updated ? 'Data tindakan medis berhasil diperbarui' : 'Gagal memperbarui data',
            'data'    => $tindakanMedis->fresh() // Mengambil data terbaru setelah di-update
        ], 200);
    }

    /**
     * Remove the specified resource from storage.
     */
    public function destroy(TindakanMedis $tindakanMedis)
    {
        $deleted = $tindakanMedis->delete();

        return response()->json([
            'success' => $deleted,
            'message' => $deleted ? 'Data tindakan medis berhasil dihapus' : 'Gagal menghapus data'
        ], 200);
    }
}
