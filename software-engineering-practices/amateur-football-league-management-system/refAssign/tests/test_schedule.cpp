#include <iostream>
#include "../logic/ScheduleGenerator.hpp"

void test_schedule_generation() {
    Team* t1 = new Team("Lions", "U13");
    Team* t2 = new Team("Tigers", "U13");
    Team* t3 = new Team("Bears", "U13");

    std::vector<Team*> teams = {t1, t2, t3};

    std::vector<Match> matches = ScheduleGenerator::generateFixtures(teams, "U13");

    std::cout << "Schedule generated with " << matches.size() << " matches:\n";
    for (const auto& match : matches) {
        std::cout << "- " << match.getTeamA()->getName()
                  << " vs " << match.getTeamB()->getName()
                  << " on " << match.getDate()
                  << " at " << match.getTime() << "\n";
    }

    delete t1;
    delete t2;
    delete t3;
}
