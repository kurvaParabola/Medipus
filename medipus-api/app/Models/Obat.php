<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class Obat extends Model
{
    use HasFactory;

    protected $table = 'obats';

    protected $fillable = [
        'kode_obat',
        'nama_obat',
        'satuan_obat',
        'kategori',
        'stok',
        'harga_satuan',
        'kadaluarsa',
        'lokasi_penyimpanan',
    ];

    public function detailReseps()
    {
        return $this->hasMany(DetailResep::class);
    }
}
