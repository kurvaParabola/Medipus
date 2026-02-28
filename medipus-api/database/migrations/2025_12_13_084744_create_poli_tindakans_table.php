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
        Schema::create('poli_tindakans', function (Blueprint $table) {
            $table->id();

            $table->foreignId('poli_id')
                  ->constrained('polis')
                  ->cascadeOnDelete()
                  ->cascadeOnUpdate();

            $table->foreignId('tindakan_medis_id')
                  ->constrained('tindakan_medis')
                  ->cascadeOnDelete()
                  ->cascadeOnUpdate();
                  
            $table->unique(['poli_id', 'tindakan_medis_id']);
            
            $table->timestamps();
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('poli_tindakans');
    }
};
