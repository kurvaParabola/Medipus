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
        Schema::create('biodata_users', function (Blueprint $table) {
            $table->id();
            $table->unsignedBigInteger('nik_user');
            $table->string('nama_user');
            $table->date('tanggal_lahir_user');
            $table->enum('jenis_kelamin_user', ['Laki-laki', 'Perempuan']);
            $table->text('alamat_user');

            $table->foreignId('user_id') 
                  ->constrained('users')
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
        Schema::dropIfExists('biodata_users');
    }
};
