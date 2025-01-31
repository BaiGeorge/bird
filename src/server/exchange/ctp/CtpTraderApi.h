#ifndef CTP_TRADER_API_H
#define CTP_TRADER_API_H

#include <mutex>
#include <condition_variable>
#include "../manager/TraderApi.h"
#include "base/concurrency/blockingconcurrentqueue.h"
#include "3rd_library/ctp/include/ThostFtdcTraderApi.h"

class CtpTraderSpi;
class CtpTraderApi : public TraderApi {
 public:
  CtpTraderApi();
  ~CtpTraderApi();

  void Init() override;
  void Login() override;
  void Logout() override;

  bool IsMMOption(const Instrument *option) override;
  bool GetMMPrice(
      const Instrument *option,
      double theo,
      double &bid,
      double &ask) override;
  bool GetQRPrice(
      const Instrument *option,
      double theo,
      double &bid,
      double &ask) override;
  bool MeetMMObligation(const OrderPtr &bid, const OrderPtr &ask, double &ratio) override;
  bool MeetQRObligation(const OrderPtr &bid, const OrderPtr &ask, double &ratio) override;

  // virtual void CancelAll();

  void QueryInstruments();
  void QueryFutureCommissionRate();
  void QueryOptionCommissionRate();
  void QueryMMOptionCommissionRate();
  void QueryMarketData();
  void QueryOrders();
  void QueryTrades();
  void QueryPositions();
  void NotifyInstrumentReady(const boost::gregorian::date &first_maturity);
  // void SetMaxOrderRef(int order_ref)
  // {
  //   order_ref_ = order_ref;
  // }
  void OnUserLogin(int order_ref, int front_id, int session_id);
  OrderPtr FindAndUpdate(const char *order_ref);
  OrderPtr FindAndUpdate(const char *order_ref, const char *exchange_id);
  OrderPtr FindAndUpdate(const char *order_ref, int trade_volume);
  OrderPtr FindAndRemove(const char *order_ref);
  void FindAndCancel(size_t id);
  OrderPtr FindOrder(const char *order_ref);
  bool FindOrder(size_t id, std::string &exchange_id);
  OrderPtr RemoveOrder(const char *order_ref);

 protected:
  void SubmitOrder(const OrderPtr &order) override;
  void SubmitQuote(const OrderPtr &bid, const OrderPtr &ask) override;
  void AmendOrder(const OrderPtr &order) override;
  void AmendQuote(const OrderPtr &bid, const OrderPtr &ask) override;
  void CancelOrder(const OrderPtr &order) override;
  void CancelQuote(const OrderPtr &bid, const OrderPtr &ask) override;

  virtual void QueryCash() override;

  void CancelOrder(const Instrument *inst, size_t id, const std::string &exchange_id);

 private:
  void BuildTemplate();
  char GetOffsetFlag(Proto::Side side);

  CThostFtdcTraderApi *api_ = nullptr;
  CtpTraderSpi *spi_ = nullptr;
  std::atomic<int> req_id_ = {0};
  const std::string exchange_;
  const std::string broker_;
  const std::string investor_;
  int order_ref_ = 0;
  CThostFtdcInputOrderField new_order_;
  CThostFtdcInputOrderActionField pull_order_;
  CThostFtdcInputQuoteField new_quote_;
  CThostFtdcInputQuoteActionField pull_quote_;

  std::mutex ref_mtx_;
  std::unordered_map<int, OrderPtr> order_refs_;
  std::unordered_map<size_t, OrderPtr> order_ids_;
  // std::mutex pull_mtx_;
  // std::unordered_set<int> pulling_refs_;
  static const size_t capacity_ = 256;
  moodycamel::BlockingConcurrentQueue<size_t> pulling_ids_;
  std::unique_ptr<std::thread> pulling_thread_;

  std::mutex inst_mtx_;
  std::condition_variable inst_cv_;
  bool inst_ready_ = false;
  boost::gregorian::date first_maturity_;
};

#endif // CTP_TRADER_API_H
