<?php

namespace App\Http\Controllers;

use App\Models\BiodataUser;
use App\Models\User;
use Illuminate\Http\Request;

class BiodataUserController extends Controller
{
    /**
     * Display a listing of the resource.
     */
    public function index()
    {
        $biodata = BiodataUser::with('user')->get();
        return response()->json([
            'success' => true,
            'data'    => $biodata
        ], 200);
    }

    /**
     * Show the form for creating a new resource.
     */
    public function create()
    {
        $users = User::where('status_user', 'Aktif')->get();
        return view('biodata_users.create', compact('users'));
    }

    /**
     * Store a newly created resource in storage.
     */
    public function store(Request $request)
    {
        $validated = $request->validate([
            'user_id'            => 'required|exists:users,id|unique:biodata_users,user_id',
            'nik_user'           => 'required|numeric|unique:biodata_users,nik_user', 
            'nama_user'          => 'required|string|max:100',
            'tanggal_lahir_user' => 'required|date|before:today',
            'jenis_kelamin_user' => 'required|in:Laki-laki,Perempuan',
            'alamat_user'        => 'required|string|max:255',
        ]);

        $biodata = BiodataUser::create($validated);

        return response()->json([
            'success' => true,
            'message' => 'Biodata user berhasil ditambahkan',
            'data'    => $biodata
        ], 201);
    }

    /**
     * Display the specified resource.
     */
    public function show(BiodataUser $biodataUser)
    {
        $biodataUser = BiodataUser::findOrFail($id);
        return view('biodata_users.show', compact('biodataUser'));
    }

    /**
     * Show the form for editing the specified resource.
     */
    public function edit(BiodataUser $biodataUser)
    {
        $users = User::where('status_user', 'Aktif')->get();
        return view('biodata_users.edit', compact('biodataUser', 'users'));
    }

    /**
     * Update the specified resource in storage.
     */
    public function update(Request $request, BiodataUser $biodataUser)
    {
        $validated = $request->validate([
            'user_id'            => 'required|exists:users,id|unique:biodata_users,user_id,' . $biodataUser->id,
            'nik_user'           => 'required|numeric|unique:biodata_users,nik_user,' . $biodataUser->id,
            'nama_user'          => 'required|string|max:100',
            'tanggal_lahir_user' => 'required|date|before:today',
            'jenis_kelamin_user' => 'required|in:Laki-laki,Perempuan',
            'alamat_user'        => 'required|string|max:255',
        ]);

        $updated = $biodataUser->update($validated);

        return response()->json([
            'success' => $updated,
            'message' => $updated ? 'Biodata berhasil diperbarui' : 'Gagal memperbarui biodata',
            'data'    => $biodataUser->fresh()
        ], 200);
    }

    /**
     * Remove the specified resource from storage.
     */
    public function destroy(BiodataUser $biodataUser)
    {
        $deleted = $biodataUser->delete();

        return response()->json([
            'success' => $deleted,
            'message' => $deleted ? 'Data biodata user berhasil dihapus' : 'Gagal menghapus data'
        ], 200);
    }
}
