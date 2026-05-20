#include "Team.hpp"
#include <algorithm>
#include <utility>

Team::Team(std::string name, std::string ageCategory)
    : name(std::move(name)), ageCategory(std::move(ageCategory)) {}

void Team::addPlayer(const Player& player) {
    players.push_back(player);
}

void Team::removePlayer(std::string_view playerName) {
    players.erase(std::remove_if(players.begin(), players.end(),
        [&](const Player& p) {
            return p.getName() == playerName;
        }), players.end());
}

void Team::updateStats(int scored, int conceded) {
    goalsFor += scored;
    goalsAgainst += conceded;

    if (scored > conceded) {
        points += 3;
    } else if (scored == conceded) {
        points += 1;
    }
    // 0 points for loss
}

std::string_view Team::getName() const noexcept {
    return name;
}

std::string_view Team::getCategory() const noexcept {
    return ageCategory;
}

int Team::getPoints() const noexcept {
    return points;
}

int Team::getGoalsFor() const noexcept {
    return goalsFor;
}

int Team::getGoalsAgainst() const noexcept {
    return goalsAgainst;
}

int Team::getGoalDifference() const noexcept {
    return goalsFor - goalsAgainst;
}
