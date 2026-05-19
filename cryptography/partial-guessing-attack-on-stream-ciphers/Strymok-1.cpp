#include <iostream>
#include "strumok_tables-1.h"

#include <cstdint>
#include <vector>
#include <array>
#include <stdexcept>
#include <cstring>
#include <iomanip>
#include <fstream>

constexpr size_t KEY_LENGTH_256 = 4;  // 256-bit key
constexpr size_t KEY_LENGTH_512 = 8;  // 512-bit key
constexpr size_t IVECTOR_SIZE = 4;    // 256-bit initialization vector
constexpr size_t LFSR_SIZE = 16;       // LFSR length

// Enum for key size
enum class KeySize {
    KEY_LENGTH_256,
    KEY_LENGTH_512
};

void printVectorAsHex(const std::vector<uint64_t>& data) {
    for (uint64_t c : data) {
        // Print each 64-bit word in hexadecimal format
        std::cout << "0x" << std::hex << std::setw(16) << std::setfill('0') << c << " ";
    }
    std::cout << std::endl;
}

void printVectorAsString(const std::vector<uint64_t>& data) {
    for (uint64_t c : data) {
        std::cout << c;
    }
    std::cout << std::endl;
}

// Strymok stream cipher class
class Strymok {
public:

    Strymok(const std::vector<uint64_t>& key, const std::vector<uint64_t>& ivector, KeySize key_size) {
        initialize(key, ivector, key_size);
    }

    std::vector<uint64_t> Strm(size_t num_output_words) {
        std::vector<uint64_t> Z;

        for (size_t i = 0; i < num_output_words; i++) {

            uint64_t Zi = FSM(state_[0], fsr_[1], fsr_[0]) ^ state_[15];

            Z.push_back(Zi);
            Next();
        }
        return Z;
    }

private:
    std::vector<uint64_t> state_; 
    std::vector<uint64_t> fsr_;
    std::vector<uint64_t> key_;
    std::vector<uint64_t> ivector_;
    KeySize key_size_;

    uint64_t FSM(uint64_t x, uint64_t y, uint64_t z) {
        return (x + y) ^ z;
    }

    uint64_t a_mul(uint64_t x) {
        return (((x) << 8) ^ (strumok_alpha_mul[x >> 56]));
    }

    uint64_t ainv_mul(const uint64_t x) {
        return (((x) >> 8) ^ (strumok_alphainv_mul[x & 0xff]));
    }

    uint64_t transform_T(const uint64_t x) {
        return ((strumok_T0[x & 0xff]) ^
            (strumok_T1[(x >> 8) & 0xff]) ^
            (strumok_T2[(x >> 16) & 0xff]) ^
            (strumok_T3[(x >> 24) & 0xff]) ^
            (strumok_T4[(x >> 32) & 0xff]) ^
            (strumok_T5[(x >> 40) & 0xff]) ^
            (strumok_T6[(x >> 48) & 0xff]) ^
            (strumok_T7[(x >> 56) & 0xff]));
    }

    void initialize(const std::vector<uint64_t>& key, const std::vector<uint64_t>& ivector, KeySize key_size) {
        // Validate key size
        if (key_size != KeySize::KEY_LENGTH_256 && key_size != KeySize::KEY_LENGTH_512) {
            throw std::invalid_argument("Invalid key size");
        }

        // Validate key and ivector sizes
        if ((key_size == KeySize::KEY_LENGTH_256 && key.size() != KEY_LENGTH_256) ||
            (key_size == KeySize::KEY_LENGTH_512 && key.size() != KEY_LENGTH_512)) {
            throw std::invalid_argument("Invalid key size for the provided key");
        }

        if (ivector.size() != IVECTOR_SIZE) {
            throw std::invalid_argument("Invalid ivector size");
        }

        key_size_ = key_size;
        key_ = key;
        ivector_ = ivector;

        // Initialize the state
        state_.resize(LFSR_SIZE, 0);
        fsr_.resize(2, 0);

        // Perform mixing based on key size
        if (key_size == KeySize::KEY_LENGTH_256) {
            mixState256();
        }
        else if (key_size == KeySize::KEY_LENGTH_512) {
            mixState512();
        }

        for (int i = 0; i < 32; i++) { 
            Next(true);
        }
        Next();
    }

    void Next(bool IsInit = false) {
        
        uint64_t new_r0 = transform_T(fsr_[1]);
        uint64_t new_r1 = fsr_[0] + state_[2];

        uint64_t tempfsr1 = fsr_[1];
        uint64_t tempfsr0 = fsr_[0];

        fsr_[1] = new_r1;
        fsr_[0] = new_r0;
                
        uint64_t temp0 = state_[0];
        uint64_t temp15 = state_[15];
        uint64_t temp4 = state_[4];
        uint64_t temp2 = state_[2];

        for (size_t i = LFSR_SIZE - 1; i > 0; i--) {
            state_[i] = state_[i - 1];
        }

        if (IsInit) {
            uint64_t s0_new = FSM(temp0, tempfsr1, tempfsr0) ^ a_mul(temp15) ^ ainv_mul(temp4) ^ temp2;
            state_[0] = s0_new;
        }
        else {
            uint64_t s0_new = a_mul(temp15) ^ ainv_mul(temp4) ^ temp2;
            state_[0] = s0_new;
        }
    }

    void mixState256() {
        state_[0] = ~key_[3];
        state_[1] = key_[2];
        state_[2] = ~key_[1];
        state_[3] = key_[0];
        state_[4] = key_[3];
        state_[5] = ~key_[2];
        state_[6] = key_[1];
        state_[7] = key_[0];
        state_[8] = ~key_[3];
        state_[9] = ~key_[2];
        state_[10] = key_[1] ^ ivector_[0];
        state_[11] = key_[0];
        state_[12] = key_[3] ^ ivector_[1];
        state_[13] = key_[2] ^ ivector_[2];
        state_[14] = key_[1];
        state_[15] = key_[0] ^ ivector_[3];
    }

    void mixState512() {
        state_[0] = key_[7];
        state_[1] = ~key_[6];
        state_[2] = key_[5];
        state_[3] = key_[4];
        state_[4] = ~key_[0];
        state_[5] = key_[2];
        state_[6] = ~key_[1];
        state_[7] = key_[3] ^ ivector_[0];
        state_[8] = ~key_[7];
        state_[9] = key_[6];
        state_[10] = key_[5] ^ ivector_[1];
        state_[11] = key_[4];
        state_[12] = key_[3] ^ ivector_[2];
        state_[13] = key_[2];
        state_[14] = key_[1];
        state_[15] = key_[0] ^ ivector_[3];
    }
};

int main() {
    std::vector<uint64_t> key = { 0x8000000000000000 , 0x0000000000000000 , 0x0000000000000000 , 0x0000000000000000 };
    std::vector<uint64_t> ivector = { 0x0000000000000000 , 0x0000000000000000 , 0x0000000000000000 , 0x0000000000000000 };

    Strymok strymok(key, ivector, KeySize::KEY_LENGTH_256); // ініціалізація шифру

    auto stream = strymok.Strm(8); // отримуємо 8 64-бітних слів

    // Зберігаємо результат у файл
    std::ofstream out("keystream.bin", std::ios::binary);
    for (const auto& word : stream) {
        for (int i = 0; i < 8; i++) {
            uint8_t byte = (word >> (8 * (7 - i))) & 0xFF;
            out.write(reinterpret_cast<const char*>(&byte), sizeof(byte));
        }
    }
    out.close();

    return 0;
}
