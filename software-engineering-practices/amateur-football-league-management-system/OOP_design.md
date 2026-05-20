# OOP Design of the application

**Base clases**:

```cpp

1. Team
std::string name
std::string ageCategory // U11–U19
std::vector<Player> players
int points, int goalsFor, int goalsAgainst, int goalDifference

// Methods:

void addPlayer(const Player& p)
void removePlayer(std::string playerName)
void updateStats(int scored, int conceded)


2. Player
std::string name
std::string dateOfBirth
int jerseyNumber


3. Referee
std::string name
int experience // number of matches
std::string photoPath

// Methods:

void incrementExperience()
bool canOfficiateCategory(const std::string& ageCategory)


4. Match
int id
Team* teamA
Team* teamB
std::string ageCategory
std::string date
std::string time
Referee* referee (nullable)
bool isPlayed
MatchReport report

// Methods:

void assignReferee(Referee* r)
void submitReport(const MatchReport& report)
bool hasTimeConflict(const Match& other) const


5. MatchReport
int goalsTeamA
int goalsTeamB
std::vector<std::string> redCards
std::string comment


6. ScheduleGenerator

// Methods:

std::vector<Match> generateFixtures(const std::vector<Team>& teamsByCategory)


7. RefereeAssigner

// Methods:

void assignReferees(std::vector<Match>& matches, std::vector<Referee>& referees)


8. LeagueTable

std::map<std::string, std::vector<Team>> tableByCategory

// Methods:

void updateAfterMatch(const Match& match)
std::vector<Team> getTableForCategory(const std::string& ageCategory)


9. User
// Base class for authorization

std::string username, std::string role (admin, coach, referee, player)

// Derived classes:

Admin : public User
Coach : public User
RefereeUser : public User → прив’язаний до Referee
PlayerUser : public User → прив’язаний до Team


10. AccessController

// Methods:

bool canViewMatch(User* user, const Match& match)
bool canEditTeam(User* user, const Team& team)
bool canSubmitReport(User* user, const Match& match)
bool canEditMatch(User* user, const Match& match)
```

## Also there is a UML diagram inside the project

### How does this cover P0 and P1:

- All CRUD functionality, generation, assignment, access — distributed across classes
- APIs are logically structured — easy to write unit tests
- Accessibility and control are implemented through AccessController and role model


**To build and execute:** 

```bash
clang++ -std=c++23 -Wall -Werror \
  main.cpp \
  models/Player.cpp models/Team.cpp models/Referee.cpp \
  models/Match.cpp models/MatchReport.cpp \
  logic/ScheduleGenerator.cpp logic/RefereeAssigner.cpp logic/LeagueTable.cpp \
  -o refassign
  ./refassign
```
...
