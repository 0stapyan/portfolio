#include "LeagueTable.hpp"
#include "../models/Match.hpp"
#include "../models/Team.hpp"
#include <algorithm>

void LeagueTable::addTeam(Team* team) {
    teamsByCategory[std::string(team->getCategory())].push_back(team);
}

void LeagueTable::updateFromMatch(const Match& match) {
    if (!match.isPlayed()) return;

    // Points already updated inside Match::submitReport
    // We just need to keep the reference to teams correct
}

std::vector<Team*> LeagueTable::getSortedTable(const std::string& category) const {
    auto it = teamsByCategory.find(category);
    if (it == teamsByCategory.end()) return {};

    std::vector<Team*> sorted = it->second;

    std::sort(sorted.begin(), sorted.end(), [](Team* a, Team* b) {
        if (a->getPoints() != b->getPoints())
            return a->getPoints() > b->getPoints();
        if (a->getGoalDifference() != b->getGoalDifference())
            return a->getGoalDifference() > b->getGoalDifference();
        return a->getGoalsFor() > b->getGoalsFor();
    });

    return sorted;
}
