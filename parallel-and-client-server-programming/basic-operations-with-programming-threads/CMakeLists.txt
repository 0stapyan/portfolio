cmake_minimum_required(VERSION 3.26)
project(parallel_assignment_1_0stapyan)

set(CMAKE_CXX_STANDARD 14)

add_executable(parallel_assignment_1_0stapyan
        matrix-vector_multiplication.cpp
        time_measurement_mech.cpp
        multiplication_parallel.cpp
        main.cpp)
