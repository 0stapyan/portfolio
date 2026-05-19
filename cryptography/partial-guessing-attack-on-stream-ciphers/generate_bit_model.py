def generate_bit_model(filename="strumok_model_448_bits.txt", rounds=11):
    with open(filename, "w") as f:
        f.write("connection relations\n\n")

        for t in range(rounds):
            # Слово S[t+16] = S[t+13] ^ S[t+11] ^ S[t]
            for b in range(64):
                f.write(f"S_{t+16}_{b} S_{t+13}_{b} S_{t+11}_{b} S_{t}_{b}\n")
            
            # Слово Z[t] = S[t+15] ^ R[t+1] ^ R[t] ^ S[t]
            for b in range(64):
                f.write(f"Z_{t}_{b} S_{t+15}_{b} R_{t+1}_{b} R_{t}_{b} S_{t}_{b}\n")
            
            # R[t+2] = S[t+13] ^ R[t]
            for b in range(64):
                f.write(f"R_{t+2}_{b} S_{t+13}_{b} R_{t}_{b}\n")

            f.write("\n")

        # Декларація всіх Z_t_b, навіть якщо не використовуються у зв’язках
        for t in range(rounds):
            for b in range(64):
                f.write(f"Z_{t}_{b} Z_{t}_{b}\n")

        # Відомі значення: Autoguess сам розширює на 64 біти
        f.write("\nknown\n\n")
        for t in range(rounds):
            f.write(f"Z_{t}\n")

        f.write("\nend\n")


# ▶️ Запуск
generate_bit_model()
