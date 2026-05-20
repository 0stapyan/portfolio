#ifndef TEAM_HPP
#define TEAM_HPP

#include <string>
#include <vector>
#include <string_view>
#include "Player.hpp"

class Team {
public:
    explicit Team(std::string name, std::string ageCategory);

    void addPlayer(const Player& player);
    void removePlayer(std::string_view playerName);

    void updateStats(int scored, int conceded);

    [[nodiscard]] std::string_view getName() const noexcept;
    [[nodiscard]] std::string_view getCategory() const noexcept;

    [[nodiscard]] int getPoints() const noexcept;
    [[nodiscard]] int getGoalsFor() const noexcept;
    [[nodiscard]] int getGoalsAgainst() const noexcept;
    [[nodiscard]] int getGoalDifference() const noexcept;

private:
    std::string name;
    std::string ageCategory;
    std::vector<Player> players;

    int points = 0;
    int goalsFor = 0;
    int goalsAgainst = 0;
};

#endif // TEAM_HPP
