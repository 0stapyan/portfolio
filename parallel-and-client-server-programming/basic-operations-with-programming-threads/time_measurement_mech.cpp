#include <iostream>
#include <chrono>
using namespace std;

void exampleFunction() {
    for (int i = 0; i < 1e7; ++i);
}

int time_measurement() {
    auto start = chrono::high_resolution_clock::now();

    exampleFunction();

    auto end = chrono::high_resolution_clock::now();

    auto duration = chrono::duration_cast<chrono::microseconds>(end - start);

    cout << "Execution time: " << duration.count() << " microseconds" << endl;

    return 0;
}