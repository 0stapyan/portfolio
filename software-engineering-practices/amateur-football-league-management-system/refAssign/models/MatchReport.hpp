#ifndef MATCHREPORT_HPP
#define MATCHREPORT_HPP

#include <string>
#include <vector>
#include <string_view>

class MatchReport {
public:
    MatchReport(int goalsA = 0, int goalsB = 0,
                std::vector<std::string> redCards = {},
                std::string comment = "");

    [[nodiscard]] int getGoalsTeamA() const noexcept;
    [[nodiscard]] int getGoalsTeamB() const noexcept;
    [[nodiscard]] std::string_view getComment() const noexcept;
    [[nodiscard]] const std::vector<std::string>& getRedCards() const noexcept;

private:
    int goalsTeamA;
    int goalsTeamB;
    std::vector<std::string> redCards;
    std::string comment;
};

#endif // MATCHREPORT_HPP
