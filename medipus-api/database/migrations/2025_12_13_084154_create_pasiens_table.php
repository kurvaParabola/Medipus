<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    /**
     * Run the migrations.
     */
    public function up(): void
    {
        Schema::create('pasiens', function (Blueprint $table) {
            $table->id();
            $table->string('kode_pasien');
            $table->unsignedBigInteger('nik_pasien');
            $table->string('nama_pasien');
            $table->date('tanggal_lahir_pasien');
            $table->enum('jenis_kelamin_pasien', ['Laki-laki', 'Perempuan']);
            $table->text('alamat_pasien');
            $table->unsignedBigInteger('nomor_hp_pasien');
            $table->timestamps();
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('pasiens');
    }
};
