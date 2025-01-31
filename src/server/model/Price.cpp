#include "Price.h"
#include "boost/format.hpp"
#include "Price.pb.h"
#include "base/common/Float.h"
#include "base/logger/Logging.h"

// std::string Price::Dump() const
// {
//   // return (boost::format("instrument(%1%), last(%2%), bid(%3%), ask(%4%), open(%5%), high(%6%), "
//   //   "low(%7%), close(%8%), amount(%9%), bid_volume(%10%), ask_volume(%11%), volume(%12%)")
//   //   % instrument->Id() % last % bid % ask % open % high % low % close % amount % bid_volume
//   //   % ask_volume % volume).str();
//   std::ostringstream oss;
//   if (instrument)
//   {
//     oss << instrument->Id() << ':';
//   }
//   if (last.price != base::PRICE_UNDEFINED)
//   {
//     oss << " last(" << last.price << '/' << last.volume << ") ";
//   }
//   if (bids[0])
//   {
//     oss << " bids { ";
//     for (int i = 0; i < LEVELS && bids[i]; ++i)
//     {
//       oss << bids[i].price << '/' << bids[i].volume << ' ';
//     }
//     oss << '}';
//   }
//   if (asks[0])
//   {
//     oss << " asks { ";
//     for (int i = 0; i < LEVELS && asks[i]; ++i)
//     {
//       oss << asks[i].price << '/' << asks[i].volume << ' ';
//     }
//     oss << '}';
//   }
//   if (adjusted_price != base::PRICE_UNDEFINED)
//   {
//     oss << " adjusted_price(" << adjusted_price << ") ";
//   }
//   if (adjust != base::PRICE_UNDEFINED)
//   {
//     oss << " adjust(" << adjust << ") ";
//   }
//   if (open != base::PRICE_UNDEFINED)
//   {
//     oss << " open(" << open << ") ";
//   }
//   if (high != base::PRICE_UNDEFINED)
//   {
//     oss << " high(" << high << ") ";
//   }
//   if (low != base::PRICE_UNDEFINED)
//   {
//     oss << " low(" << low << ") ";
//   }
//   if (close != base::PRICE_UNDEFINED)
//   {
//     oss << " close(" << close << ") ";
//   }
//   if (pre_close != base::PRICE_UNDEFINED)
//   {
//     oss << " pre_close(" << pre_close << ") ";
//   }
//   if (pre_settlement != base::PRICE_UNDEFINED)
//   {
//     oss << " pre_settlement(" << pre_settlement << ") ";
//   }
//   if (amount != base::PRICE_UNDEFINED)
//   {
//     oss << " amount(" << amount << ") ";
//   }
//   if (volume != base::VOLUME_UNDEFINED)
//   {
//     oss << " volume(" << volume << ") ";
//   }
//   return oss.str();
// }

std::shared_ptr<Proto::Price> Price::Serialize() const {
  auto p = std::make_shared<Proto::Price>();
  p->set_instrument(instrument->Id());
  p->set_exchange(instrument->Exchange());
  if (last.price != base::PRICE_UNDEFINED) {
    auto *l = p->mutable_last();
    l->set_price(last.price);
    l->set_volume(last.volume);
  }
  for(int i = 0; i < LEVELS && bids[i]; ++i) {
    auto *b = p->add_bids();
    b->set_price(bids[i].price);
    b->set_volume(bids[i].volume);
  }
  for(int i = 0; i < LEVELS && asks[i]; ++i) {
    auto *a = p->add_asks();
    a->set_price(asks[i].price);
    a->set_volume(asks[i].volume);
  }
  if (adjusted_price != base::PRICE_UNDEFINED) {
    p->set_adjusted_price(adjusted_price);
  }
  if (adjust != base::PRICE_UNDEFINED) {
    p->set_adjust(adjust);
  }
  if (open != base::PRICE_UNDEFINED) {
    p->set_open(open);
  }
  if (high != base::PRICE_UNDEFINED) {
    p->set_high(high);
  }
  if (low != base::PRICE_UNDEFINED) {
    p->set_low(low);
  }
  if (close != base::PRICE_UNDEFINED) {
    p->set_close(close);
  }
  if (pre_close != base::PRICE_UNDEFINED) {
    p->set_pre_close(pre_close);
  }
  if (pre_settlement != base::PRICE_UNDEFINED) {
    p->set_pre_settlement(pre_settlement);
  }
  if (amount != base::PRICE_UNDEFINED) {
    p->set_amount(amount);
  }
  if (volume != base::VOLUME_UNDEFINED) {
    p->set_volume(volume);
  }
  return p;
}

