#include "RefereeAssigner.hpp"
#include <algorithm>
#include <map>

void RefereeAssigner::assignReferees(std::vector<Match>& matches, std::vector<Referee*>& referees) {
    std::unordered_map<Referee*, int> refereeWorkload;

    for (auto* ref : referees) {
        refereeWorkload[ref] = 0;
    }

    for (auto& match : matches) {
        Referee* selected = nullptr;
        int minMatches = INT_MAX;

        for (auto* ref : referees) {
            if (!ref->canOfficiateCategory(match.getAgeCategory())) {
                continue;
            }

            int count = refereeWorkload[ref];
            if (count < minMatches) {
                selected = ref;
                minMatches = count;
            }
        }

        if (selected) {
            match.assignReferee(selected);
            refereeWorkload[selected]++;
        }
    }
}
