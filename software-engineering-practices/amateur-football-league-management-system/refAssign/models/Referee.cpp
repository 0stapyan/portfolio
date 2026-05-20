#include "Referee.hpp"
#include <utility>

Referee::Referee(std::string name, int experience, std::string photoPath)
    : name(std::move(name)), experience(experience), photoPath(std::move(photoPath)) {}

void Referee::incrementExperience() {
    ++experience;
}

int Referee::getExperience() const noexcept {
    return experience;
}

std::string_view Referee::getName() const noexcept {
    return name;
}

std::string_view Referee::getPhotoPath() const noexcept {
    return photoPath;
}

bool Referee::canOfficiateCategory(std::string_view ageCategory) const {
    if (ageCategory == "U11") return experience < 10;
    if (ageCategory == "U12") return experience >= 10 && experience < 20;
    if (ageCategory == "U13") return experience >= 20 && experience < 30;
    if (ageCategory == "U14") return experience >= 30 && experience < 40;
    return experience >= 40;
}
