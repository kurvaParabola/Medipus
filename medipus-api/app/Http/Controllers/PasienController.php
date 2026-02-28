<?php

namespace App\Http\Controllers;

use App\Models\Pasien;
use Illuminate\Http\Request;

class PasienController extends Controller
{
    /**
     * Display a listing of the resource.
     */
    public function index()
    {
        $pasiens = Pasien::all();
        return response()->json([
            'success' => true,
            'data'    => $pasiens
        ], 200);
    }

    /**
     * Show the form for creating a new resource.
     */
    public function create()
    {
        return view('pasiens.create');
    }

    /**
     * Store a newly created resource in storage.
     */
    public function store(Request $request)
    {
        $request->validate([
            'nik_pasien'           => 'required|numeric|unique:pasiens,nik_pasien',
            'nama_pasien'          => 'required|max:100',
            'tanggal_lahir_pasien' => 'required|date',
            'jenis_kelamin_pasien' => 'required|in:Laki-laki,Perempuan',
            'alamat_pasien'        => 'required',
            'nomor_hp_pasien'      => 'required|numeric',
        ]);

        $lastPasien = Pasien::orderBy('kode_pasien', 'desc')->first();

        if (!$lastPasien) {
            $kodeOtomatis = 'PU-001';
        } else {
            $lastNumber = (int) substr($lastPasien->kode_pasien, 3);
            $kodeOtomatis = 'PU-' . str_pad($lastNumber + 1, 3, '0', STR_PAD_LEFT);
        }

        $pasien = Pasien::create(array_merge($request->all(), [
            'kode_pasien' => $kodeOtomatis
        ]));

        return response()->json([
            'success' => true,
            'message' => 'Data pasien berhasil ditambahkan' . $kodeOtomatis,
            'data'    => $pasien
        ], 201);
    }

    /**
     * Display the specified resource.
     */
    public function show(Pasien $pasien)
    {
        $pasien = Pasien::findOrFail($id);
        return view('pasiens.show', compact('pasien'));
    }

    /**
     * Show the form for editing the specified resource.
     */
    public function edit(Pasien $pasien)
    {
        $pasien = Pasien::findOrFail($id);
        return view('pasiens.edit', compact('pasien'));
    }

    /**
     * Update the specified resource in storage.
     */
    public function update(Request $request, Pasien $pasien)
    {
        $request->validate([
        'nik_pasien'           => 'required|numeric|unique:pasiens,nik_pasien,' . $pasien->id,
        'nama_pasien'          => 'required|max:100',
        'tanggal_lahir_pasien' => 'required|date',
        'jenis_kelamin_pasien' => 'required|in:Laki-laki,Perempuan',
        'alamat_pasien'        => 'required',
        'nomor_hp_pasien'      => 'required|numeric',
        ]);

        $updated = $pasien->update($request->except('kode_pasien'));

        return response()->json([
            'success' => $updated,
            'message' => $updated ? 'Data pasien berhasil diperbarui' : 'Gagal memperbarui data',
            'data'    => $pasien->fresh()
        ], 200);
    }

    /**
     * Remove the specified resource from storage.
     */
    public function destroy(Pasien $pasien)
    {
        $deleted = $pasien->delete();

        return response()->json([
            'success' => $deleted,
            'message' => $deleted ? 'Data pasien berhasil dihapus' : 'Gagal menghapus data'
        ], 200);
    }
}
