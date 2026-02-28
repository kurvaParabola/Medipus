<?php

namespace App\Http\Controllers;

use App\Models\Ruangan;
use Illuminate\Http\Request;

class RuanganController extends Controller
{
    /**
     * Display a listing of the resource.
     */
    public function index()
    {
        $ruangans = Ruangan::all();
        return response()->json([
            'success' => true,
            'data'    => $ruangans
        ], 200);
    }

    /**
     * Show the form for creating a new resource.
     */
    public function create()
    {
        return view('ruangans.create');
    }

    /**
     * Store a newly created resource in storage.
     */
    public function store(Request $request)
    {
        $request->validate([
            'nama_ruangan'       => 'required|max:100',
            'keterangan_ruangan' => 'nullable|max:255',
        ]);

        $lastRuangan = Ruangan::orderBy('kode_ruangan', 'desc')->first();

        if (!$lastRuangan) {
            $kodeOtomatis = 'R-001';
        } else {
            $lastNumber = (int) substr($lastRuangan->kode_ruangan, 2);
            $kodeOtomatis = 'R-' . str_pad($lastNumber + 1, 3, '0', STR_PAD_LEFT);
        }

        $ruangan = Ruangan::create([
            'kode_ruangan'       => $kodeOtomatis,
            'nama_ruangan'       => $request->nama_ruangan,
            'keterangan_ruangan' => $request->keterangan_ruangan,
        ]);

        return response()->json([
            'success' => true,
            'message' => 'Data ruangan berhasil ditambahkan'. $kodeOtomatis,
            'data'    => $ruangan
        ], 201);
    }

    /**
     * Display the specified resource.
     */
    public function show(Ruangan $ruangan)
    {
        $ruangan = Ruangan::findOrFail($id);
        return view('ruangans.show', compact('ruangan'));
    }

    /**
     * Show the form for editing the specified resource.
     */
    public function edit(Ruangan $ruangan)
    {
        $ruangan = Ruangan::findOrFail($id);
        return view('ruangans.edit', compact('ruangan'));
    }

    /**
     * Update the specified resource in storage.
     */
    public function update(Request $request, Ruangan $ruangan)
    {
        $request->validate([
        'nama_ruangan'       => 'required|max:100',
        'keterangan_ruangan' => 'nullable|max:255',
        ]);

        $updated = $ruangan->update($request->only([
            'nama_ruangan', 
            'keterangan_ruangan'
        ]));

        return response()->json([
            'success' => $updated,
            'message' => $updated ? 'Data ruangan berhasil diperbarui' : 'Gagal memperbarui data',
            'data'    => $ruangan->fresh()
        ], 200);
    }

    /**
     * Remove the specified resource from storage.
     */
    public function destroy(Ruangan $ruangan)
    {
        $deleted = $ruangan->delete();

        return response()->json([
            'success' => $deleted,
            'message' => $deleted ? 'Data ruangan berhasil dihapus' : 'Gagal menghapus data'
        ], 200);
    }
}
