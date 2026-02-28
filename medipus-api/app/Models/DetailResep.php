<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class DetailResep extends Model
{
    use HasFactory;

    protected $fillable = [
        'dosis_obat',
        'frekuensi_obat',
        'durasi_obat',
        'catatan_obat',
        'jumlah_obat',
        'subtotal_obat',
        'resep_id',
        'obat_id',
    ];

    public function resep()
    {
        return $this->belongsTo(Resep::class);
    }

    public function obat()
    {
       return $this->belongsTo(Obat::class, 'obat_id');
    }
}
