<?php

namespace App\Http\Controllers;

use App\Models\Resep;
use App\Models\Pemeriksaan;
use App\Models\DetailResep;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class ResepController extends Controller
{
    /**
     * Display a listing of the resource.
     */
    public function index(Request $request)
    {
        try {
            $query = Resep::with([
                'pemeriksaan.register.pasien',
                'pemeriksaan.register.poli', 
                'pemeriksaan.dokter.user'
            ])
            ->where('status_resep', 'Menunggu')
            ->has('detail_reseps');

            if ($request->has('search') && $request->search != '') {
                $search = $request->search;
                $query->whereHas('pemeriksaan.register.pasien', function($q) use ($search) {
                    $q->where('nama_pasien', 'like', "%{$search}%")
                    ->orWhere('kode_pasien', 'like', "%{$search}%");
                });
            }

            if ($request->has('poli') && $request->poli != '' && $request->poli != 'Semua Poli') {
                $poli = $request->poli;
                $query->whereHas('pemeriksaan.register.poli', function($q) use ($poli) {
                    $q->where('nama_poli', $poli);
                });
            }

            $reseps = $query->orderBy('created_at', 'asc')->get();

            return response()->json([
                'success' => true,
                'data'    => $reseps
            ], 200);

        } catch (\Exception $e) {
            return response()->json([
                'success' => false,
                'message' => $e->getMessage()
            ], 500);
        }
    }

    public function indexSelesai(Request $request)
    {
        try {
            $query = Resep::with([
                'pemeriksaan.register.pasien',
                'pemeriksaan.register.poli', 
                'pemeriksaan.dokter.user'
            ])
            ->where('status_resep', 'Selesai');

            if ($request->has('tanggal') && $request->tanggal != '') {
                $query->whereDate('tanggal_resep', $request->tanggal);
            }

            $reseps = $query->orderBy('updated_at', 'desc')->get();

            return response()->json([
                'success' => true,
                'data'    => $reseps
            ], 200);

        } catch (\Exception $e) {
            return response()->json([
                'success' => false,
                'message' => $e->getMessage()
            ], 500);
        }
    }

    /**
     * Show the form for creating a new resource.
     */
    public function create()
    {
        $pemeriksaans = Pemeriksaan::all();

        return view('reseps.create', compact('pemeriksaans'));
    }

    /**
     * Store a newly created resource in storage.
     */
    public function store(Request $request)
    {
        $validated = $request->validate([
            'status_resep'   => 'required|in:Menunggu,Diproses,Selesai',
            'tanggal_resep'  => 'required|date',
            'pemeriksaan_id' => 'required|exists:pemeriksaans,id',
        ]);

        return DB::transaction(function () use ($request) {
            $resep = Resep::create($request->all());

            $pemeriksaan = Pemeriksaan::find($request->pemeriksaan_id);
            
            if ($pemeriksaan && $pemeriksaan->register) {
  
                $pemeriksaan->register->update([
                    'status_register' => 'Selesai'
                ]);
            }

            return response()->json([
                'success' => true,
                'message' => 'Resep berhasil dibuat',
                'data'    => $resep
            ], 201);

        });
    }

    /**
     * Display the specified resource.
     */
    public function show(Resep $resep)
    {
        return view('reseps.show', compact('resep'));
    }

    /**
     * Show the form for editing the specified resource.
     */
    public function edit(Resep $resep)
    {
        return view('reseps.edit', compact('resep'));
    }

    /**
     * Update the specified resource in storage.
     */
    public function update(Request $request, Resep $resep)
    {
        $validated = $request->validate([
            'status_resep'   => 'required|in:Menunggu,Diproses,Selesai',
            'tanggal_resep'  => 'required|date',
            'pemeriksaan_id' => 'required|exists:pemeriksaans,id',
        ]);

        $resep->update($validated);

        return response()->json([
            'success' => true,
            'message' => 'Data resep berhasil diperbarui',
            'data'    => $resep->fresh()
        ], 200);
    }

    /**
     * Remove the specified resource from storage.
     */
    public function destroy(Resep $resep)
    {
        $deleted = $resep->delete();

        return response()->json([
            'success' => $deleted,
            'message' => $deleted ? 'Data resep berhasil dihapus' : 'Gagal menghapus data'
        ], 200);
    }
}
