#include <vector>
#include <iostream>
#include <cstdlib>
#include <iostream>
#include <ctime>
using namespace std;

class Cell {
private:
    bool isMine;
    bool isRevealed;
    bool isFlagged;
    int adjacentMinesCount;

public:
    Cell() : isMine(false), isRevealed(false), isFlagged(false), adjacentMinesCount(0) {}

    void setMine() { isMine = true; }
    bool hasMine() const { return isMine; }

    void reveal() { isRevealed = true; }
    bool isCellRevealed() const { return isRevealed; }

    void toggleFlag() { isFlagged = !isFlagged; }
    bool isCellFlagged() const { return isFlagged; }

    void setAdjacentMines(int count) { adjacentMinesCount = count; }
    int getAdjacentMines() const { return adjacentMinesCount; }
};

class Board {
private:
    int width, height, mineCount;
    vector<vector<Cell>> cells;

    void placeMines() {
        int placedMines = 0;
        while (placedMines < mineCount) {
            int row = rand() % height;
            int col = rand() % width;
            if (!cells[row][col].hasMine()) {
                cells[row][col].setMine();
                placedMines++;
            }
        }
    }

    void calculateAdjacentMines() {
        for (int row = 0; row < height; row++) {
            for (int col = 0; col < width; col++) {
                if (cells[row][col].hasMine()) continue;

                int mineCount = 0;
                for (int dr = -1; dr <= 1; dr++) {
                    for (int dc = -1; dc <= 1; dc++) {
                        int r = row + dr, c = col + dc;
                        if (r >= 0 && r < height && c >= 0 && c < width && cells[r][c].hasMine()) {
                            mineCount++;
                        }
                    }
                }
                cells[row][col].setAdjacentMines(mineCount);
            }
        }
    }

public:
    Board(int w, int h, int m) : width(w), height(h), mineCount(m) {
        cells.resize(height, vector<Cell>(width));
        placeMines();
        calculateAdjacentMines();
    }

    Cell& getCell(int row, int col) { return cells[row][col]; }

    void display(bool revealAll = false) const {
        for (int row = 0; row < height; row++) {
            for (int col = 0; col < width; col++) {
                const Cell& cell = cells[row][col];
                if (revealAll || cell.isCellRevealed()) {
                    if (cell.hasMine()) {
                        cout << "* ";
                    } else {
                        cout << cell.getAdjacentMines() << " ";
                    }
                } else if (cell.isCellFlagged()) {
                    cout << "F ";
                } else {
                    cout << "- ";
                }
            }
            cout << endl;
        }
    }

    bool isValid(int row, int col) const {
        return row >= 0 && row < height && col >= 0 && col < width;
    }
};

class GameEngine {
private:
    Board* board;
    int width, height, mineCount;
    int movesLeft; // Number of moves left to reveal all non-mine cells
    bool isGameOver;

    GameEngine() : board(nullptr), isGameOver(false), movesLeft(0) {}

public:
    static GameEngine& getInstance() {
        static GameEngine instance;
        return instance;
    }

    void startGame(int w, int h, int m) {
        width = w;
        height = h;
        mineCount = m;
        movesLeft = (width * height) - mineCount; // Total non-mine cells
        board = new Board(width, height, mineCount);
        isGameOver = false;
    }

    void processInput(char action, int row, int col) {
        if (isGameOver) return;

        if (action == 'r') {
            if (playMinesweeperUtil(row, col)) {
                isGameOver = true;
                board->display(true);
                cout << "You lost!\n";
            } else if (movesLeft == 0) { // Check for win
                isGameOver = true;
                board->display(true);
                cout << "Congratulations! You won!\n";
            }
        } else if (action == 'f') {
            board->getCell(row, col).toggleFlag();
        }
    }

    bool playMinesweeperUtil(int row, int col) {
        if (!board->isValid(row, col)) return false;

        Cell& cell = board->getCell(row, col);

        if (cell.isCellRevealed() || cell.isCellFlagged()) return false;

        if (cell.hasMine()) {
            return true; // Player loses
        }

        cell.reveal();
        movesLeft--; // Decrement moves left

        int count = cell.getAdjacentMines();
        if (count > 0) {
            return false; // Stop recursion
        }

        // Recur for all neighbors
        int dx[8] = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int dy[8] = { -1, 0, 1, -1, 1, -1, 0, 1 };

        for (int d = 0; d < 8; d++) {
            playMinesweeperUtil(row + dx[d], col + dy[d]);
        }

        return false;
    }

    void displayBoard() { board->display(); }

    bool isRunning() const { return !isGameOver; }
};

int main() {
    srand(static_cast<unsigned int>(time(nullptr)));

    GameEngine& game = GameEngine::getInstance();
    game.startGame(5, 5, 3);

    cout << "Welcome to Minesweeper!\n";
    game.displayBoard();

    while (game.isRunning()) {
        char action;
        int row, col;

        cout << "\nEnter your action (r for reveal, f for flag) followed by row and column (e.g. : r 1 1)";
        cin >> action >> row >> col;

        game.processInput(action, row - 1, col - 1);
        game.displayBoard();
    }

    cout << "Thanks for playing!\n";
    return 0;
}
