#include <iostream>
#include <thread>
#include <cstring>
#include <unistd.h>
#include <arpa/inet.h>

#define PORT 12345
#define BUFFER_SIZE 1024

using namespace std;

void receiveMessages(int clientSocket) {
    char buffer[BUFFER_SIZE];
    while (true) {
        memset(buffer, 0, BUFFER_SIZE);
        ssize_t bytesReceived = recv(clientSocket, buffer, BUFFER_SIZE, 0);
        if (bytesReceived <= 0) {
            cout << "Disconnected from server.\n";
            exit(0);
        }
        cout << buffer << flush;

        // Обробка повідомлення про файл
        if (string(buffer).rfind("FILE ", 0) == 0) {
            size_t spacePos = string(buffer).find(' ', 5);
            if (spacePos == string::npos) {
                continue;
            }

            string fileName = string(buffer).substr(5, spacePos - 5);
            string fileSizeStr = string(buffer).substr(spacePos + 1);
            long fileSize = stol(fileSizeStr);

            cout << "Do you want to receive the file " << fileName << " (" << fileSize << " bytes)? (yes/no): ";
            string response;
            getline(cin, response);

            if (response == "yes") {
                send(clientSocket, "AcceptFile\n", 11, 0);

                // Зберігаємо файл
                FILE* file = fopen(fileName.c_str(), "wb");
                if (!file) {
                    cout << "Error creating file\n";
                    continue;
                }

                long remainingBytes = fileSize;
                while (remainingBytes > 0) {
                    memset(buffer, 0, BUFFER_SIZE);
                    bytesReceived = recv(clientSocket, buffer, BUFFER_SIZE, 0);
                    if (bytesReceived <= 0) {
                        break;
                    }
                    fwrite(buffer, 1, bytesReceived, file);
                    remainingBytes -= bytesReceived;
                }
                fclose(file);

                cout << "File " << fileName << " has been downloaded\n";
            } else {
                send(clientSocket, "DeclineFile\n", 12, 0);
            }
        }
    }
}

int main() {
    string roomName, clientName;

    cout << "Enter room name: ";
    getline(cin, roomName);
    cout << "Enter your name: ";
    getline(cin, clientName);

    int clientSocket = socket(AF_INET, SOCK_STREAM, 0);
    if (clientSocket == -1) {
        cout << "Error creating socket";
        return 1;
    }

    sockaddr_in serverAddr{};
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_port = htons(PORT);
    inet_pton(AF_INET, "127.0.0.1", &serverAddr.sin_addr);

    if (connect(clientSocket, (struct sockaddr *)&serverAddr, sizeof(serverAddr)) == -1) {
        perror("Connect failed");
        close(clientSocket);
        return 1;
    }

    string clientData = roomName + " " + clientName;
    send(clientSocket, clientData.c_str(), clientData.length(), 0);

    cout << "Connected to room " << roomName << " as " << clientName << "!\n";

    thread receiveThread(receiveMessages, clientSocket);

    while (true) {
        string message;
        getline(cin, message);

        if (message == "EXIT") break;

        if (message.rfind("SendFile ", 0) == 0) {
            string fileName = message.substr(9);
            send(clientSocket, ("SendFile " + fileName + "\n").c_str(), 9 + fileName.length() + 1, 0);

            // Очікуємо відповіді від сервера
            char buffer[BUFFER_SIZE];
            memset(buffer, 0, BUFFER_SIZE);
            ssize_t bytesReceived = recv(clientSocket, buffer, BUFFER_SIZE, 0);
            if (bytesReceived <= 0) {
                cout << "Server disconnected.\n";
                break;
            }

            string response(buffer);
            if (response == "File not found\n") {
                cout << "File not found on server\n";
            } else if (response.rfind("FILE ", 0) == 0) {
                // Отримуємо інформацію про файл
                size_t spacePos = response.find(' ', 5);
                string fileName = response.substr(5, spacePos - 5);
                long fileSize = stol(response.substr(spacePos + 1));

                cout << "Receiving file " << fileName << " (" << fileSize << " bytes)...\n";

                // Зберігаємо файл
                FILE* file = fopen(fileName.c_str(), "wb");
                if (!file) {
                    cout << "Error creating file\n";
                    continue;
                }

                long remainingBytes = fileSize;
                while (remainingBytes > 0) {
                    memset(buffer, 0, BUFFER_SIZE);
                    bytesReceived = recv(clientSocket, buffer, BUFFER_SIZE, 0);
                    if (bytesReceived <= 0) {
                        break;
                    }
                    fwrite(buffer, 1, bytesReceived, file);
                    remainingBytes -= bytesReceived;
                }
                fclose(file);

                cout << "File " << fileName << " has been downloaded\n";
            } else {
                cout << "Unknown response from server\n";
            }
        } else {
            send(clientSocket, message.c_str(), message.size(), 0);
        }
    }

    close(clientSocket);
    return 0;
}