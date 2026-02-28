<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class Transaksi extends Model
{
    use HasFactory;

    protected $fillable = [
        'biaya_tindakan',
        'biaya_obat',
        'total_biaya_transaksi',
        'tanggal_transaksi',
        'status_pembayaran',
        'pemeriksaan_id',
        'resep_id',
        'staff_id',
    ];

    public function pemeriksaan()
    {
        return $this->belongsTo(Pemeriksaan::class);
    }

    public function resep()
    {
        return $this->belongsTo(Resep::class);
    }

    public function staff()
    {
        return $this->belongsTo(Staff::class);
    }
}
