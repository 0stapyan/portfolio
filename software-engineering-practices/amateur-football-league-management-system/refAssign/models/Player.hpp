#ifndef PLAYER_HPP
#define PLAYER_HPP

#include <string>
#include <string_view>

class Player {
public:
    explicit Player(std::string name, std::string dateOfBirth, int jerseyNumber);

    [[nodiscard]] std::string_view getName() const noexcept;
    [[nodiscard]] std::string_view getDateOfBirth() const noexcept;
    [[nodiscard]] int getJerseyNumber() const noexcept;

private:
    std::string name;
    std::string dateOfBirth;
    int jerseyNumber;
};

#endif // PLAYER_HPP
