<?php

namespace App\Http\Controllers;

use App\Models\Apoteker;
use App\Models\User;
use Illuminate\Http\Request;

class ApotekerController extends Controller
{
    /**
     * Display a listing of the resource.
     */
    public function index()
    {
        $apotekers = Apoteker::with('user')->get();

        return response()->json([
            'success' => true,
            'data'    => $apotekers
        ], 200);
    }

    /**
     * Show the form for creating a new resource.
     */
    public function create()
    {

    }

    /**
     * Store a newly created resource in storage.
     */
    public function store(Request $request)
    {
        $validated = $request->validate([
            'nomor_induk_apoteker' => 'required|numeric|unique:apotekers,nomor_induk_apoteker',
            'user_id'              => 'required|exists:users,id|unique:apotekers,user_id',
        ]);

        $apoteker = Apoteker::create($validated);

        return response()->json([
            'success' => true,
            'message' => 'Data apoteker berhasil ditambahkan',
            'data'    => $apoteker
        ], 201);
    }

    /**
     * Display the specified resource.
     */
    public function show(Apoteker $apoteker)
    {
        $apoteker = Apoteker::findOrFail($id);
        return view('apotekers.show', compact('apoteker'));
    }

    /**
     * Show the form for editing the specified resource.
     */
    public function edit(Apoteker $apoteker)
    {
        return view('apotekers.edit', [
            'apoteker' => $apoteker,
            'biodataUsers' => BiodataUser::all(),
        ]);
    }

    /**
     * Update the specified resource in storage.
     */
    public function update(Request $request, Apoteker $apoteker)
    {
        $validated = $request->validate([
            'nomor_induk_apoteker' => 'required|numeric|unique:apotekers,nomor_induk_apoteker,' . $apoteker->id,
            'user_id'              => 'required|exists:users,id|unique:apotekers,user_id' . $apoteker->id,
        ]);

        $updated = $apoteker->update($validated);

        return response()->json([
            'success' => $updated,
            'message' => $updated ? 'Data apoteker berhasil diperbarui' : 'Gagal memperbarui data',
            'data'    => $apoteker->fresh()
        ], 200);
    }

    /**
     * Remove the specified resource from storage.
     */
    public function destroy(Apoteker $apoteker)
    {
        $deleted = $apoteker->delete();

        return response()->json([
            'success' => $deleted,
            'message' => $deleted ? 'Data apoteker berhasil dihapus' : 'Gagal menghapus data'
        ], 200);
    }
}
