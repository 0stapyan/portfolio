#include "tests/test_team.cpp"
#include "tests/test_schedule.cpp"
#include "tests/test_assigner.cpp"

int main() {
    test_team_creation_and_basic_info();
    test_team_update_stats();
    test_schedule_generation();
    test_referee_assigning();
    return 0;
}
