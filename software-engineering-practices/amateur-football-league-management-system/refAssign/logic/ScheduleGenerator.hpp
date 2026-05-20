#ifndef SCHEDULEGENERATOR_HPP
#define SCHEDULEGENERATOR_HPP

#include <vector>
#include <string>
#include "../models/Team.hpp"
#include "../models/Match.hpp"

class ScheduleGenerator {
public:
    static std::vector<Match> generateFixtures(
        const std::vector<Team*>& teams,
        const std::string& ageCategory,
        const std::string& startDate = "2025-09-01");
};

#endif // SCHEDULEGENERATOR_HPP
