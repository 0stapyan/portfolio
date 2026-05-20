#include <iostream>
#include "../logic/ScheduleGenerator.hpp"
#include "../logic/RefereeAssigner.hpp"

void test_referee_assigning() {
    Team* t1 = new Team("Lions", "U13");
    Team* t2 = new Team("Tigers", "U13");

    std::vector<Team*> teams = {t1, t2};
    std::vector<Match> matches = ScheduleGenerator::generateFixtures(teams, "U13");

    Referee* r1 = new Referee("Alice", 2);
    Referee* r2 = new Referee("Bob", 3);
    std::vector<Referee*> referees = {r1, r2};

    RefereeAssigner::assignReferees(matches, referees);

    std::cout << "Assigned referees:\n";
    for (const auto& match : matches) {
        std::string refName = match.getReferee() ? std::string(match.getReferee()->getName()) : "None";
        std::cout << "- Match " << match.getId()
                  << ": " << match.getTeamA()->getName()
                  << " vs " << match.getTeamB()->getName()
                  << " | Referee: " << refName << "\n";
    }

    delete t1;
    delete t2;
    delete r1;
    delete r2;
}
