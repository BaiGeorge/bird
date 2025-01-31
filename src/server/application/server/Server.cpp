#include <mutex>
#include <thread>
#include <condition_variable>
#include <boost/format.hpp>
#include "base/common/Version.h"
#include "base/common/Dtoa.h"
#include "base/logger/Logging.h"
#include "Server.pb.h"
#include "Strategy.pb.h"
#include "config/EnvConfig.h"
#include "exchange/manager/ExchangeManager.h"
#include "model/Middleware.h"
#include "model/InstrumentManager.h"
#include "model/ParameterManager.h"
#include "model/OrderManager.h"
#include "model/TradeManager.h"
#include "model/PositionManager.h"
#include "strategy/base/DeviceManager.h"
#include "strategy/base/StrategyDevice.h"
#include "strategy/base/ClusterManager.h"

using namespace base;
using namespace std;

std::mutex mtx;
std::condition_variable cv;

int main(int argc, char *args[]) {
  Logger::InitFileLogger(EnvConfig::GetInstance()->GetString(EnvVar::LOGGING_DIR).c_str(),
                         EnvConfig::GetInstance()->GetString(EnvVar::APP_NAME).c_str(),
                         EnvConfig::GetInstance()->GetBool(EnvVar::ASYNC_LOGGING) == false);
  Logger::SetLogLevel(static_cast<Logger::LogLevel>(
                      EnvConfig::GetInstance()->GetInt32(EnvVar::LOGGING_LEVEL)));
  Proto::Exchange exchange;
  Proto::Exchange_Parse(EnvConfig::GetInstance()->GetString(EnvVar::EXCHANGE), &exchange);
  Logger::SetNetOutput([&](Logger::LogLevel lvl, const char *data, int n) {
        auto info = std::make_shared<Proto::ServerInfo>();
        info->set_exchange(exchange);
        info->set_info(data, n);
        info->set_type(static_cast<Proto::InfoType>(lvl - Logger::PUBLISH));
        info->set_time(time(NULL));
        Middleware::GetInstance()->Publish(info);
      });
  LOG_INF << boost::format("Welcome BIRD-Server %1%.%2%.%3%, build date: %4%") %
             VER_MAJOR % VER_MINOR % VER_PATCH % __DATE__;
  Middleware::GetInstance()->Init();
  LOG_INF << "Middleware is running now";

  ClusterManager::GetInstance()->Init();
  ParameterManager::GetInstance()->InitGlobal();
  ExchangeManager::GetInstance()->Init();
  LOG_INF << "Exchange has been initialized";

  OrderManager::GetInstance()->Init();
  LOG_INF << "Order manager has been initialized";

  TradeManager::GetInstance()->Init();
  LOG_INF << "Trade manager has been initialized";

  PositionManager::GetInstance()->Init();
  LOG_INF << "Position manager has been initialized";

  ParameterManager::GetInstance()->Init();

  LOG_INF << "==================================================";
  LOG_PUB << "Initialization is done:)";
  LOG_INF << "==================================================";

  ///// test
  std::this_thread::sleep_for(std::chrono::seconds(10));
  //// const Instrument* inst = InstrumentManager::GetInstance()->FindId("SR801");
  const std::string id = "IF1904";
  const Instrument* inst = InstrumentManager::GetInstance()->FindId(id);
  if (inst)
  {
    auto *dm = ClusterManager::GetInstance()->FindDevice(inst);
    if (dm)
    {
      Proto::StrategyOperate op;
      op.set_name("IF1904_Q");
      op.set_underlying("IF1904");
      op.set_operate(Proto::Start);
      dm->OnStrategyOperate("test", op);
      std::this_thread::sleep_for(std::chrono::seconds(600));
      op.set_operate(Proto::Stop);
      dm->OnStrategyOperate("test", op);
    }
    else
    {
      LOG_ERR << "Can't find device " << inst->Id();
    }
  }
  else
  {
    LOG_ERR << "Can't find instrument " << id;
  }
  std::this_thread::sleep_for(std::chrono::seconds(60));
  OrderManager::GetInstance()->Dump();
  /// test

  std::unique_lock<std::mutex> lck(mtx);
  cv.wait(lck);
  return 0;
}
