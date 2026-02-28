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
        Schema::create('obats', function (Blueprint $table) {
            $table->id();
            $table->string('kode_obat');
            $table->string('nama_obat');
            $table->string('satuan_obat');
            $table->enum('kategori', ['Bebas', 'Bebas Terbatas', 'Keras']);
            $table->integer('stok');
            $table->integer('harga_satuan');
            $table->date('kadaluarsa');
            $table->enum('lokasi_penyimpanan', ['Rak Obat Bebas', 'Rak Obat Bebat Terbatas', 'Rak Obat Keras', 'Kulkas']);
            $table->timestamps();
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('obats');
    }
};
