<?php

namespace App\Http\Controllers;

use App\Models\Register;
use App\Models\Pasien;
use App\Models\JadwalDokter;
use App\Models\Dokter;
use App\Models\Poli;
use App\Models\Api\Auth;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

use Carbon\Carbon;

class RegisterController extends Controller
{
    /**
     * Display a listing of the resource.
     */
    public function index()
    {
        $data = Register::with(['pasien', 'poli', 'jadwalDokter.ruangan', 'jadwalDokter.dokter.user']);

        return response()->json([
            'success' => true,
            'data'    => $data
        ], 200);
    }

    public function indexSemuaPoli(Request $request)
    {
        try {
            $query = Register::with(['pasien', 'poli', 'jadwalDokter.ruangan'])
                ->where('status_register', 'Menunggu')
                ->whereDate('tanggal_register', ">=", now()->toDateString());

            if ($request->has('search') && $request->search != '') {
                $search = $request->search;
                $query->whereHas('pasien', function($q) use ($search) {
                    $q->where('nama_pasien', 'like', "%{$search}%")
                    ->orWhere('kode_pasien', 'like', "%{$search}%");
                });
            }

            if ($request->has('poli') && $request->poli != '' && $request->poli != 'Semua Poli') {
                $poli = $request->poli;
                $query->whereHas('poli', function($q) use ($poli) {
                    $q->where('nama_poli', $poli);
                });
            }

            $data = $query->orderBy('tanggal_register', 'asc')
                        ->orderBy('nomor_antrian', 'asc')
                        ->get();

            return response()->json([
                'success' => true, 
                'data' => $data
            ]);
            
        } catch (\Exception $e) {
            return response()->json([
                'success' => false, 
                'message' => 'Terjadi kesalahan: ' . $e->getMessage()
            ], 500);
        }
    }

    public function indexDokter(Request $request)
    {
        $user = auth()->user();
        $dokter = \App\Models\Dokter::where('user_id', $user->id)->first();

        if (!$dokter) {
            return response()->json(['message' => 'Akses Ditolak'], 403);
        }

        $query = Register::with(['pasien', 'poli', 'jadwalDokter.ruangan'])
            ->where('poli_id', $dokter->poli_id)
            ->where('status_register', 'Menunggu')
            ->whereDate('tanggal_register',">=", now()->toDateString());

        if ($request->has('search')) {
            $query->whereHas('pasien', function($q) use ($request) {
                $q->where('nama_pasien', 'like', '%' . $request->search . '%')
                ->orWhere('kode_pasien', 'like', '%' . $request->search . '%');
            });
        }

    $data = $query->orderBy('nomor_antrian', 'asc')->get();

        return response()->json(['success' => true, 'data' => $data]);
    }

