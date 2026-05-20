#include <iostream>
#include <thread>
#include <vector>
#include <string>
#include <mutex>
#include <queue>
#include <condition_variable>
#include <unordered_map>
#include <unistd.h>
#include <arpa/inet.h>
#include <fstream>

#define PORT 12345
#define BUFFER_SIZE 1024

using namespace std;

struct Client {
    int socket;
    string name;
    int roomId;
};

struct Room {
    vector<Client> clients;
    queue<string> messageQueue;
    mutex mtx;
    condition_variable cv;
};

unordered_map<int, Room> rooms;
mutex roomsMutex;

void broadcastMessage(int roomId, const string& message, int senderSocket) {
    lock_guard<mutex> lock(roomsMutex);
    Room& room = rooms[roomId];

    unique_lock<mutex> roomLock(room.mtx);
    for (const Client& client : room.clients) {
        if (client.socket != senderSocket) {
            send(client.socket, message.c_str(), message.size(), 0);
        }
    }
}

void handleFileTransfer(Client sender, const string& filename) {
    Room& room = rooms[sender.roomId];

    for (const Client& client : room.clients) {
        if (client.socket != sender.socket) {
            string prompt = sender.name + " wants to send file " + filename + ". Do you want to receive it? (Y/N)";
            send(client.socket, prompt.c_str(), prompt.size(), 0);

            char response[BUFFER_SIZE];
            memset(response, 0, BUFFER_SIZE);
            ssize_t bytesReceived = recv(client.socket, response, BUFFER_SIZE, 0);

            if (bytesReceived > 0 && (response[0] == 'Y' || response[0] == 'y')) {
                string successMsg = "Sending file " + filename + " to " + client.name;
                send(sender.socket, successMsg.c_str(), successMsg.size(), 0);

                ifstream file(filename, ios::binary);
                if (file) {
                    char buffer[BUFFER_SIZE];
                    ssize_t bytesSent;
                    while (file.read(buffer, sizeof(buffer))) {
                        bytesSent = send(client.socket, buffer, file.gcount(), 0);
                        if (bytesSent == -1) {
                            cerr << "Error sending file.\n";
                            break;
                        }
                    }
                    file.close();
                    cout << "File " << filename << " sent to " << client.name << " successfully.\n";
                } else {
                    string errorMsg = "ERROR: File not found.";
                    send(sender.socket, errorMsg.c_str(), errorMsg.size(), 0);
                }
            } else {
                string rejectMsg = client.name + " rejected the file " + filename;
                send(sender.socket, rejectMsg.c_str(), rejectMsg.size(), 0);
            }
        }
    }
}

void handleClient(Client client) {
    char buffer[BUFFER_SIZE];
    while (true) {
        memset(buffer, 0, BUFFER_SIZE);
        ssize_t bytesReceived = recv(client.socket, buffer, sizeof(buffer), 0);

        if (bytesReceived <= 0) {
            cout << "Client " << client.name << " disconnected.\n";
            break;
        }

        string message(buffer);
        if (message == "EXIT") {
            {
                lock_guard<mutex> lock(roomsMutex);
                auto& room = rooms[client.roomId];
                room.clients.erase(
                        remove_if(room.clients.begin(), room.clients.end(),
                                  [&](const Client& c) { return c.socket == client.socket; }),
                        room.clients.end()
                );
            }
            cout << "Client " << client.name << " has left the room.\n";
            close(client.socket);
            break;
        } else if (message.find("CHANGE_ROOM") == 0) {
            int newRoomId = stoi(message.substr(12));
            {
                lock_guard<mutex> lock(roomsMutex);
                auto& oldRoom = rooms[client.roomId];
                oldRoom.clients.erase(
                        remove_if(oldRoom.clients.begin(), oldRoom.clients.end(),
                                  [&](const Client& c) { return c.socket == client.socket; }),
                        oldRoom.clients.end()
                );
                client.roomId = newRoomId;
                rooms[newRoomId].clients.push_back(client);
            }
            cout << "Client " << client.name << " moved to room " << newRoomId << endl;
            string successMsg = "You have joined room " + to_string(newRoomId);
            send(client.socket, successMsg.c_str(), successMsg.size(), 0);
        } else if (message.find("SEND_FILE") == 0) {
            string filename = message.substr(10);
            handleFileTransfer(client, filename);
        } else {
            broadcastMessage(client.roomId, client.name + ": " + message, client.socket);
        }
    }
    close(client.socket);
}

int main() {
    int serverSocket = socket(AF_INET, SOCK_STREAM, 0);
    if (serverSocket == -1) {
        perror("Socket creation failed");
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

    while (true) {
        sockaddr_in clientAddr = {};
        socklen_t clientAddrLen = sizeof(clientAddr);
        int clientSocket = accept(serverSocket, (struct sockaddr*)&clientAddr, &clientAddrLen);
        if (clientSocket == -1) {
            perror("Accept failed");
            continue;
        }

        char buffer[BUFFER_SIZE];
        memset(buffer, 0, BUFFER_SIZE);
        recv(clientSocket, buffer, BUFFER_SIZE, 0);
        string clientInfo(buffer);
        size_t delimiter = clientInfo.find(':');
        string clientName = clientInfo.substr(0, delimiter);
        int roomId = stoi(clientInfo.substr(delimiter + 1));

        Client client = {clientSocket, clientName, roomId};

        lock_guard<mutex> lock(roomsMutex);
        rooms[roomId].clients.push_back(client);

        cout << "Client " << clientName << " joined room " << roomId << endl;

        thread clientThread(handleClient, client);
        clientThread.detach();
    }

    close(serverSocket);
    return 0;
}