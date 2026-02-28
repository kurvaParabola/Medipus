<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class Pasien extends Model
{
    use HasFactory;

    protected $table = 'pasiens';

    protected $fillable = [
        'kode_pasien',
        'nik_pasien',
        'nama_pasien',
        'tanggal_lahir_pasien',
        'jenis_kelamin_pasien',
        'alamat_pasien',
        'nomor_hp_pasien',
    ];

    public function registers()
    {
        return $this->hasMany(Register::class);
    }
}
