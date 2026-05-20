#ifndef REFEREEASSIGNER_HPP
#define REFEREEASSIGNER_HPP

#include <vector>
#include <unordered_map>
#include "../models/Referee.hpp"
#include "../models/Match.hpp"

class RefereeAssigner {
public:
    static void assignReferees(std::vector<Match>& matches, std::vector<Referee*>& referees);
};

#endif // REFEREEASSIGNER_HPP
