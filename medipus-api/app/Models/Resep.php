<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class Resep extends Model
{
    use HasFactory;

    protected $fillable = [
        'status_resep',
        'tanggal_resep',
        'pemeriksaan_id',
    ];

    public function pemeriksaan()
    {
        return $this->belongsTo(Pemeriksaan::class);
    }

    public function detail_reseps()
    {
        return $this->hasMany(DetailResep::class, 'resep_id', 'id');
    }
}
