#include "../models/Team.hpp"
#include "../models/Player.hpp"
#include <cassert>
#include <iostream>

void test_team_creation_and_basic_info() {
    Team team("Lions", "U13");

    assert(team.getName() == "Lions");
    assert(team.getCategory() == "U13");

    assert(team.getPoints() == 0);
    assert(team.getGoalsFor() == 0);
    assert(team.getGoalsAgainst() == 0);
    assert(team.getGoalDifference() == 0);

    std::cout << "✅ test_team_creation_and_basic_info passed.\n";
}

void test_team_update_stats() {
    Team team("Tigers", "U15");

    team.updateStats(3, 1); // Перемога
    assert(team.getGoalsFor() == 3);
    assert(team.getGoalsAgainst() == 1);
    assert(team.getGoalDifference() == 2);
    assert(team.getPoints() == 3);

    team.updateStats(2, 2); // Нічия
    assert(team.getPoints() == 4);
    assert(team.getGoalsFor() == 5);
    assert(team.getGoalsAgainst() == 3);
    assert(team.getGoalDifference() == 2);

    team.updateStats(0, 1); // Поразка
    assert(team.getPoints() == 4);
    assert(team.getGoalsFor() == 5);
    assert(team.getGoalsAgainst() == 4);
    assert(team.getGoalDifference() == 1);

    std::cout << "✅ test_team_update_stats passed.\n";
}
