<?php

namespace App\Http\Controllers;

use App\Models\Dokter;
use App\Models\User;
use App\Models\Poli;
use Illuminate\Http\Request;

class DokterController extends Controller
{
    /**
     * Display a listing of the resource.
     */
    public function index()
    {
        $dokters = Dokter::with(['user', 'poli'])->get();

        return response()->json([
            'success' => true,
            'data'    => $dokters
        ], 200);
    }

    /**
     * Show the form for creating a new resource.
     */
    public function create()
    {
        return view('dokters.create', [
            'biodataUsers' => BiodataUser::all(),
            'polis' => Poli::all(),
        ]);
    }

    /**
     * Store a newly created resource in storage.
     */
    public function store(Request $request)
    {
        $validated = $request->validate([
            'nomor_induk_dokter' => 'required|numeric|unique:dokters,nomor_induk_dokter',
            'user_id'            => 'required|exists:users,id|unique:dokters,user_id',
            'poli_id'            => 'required|exists:polis,id',
        ]);

        $dokter = Dokter::create($validated);

        return response()->json([
            'success' => true,
            'message' => 'Data dokter berhasil ditambahkan',
            'data'    => $dokter
        ], 201);
    }

    /**
     * Display the specified resource.
     */
    public function show(Dokter $dokter)
    {
        $dokter = Dokter::findOrFail($id);
        return view('dokters.show', compact('dokter'));
    }

    /**
     * Show the form for editing the specified resource.
     */
    public function edit(Dokter $dokter)
    {
        return view('dokters.edit', [
            'dokter' => $dokter,
            'biodataUsers' => BiodataUser::all(),
            'polis' => Poli::all(),
        ]);
    }

    /**
     * Update the specified resource in storage.
     */
    public function update(Request $request, Dokter $dokter)
    {
        $validated = $request->validate([
            'nomor_induk_dokter' => 'required|numeric|unique:dokters,nomor_induk_dokter,' . $dokter->id,
            'user_id'            => 'required|exists:users,id|unique:dokters,user_id,' . $dokter->id,
            'poli_id'            => 'required|exists:polis,id',
        ]);

        $updated = $dokter->update($validated);

        return response()->json([
            'success' => $updated,
            'message' => $updated ? 'Data dokter berhasil diperbarui' : 'Gagal memperbarui data',
            'data'    => $dokter->fresh()
        ], 200);
    }

    /**
     * Remove the specified resource from storage.
     */
    public function destroy(Dokter $dokter)
    {
        $deleted = $dokter->delete();

        return response()->json([
            'success' => $deleted,
            'message' => $deleted ? 'Data dokter berhasil dihapus' : 'Gagal menghapus data'
        ], 200);
    }
}
