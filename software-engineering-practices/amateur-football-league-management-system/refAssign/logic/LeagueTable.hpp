#ifndef LEAGUETABLE_HPP
#define LEAGUETABLE_HPP

#include <string>
#include <vector>
#include <map>
#include "../models/Team.hpp"
#include "../models/Match.hpp"

class LeagueTable {
public:
    void addTeam(Team* team);
    void updateFromMatch(const Match& match);
    std::vector<Team*> getSortedTable(const std::string& category) const;

private:
    std::map<std::string, std::vector<Team*>> teamsByCategory;
};

#endif // LEAGUETABLE_HPP