namespace base {

LogStream& operator<<(LogStream& stream, const PricePtr &price) {
  assert(price && price->instrument);
  stream << price->instrument->Id() << ':';

  if (price->last.price != base::PRICE_UNDEFINED) {
    stream << " last(" << price->last.price << '/' << price->last.volume << ") ";
  }
  if (price->bids[0]) {
    stream << " bids { ";
    for (int i = 0; i < price->LEVELS && price->bids[i]; ++i) {
      stream << price->bids[i].price << '/' << price->bids[i].volume << ' ';
    }
    stream << '}';
  }
  if (price->asks[0]) {
    stream << " asks { ";
    for (int i = 0; i < price->LEVELS && price->asks[i]; ++i) {
      stream << price->asks[i].price << '/' << price->asks[i].volume << ' ';
    }
    stream << '}';
  }
  if (price->adjusted_price != base::PRICE_UNDEFINED) {
    stream << " adjusted_price(" << price->adjusted_price << ") ";
  }
  if (price->adjust != base::PRICE_UNDEFINED) {
    stream << " adjust(" << price->adjust << ") ";
  }
  if (price->open != base::PRICE_UNDEFINED) {
    stream << " open(" << price->open << ") ";
  }
  if (price->high != base::PRICE_UNDEFINED) {
    stream << " high(" << price->high << ") ";
  }
  if (price->low != base::PRICE_UNDEFINED) {
    stream << " low(" << price->low << ") ";
  }
  if (price->close != base::PRICE_UNDEFINED) {
    stream << " close(" << price->close << ") ";
  }
  if (price->pre_close != base::PRICE_UNDEFINED) {
    stream << " pre_close(" << price->pre_close << ") ";
  }
  if (price->pre_settlement != base::PRICE_UNDEFINED) {
    stream << " pre_settlement(" << price->pre_settlement << ") ";
  }
  if (price->amount != base::PRICE_UNDEFINED) {
    stream << " amount(" << price->amount << ") ";
  }
  if (price->volume != base::VOLUME_UNDEFINED) {
    stream << " volume(" << price->volume << ") ";
  }
  return stream;
}

} // namespace base

UnderlyingPrice::UnderlyingPrice(const Instrument *underlying)
    : underlying_(underlying),
      price_(new Price()) {}

void UnderlyingPrice::SetParameter(
    Proto::UnderlyingTheoType type,
    double elastic,
    double elastic_limit) {
  LOG_INF << boost::format("%1%: type(%2%), elastic(%3%), elastic limit(%4%)") %
    underlying_->Id() % Proto::UnderlyingTheoType_Name(type) % elastic % elastic_limit;
  std::lock_guard<std::mutex> lck(mtx_);
  type_ = type;
  elastic_ = elastic;
  elastic_limit_ = elastic_limit;
}

PriceType UnderlyingPrice::ApplyElastic(PricePtr &price, double delta) {
  price->adjust = 0;
  price->adjusted_price = CalcTheo(price);

  std::lock_guard<std::mutex> lck(mtx_);
  if (!base::IsEqual(elastic_, 0) && !base::IsEqual(elastic_limit_, 0)) {
    if (base::IsLessThan(delta, 0)) {
      price->adjust = std::min(elastic_ * (-delta) / base::MILLION, elastic_limit_);
    } else {
      price->adjust = -std::min(elastic_ * delta / base::MILLION, elastic_limit_);
    }
    price->adjusted_price += price->adjust;
    LOG_INF << boost::format("%1%: theo adjusted %2% -> %3%") % underlying_->Id() % theo_
      % price->adjusted_price;
  }

  *price_ = *price;
  adjust_ = price->adjust;
  theo_ = price->adjusted_price;
  return theo_;
}

