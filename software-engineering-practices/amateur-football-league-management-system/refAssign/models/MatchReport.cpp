#include "MatchReport.hpp"
#include <utility>

MatchReport::MatchReport(int goalsA, int goalsB,
                         std::vector<std::string> redCards,
                         std::string comment)
    : goalsTeamA(goalsA),
      goalsTeamB(goalsB),
      redCards(std::move(redCards)),
      comment(std::move(comment)) {}

int MatchReport::getGoalsTeamA() const noexcept {
    return goalsTeamA;
}

int MatchReport::getGoalsTeamB() const noexcept {
    return goalsTeamB;
}

std::string_view MatchReport::getComment() const noexcept {
    return comment;
}

const std::vector<std::string>& MatchReport::getRedCards() const noexcept {
    return redCards;
}
