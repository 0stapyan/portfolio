def generate_autoguess_model(filename="strumok_model_autoguess.txt", rounds=11):
    with open(filename, "w") as f:
        f.write("# Strumok-512 cipher model for Autoguess\n\n")
        
        # Секція variables для визначення всіх змінних
        f.write("variables\n\n")
        
        # LFSR стан (S регістри)
        for i in range(16 + rounds):  # Початковий стан + додаткові раунди
            f.write(f"S_{i}\n")
        
        # FSM стан (R регістри)
        for i in range(2 + rounds):  # Початковий стан R_0, R_1 + додаткові
            f.write(f"R_{i}\n")
        
        # Вихідний потік Z
        for i in range(rounds):
            f.write(f"Z_{i}\n")
        
        # Проміжні значення для нелінійної функції T та множення на alpha
        for i in range(rounds):
            f.write(f"T_R_{i}\n")  # Результат T(R_i)
            f.write(f"AMUL_R_{i}\n")  # Результат a_mul(R_i)
        
        # Секція constraints для визначення зв'язків
        f.write("\nconstraints\n\n")
        
        for t in range(rounds):
            # 1. Оновлення LFSR: S[t+16] = S[t+13] ^ S[t+11] ^ a_mul(S[t])
            # Спрощено до XOR для початку
            f.write(f"S_{t+16} = S_{t+13} ^ S_{t+11} ^ S_{t}\n")
            
            # 2. Оновлення FSM
            # R[t+2] = S[t+15] ^ T(R[t])
            f.write(f"T_R_{t} = T(R_{t})\n")  # Функція T
            f.write(f"R_{t+2} = S_{t+15} ^ T_R_{t}\n")
            
            # 3. Вихідний потік
            # Z[t] = S[t+15] ^ R[t+1] ^ a_mul(R[t]) ^ S[t]
            f.write(f"AMUL_R_{t} = a_mul(R_{t})\n")  # Множення на alpha
            f.write(f"Z_{t} = S_{t+15} ^ R_{t+1} ^ AMUL_R_{t} ^ S_{t}\n")
        
        # Секція known для визначення відомих значень
        f.write("\nknown\n\n")
        for t in range(rounds):
            f.write(f"Z_{t}\n")
        
        f.write("\nend\n")

# Запуск
generate_autoguess_model()