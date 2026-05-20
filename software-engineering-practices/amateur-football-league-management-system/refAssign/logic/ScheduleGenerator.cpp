#include "ScheduleGenerator.hpp"
#include <chrono>
#include <iomanip>
#include <sstream>

// Для генерації дати наступного тижня
std::string addDays(const std::string& dateStr, int daysToAdd) {
    std::tm tm{};
    std::istringstream ss(dateStr);
    ss >> std::get_time(&tm, "%Y-%m-%d");
    std::time_t t = std::mktime(&tm);
    t += daysToAdd * 86400;
    std::tm* newTm = std::localtime(&t);
    std::ostringstream out;
    out << std::put_time(newTm, "%Y-%m-%d");
    return out.str();
}

std::vector<Match> ScheduleGenerator::generateFixtures(
    const std::vector<Team*>& teams,
    const std::string& ageCategory,
    const std::string& startDate
) {
    std::vector<Match> fixtures;
    int matchId = 1;
    int daysBetweenRounds = 7;
    int currentDayOffset = 0;

    for (size_t i = 0; i < teams.size(); ++i) {
        for (size_t j = i + 1; j < teams.size(); ++j) {
            std::string date = addDays(startDate, currentDayOffset);
            std::string time = "15:00";
            fixtures.emplace_back(matchId++, teams[i], teams[j], date, time, ageCategory);

            // Optional: змінювати дату кожного 5-го матчу
            if (fixtures.size() % 5 == 0) {
                currentDayOffset += daysBetweenRounds;
            }
        }
    }

    return fixtures;
}
