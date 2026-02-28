<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class Pemeriksaan extends Model
{
    use HasFactory;

    protected $fillable = [
        'diagnosa_dokter',
        'catatan_dokter',
        'tanggal_pemeriksaan',
        'tekanan_darah',
        'denyut_nadi',
        'suhu_badan',
        'berat_badan',
        'tinggi_badan',
        'register_id',
        'dokter_id',
        'tindakan_medis_id',
    ];

    public function register()
    {
        return $this->belongsTo(Register::class, 'register_id');
    }

    public function dokter()
    {
        return $this->belongsTo(Dokter::class);
    }

    public function tindakanMedis()
    {
        return $this->belongsTo(TindakanMedis::class, 'tindakan_medis_id');
    }
}
