#include <iostream>
#include "models/Player.hpp"
#include "models/Team.hpp"
#include "models/Referee.hpp"
#include "models/Match.hpp"
#include "models/MatchReport.hpp"
#include "logic/ScheduleGenerator.hpp"
#include "logic/RefereeAssigner.hpp"
#include "logic/LeagueTable.hpp"

int main() {
    // --- Створюємо команди ---
    Team team1("Sharks", "U13");
    Team team2("Lions", "U13");
    Team team3("Eagles", "U13");

    // --- Створюємо арбітрів ---
    Referee ref1("Alice", 12);
    Referee ref2("Bob", 25);
    std::vector<Referee*> referees = { &ref1, &ref2 };

    // --- Формуємо календар матчів ---
    std::vector<Team*> teams = { &team1, &team2, &team3 };
    std::vector<Match> matches = ScheduleGenerator::generateFixtures(teams, "U13");

    // --- Призначаємо арбітрів ---
    RefereeAssigner::assignReferees(matches, referees);

    // --- Симулюємо рапорти ---
    for (size_t i = 0; i < matches.size(); ++i) {
        int goalsA = static_cast<int>(i % 4);
        int goalsB = static_cast<int>((i + 1) % 3);
        MatchReport report(goalsA, goalsB, {}, "Fair game");
        matches[i].submitReport(report);
    }

    // --- Формуємо турнірну таблицю ---
    LeagueTable table;
    for (Team* t : teams) {
        table.addTeam(t);
    }

    std::vector<Team*> sorted = table.getSortedTable("U13");

    // --- Виводимо таблицю ---
    std::cout << "=== League Table U13 ===\n";
    for (const Team* team : sorted) {
        std::cout << team->getName() << " | Points: " << team->getPoints()
                  << " | GD: " << team->getGoalDifference()
                  << " | GF: " << team->getGoalsFor() << '\n';
    }

    return 0;
}
