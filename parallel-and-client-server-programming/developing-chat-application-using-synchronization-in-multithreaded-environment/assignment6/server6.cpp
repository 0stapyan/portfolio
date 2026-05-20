#include <iostream>
#include <thread>
#include <vector>
#include <map>
#include <set>
#include <cstring>
#include <unistd.h>
#include <arpa/inet.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <dirent.h>
#include <mutex>

#define PORT 12345
#define BUFFER_SIZE 1024
#define SERVER_DIR "./server_files/"

using namespace std;

mutex roomMutex;
map<string, set<int>> rooms; // Кімнати та їхні клієнти

void removeDirectory(const string& path) {
    DIR* dir = opendir(path.c_str());
    if (!dir) return;

    struct dirent* entry;
    while ((entry = readdir(dir)) != nullptr) {
        if (string(entry->d_name) == "." || string(entry->d_name) == "..") continue;
        string filePath = path + entry->d_name;
        remove(filePath.c_str());
    }
    closedir(dir);
    rmdir(path.c_str());
}

void broadcastMessage(const string& roomName, const string& message, int senderSocket) {
    lock_guard<mutex> lock(roomMutex);
    for (int clientSocket : rooms[roomName]) {
        if (clientSocket != senderSocket) {
            send(clientSocket, message.c_str(), message.size(), 0);
        }
    }
}

void handleClient(int clientSocket) {
    char buffer[BUFFER_SIZE];

    memset(buffer, 0, BUFFER_SIZE);
    recv(clientSocket, buffer, BUFFER_SIZE - 1, 0);
    string clientData(buffer);

    size_t spacePos = clientData.find(' ');
    if (spacePos == string::npos) {
        cout << "Invalid client data format. Disconnecting...\n";
        close(clientSocket);
        return;
    }

    string roomName = clientData.substr(0, spacePos);
    string clientName = clientData.substr(spacePos + 1);

    string roomDir = SERVER_DIR + roomName + "/";
    string clientDir = roomDir + clientName + "/";
    mkdir(SERVER_DIR, 0777);
    mkdir(roomDir.c_str(), 0777);
    mkdir(clientDir.c_str(), 0777);

    {
        lock_guard<mutex> lock(roomMutex);
        rooms[roomName].insert(clientSocket);
    }

    cout << "Client " << clientName << " connected to room " << roomName << "\n";
    broadcastMessage(roomName, clientName + " joined the chat\n", clientSocket);

    while (true) {
        memset(buffer, 0, BUFFER_SIZE);
        ssize_t bytesReceived = recv(clientSocket, buffer, BUFFER_SIZE - 1, 0);
        if (bytesReceived <= 0) {
            break;
        }
        buffer[bytesReceived] = '\0'; // Додаємо термінатор
        string command(buffer);

        if (command.rfind("ChangeRoom ", 0) == 0) {
            string newRoom = command.substr(11);
            string newRoomDir = SERVER_DIR + newRoom + "/";
            string newClientDir = newRoomDir + clientName + "/";

            removeDirectory(clientDir);
            mkdir(newRoomDir.c_str(), 0777);
            mkdir(newClientDir.c_str(), 0777);

            {
                lock_guard<mutex> lock(roomMutex);
                rooms[roomName].erase(clientSocket);
                rooms[newRoom].insert(clientSocket);
            }

            broadcastMessage(roomName, clientName + " left the chat\n", clientSocket);
            roomName = newRoom;
            clientDir = newClientDir;
            cout << "Client " << clientName << " moved to room " << newRoom << "\n";
            broadcastMessage(roomName, clientName + " joined the chat\n", clientSocket);
        } else if (command.rfind("SendFile ", 0) == 0) {
            // Обробка команди передачі файлу
            string fileName = command.substr(9);
            string filePath = clientDir + fileName;

            // Перевіряємо, чи файл існує
            FILE* file = fopen(filePath.c_str(), "rb");
            if (!file) {
                send(clientSocket, "File not found\n", 15, 0);
                continue;
            }

            // Отримуємо розмір файлу
            fseek(file, 0, SEEK_END);
            long fileSize = ftell(file);
            fseek(file, 0, SEEK_SET);

            // Повідомляємо клієнта про розмір файлу
            string fileInfo = "FILE " + fileName + " " + to_string(fileSize) + "\n";
            send(clientSocket, fileInfo.c_str(), fileInfo.size(), 0);

            // Передаємо файл
            char fileBuffer[BUFFER_SIZE];
            while (true) {
                size_t bytesRead = fread(fileBuffer, 1, BUFFER_SIZE, file);
                if (bytesRead <= 0) {
                    break;
                }
                send(clientSocket, fileBuffer, bytesRead, 0);
            }
            fclose(file);

            // Повідомляємо клієнта про успішну передачу
            send(clientSocket, "File transfer complete\n", 23, 0);
        } else {
            string fullMessage = clientName + ": " + command + "\n";
            broadcastMessage(roomName, fullMessage, clientSocket);
        }
    }

    {
        lock_guard<mutex> lock(roomMutex);
        rooms[roomName].erase(clientSocket);
    }
    broadcastMessage(roomName, clientName + " left the chat\n", clientSocket);
    close(clientSocket);
}

int main() {
    mkdir(SERVER_DIR, 0777);

    int serverSocket = socket(AF_INET, SOCK_STREAM, 0);
    if (serverSocket == -1) {
        perror("Error creating socket");
        return 1;
    }

    sockaddr_in serverAddr = {};
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = INADDR_ANY;
    serverAddr.sin_port = htons(PORT);

    if (::bind(serverSocket, (struct sockaddr*)&serverAddr, sizeof(serverAddr)) == -1) {
        perror("Bind failed");
        close(serverSocket);
        return 1;
    }

    if (listen(serverSocket, SOMAXCONN) == -1) {
        perror("Listen failed");
        close(serverSocket);
        return 1;
    }

    cout << "Server listening on port " << PORT << endl;

    vector<thread> threads;
    while (true) {
        sockaddr_in clientAddr = {};
        socklen_t clientAddrLen = sizeof(clientAddr);
        int clientSocket = accept(serverSocket, (struct sockaddr*)&clientAddr, &clientAddrLen);
        if (clientSocket == -1) {
            perror("Accept failed");
            continue;
        }

        threads.emplace_back([clientSocket]() { handleClient(clientSocket); });
    }

    for (auto& t : threads) {
        if (t.joinable()) {
            t.join();
        }
    }

    close(serverSocket);
    return 0;
}