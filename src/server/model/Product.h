#ifndef MODEL_PRODUCT_H
#define MODEL_PRODUCT_H

#include <mutex>
#include "boost/date_time/gregorian/gregorian.hpp"
#include "boost/date_time/posix_time/posix_time.hpp"
#include "Option.h"
#include "ProductParameter.pb.h"

class Product {
 public:
  static const int32_t kAnnualTradingDays = 245;

  void OnProductParameter(const Proto::ProductParameter &param);
  const boost::gregorian::date& TradingDay() { return trading_day_; }
  double GetTimeValue(const boost::gregorian::date &maturity);
  bool IsTradingTime(const boost::gregorian::date &maturity);

private:
  boost::gregorian::date trading_day_;
  std::map<boost::gregorian::date, double> holidays_;
  typedef std::pair<boost::posix_time::ptime, boost::posix_time::ptime> Session;
  std::vector<Session> sessions_;
  std::vector<Session> maturity_sessions_;
  int32_t session_seconds_;
  int32_t maturity_session_seconds_;
  std::map<boost::gregorian::date, double> days_to_maturity_;
  std::mutex mtx_;
};

#endif // MODEL_PRODUCT_H
