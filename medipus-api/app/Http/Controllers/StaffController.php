<?php

namespace App\Http\Controllers;

use App\Models\Staff;
use App\Models\User;
use Illuminate\Http\Request;

class StaffController extends Controller
{
    /**
     * Display a listing of the resource.
     */
    public function index()
    {
        $staff = Staff::with('user')->get();

        return response()->json([
            'success' => true,
            'data'    => $staff
        ], 200);
    }

    /**
     * Show the form for creating a new resource.
     */
    public function create()
    {
        return view('staff.create', [
            'biodataUsers' => BiodataUser::all()
        ]);
    }

    /**
     * Store a newly created resource in storage.
     */
    public function store(Request $request)
    {
        $validated = $request->validate([
            'nomor_induk_staff' => 'required|numeric|unique:staff,nomor_induk_staff',
            'user_id'           => 'required|exists:users,id|unique:staff,user_id',
        ]);

        $staff = Staff::create($validated);

        return response()->json([
            'success' => true,
            'message' => 'Data staff berhasil ditambahkan',
            'data'    => $staff
        ], 201);
    }

    /**
     * Display the specified resource.
     */
    public function show(Staff $staff)
    {
        $staff = Staff::findOrFail($id);
        return view('staff.show', compact('staff'));
    }

    /**
     * Show the form for editing the specified resource.
     */
    public function edit(Staff $staff)
    {
        return view('staff.edit', [
            'staff' => $staff,
            'biodataUsers' => BiodataUser::all(),
        ]);
    }

    /**
     * Update the specified resource in storage.
     */
    public function update(Request $request, Staff $staff)
    {
        $validated = $request->validate([
            'nomor_induk_staff' => 'required|numeric|unique:staff,nomor_induk_staff,' . $staff->id,
            'user_id'           => 'required|exists:users,id|unique:staff,user_id,' . $staff->id,
        ]);

        $updated = $staff->update($validated);

        return response()->json([
            'success' => $updated,
            'message' => $updated ? 'Data staff berhasil diperbarui' : 'Gagal memperbarui data',
            'data'    => $staff->fresh()
        ], 200);
    }

    /**
     * Remove the specified resource from storage.
     */
    public function destroy(Staff $staff)
    {
        $deleted = $staff->delete();

        return response()->json([
            'success' => $deleted,
            'message' => $deleted ? 'Data staff berhasil dihapus' : 'Gagal menghapus data'
        ], 200);
    }
}
