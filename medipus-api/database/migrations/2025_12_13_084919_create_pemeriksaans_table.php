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
        Schema::create('pemeriksaans', function (Blueprint $table) {
            $table->id();
            $table->text('diagnosa_dokter');
            $table->text('catatan_dokter');
            $table->date('tanggal_pemeriksaan');
            $table->string('tekanan_darah');
            $table->integer('denyut_nadi');
            $table->integer('suhu_badan');
            $table->integer('berat_badan');
            $table->integer('tinggi_badan');

            $table->foreignId('register_id')
                  ->constrained('registers')
                  ->cascadeOnDelete()
                  ->cascadeOnUpdate();

            $table->foreignId('dokter_id')
                  ->constrained('dokters')
                  ->cascadeOnDelete()
                  ->cascadeOnUpdate();

            $table->foreignId('tindakan_medis_id')
                  ->constrained('tindakan_medis')
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
        Schema::dropIfExists('pemeriksaans');
    }
};
