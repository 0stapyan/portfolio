#include <vector>
#include <iostream>
#include <cstdlib>
#include <iostream>
#include <ctime>
#include <SFML/Graphics.hpp>
#include <SFML/Window.hpp>
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
    int movesLeft;
    bool isGameOver;

    GameEngine() : board(nullptr), isGameOver(false), movesLeft(0) {}

public:
    static GameEngine& getInstance() {
        static GameEngine instance;
        return instance;
    }

    int getMovesLeft() const {
        return movesLeft;
    }

    void startGame(int w, int h, int m) {
        width = w;
        height = h;
        mineCount = m;
        movesLeft = (width * height) - mineCount;
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
            } else if (movesLeft == 0) {
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
            return true;
        }

        cell.reveal();
        movesLeft--;

        int count = cell.getAdjacentMines();
        if (count > 0) {
            return false;
        }

        int dx[8] = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int dy[8] = { -1, 0, 1, -1, 1, -1, 0, 1 };

        for (int d = 0; d < 8; d++) {
            playMinesweeperUtil(row + dx[d], col + dy[d]);
        }

        return false;
    }

    void displayBoard() { board->display(); }

    bool isRunning() const { return !isGameOver; }

    Board* getBoard() const { return board; }
    int getWidth() const { return width; }
    int getHeight() const { return height; }
};

class RenderingEngine {
private:
    sf::RenderWindow window;
    GameEngine& gameEngine;
    const int cellSize = 50;
    const int padding = 5;

    sf::Color hiddenColor = sf::Color::Blue;
    sf::Color revealedColor = sf::Color::White;
    sf::Color mineColor = sf::Color::Yellow;
    sf::Color flagColor = sf::Color::Red;

    sf::RectangleShape restartButton;
    sf::Text restartText;
    sf::Font font;

    void setupRestartButton() {
        // Set up the button
        restartButton.setSize(sf::Vector2f(200, 50));
        restartButton.setFillColor(sf::Color::Green);
        restartButton.setPosition(window.getSize().x / 2 - 100, window.getSize().y - 70);

        restartText.setFont(font);
        restartText.setString("Restart");
        restartText.setCharacterSize(24);
        restartText.setFillColor(sf::Color::White);
        restartText.setPosition(restartButton.getPosition().x + 50, restartButton.getPosition().y + 10);
    }

    void drawBoard() {
        Board* board = gameEngine.getBoard();
        int rows = gameEngine.getHeight();
        int cols = gameEngine.getWidth();

        for (int row = 0; row < rows; row++) {
            for (int col = 0; col < cols; col++) {
                sf::RectangleShape cellShape(sf::Vector2f(cellSize, cellSize));
                cellShape.setPosition(col * (cellSize + padding), row * (cellSize + padding));

                Cell& cell = board->getCell(row, col);

                if (cell.isCellRevealed()) {
                    cellShape.setFillColor(cell.hasMine() ? mineColor : revealedColor);
                } else if (cell.isCellFlagged()) {
                    cellShape.setFillColor(flagColor);
                } else {
                    cellShape.setFillColor(hiddenColor);
                }

                window.draw(cellShape);

                if (cell.isCellRevealed() && !cell.hasMine()) {
                    sf::Text numberText;
                    numberText.setFont(font);
                    numberText.setCharacterSize(20);
                    numberText.setFillColor(sf::Color::Black);
                    numberText.setPosition(col * (cellSize + padding) + 15, row * (cellSize + padding) + 10);
                    numberText.setString(std::to_string(cell.getAdjacentMines()));
                    window.draw(numberText);
                }
            }
        }
    }

    void restartGame() {
        gameEngine.startGame(10, 10, 10);
    }

public:
    RenderingEngine(GameEngine& engine) : gameEngine(engine) {
        int windowWidth = gameEngine.getWidth() * (cellSize + padding) - padding;
        int windowHeight = gameEngine.getHeight() * (cellSize + padding) - padding + 100;
        window.create(sf::VideoMode(windowWidth, windowHeight), "Minesweeper");

        if (!font.loadFromFile("arial.ttf")) {
            std::cerr << "Error: Could not load font.\n";
        }

        setupRestartButton();
    }

    void run() {
        while (window.isOpen()) {
            sf::Event event;
            while (window.pollEvent(event)) {
                if (event.type == sf::Event::Closed) {
                    window.close();
                } else if (event.type == sf::Event::MouseButtonPressed) {
                    if (event.mouseButton.button == sf::Mouse::Left) {
                        int col = event.mouseButton.x / (cellSize + padding);
                        int row = event.mouseButton.y / (cellSize + padding);

                        if (gameEngine.isRunning()) {
                            if (sf::Keyboard::isKeyPressed(sf::Keyboard::LControl)) {
                                gameEngine.processInput('f', row, col);
                            } else {
                                gameEngine.processInput('r', row, col);
                            }
                        }

                        if (restartButton.getGlobalBounds().contains(event.mouseButton.x, event.mouseButton.y)) {
                            restartGame();
                        }
                    }
                }
            }

            window.clear(sf::Color::Black);

            if (gameEngine.isRunning()) {
                drawBoard();
            } else {
                sf::Text endText;
                endText.setFont(font);
                endText.setCharacterSize(40);
                endText.setFillColor(sf::Color::White);
                endText.setPosition(85, window.getSize().y / 2 - 25);

                if (gameEngine.getMovesLeft() == 0) {
                    endText.setString("Congrats! You won!");
                } else {
                    endText.setString("Game Over! You lost!");
                }

                window.draw(endText);
            }

            window.draw(restartButton);
            window.draw(restartText);

            window.display();
        }
    }
};

int main() {
    srand(static_cast<unsigned int>(time(nullptr)));

    GameEngine& game = GameEngine::getInstance();
    game.startGame(10, 10, 10);

    RenderingEngine renderer(game);
    renderer.run();

    return 0;
}
