#ifndef DATABASED_POSITION_DB_H
#define DATABASED_POSITION_DB_H

#include <unordered_map>
#include <thread>
#include <mutex>
#include "DbBase.h"
#include "base/concurrency/blockingconcurrentqueue.h"
#include "Position.pb.h"

class PositionDB : public DbBase {
 public:
  typedef std::unordered_map<std::string, std::shared_ptr<Proto::Position>> PositionMap;
  PositionDB(ConcurrentSqliteDB &db, const std::string &table_name);

 private:
  virtual void RefreshCache() override;
  virtual void RegisterCallback(base::ProtoMessageDispatcher<base::ProtoMessagePtr> &dispatcher);

  base::ProtoMessagePtr OnRequest(const std::shared_ptr<Proto::PositionReq> &msg);
  base::ProtoMessagePtr OnUpdate(const std::shared_ptr<Proto::Position> &msg);
  void UpdatePosition(const Proto::Position &inst, PositionMap &cache);

  void Run();

  static int Callback(void *data, int argc, char **argv, char **col_name);

  PositionMap positions_;
  std::mutex mtx_;

  const int capacity_ = 128;
  moodycamel::BlockingConcurrentQueue<std::shared_ptr<Proto::Position>> requests_;
  std::thread thread_;
};

#endif // DATABASED_POSITION_DB_H
