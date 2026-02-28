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
        Schema::create('registers', function (Blueprint $table) {
            $table->id();
            $table->date('tanggal_register');
            $table->time('jadwal');
            $table->string('nomor_antrian');
            $table->enum('status_register', ['Menunggu', 'Diperiksa', 'Selesai']);
            $table->text('keluhan_pasien');

            $table->foreignId('pasien_id')
                  ->constrained('pasiens')
                  ->cascadeOnDelete()
                  ->cascadeOnUpdate();

            $table->foreignId('jadwal_dokter_id')
                  ->constrained('jadwal_dokters')
                  ->cascadeOnDelete()
                  ->cascadeOnUpdate();

            $table->foreignId('poli_id')
                  ->constrained('polis')
                  ->cascadeOnDelete()
                  ->cascadeOnUpdate();

            $table->timestamps();
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('registers');
    }
};
