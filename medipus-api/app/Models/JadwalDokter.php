<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class JadwalDokter extends Model
{
    use HasFactory;

    protected $table = 'jadwal_dokters';

    protected $fillable = [
        'hari',
        'jam_mulai',
        'jam_selesai',
        'dokter_id',
        'ruangan_id',
    ];

    public function dokter()
    {
        return $this->belongsTo(Dokter::class);
    }

    public function ruangan()
    {
        return $this->belongsTo(Ruangan::class);
    }

    public function register()
    {
        return $this->hasMany(Register::class);
    }
}
