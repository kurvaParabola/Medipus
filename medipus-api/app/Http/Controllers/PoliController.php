<?php

namespace App\Http\Controllers;

use App\Models\Poli;
use Illuminate\Http\Request;

class PoliController extends Controller
{
    /**
     * Display a listing of the resource.
     */
    public function index()
    {
        $polis = Poli::all();
        return response()->json([
            'success' => true,
            'data'    => $polis
        ], 200);
    }

    /**
     * Show the form for creating a new resource.
     */
    public function create()
    {
        return view('polis.create');
    }

    /**
     * Store a newly created resource in storage.
     */
    public function store(Request $request)
    {
        $request->validate([
            'nama_poli' => 'required|max:100|unique:polis,nama_poli',
        ]);

    // 2. Simpan ke Database
        $poli = \App\Models\Poli::create([
            'nama_poli' => $request->nama_poli,
        ]);

    // 3. Response JSON (Bukan Redirect)
        return response()->json([
            'success' => true,
            'message' => 'Data poli berhasil ditambahkan',
            'data'    => $poli
        ], 201);
    }

    /**
     * Display the specified resource.
     */
    public function show(Poli $poli)
    {
        $poli = Poli::findOrFail($id);
        return view('polis.show', compact('poli'));
    }

    /**
     * Show the form for editing the specified resource.
     */
    public function edit(Poli $poli)
    {
        $poli = Poli::findOrFail($id);
        return view('polis.edit', compact('poli'));
    }

    /**
     * Update the specified resource in storage.
     */
    public function update(Request $request, Poli $poli)
    {
        $request->validate([
            'nama_poli' => 'required|max:100|unique:polis,nama_poli,' . $poli->id,
        ]);

        // Proses Update
        $updated = $poli->update([
            'nama_poli' => $request->nama_poli,
        ]);

        return response()->json([
            'success' => $updated,
            'message' => $updated ? 'Data poli berhasil diperbarui' : 'Gagal memperbarui data',
            'data'    => $poli->fresh()
        ], 200);
    }

    /**
     * Remove the specified resource from storage.
     */
    public function destroy(Poli $poli)
    {
        $deleted = $poli->delete();

        return response()->json([
            'success' => $deleted,
            'message' => $deleted ? 'Data poli berhasil dihapus' : 'Gagal menghapus data'
        ], 200);
    }
}
