#include <iostream>
#include <cstring>
#include <unistd.h>
#include <arpa/inet.h>
#include <fstream>

#define PORT 12345
#define BUFFER_SIZE 1024

using namespace std;

string clientName;

void sendFile(int clientSocket, const string &filename) {
    ifstream file(filename, ios::binary);
    if (!file) {
        cout << "ERROR: file not found\n";
        return;
    }

    string command = "PUT " + filename;
    send(clientSocket, command.c_str(), command.length(), 0);

    char buffer[BUFFER_SIZE];
    while (file.read(buffer, sizeof(buffer))) {
        send(clientSocket, buffer, file.gcount(), 0);
    }
    send(clientSocket, buffer, file.gcount(), 0);

    file.close();

    memset(buffer, 0, BUFFER_SIZE);
    recv(clientSocket, buffer, sizeof(buffer), 0);
    cout << "Server response: " << buffer << endl;
}

void downloadFile(int clientSocket, const string &filename) {
    string command = "GET " + filename;
    send(clientSocket, command.c_str(), command.length(), 0);

    char buffer[BUFFER_SIZE];
    memset(buffer, 0, BUFFER_SIZE);
    ssize_t bytesReceived = recv(clientSocket, buffer, sizeof(buffer), 0);

    if (strncmp(buffer, "ERROR", 5) == 0) {
        cout << "Server response: " << buffer;
        return;
    }

    ofstream file(filename, ios::binary);
    if (!file) {
        cout << "ERROR: Could not create file" << endl;
        return;
    }

    while((bytesReceived = recv(clientSocket, buffer, BUFFER_SIZE, 0)) > 0) {
        file.write(buffer, bytesReceived);
        if (bytesReceived < BUFFER_SIZE) break;
    }

    file.close();
    cout << "File downloaded successfully: " << filename << endl;
}

void deleteFile(int clientSocket, const string &filename) {
    string command = "DELETE " + filename;
    send(clientSocket, command.c_str(), command.length(), 0);

    char buffer[BUFFER_SIZE];
    memset(buffer, 0, BUFFER_SIZE);
    recv(clientSocket, buffer, sizeof(buffer), 0);

    cout << "Server response: " << buffer << endl;
}

void getFileInfo(int clientSocket, const string &filename) {
    string command = "INFO " + filename;
    send(clientSocket, command.c_str(), command.length(), 0);

    char buffer[BUFFER_SIZE];
    memset(buffer, 0, BUFFER_SIZE);
    recv(clientSocket, buffer, sizeof(buffer), 0);

    cout << "Server response:\n" << buffer << endl;
}

int main() {
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

    send(clientSocket, clientName.c_str(), clientName.length(), 0);

    cout << "Connected to server as " << clientName << "!\n";

    while(true) {
        cout << "Enter command (LIST, PUT <file>, GET <file>, DELETE <file>, INFO <file>, EXIT): ";
        string command;
        getline(cin, command);

        if (command == "EXIT") break;
        if (command == "LIST") {
            send(clientSocket, command.c_str(), command.length(), 0);

            char buffer[BUFFER_SIZE];
            memset(buffer, 0, BUFFER_SIZE);
            recv(clientSocket, buffer, sizeof(buffer), 0);
            cout << "Server response:\n" << buffer << endl;
        } else if (command.find("PUT ") == 0) {
            string filename = command.substr(4);
            sendFile(clientSocket, filename);
        } else if (command.find("GET ") == 0) {
            string filename = command.substr(4);
            downloadFile(clientSocket, filename);
        } else if (command.find("DELETE ") == 0) {
            string filename = command.substr(7);
            deleteFile(clientSocket, filename);
        } else if (command.find("INFO ") == 0) {
            string filename = command.substr(5);
            getFileInfo(clientSocket, filename);
        } else {
            cout << "ERROR: unknown command\n";
        }
    }

    close(clientSocket);
    return 0;
}