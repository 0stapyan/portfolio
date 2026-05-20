#include "matrix-vector_multiplication.h"
#include <iostream>
#include <vector>
#include <random>
#include <chrono>

using namespace std;

vector<vector<int>> generateMatrix(int n) {
    vector<vector<int>> matrix(n, vector<int>(n));
    random_device rd;
    mt19937 gen(rd());
    uniform_int_distribution<> dis(1, 100);

    for (int i = 0; i < n; ++i) {
        for (int j = 0; j < n; ++j) {
            matrix[i][j] = dis(gen);
        }
    }
    return matrix;
}

vector<int> generateVector(int n) {
    vector<int> vec(n);
    random_device rd;
    mt19937 gen(rd());
    uniform_int_distribution<> dis(1, 100);

    for (int i = 0; i < n; ++i) {
        vec[i] = dis(gen);
    }
    return vec;
}

vector<int> multiplyMatrixVector(const vector<vector<int>>& matrix, const vector<int>& vec) {
    int n = matrix.size();
    vector<int> result(n, 0);

    for (int i = 0; i < n; ++i) {
        for (int j = 0; j < n; ++j) {
            result[i] += matrix[i][j] * vec[j];
        }
    }
    return result;
}

void runSequentialMultiplication(int n) {
    auto matrix = generateMatrix(n);
    auto vec = generateVector(n);

    auto start = chrono::high_resolution_clock::now();
    auto result = multiplyMatrixVector(matrix, vec);
    auto end = chrono::high_resolution_clock::now();

    auto duration = chrono::duration_cast<chrono::microseconds>(end - start);
    cout << "Execution time (sequential): " << duration.count() << " ms" << endl;
}
