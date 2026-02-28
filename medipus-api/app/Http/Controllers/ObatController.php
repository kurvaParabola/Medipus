<?php

namespace App\Http\Controllers;

use App\Models\Obat;
use Illuminate\Http\Request;

class ObatController extends Controller
{
    /**
     * Display a listing of the resource.
     */
   public function index(Request $request)
    {
        try {
            $query = Obat::query();

            if ($request->has('search') && $request->search != '') {
                $search = $request->search;
                $query->where(function($q) use ($search) {
                    $q->where('nama_obat', 'like', "%{$search}%")
                    ->orWhere('kode_obat', 'like', "%{$search}%");
                });
            }

            if ($request->has('kategori') && $request->kategori != '' && $request->kategori != 'Semua Kategori') {
                $query->where('kategori', $request->kategori);
            }

            $obats = $query->select([
                'id',
                'kode_obat',
                'nama_obat',
                'kategori',
                'stok',
                'satuan_obat', 
                'harga_satuan',
                'lokasi_penyimpanan',
                'kadaluarsa'  
            ])
            ->orderBy('kode_obat', 'asc')
            ->get();

            return response()->json([
                'success' => true,
                'message' => 'Daftar data obat berhasil diambil',
                'data'    => $obats
            ], 200);

        } catch (\Exception $e) {
            return response()->json([
                'success' => false,
                'message' => 'Gagal memuat data: ' . $e->getMessage()
            ], 500);
        }
    }

    /**
     * Show the form for creating a new resource.
     */
    public function create()
    {
        return view('obats.create');
    }

    /**
     * Store a newly created resource in storage.
     */
    public function store(Request $request)
    {
        $request->validate([
            'nama_obat'          => 'required|max:100',
            'satuan_obat'        => 'required|max:50',
            'kategori'           => 'required|in:Bebas,Bebas Terbatas,Keras',
            'stok'               => 'required|integer|min:0',
            'harga_satuan'       => 'required|integer|min:0',
            'kadaluarsa'         => 'required|date|after:today',
            'lokasi_penyimpanan' => 'required|in:Rak Obat Bebas,Rak Obat Bebat Terbatas,Rak Obat Keras,Kulkas',
        ]);

        $lastObat = Obat::orderBy('id', 'desc')->first();

        if (!$lastObat) {
            $kodeOtomatis = 'OB-001';
        } else {
            $lastNumber = (int) substr($lastObat->kode_obat, 3);
            $kodeOtomatis = 'OB-' . str_pad($lastNumber + 1, 3, '0', STR_PAD_LEFT);
        }

        $obat = new Obat();
        $obat->kode_obat = $kodeOtomatis;
        $obat->nama_obat = $request->nama_obat;
        $obat->stok = $request->stok;
        $obat->satuan_obat = $request->satuan_obat;
        $obat->kadaluarsa = $request->kadaluarsa;
        $obat->kategori = $request->kategori;
        $obat->harga_satuan = $request->harga_satuan;
        $obat->lokasi_penyimpanan = $request->lokasi_penyimpanan;

       if ($obat->save()) {
        // Jangan gunakan refresh() jika primary key bermasalah
        // Langsung kembalikan data yang baru saja dibuat
        return response()->json([
            'success' => true,
            'message' => 'Data obat berhasil ditambahkan dengan kode: ' . $kodeOtomatis,
            'data'    => $obat
        ], 201);
    }
    }

    /**
     * Display the specified resource.
     */
    public function show($id)
    {
        $obat = Obat::findOrFail($id);
        if (!$obat) {
            return response()->json([
                'success' => false,
                'message' => 'Obat tidak ditemukan'
            ], 404);
        }

        return response()->json([
            'success' => true,
            'message' => 'Detail obat ditemukan',
            'data'    => $obat // Data ini yang akan di-parse ke ObatData di C#
        ], 200);
    }

    /**
     * Show the form for editing the specified resource.
     */
    public function edit(Obat $obat)
    {
        $obat = Obat::findOrFail($id);
        return view('obats.edit', compact('obat'));
    }

    /**
     * Update the specified resource in storage.
     */
    // Pastikan parameter fungsinya adalah (Request $request, Obat $obat)
    public function update(Request $request, Obat $obat)
    {
        $request->validate([
            'nama_obat'          => 'required|max:100',
            'satuan_obat'        => 'required|max:50',
            'kategori'           => 'required|in:Bebas,Bebas Terbatas,Keras',
            'stok'               => 'required|integer|min:0',
            'harga_satuan'       => 'required|integer|min:0',
            'kadaluarsa'         => 'required|date',
            'lokasi_penyimpanan' => 'required|in:Rak Obat Bebas,Rak Obat Bebat Terbatas,Rak Obat Keras,Kulkas',
        ]);

        $updated = $obat->update($request->except(['kode_obat']));

        return response()->json([
            'success' => $updated,
            'message' => $updated ? 'Data obat berhasil diperbarui' : 'Gagal memperbarui data',
            'data'    => $obat->fresh() 
        ], 200);
    }

    /**
     * Remove the specified resource from storage.
     */
    public function destroy(Obat $obat)
    {
        $deleted = $obat->delete();

        return response()->json([
            'success' => $deleted,
            'message' => $deleted ? 'Data obat berhasil dihapus' : 'Gagal menghapus data'
        ], 200);
    }
}
