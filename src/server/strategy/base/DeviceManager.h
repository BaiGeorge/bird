#ifndef STRATEGY_DEVICE_MANAGER_H
#define STRATEGY_DEVICE_MANAGER_H

#include <unordered_map>
#include "StrategyTypes.h"
#include "Strategy.pb.h"
#include "Price.pb.h"
#include "Quoter.pb.h"
#include "base/disruptor/BusySpinWaitStrategy.h"
#include "base/disruptor/MultiProducerSequencer.h"
#include "boost/circular_buffer.hpp"

class StrategyDevice;
class Hedger;
class DeviceManager {
 public:
  DeviceManager(const Instrument *underlying);
  DeviceManager(const DeviceManager&) = delete;
  DeviceManager& operator=(const DeviceManager&) = delete;
  ~DeviceManager();

  void Init();
  const Instrument* GetUnderlying() const { return underlying_; }

  template<class E> void Publish(const E &e) {
    int64_t seq = rb_.Next();
    rb_.Get(seq) = std::move(e);
    rb_.Publish(seq);
  }

  void Start(const std::string& name);
  void StartAll();
  void Stop(const std::string& name, const std::string &reason);
  void StopAll(const std::string &reason);
  void Publish(std::shared_ptr<Price> &price);
  std::shared_ptr<StrategyDevice> Find(const std::string &name) const;
  void Remove(const std::string &name);

  void OnStrategyOperate(const std::string &user, const Proto::StrategyOperate &op);
  // void OnQuoterSpec(const std::string &user, Proto::RequestType type,
  //     const std::shared_ptr<Proto::QuoterSpec> &quoter);

  bool IsStrategyRunning(const std::string &name) const;
  bool IsStrategiesRunning() const;

  void UpdatePricer(const Proto::Pricer &pricing);
  double GetUnderlyingTheo() { return theo_.Get(); }

 private:
  const Instrument *underlying_;
  UnderlyingPrice theo_;
  boost::circular_buffer<base::PriceType> underlying_prices_;
  // int32_t warn_tick_change_ = 5;
  double max_price_change_;
  bool normal_ = false;

  std::unordered_map<std::string, std::shared_ptr<StrategyDevice>> devices_;
  std::unique_ptr<StrategyDevice> pricer_;
  std::unique_ptr<StrategyDevice> monitor_;
  std::unique_ptr<Hedger> hedger_;

  base::BusySpinWaitStrategy strategy_;
  base::MultiProducerSequencer<BUFFER_SIZE> sequencer_;
  StrategyRingBuffer rb_;
  std::vector<base::Sequence*> sequences_;
  base::SequenceBarrier *barrier_;
};

#endif // STRATEGY_DEVICE_MANAGER_H
