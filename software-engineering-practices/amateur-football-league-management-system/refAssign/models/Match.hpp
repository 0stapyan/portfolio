#ifndef MATCH_HPP
#define MATCH_HPP

#include <string>
#include <memory>
#include "Team.hpp"
#include "Referee.hpp"
#include "MatchReport.hpp"

class Match {
public:
    Match(int id, Team* teamA, Team* teamB,
          std::string date, std::string time, std::string ageCategory);

    void assignReferee(Referee* ref);
    void submitReport(const MatchReport& report);

    [[nodiscard]] int getId() const noexcept;
    [[nodiscard]] Team* getTeamA() const noexcept;
    [[nodiscard]] Team* getTeamB() const noexcept;
    [[nodiscard]] Referee* getReferee() const noexcept;
    [[nodiscard]] bool isPlayed() const noexcept;
    [[nodiscard]] std::string_view getDate() const noexcept;
    [[nodiscard]] std::string_view getTime() const noexcept;
    [[nodiscard]] std::string_view getAgeCategory() const noexcept;
    [[nodiscard]] const MatchReport& getReport() const;

private:
    int id;
    Team* teamA;
    Team* teamB;
    Referee* referee = nullptr;
    std::string date;
    std::string time;
    std::string ageCategory;
    MatchReport report;
    bool played = false;
};

#endif // MATCH_HPP
