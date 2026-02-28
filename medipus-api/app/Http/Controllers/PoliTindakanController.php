<?php

namespace App\Http\Controllers;

use App\Models\PoliTindakan;
use Illuminate\Http\Request;

class PoliTindakanController extends Controller
{
    /**
     * Display a listing of the resource.
     */
    public function index()
    {
        $data = PoliTindakan::with(['poli', 'tindakanMedis'])->get();

        return response()->json([
            'success' => true,
            'data'    => $data
        ], 200);
    }

    /**
     * Show the form for creating a new resource.
     */
    public function create()
    {
        //
    }

    /**
     * Store a newly created resource in storage.
     */
    public function store(Request $request)
    {
        $request->validate([
            'poli_id'           => 'required|exists:polis,id',
            'tindakan_medis_id' => 'required|exists:tindakan_medis,id',
            // Validasi agar kombinasi tidak duplikat
            'poli_id'           => 'unique:poli_tindakans,poli_id,NULL,id,tindakan_medis_id,' . $request->tindakan_medis_id
        ], [
            'poli_id.unique' => 'Tindakan medis ini sudah terdaftar di poli tersebut.'
        ]);

        $poliTindakan = PoliTindakan::create([
            'poli_id'           => $request->poli_id,
            'tindakan_medis_id' => $request->tindakan_medis_id,
        ]);

        return response()->json([
            'success' => true,
            'message' => 'Relasi poli dan tindakan berhasil ditambahkan',
            'data'    => $poliTindakan
        ], 201);
    }

    /**
     * Display the specified resource.
     */
    public function show(PoliTindakan $poliTindakan)
    {
        //
    }

    /**
     * Show the form for editing the specified resource.
     */
    public function edit(PoliTindakan $poliTindakan)
    {
        //
    }

    /**
     * Update the specified resource in storage.
     */
    public function update(Request $request, PoliTindakan $poliTindakan)
    {
        $request->validate([
            'poli_id'           => 'required|exists:polis,id',
            'tindakan_medis_id' => 'required|exists:tindakan_medis,id',
        ]);

        $updated = $poliTindakan->update($request->all());

        return response()->json([
            'success' => $updated,
            'message' => $updated ? 'Data berhasil diperbarui' : 'Gagal memperbarui data',
            'data'    => $poliTindakan->fresh()
        ], 200);
    }

    /**
     * Remove the specified resource from storage.
     */
    public function destroy(PoliTindakan $poliTindakan)
    {
        $deleted = $poliTindakan->delete();

        return response()->json([
            'success' => $deleted,
            'message' => $deleted ? 'Data berhasil dihapus' : 'Gagal menghapus data'
        ], 200);
    }
    
}
