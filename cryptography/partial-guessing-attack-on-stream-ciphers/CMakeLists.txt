cmake_minimum_required(VERSION 3.26)
project(CryptographyStrymok)

set(CMAKE_CXX_STANDARD 14)

include_directories(.)

add_executable(CryptographyStrymok
        strumok_tables-1.h
        Strymok-1.cpp)
