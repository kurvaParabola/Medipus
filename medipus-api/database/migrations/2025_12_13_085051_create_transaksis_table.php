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
        Schema::create('transaksis', function (Blueprint $table) {
            $table->id();
            $table->integer('biaya_tindakan');
            $table->integer('biaya_obat');
            $table->integer('total_biaya_transaksi');
            $table->date('tanggal_transaksi');
            $table->enum('status_pembayaran', ['Menunggu', 'Lunas','Batal']);

            $table->foreignId('pemeriksaan_id')
                  ->constrained('pemeriksaans')
                  ->cascadeOnDelete()
                  ->cascadeOnUpdate();

            $table->foreignId('resep_id')
                  ->constrained('reseps')
                  ->cascadeOnDelete()
                  ->cascadeOnUpdate();

            $table->foreignId('staff_id')
                  ->constrained('staff')
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
        Schema::dropIfExists('transaksis');
    }
};
