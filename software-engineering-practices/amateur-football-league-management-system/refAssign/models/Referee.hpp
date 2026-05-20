#ifndef REFEREE_HPP
#define REFEREE_HPP

#include <string>
#include <string_view>

class Referee {
public:
    explicit Referee(std::string name, int experience = 0, std::string photoPath = "");

    void incrementExperience();
    [[nodiscard]] int getExperience() const noexcept;
    [[nodiscard]] std::string_view getName() const noexcept;
    [[nodiscard]] std::string_view getPhotoPath() const noexcept;

    [[nodiscard]] bool canOfficiateCategory(std::string_view ageCategory) const;

private:
    std::string name;
    int experience;
    std::string photoPath;
};

#endif // REFEREE_HPP
