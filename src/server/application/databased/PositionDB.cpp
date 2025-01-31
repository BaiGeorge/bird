#include "PositionDB.h"
#include "base/logger/Logging.h"
#include "config/EnvConfig.h"
#include "model/Message.h"
#include "boost/format.hpp"

PositionDB::PositionDB(ConcurrentSqliteDB &db,
                       const std::string &table_name)
    : DbBase(db, table_name), requests_(capacity_) {
  thread_ = std::thread(std::bind(&PositionDB::Run, this));
}

void PositionDB::RefreshCache() {
  if (EnvConfig::GetInstance()->GetBool(EnvVar::DEL_POSITION_AT_INITIAL, true)) {
    char sql[1024];
    sprintf(sql, "DELETE FROM %s", table_name_.c_str());
    ExecSql(sql);
  }
}

void PositionDB::RegisterCallback(base::ProtoMessageDispatcher<base::ProtoMessagePtr> &dispatcher) {
  dispatcher.RegisterCallback<Proto::PositionReq>(
      std::bind(&PositionDB::OnRequest, this, std::placeholders::_1));
  dispatcher.RegisterCallback<Proto::Position>(
      std::bind(&PositionDB::OnUpdate, this, std::placeholders::_1));
}

base::ProtoMessagePtr PositionDB::OnRequest(const std::shared_ptr<Proto::PositionReq> &msg) {
  LOG_INF << "Position request: " << msg->ShortDebugString();
  auto reply = std::make_shared<Proto::PositionRep>();
  assert (msg->type() == Proto::RequestType::Get);
  // if (msg->type() == Proto::RequestType::Get) {
    if (msg->instrument().empty()) {
      std::lock_guard<std::mutex> lck(mtx_);
      for (auto &position : positions_) {
        reply->add_positions()->CopyFrom(*position.second);
      }
    } else {
      std::lock_guard<std::mutex> lck(mtx_);
      auto it = positions_.find(msg->instrument());
      if (it != positions_.end()) {
        reply->add_positions()->CopyFrom(*it->second);
      } else {
        auto *result = reply->mutable_result();
        result->set_result(false);
        result->set_error("Can't find instrument " + msg->instrument());
        return reply;
      }
    }
    LOG_INF << boost::format("Get %1% positions totally.") % reply->positions_size();
  // } else {
  //   requests_.enqueue(msg);
  // }
  reply->mutable_result()->set_result(true);
  return reply;
}

base::ProtoMessagePtr PositionDB::OnUpdate(const std::shared_ptr<Proto::Position> &msg) {
  LOG_INF << "Position: " << msg->ShortDebugString();
  requests_.enqueue(msg);
  return nullptr;
}

void PositionDB::Run() {
  LOG_INF << "Position update thread is running...";
  std::shared_ptr<Proto::Position> requests[capacity_];
  while (true) {
    size_t cnt = requests_.wait_dequeue_bulk(requests, capacity_);
    char sql[1024];
    TransactionGuard tg(this);
    for (size_t i = 0; i < cnt; ++i) {
      auto &p = requests[i];
      char time[32];
      time_t seconds = p->time() / 1000000;
      struct tm *t = localtime(&seconds);
      size_t cnt = strftime(time, sizeof(time), "%Y-%m-%d %H:%M:%S", t);
      snprintf(time + cnt, sizeof(time) - cnt, ".%06ld", p->time() % 1000000);
      sprintf(sql, "INSERT OR REPLACE INTO %s VALUES('%s', %d, %d, %d, %d, %d, %d, %d, "
          "%d, '%s')" ,table_name_.c_str(), p->instrument().c_str(), p->total_long(),
          p->liquid_long(), p->liquid_yesterday_long(), p->yesterday_long(),
          p->total_short(), p->liquid_short(), p->liquid_yesterday_short(),
          p->yesterday_short(), time);
      ExecSql(sql);
      {
        std::lock_guard<std::mutex> lck(mtx_);
        positions_[p->instrument()] = p;
      }
    }
  }
}

int PositionDB::Callback(void *data, int argc, char **argv, char **col_name) {
  return 0;
}
