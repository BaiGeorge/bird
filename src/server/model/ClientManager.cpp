#include "ClientManager.h"
#include "boost/format.hpp"
#include "Middleware.h"
// #include "Message.h"
#include "strategy/base/ClusterManager.h"
#include "base/logger/Logging.h"
#include "base/common/Version.h"

ClientManager* ClientManager::GetInstance() {
  static ClientManager manager;
  return &manager;
}

std::shared_ptr<Proto::Reply> ClientManager::Login(const std::shared_ptr<Proto::Login> &login) {
  std::string server_version = (boost::format("%1%.%2%") % base::VER_MAJOR % base::VER_MINOR).str();
  const std::string &client_version = login->version();
  assert(client_version.size() > server_version.size());
  auto reply = std::make_shared<Proto::Reply>();
  reply->set_result(true);
  for (int i = server_version.size() - 1; i >= 0 ; --i) {
    if (server_version[i] != client_version[i]) {
      const std::string err = (boost::format("Version mismatch: Server(%1%.%2%), Client(%3%)") %
                               server_version % base::VER_PATCH % client_version).str();
      LOG_ERR << boost::format("%1% login failed: %2%") % login->user() % err;
      reply->set_result(false);
      reply->set_error(err);
      return reply;
    }
  }
  auto r = std::dynamic_pointer_cast<Proto::Reply>(Middleware::GetInstance()->Request(*login));
  if (r) {
    reply->set_result(r->result());
    reply->set_error(r->error());
  } else {
    reply->set_result(false);
    reply->set_error("Can't access db");
  }
  if (reply->result()) {
    bool success = false;
    {
      std::lock_guard<std::mutex> lck(mtx_);
      success = clients_.emplace(login->user(), std::make_tuple(login->role(), time(NULL))).second;
    }
    if (success) {
      LOG_PUB << login->user() << " login successfully";
    } else {
      reply->set_result(false);
      reply->set_error("Duplicate login");
      LOG_ERR << "Duplicate user login : " << login->user();
    }
  } else {
    LOG_ERR << login->user() << " login failed : " << reply->error();
  }
  return reply;
}

std::shared_ptr<Proto::Reply> ClientManager::Logout(const std::shared_ptr<Proto::Logout> &logout) {
  auto reply = std::make_shared<Proto::Reply>();
  const std::string &user = logout->user();
  std::lock_guard<std::mutex> lck(mtx_);
  if (clients_.size() == 1 && clients_.find(user) != clients_.end()) {
    LOG_INF << "Last user want to logout...";
    if (ClusterManager::GetInstance()->IsStrategiesRunning()) {
      reply->set_result(false);
      reply->set_error("Last user logout and strategies are running.");
      return reply;
    }
  }
  if (clients_.erase(user)) {
    LOG_PUB << user << " logout";
    reply->set_result(true);
  } else {
    LOG_ERR << user << " logout failed, this user isn't logined or is disconnected";
    reply->set_result(false);
    reply->set_error(user + " isn't logined or is disconnected");
  }
  return reply;
}

ClientManager::ProtoReplyPtr ClientManager::OnHeartbeat(
    const std::shared_ptr<Proto::Heartbeat> &heartbeat) {
  if (heartbeat->type() == Proto::ProcessorType::Middleware) {
    bool erased = false;
    std::lock_guard<std::mutex> lck(mtx_);
    for (auto it = clients_.begin(); it != clients_.end();) {
      uint64_t now = time(NULL);
      const uint64_t HEARTBEAT_TIMEOUT = 15;
      uint64_t interval = time(NULL) - std::get<1>(it->second);
      if (interval < HEARTBEAT_TIMEOUT) {
        ++it;
      } else {
        LOG_ERR << boost::format("Client(%1%) : heartbeat lost for %2%s") % it->first % interval;
        it = clients_.erase(it);
        erased = true;
      }
    }
    if (erased && clients_.size() == 0) {
      LOG_ERR << "All clients are disconnected, stopping all strategies...";
      ClusterManager::GetInstance()->StopAll("all clients disconnected");
    }
  } else if (heartbeat->type() == Proto::ProcessorType::GUI) {
    std::lock_guard<std::mutex> lck(mtx_);
    auto it = clients_.find(heartbeat->name());
    if (it != clients_.end()) {
      std::get<1>(it->second) = time(NULL);
    }
  }
  return nullptr;
}
