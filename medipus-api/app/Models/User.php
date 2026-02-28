<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Foundation\Auth\User as Authenticatable;
use Illuminate\Notifications\Notifiable;
use Laravel\Sanctum\HasApiTokens; // <-- Make sure this is imported

class User extends Authenticatable
{
    use HasApiTokens, HasFactory, Notifiable; // <-- Make sure HasApiTokens is used

    /**
     * The attributes that are mass assignable.
     *
     * Add your new fields here
     * 'status_user' is automatically set in the controller, but adding it won't hurt
     */
    protected $fillable = [
        'username',
        'email',
        'password',
        'role',
        'status_user',
        'nik_user',
        'nama_user',
        'tanggal_lahir_user',
        'jenis_kelamin_user',
        'alamat_user',
        'nomor_hp_user',
    ];

    /**
     * The attributes that should be hidden for serialization.
     */
    protected $hidden = [
        'password',
        'remember_token',
    ];

    /**
     * The attributes that should be cast.
     */
    protected $casts = [
        'email_verified_at' => 'datetime',
        'password' => 'hashed',
    ];

    public function dokter()
    {
        return $this->hasOne(Dokter::class);
    }
    
    public function staff()
    {
        return $this->hasOne(Staff::class);
    }

    public function apoteker()
    {
        return $this->hasOne(Apoteker::class);
    }

}