<?php

namespace App\Http\Controllers;

use App\Models\JadwalDokter;
use App\Models\Dokter;
use App\Models\Ruangan;
use Illuminate\Http\Request;

class JadwalDokterController extends Controller
{
    /**
     * Display a listing of the resource.
     */
    public function index(Request $request)
    {
        try {
            $query = JadwalDokter::with(['dokter.user', 'dokter.poli', 'ruangan']);

            if ($request->has('search') && $request->search != '') {
                $search = $request->search;
                $query->whereHas('dokter.user', function($q) use ($search) {
                    $q->where('nama_user', 'like', "%{$search}%");
                });
            }

            if ($request->has('hari') && $request->hari != '' && $request->hari != 'Semua Hari') {
                $query->where('hari', $request->hari);
            }


            if ($request->has('poli') && $request->poli != '' && $request->poli != 'Semua Poli') {
                $poli = $request->poli;
                $query->whereHas('dokter.poli', function($q) use ($poli) {
                    $q->where('nama_poli', $poli);
                });
            }

            $jadwal = $query->get();

            return response()->json([
                'success' => true,
                'data' => $jadwal
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
        return view('jadwalDokters.create', [
            'dokters' => Dokter::all(),
            'ruangans' => Ruangan::all(),
        ]);
    }

    /**
     * Store a newly created resource in storage.
     */
    public function store(Request $request)
    {
        $validated = $request->validate([
            'hari'        => 'required|in:Senin,Selasa,Rabu,Kamis,Jumat,Sabtu,Minggu',
            'jam_mulai'   => 'required',
            'jam_selesai' => 'required|after:jam_mulai',
            'dokter_id'   => 'required|exists:dokters,id',
            'ruangan_id'  => 'required|exists:ruangans,id',
        ]);

        $jadwal = JadwalDokter::create($validated);

        return response()->json([
            'success' => true,
            'message' => 'Jadwal dokter berhasil ditambahkan',
            'data'    => $jadwal
        ], 201);
    }

    /**
     * Display the specified resource.
     */
    public function show(JadwalDokter $jadwalDokter)
    {
        return view('jadwal_dokter.show', compact('jadwalDokter'));
    }

    /**
     * Show the form for editing the specified resource.
     */
    public function edit(JadwalDokter $jadwalDokter)
    {
        $dokters = Dokter::all();
        $ruangans = Ruangan::all();

        return view('jadwal_dokter.edit', compact('jadwalDokter', 'dokters', 'ruangans'));
    }

    /**
     * Update the specified resource in storage.
     */
    public function update(Request $request, JadwalDokter $jadwalDokter)
    {
        $validated = $request->validate([
            'hari'        => 'required|in:Senin,Selasa,Rabu,Kamis,Jumat,Sabtu,Minggu',
            'jam_mulai'   => 'required',
            'jam_selesai' => 'required|after:jam_mulai',
            'dokter_id'   => 'required|exists:dokters,id',
            'ruangan_id'  => 'required|exists:ruangans,id',
        ]);

        $updated = $jadwalDokter->update($validated);

        return response()->json([
            'success' => $updated,
            'message' => $updated ? 'Jadwal berhasil diperbarui' : 'Gagal memperbarui jadwal',
            'data'    => $jadwalDokter->fresh()
        ], 200);
    }

    /**
     * Remove the specified resource from storage.
     */
    public function destroy(JadwalDokter $jadwalDokter)
    {
        $deleted = $jadwalDokter->delete();

        return response()->json([
            'success' => $deleted,
            'message' => $deleted ? 'Jadwal berhasil dihapus' : 'Gagal menghapus jadwal'
        ], 200);
    }
}
