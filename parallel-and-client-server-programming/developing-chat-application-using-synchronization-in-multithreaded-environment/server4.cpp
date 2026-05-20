#include <iostream>
#include <thread>
#include <vector>
#include <cstring>
#include <unistd.h>
#include <arpa/inet.h>
#include <dirent.h>
#include <sys/stat.h>
#include <fstream>
#include <sstream>
#include <mutex>
#include <ctime>

#define PORT 12345
#define BUFFER_SIZE 1024
#define SERVER_DIR "./server_files/"

using namespace std;

void processRequest(int clientSocket, const string& clientName) {
    char buffer[BUFFER_SIZE];
    string clientDir = SERVER_DIR + clientName + "/";
    mkdir(clientDir.c_str(), 0777);

    while (true) {
        memset(buffer, 0, BUFFER_SIZE);
        ssize_t bytesReceived = recv(clientSocket, buffer, sizeof(buffer), 0);

        if (bytesReceived <= 0) {
            cout << "Client disconnected\n";
            break;
        }

        string request(buffer);
        istringstream iss(request);
        string command, filename;
        iss >> command >> filename;

        if (command == "EXIT") {
            cout << "Client " << clientName << " requested to disconnect.\n";
            break;
        } else if (command == "LIST") {
            DIR *dir = opendir(clientDir.c_str());
            string fileList;
            struct dirent *entry;
            while ((entry = readdir(dir)) != nullptr) {
                if (entry->d_name[0] != '.') fileList += entry->d_name + string("\n");
            }
            closedir(dir);
            if (fileList.empty()) fileList = "No files available\n";
            send(clientSocket, fileList.c_str(), fileList.size(), 0);
        } else if (command == "PUT") {
            ofstream file(clientDir + filename, ios::binary);
            if (!file) {
                string errorMsg = "ERROR: Could not create file\n";
                send(clientSocket, errorMsg.c_str(), errorMsg.size(), 0);
                continue;
            }

            while ((bytesReceived = recv(clientSocket, buffer, BUFFER_SIZE, 0)) > 0) {
                file.write(buffer, bytesReceived);
                if (bytesReceived < BUFFER_SIZE) break;
            }

            file.close();
            string successMsg = "OK: File uploaded successfully\n";
            send(clientSocket, successMsg.c_str(), successMsg.size(), 0);
        }  else if (command == "GET") {
            ifstream file(clientDir + filename, ios::binary);
            if (!file) {
                string errorMsg = "ERROR: file not found\n";
                send(clientSocket, errorMsg.c_str(), errorMsg.size(), 0);
                continue;
            }

            string successMsg = "OK\n";
            send(clientSocket, successMsg.c_str(), successMsg.size(), 0);

            while (file.read(buffer, sizeof(buffer))) {
                send(clientSocket, buffer, file.gcount(), 0);
            }
            send(clientSocket, buffer, file.gcount(), 0);
        } else if (command == "DELETE") {
            if (remove((clientDir + filename).c_str()) == 0) {
                string successMsg = "OK: file deleted successfully\n";
                send(clientSocket, successMsg.c_str(), successMsg.size(), 0);
            } else {
                string errorMsg = "ERROR: file not found or could not be deleted\n";
                send(clientSocket, errorMsg.c_str(), errorMsg.size(), 0);
            }
        } else if (command == "INFO") {
            struct stat fileStat;
            if (stat((clientDir + filename).c_str(), &fileStat) == 0) {
                ostringstream fileInfo;
                fileInfo << "File: " << filename << "\n";
                fileInfo << "Size: " << fileStat.st_size << " bytes\n";

                char timeBuffer[80];
                struct tm *timeinfo = localtime(&fileStat.st_mtime);
                strftime(timeBuffer, sizeof(timeBuffer), "%Y-%m-%d %H:%M:%S", timeinfo);
                fileInfo << "Last Modified: " << timeBuffer << "\n";

                send(clientSocket, fileInfo.str().c_str(), fileInfo.str().size(), 0);
            } else {
                string errorMsg = "ERROR: file not found\n";
                send(clientSocket, errorMsg.c_str(), errorMsg.size(), 0);
            }
        } else {
            string errorMsg = "ERROR: unknown command\n";
            send(clientSocket, errorMsg.c_str(), errorMsg.size(), 0);
        }
    }
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

        char buffer[BUFFER_SIZE];
        memset(buffer, 0, BUFFER_SIZE);
        recv(clientSocket, buffer, BUFFER_SIZE, 0);
        string clientName(buffer);

        cout << "Accepted connection from " << inet_ntoa(clientAddr.sin_addr) << " as " << clientName << endl;
        threads.emplace_back([clientSocket, clientName]() {
            processRequest(clientSocket, clientName);
        });
    }

    for (size_t i = 0; i < threads.size(); i++) {
        if (threads[i].joinable()) {
            threads[i].join();
        }
    }

    close(serverSocket);
    return 0;
}
