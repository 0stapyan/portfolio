#include <iostream>
#include "matrix-vector_multiplication.h"
#include "multiplication_parallel.h"

using namespace std;

int main() {
    int n = 500;
    int threadCount = 4;

    cout << "Running sequential matrix-vector multiplication..." << endl;
    runSequentialMultiplication(n);

    cout << "Running parallel matrix-vector multiplication with " << threadCount << " threads..." << endl;
    runParallelMultiplication(n, threadCount);

    return 0;
}
