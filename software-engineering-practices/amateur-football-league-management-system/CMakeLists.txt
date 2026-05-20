cmake_minimum_required(VERSION 3.26)
project(final_project_0stapyan)

set(CMAKE_CXX_STANDARD 14)

include_directories(refAssign/logic)
include_directories(refAssign/models)

add_executable(final_project_0stapyan
        refAssign/logic/LeagueTable.cpp
        refAssign/logic/LeagueTable.hpp
        refAssign/logic/RefereeAssigner.cpp
        refAssign/logic/RefereeAssigner.hpp
        refAssign/logic/ScheduleGenerator.cpp
        refAssign/logic/ScheduleGenerator.hpp
        refAssign/models/Match.cpp
        refAssign/models/Match.hpp
        refAssign/models/MatchReport.cpp
        refAssign/models/MatchReport.hpp
        refAssign/models/Player.cpp
        refAssign/models/Player.hpp
        refAssign/models/Referee.cpp
        refAssign/models/Referee.hpp
        refAssign/models/Team.cpp
        refAssign/models/Team.hpp
        refAssign/tests/test_assigner.cpp
        refAssign/tests/test_schedule.cpp
        refAssign/tests/test_team.cpp
        refAssign/main.cpp
        refAssign/main_tests.cpp)
