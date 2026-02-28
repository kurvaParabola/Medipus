<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Factories\HasFactory;

class TindakanMedis extends Model
{
    use HasFactory;

    protected $table = 'tindakan_medis';

    protected $fillable = [
        'nama_tindakan_medis',
        'deskripsi_tindakan_medis',
        'biaya_tindakan_medis',
    ];

    public function polis()
    {
        return $this->belongsToMany(
            Poli::class,
            'poli_tindakans'
        );
    }

    public function pemeriksaans()
    {
        return $this->hasMany(Pemeriksaan::class);
    }
}
