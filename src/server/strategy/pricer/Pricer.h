#ifndef STRATEGY_PRICER_H
#define STRATEGY_PRICER_H

#include "strategy/base/Strategy.h"
#include "TheoCalculator.h"

class Pricer : public Strategy {
 public:
  Pricer(const std::string &name, DeviceManager *dm);

  virtual void OnStart() override;
  virtual void OnStop() override;

 protected:
  virtual void OnPrice(const PricePtr &price) override;
  virtual void OnTrade(const TradePtr &trade) override;
  virtual bool OnHeartbeat(const std::shared_ptr<Proto::Heartbeat> &heartbeat) override;

private:
  TheoCalculator calc_;
};

#endif // STRATEGY_PRICER_H
