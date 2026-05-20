#include "Match.hpp"
#include <utility>

Match::Match(int id, Team* teamA, Team* teamB,
             std::string date, std::string time, std::string ageCategory)
    : id(id), teamA(teamA), teamB(teamB),
      date(std::move(date)), time(std::move(time)),
      ageCategory(std::move(ageCategory)) {}

void Match::assignReferee(Referee* ref) {
    referee = ref;
}

void Match::submitReport(const MatchReport& r) {
    report = r;
    played = true;

    teamA->updateStats(r.getGoalsTeamA(), r.getGoalsTeamB());
    teamB->updateStats(r.getGoalsTeamB(), r.getGoalsTeamA());

    if (referee) {
        referee->incrementExperience();
    }
}

int Match::getId() const noexcept { return id; }
Team* Match::getTeamA() const noexcept { return teamA; }
Team* Match::getTeamB() const noexcept { return teamB; }
Referee* Match::getReferee() const noexcept { return referee; }
bool Match::isPlayed() const noexcept { return played; }
std::string_view Match::getDate() const noexcept { return date; }
std::string_view Match::getTime() const noexcept { return time; }
std::string_view Match::getAgeCategory() const noexcept { return ageCategory; }
const MatchReport& Match::getReport() const { return report; }
