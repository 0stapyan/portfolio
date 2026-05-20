#include "Player.hpp"

Player::Player(std::string name, std::string dateOfBirth, int jerseyNumber)
    : name(std::move(name)), dateOfBirth(std::move(dateOfBirth)), jerseyNumber(jerseyNumber) {}

std::string_view Player::getName() const noexcept {
    return name;
}

std::string_view Player::getDateOfBirth() const noexcept {
    return dateOfBirth;
}

int Player::getJerseyNumber() const noexcept {
    return jerseyNumber;
}
