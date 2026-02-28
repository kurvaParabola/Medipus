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
        Schema::create('detail_reseps', function (Blueprint $table) {
            $table->id();

            $table->string('dosis_obat');
            $table->string('frekuensi_obat');
            $table->string('durasi_obat');
            $table->text('catatan_obat');

            $table->integer('jumlah_obat');
            $table->integer('subtotal_obat');

            $table->foreignId('resep_id')
                  ->constrained('reseps')
                  ->cascadeOnDelete()
                  ->cascadeOnUpdate();

            $table->foreignId('obat_id')
                  ->constrained('obats')
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
        Schema::dropIfExists('detail_reseps');
    }
};
