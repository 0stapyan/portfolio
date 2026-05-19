def generate_autoguess_file(rounds=11):
    with open("strumok_model_448_autogen.txt", "w") as f:
        f.write("#strumok {} Rounds\n\n".format(rounds))
        f.write("connection relations\n\n")
        for t in range(rounds):
            f.write("S_{} S_{} S_{} S_{}\n".format(t + 16, t + 13, t + 11, t))
            f.write("Z_{} S_{} R_{} R_{} S_{}\n".format(t, t + 15, t + 1, t, t))
            f.write("R_{} S_{} R_{}\n\n".format(t + 2, t + 13, t))
        f.write("known\n\n")
        for t in range(rounds):
            f.write("Z_{}\n".format(t))
        f.write("\nend\n")

# Запуск функції
generate_autoguess_file()
