<?php

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use App\Models\User;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Hash;
use Illuminate\Validation\Rule;
use Illuminate\Validation\ValidationException;

class AuthController extends Controller
{


     public function index()
    {
        $users = User::all();
        return response()->json([
            'success' => true,
            'data'    => $users
        ], 200);
    }
    
    /**
     * Handle an incoming registration request.
     */
    public function register(Request $request)
    {
        $request->validate([
            'username' => ['required', 'string', 'max:255'],
            'email' => ['required', 'string', 'email', 'max:255', 'unique:users'],
            'password' => ['required', 'string', 'min:8', 'confirmed'],
            // Note: nomor_hp_user is defined as 'integer' in your migration
            'nomor_hp_user' => ['required', 'integer'],
            // Validate the 'role' field against the allowed enum values
            'role' => ['required', Rule::in(['Staff Register', 'Staff Transaksi', 'Dokter', 'Apoteker'])],

            'nik_user' => ['required', 'numeric', 'unique:users,nik_user'], // NIK biasanya unik
            'nama_user' => ['required', 'string', 'max:255'],
            'tanggal_lahir_user' => ['required', 'date'],
            'jenis_kelamin_user' => ['required', Rule::in(['Laki-laki', 'Perempuan'])],
            'alamat_user' => ['required', 'string'],
        ]);

        $user = User::create([
            'username' => $request->username,
            'email' => $request->email,
            'password' => Hash::make($request->password),
            'nomor_hp_user' => $request->nomor_hp_user,
            'role' => $request->role,
            // Automatically set status_user to 'Aktif' for new registrations
            'status_user' => 'Aktif',
            'nik_user' => $request->nik_user,
            'nama_user' => $request->nama_user,
            'tanggal_lahir_user' => $request->tanggal_lahir_user,
            'jenis_kelamin_user' => $request->jenis_kelamin_user,
            'alamat_user' => $request->alamat_user,
        ]);

        // Create an API token for the newly registered user
        $token = $user->createToken('auth_token')->plainTextToken;

        return response()->json([
            'message' => 'User successfully registered.',
            'user' => $user,
            'token' => $token,
        ], 201);
    }

    /**
     * Handle an incoming login request.
     */
    public function login(Request $request)
    {
        $request->validate([
        'username' => 'required',
        'password' => 'required',
        ]);

        $user = User::where('username', $request->username)->first();

        if (!$user || !Hash::check($request->password, $user->password)) {
            return response()->json([
                'success' => false,
                'message' => 'Username atau Password salah.',
                'data' => null
            ], 401);
        }

        $token = $user->createToken('auth_token')->plainTextToken;

        return response()->json([
            'success' => true,
            'message' => 'Login Berhasil',
            'data' => [
                'user' => [
                    'id' => $user->id,
                    'username' => $user->username,
                    'email'    => $user->email,
                    'role'     => $user->role,
                    'status_user' => $user->status_user,
                    'nama_user'           => $user->nama_user,
                    'nik_user'            => $user->nik_user,
                    'tanggal_lahir_user'  => $user->tanggal_lahir_user,
                    'jenis_kelamin_user'  => $user->jenis_kelamin_user,
                    'alamat_user'         => $user->alamat_user,
                    'nomor_hp_user'       => $user->nomor_hp_user,

                ],
                'token' => $token
            ]
        ], 200);
    }

    /**
     * Handle user logout (optional).
     */
    public function logout(Request $request)
    {
        // Delete the current API token being used
        $request->user()->currentAccessToken()->delete();

        return response()->json([
            'message' => 'Logged out successfully.',
        ]);
    }
} 