bool UnderlyingPrice::ApplyElastic(double delta) {
  std::lock_guard<std::mutex> lck(mtx_);
  if (price_->instrument && !base::IsEqual(elastic_, 0) &&
      !base::IsEqual(elastic_limit_, 0)) {
    PriceType adjust;
    if (base::IsLessThan(delta, 0)) {
      adjust = std::min(elastic_ * (-delta) / base::MILLION, elastic_limit_);
    } else {
      adjust = -std::min(elastic_ * delta / base::MILLION, elastic_limit_);
    }

    if (base::IsMoreOrEqual(fabs(adjust - adjust_), underlying_->Tick())) {
      price_->adjust = adjust;
      price_->adjusted_price = CalcTheo(price_) + adjust;
      LOG_PUB << boost::format("%1%: theo  adjusted by delta %2%(%3%) -> %4%(%5%)") %
        underlying_->Id() % theo_ % adjust_ % price_->adjusted_price % adjust;
      adjust_ = adjust;
      theo_ = price_->adjusted_price;
      return true;
    }
  }
  return false;
}

base::PriceType UnderlyingPrice::CalcTheo(const PricePtr &p) const {
  LOG_DBG << "calc theo of " << p->instrument->Id() << " with " <<
    Proto::UnderlyingTheoType_Name(type_);
  if (p->bids[0] && p->asks[0]) {
    switch (type_) {
      case Proto::Midpoint:
        return (p->bids[0].price + p->asks[0].price) * 0.5;
      case Proto::BaryCentre:
        return AdjustWithMidpoint(CalcBaryCentre(p->bids[0], p->asks[0]),
            p->bids[0].price, p->asks[0].price);
      case Proto::DepthBary:
        if (UseDepthBary(p)) {
          double theo = AdjustWithMidpoint(CalcDepthBary(p), p->bids[0].price, p->asks[0].price);
          LOG_PUB << boost::format("Adjusted DepthBary(%1%): %2%") % underlying_->Id() % theo;
          return theo;
        } else {
          return AdjustWithMidpoint(CalcBaryCentre(p->bids[0], p->asks[0]),
              p->bids[0].price, p->asks[0].price);
        }
      default:
        assert(false);
    }
  } else if (p->last) {
    return p->last.price;
  } else if (p->bids[0]) {
    return p->bids[0].price;
  } else if (p->asks[0]) {
    return p->asks[0].price;
  } else if (p->pre_close != base::PRICE_UNDEFINED) {
    return p->pre_close;
  } else if (p->pre_settlement != base::PRICE_UNDEFINED) {
    return p->pre_settlement;
  }

  return base::PRICE_UNDEFINED;
}

PriceType UnderlyingPrice::CalcBaryCentre(PriceLevel &bid, PriceLevel &ask) const {
  return (bid.price * ask.volume + ask.price * bid.volume) / (bid.volume + ask.volume);
}

PriceType UnderlyingPrice::CalcDepthBary(const PricePtr &p) const {
  static const int static_depth = 3;
  static const int lambda = 3;
  static std::array<std::array<std::vector<double>, lambda>, static_depth> depth_bary_weights
      { { { { {1.}, {1.}, {1.} } },
        { { { 0.5, 0.5 }, { 0.666, 0.333 }, { 0.75, 0.25 } } },
        { { { 0.333, 0.333, 0.333 }, { 0.571, 0.286, 0.143 }, { 0.692, 0.231, 0.077 } } } } };

  int depth = 0;
  while (depth < static_depth && p->bids[depth] && p->asks[depth]) ++depth;
  auto *weight = &depth_bary_weights[depth-1][lambda-1];
  assert(weight);
  PriceType result = PRICE_UNDEFINED;
  for (int i = 0; i < depth; ++i) {
    result += (*weight)[i] * CalcBaryCentre(p->bids[i], p->asks[i]);
  }
  LOG_INF << boost::format("DepthBary(%1%): %2%") % underlying_->Id() % result;
  return result;
}

PriceType UnderlyingPrice::AdjustWithMidpoint(PriceType p, PriceType bid, PriceType ask) const {
  const double step = 0.2;
  const auto ticks = underlying_->ConvertToTick(ask - bid);
  const double weight = std::max(step, 1.0 - (ticks - 1) * step);
  return weight * p + (1 - weight) * (bid + ask) * 0.5;
}

bool UnderlyingPrice::UseDepthBary(const PricePtr &p) const {
  const double spread = underlying_->Tick() * 4;
  const double gap = underlying_->Tick() * 4;
  return p->bids[0] && p->asks[0] &&
         (base::IsMoreThan(p->asks[0].price - p->bids[0].price, spread) ||
          (p->bids[1] && base::IsMoreThan(p->bids[0].price - p->bids[1].price, gap)) ||
          (p->asks[1] && base::IsMoreThan(p->asks[1].price - p->asks[0].price, gap)));
}
