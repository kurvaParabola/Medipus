<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Factories\HasFactory;

class PoliTindakan extends Model
{
    use HasFactory;

    protected $table = 'poli_tindakans';

    protected $fillable = [
        'poli_id', 
        'tindakan_medis_id'
    ];


    public function poli() {
        return $this->belongsTo(Poli::class);
    }

    public function tindakan_medis()
    {
        return $this->belongsTo(TindakanMedis::class, 'tindakan_medis_id');
    }
    
}
