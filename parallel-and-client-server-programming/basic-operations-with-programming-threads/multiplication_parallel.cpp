#include "multiplication_parallel.h"
#include <iostream>
#include <vector>
#include <thread>
#include <random>
#include <chrono>

using namespace std;

void multiplyChunk(const vector<vector<int>>& matrix, const vector<int>& vec, vector<int>& result, int startRow, int endRow) {
    for (int i = startRow; i < endRow; ++i) {
        for (int j = 0; j < matrix.size(); ++j) {
            result[i] += matrix[i][j] * vec[j];
        }
    }
}

void runParallelMultiplication(int n, int threadCount) {
    vector<vector<int>> matrix(n, vector<int>(n));
    vector<int> vec(n), result(n, 0);
    random_device rd;
    mt19937 gen(rd());
    uniform_int_distribution<> dis(1, 100);

    for (int i = 0; i < n; ++i) {
        vec[i] = dis(gen);
        for (int j = 0; j < n; ++j) {
            matrix[i][j] = dis(gen);
        }
    }

    auto start = chrono::high_resolution_clock::now();

    vector<thread> threads;
    int chunkSize = n / threadCount;

    for (int t = 0; t < threadCount; ++t) {
        int startRow = t * chunkSize;
        int endRow = (t == threadCount - 1) ? n : startRow + chunkSize;
        threads.emplace_back(multiplyChunk, cref(matrix), cref(vec), ref(result), startRow, endRow);
    }

    for (auto& t : threads) {
        t.join();
    }

    auto end = chrono::high_resolution_clock::now();
    auto duration = chrono::duration_cast<chrono::microseconds >(end - start);
    cout << "Execution time (parallel, threads: " << threadCount << "): " << duration.count() << " ms" << endl;
}
