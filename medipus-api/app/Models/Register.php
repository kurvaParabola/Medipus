<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class Register extends Model
{
    use HasFactory;

    protected $fillable = [
        'tanggal_register',
        'jadwal',
        'nomor_antrian',
        'status_register',
        'keluhan_pasien',
        'pasien_id',
        'jadwal_dokter_id',
        'poli_id',
    ];

    public function pasien()
    {
        return $this->belongsTo(Pasien::class);
    }

    public function jadwalDokter()
    {
        return $this->belongsTo(JadwalDokter::class);
    }

    public function poli()
    {
        return $this->belongsTo(Poli::class);
    }
}
