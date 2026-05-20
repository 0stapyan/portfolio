#include <iostream>
#include <vector>
#include <queue>
#include <thread>
#include <mutex>
#include <condition_variable>
#include <functional>
#include <atomic>
#include <chrono>

using namespace std;

class Task {
public:
    int id;
    int duration;

    Task(int id, int duration) : id(id), duration(duration) {}

    void execute() {
        cout << "Task " << id << " started, duration: " << duration << " seconds." << endl;
        this_thread::sleep_for(chrono::seconds(duration));
        cout << "Task " << id << " finished." << endl;
    }
};

class ThreadPool {
private:
    vector<thread> workers;
    queue<Task> taskQueue;
    mutex queueMutex;
    condition_variable workerCondition;
    condition_variable batchCondition;
    atomic<bool> stop;
    atomic<bool> processing;
    atomic<int> activeWorkers;
    mutex coutMutex;  // For synchronized console output

    void debugPrint(const string& message) {
        lock_guard<mutex> lock(coutMutex);
        cout << "[Debug] " << message << endl;
    }

public:
    ThreadPool(int threadCount) : stop(false), processing(false), activeWorkers(0) {
        debugPrint("Initializing ThreadPool with " + to_string(threadCount) + " threads");
        for (int i = 0; i < threadCount; ++i) {
            workers.emplace_back(&ThreadPool::worker, this);
        }
        debugPrint("ThreadPool initialized");
    }

    ~ThreadPool() {
        debugPrint("Destroying ThreadPool");
        stop = true;
        workerCondition.notify_all();
        batchCondition.notify_all();
        for (thread &worker : workers) {
            if (worker.joinable()) {
                worker.join();
            }
        }
        debugPrint("ThreadPool destroyed");
    }

    void addTask(const Task& task) {
        {
            lock_guard<mutex> lock(queueMutex);
            if (processing) {
                debugPrint("Task " + to_string(task.id) + " rejected (queue is processing)");
                return;
            }
            taskQueue.push(task);
            debugPrint("Task " + to_string(task.id) + " added to queue. Queue size: " +
                       to_string(taskQueue.size()));
        }
    }

    void runEvery45Seconds() {
        debugPrint("Starting 45-second cycle thread");
        while (!stop) {
            debugPrint("Waiting 45 seconds before next processing cycle");
            this_thread::sleep_for(chrono::seconds(45));

            {
                lock_guard<mutex> lock(queueMutex);
                if (taskQueue.empty()) {
                    debugPrint("No tasks to process");
                    continue;
                }

                debugPrint("Beginning processing cycle");
                processing = true;
                debugPrint("Notifying worker threads");
                workerCondition.notify_all();
            }

            {
                unique_lock<mutex> lock(queueMutex);
                debugPrint("Waiting for all tasks to complete");
                batchCondition.wait(lock, [this]() {
                    return taskQueue.empty() && activeWorkers == 0;
                });

                processing = false;
                debugPrint("Processing cycle finished");
            }
        }
    }

private:
    void worker() {
        debugPrint("Worker thread started");
        while (!stop) {
            Task task(0, 0);
            bool hasTask = false;

            {
                unique_lock<mutex> lock(queueMutex);
                debugPrint("Worker waiting for processing signal");
                workerCondition.wait(lock, [this]() {
                    return stop || (processing && !taskQueue.empty());
                });

                if (stop) {
                    debugPrint("Worker thread stopping");
                    return;
                }

                if (!taskQueue.empty()) {
                    task = taskQueue.front();
                    taskQueue.pop();
                    hasTask = true;
                    activeWorkers++;
                    debugPrint("Worker picked up task " + to_string(task.id) +
                               ". Active workers: " + to_string(activeWorkers.load()));
                }
            }

            if (hasTask) {
                task.execute();

                {
                    lock_guard<mutex> lock(queueMutex);
                    activeWorkers--;
                    debugPrint("Worker finished task " + to_string(task.id) +
                               ". Active workers: " + to_string(activeWorkers.load()));
                    if (taskQueue.empty() && activeWorkers == 0) {
                        debugPrint("All tasks completed, notifying batch processor");
                        batchCondition.notify_one();
                    }
                }
            }
        }
    }
};

int main() {
    cout << "Starting program" << endl;
    srand(time(nullptr));
    ThreadPool pool(4);

    cout << "Creating queue runner thread" << endl;
    thread queueRunner(&ThreadPool::runEvery45Seconds, &pool);

    cout << "Starting main task generation loop" << endl;
    int taskId = 1;
    while (true) {
        int duration = rand() % 7 + 6;  // 6-12 seconds
        pool.addTask(Task(taskId++, duration));
        this_thread::sleep_for(chrono::seconds(2));
    }

    queueRunner.join();
    return 0;
}