#include <iostream>
#include <cstring>
#include <unistd.h>
#include <arpa/inet.h>
#include <thread>
#include <fstream>

#define PORT 12345
#define BUFFER_SIZE 1024

using namespace std;

void receiveMessages(int clientSocket) {
    char buffer[BUFFER_SIZE];
    while (true) {
        memset(buffer, 0, BUFFER_SIZE);
        ssize_t bytesReceived = recv(clientSocket, buffer, sizeof(buffer), 0);
        if (bytesReceived <= 0) {
            cout << "Server disconnected.\n";
            break;
        }

        string message(buffer);
        if (message.find(" wants to send file") != string::npos) {
            cout << message << endl;
            string response;
            cin >> response;
            send(clientSocket, response.c_str(), response.size(), 0);
        } else {
            cout << "Message: " << message << endl;
        }
    }
}

void sendFile(int clientSocket, const string& filename) {
    ifstream file(filename, ios::binary);
    if (!file) {
        cout << "ERROR: File not found.\n";
        return;
    }

    string command = "SEND_FILE " + filename;
    send(clientSocket, command.c_str(), command.size(), 0);

    char buffer[BUFFER_SIZE];
    while (file.read(buffer, sizeof(buffer))) {
        send(clientSocket, buffer, file.gcount(), 0);
    }
    send(clientSocket, buffer, file.gcount(), 0);

    file.close();
    cout << "File " << filename << " sent successfully.\n";
}

int main() {
    int clientSocket = socket(AF_INET, SOCK_STREAM, 0);
    if (clientSocket == -1) {
        perror("Socket creation failed");
        return 1;
    }

    sockaddr_in serverAddr = {};
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_port = htons(PORT);
    inet_pton(AF_INET, "127.0.0.1", &serverAddr.sin_addr);

    if (connect(clientSocket, (struct sockaddr*)&serverAddr, sizeof(serverAddr)) == -1) {
        perror("Connection failed");
        close(clientSocket);
        return 1;
    }

    cout << "Connected to server.\n";

    string clientName, roomId;
    cout << "Enter your name: ";
    getline(cin, clientName);
    cout << "Enter room ID: ";
    getline(cin, roomId);

    string clientInfo = clientName + ":" + roomId;
    send(clientSocket, clientInfo.c_str(), clientInfo.size(), 0);

    thread receiveThread(receiveMessages, clientSocket);

    string input;
    while (true) {
        getline(cin, input);
        if (input == "EXIT") {
            send(clientSocket, input.c_str(), input.size(), 0);
            break;
        } else if (input.find("CHANGE_ROOM") == 0) {
            send(clientSocket, input.c_str(), input.size(), 0);
        } else if (input.find("SEND_FILE") == 0) {
            string filename = input.substr(10);
            sendFile(clientSocket, filename);
        } else {
            send(clientSocket, input.c_str(), input.size(), 0);
        }
    }

    close(clientSocket);
    cout << "Disconnected from server.\n";

    if (receiveThread.joinable()) {
        receiveThread.join();
    }

    return 0;
}