    public function laporanKunjungan(Request $request)
    {
        try {
            $query = Register::with(['pasien', 'poli', 'jadwalDokter.ruangan'])
                ->where('status_register', 'Selesai'); 

            if ($request->has('tanggal') && $request->tanggal != '') {
                $query->whereDate('tanggal_register', $request->tanggal);
            }

            $data = $query->orderBy('nomor_antrian', 'asc')->get();

            return response()->json([
                'success' => true, 
                'data' => $data
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
        $pasiens = Pasien::all();
        $polis   = Poli::all();
        $staff  = Staff::all();

        return view('registers.create', compact('pasiens', 'polis', 'staff'));
    }

    /**
     * Store a newly created resource in storage.
     */
    public function store(Request $request)
    {
        $request->validate([
            'nik_pasien'           => 'required|numeric',
            'nama_pasien'          => 'required|max:100',
            'tanggal_lahir_pasien' => 'required|date',
            'jenis_kelamin_pasien' => 'required|in:Laki-laki,Perempuan',
            'alamat_pasien'        => 'required',
            'nomor_hp_pasien'      => 'required|numeric',

            'tanggal_register' => 'required|date',
            'keluhan_pasien'   => 'required|string',
            'poli_id'          => 'required|exists:polis,id', 
        ]);

        return DB::transaction(function () use ($request) {
            
            $pasien = Pasien::where('nik_pasien', $request->nik_pasien)->first();

            if ($pasien) 
            {
                $pasien->update([
                    'nama_pasien'          => $request->nama_pasien,
                    'tanggal_lahir_pasien' => $request->tanggal_lahir_pasien,
                    'jenis_kelamin_pasien' => $request->jenis_kelamin_pasien,
                    'alamat_pasien'        => $request->alamat_pasien,
                    'nomor_hp_pasien'      => $request->nomor_hp_pasien,
                ]);
            } 
            else 
            {
                $lastPasien = Pasien::where('kode_pasien', 'like', 'PU-%')
                        ->orderBy('kode_pasien', 'desc')
                        ->first();

                if (!$lastPasien) {
                    $kodeOtomatis = 'PU-001';
                } else {
                    $lastNumber = (int) substr($lastPasien->kode_pasien, 3);
                    $kodeOtomatis = 'PU-' . str_pad($lastNumber + 1, 3, '0', STR_PAD_LEFT);
                }

                $pasien = Pasien::create([
                    'nik_pasien'           => $request->nik_pasien,
                    'nama_pasien'          => $request->nama_pasien,
                    'tanggal_lahir_pasien' => $request->tanggal_lahir_pasien,
                    'jenis_kelamin_pasien' => $request->jenis_kelamin_pasien,
                    'alamat_pasien'        => $request->alamat_pasien,
                    'nomor_hp_pasien'      => $request->nomor_hp_pasien,
                    'kode_pasien'          => $kodeOtomatis,
                ]);
            }

            $tanggalCek = \Carbon\Carbon::parse($request->tanggal_register);
            Carbon::setLocale('id'); 

            $slotDitemukan = false;
            $maxCariHari = 7; 
            $iterasi = 0;

            $tanggalFinal = null;
            $jamFinal = null;
            $jadwalIdFinal = null;

            while (!$slotDitemukan && $iterasi < $maxCariHari) 
            {
                $hariNama = $tanggalCek->translatedFormat('l'); 
                
                $jadwal = \App\Models\JadwalDokter::where('hari', $hariNama)
                    ->whereHas('dokter', function($q) use ($request) {
                        $q->where('poli_id', $request->poli_id);
                    })->first();

                if ($jadwal) 
                {
                    $lastReg = Register::where('poli_id', $request->poli_id)
                        ->where('tanggal_register', $tanggalCek->toDateString())
                        ->orderBy('jadwal', 'desc')
                        ->first();

                    if (!$lastReg) {
                        $jamBaru = $jadwal->jam_mulai;
                    } else {
                        $jamBaru = \Carbon\Carbon::parse($lastReg->jadwal)->addMinutes(30)->format('H:i:s');
                    }

                    if ($jamBaru < $jadwal->jam_selesai) {
                        $slotDitemukan = true;
                        $tanggalFinal = $tanggalCek->toDateString();
                        $jamFinal = $jamBaru;
                        $jadwalIdFinal = $jadwal->id;
                    } else {
                        $tanggalCek->addDay();
                    }
                } 
                else 
                {
                    $tanggalCek->addDay();
                }
                $iterasi++;
            }

            if (!$slotDitemukan) {
                return response()->json(['message' => 'Jadwal dokter tidak tersedia atau penuh dalam 7 hari ke depan'], 422);
            }

            $poli = \App\Models\Poli::find($request->poli_id);
            $prefixMap = ['Poli Umum' => 'AU', 'Poli Gigi' => 'AG', 'Poli Kandungan' => 'AK'];
            $prefix = $prefixMap[$poli->nama_poli] ?? 'AX';
            
            $countAntrian = Register::where('poli_id', $request->poli_id)
                ->where('tanggal_register', $tanggalFinal)
                ->count();
            $nomorAntrian = $prefix . '-' . str_pad($countAntrian + 1, 3, '0', STR_PAD_LEFT);

            $register = Register::create
            ([
                    'tanggal_register' => $tanggalFinal,
                    'jadwal'           => $jamFinal,
                    'nomor_antrian'    => $nomorAntrian,
                    'status_register'  => 'Menunggu',
                    'keluhan_pasien'   => $request->keluhan_pasien,
                    'pasien_id'        => $pasien->id,
                    'jadwal_dokter_id' => $jadwalIdFinal,
                    'poli_id'          => $request->poli_id,
            ]);

            $result = Register::with(['pasien', 'poli','jadwalDokter.ruangan', 'jadwalDokter.dokter.user'])->find($register->id);

            return response()->json([
                'success' => true,
                'message' => "Registrasi Berhasil!",
                'data'    => $result
            ], 201);

        });

    }

    /**
     * Display the specified resource.
     */
    public function show(Register $register)
    {
        return view('registers.show', compact('register'));

    }

    /**
     * Show the form for editing the specified resource.
     */
    public function edit(Register $register)
    {
        $pasiens = Pasien::all();
        $polis   = Poli::all();
        $staff  = Staff::all();

        return view('registers.edit', compact('register', 'pasiens', 'polis', 'staff'));
    }

    /**
     * Update the specified resource in storage.
     */
    public function update(Request $request, Register $register)
    {
        $validated = $request->validate([
            'tanggal_register' => 'required|date',
            'jadwal'           => 'required',
            'status_register'  => 'required|in:Menunggu,Diperiksa,Selesai',
            'keluhan_pasien'   => 'required|string|max:500',
            'pasien_id'        => 'required|exists:pasiens,id',
            'jadwal_dokter_id' => 'required|exists:jadwal_dokters,id', // Sesuaikan foreign key
            'poli_id'          => 'required|exists:polis,id',
        ]);

        // Update semua kecuali nomor_antrian
        $updated = $register->update($request->except(['nomor_antrian']));

        return response()->json([
            'success' => $updated,
            'message' => 'Registrasi berhasil diperbarui',
            'data'    => $register->fresh()
        ], 200);
    }

    /**
     * Remove the specified resource from storage.
     */
    public function destroy(Register $register)
    {
        $deleted = $register->delete();

        return response()->json([
            'success' => $deleted,
            'message' => $deleted ? 'Data registrasi berhasil dihapus' : 'Gagal menghapus data'
        ], 200);
    }
}
