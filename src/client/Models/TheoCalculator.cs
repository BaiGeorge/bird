﻿using Google.Protobuf;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ModelLibrary;
using Prism.Events;
using System.Globalization;
using System.Diagnostics;

namespace client.Models
{
    class TheoCalculator
    {
        public TheoCalculator(IUnityContainer container, Proto.Exchange exchange)
        {
            this.container = container;
            this.exchange = exchange;
        }

        public bool Start()
        {
            running = true;
            thread = new Thread(this.Work);
            thread.Start();
            return true;
        }

        public void Stop()
        {
            running = false;
            if (thread != null)
            {
                messages.CompleteAdding();
                thread.Abort();
                thread.Join();
                thread = null;
            }
        }

        public Proto.Price GetPrice(Instrument inst)
        {
            Proto.Price p = null;
            this.prices.TryGetValue(inst, out p);
            return p;
        }

        public GreeksData GetGreeks(Option option)
        {
            GreeksData data = null;
            this.greeks.TryGetValue(option, out data);
            return data;
        }

        public ImpliedVolatilityData GetIV(Option option)
        {
            ImpliedVolatilityData data = null;
            if (this.impliedVolatilities.TryGetValue(option, out data))
            {
                return data;
            }
            return null;
        }

        private void Work()
        {
            try
            {
                pm = this.container.Resolve<ProductManager>(exchange.ToString());
                epm = this.container.Resolve<ExchangeParameterManager>(exchange.ToString());
                InitializeParameters();
                RegisterMessages();
                while (running)
                {
                    IMessage msg = messages.Take();
                    Action<IMessage> action = null;
                    if (actions.TryGetValue(msg.GetType(), out action))
                    {
                        action(msg);
                    }
                }
            }
            catch (InvalidOperationException)
            {
                var err = new Proto.ServerInfo()
                {
                    Time = (ulong)((DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0)).TotalSeconds),
                    Type = Proto.InfoType.Error,
                    Exchange = this.exchange,
                    Info = "Theo calculator stopped(InvalidOperationException)",
                };
                this.container.Resolve<EventAggregator>().GetEvent<PubSubEvent<Proto.ServerInfo>>().Publish(err);
                return;
            }
            catch (ThreadAbortException)
            {
                return;
            }
        }

        private void RegisterMessages()
        {
            actions = new Dictionary<Type, Action<IMessage>>() {
                { typeof(Proto.Price), msg => OnPrice(msg as Proto.Price) },
                { typeof(Proto.PricerReq), msg => OnPricerReq(msg as Proto.PricerReq) },
                { typeof(Proto.ExchangeParameterReq), msg => OnExchangeParameterReq(msg as Proto.ExchangeParameterReq) },
                { typeof(Proto.InterestRateReq), msg => OnInterestRateReq(msg as Proto.InterestRateReq) },
                { typeof(Proto.SSRateReq), msg => OnSSRateReq(msg as Proto.SSRateReq) },
                { typeof(Proto.VolatilityCurveReq), msg => OnVolatilityCurveReq(msg as Proto.VolatilityCurveReq) },
                { typeof(Proto.DestrikerReq), msg => OnDestrikerReq(msg as Proto.DestrikerReq) },
                { typeof(Proto.PositionReq), msg => OnPositionReq(msg as Proto.PositionReq) },
            };

            container.Resolve<EventAggregator>().GetEvent<PubSubEvent<Proto.Price>>().Subscribe(new Action<IMessage>(msg => messages.Add(msg)));
            container.Resolve<EventAggregator>().GetEvent<PubSubEvent<Proto.PricerReq>>().Subscribe(new Action<Proto.PricerReq>(
                msg => { if (msg.Exchange == this.exchange) messages.Add(msg); }));
            container.Resolve<EventAggregator>().GetEvent<PubSubEvent<Proto.ExchangeParameterReq>>().Subscribe(new Action<Proto.ExchangeParameterReq>(
                msg => { if (msg.Exchange == this.exchange) messages.Add(msg); }));
            container.Resolve<EventAggregator>().GetEvent<PubSubEvent<Proto.InterestRateReq>>().Subscribe(new Action<Proto.InterestRateReq>(
                msg => { if (msg.Exchange == this.exchange) messages.Add(msg); }));
            container.Resolve<EventAggregator>().GetEvent<PubSubEvent<Proto.SSRateReq>>().Subscribe(new Action<Proto.SSRateReq>(
                msg => { if (msg.Exchange == this.exchange) messages.Add(msg); }));
            container.Resolve<EventAggregator>().GetEvent<PubSubEvent<Proto.VolatilityCurveReq>>().Subscribe(new Action<Proto.VolatilityCurveReq>(
                msg => { if (msg.Exchange == this.exchange) messages.Add(msg); }));
            container.Resolve<EventAggregator>().GetEvent<PubSubEvent<Proto.DestrikerReq>>().Subscribe(new Action<Proto.DestrikerReq>(
                msg => { if (msg.Exchange == this.exchange) messages.Add(msg); }));
            container.Resolve<EventAggregator>().GetEvent<PubSubEvent<Proto.PositionReq>>().Subscribe(new Action<Proto.PositionReq>(
                msg => { if (msg.Exchange == this.exchange) messages.Add(msg); }));
        }

        private void InitializeParameters()
        {
            string exch = this.exchange.ToString();
            var psm = this.container.Resolve<PricerManager>(this.exchange.ToString());
            if (psm == null) return;

            rm = this.container.Resolve<InterestRateManager>(this.exchange.ToString());
            if (rm == null) return;

            ssm = this.container.Resolve<SSRateManager>(this.exchange.ToString());
            if (ssm == null) return;

            vcm = this.container.Resolve<VolatilityCurveManager>(this.exchange.ToString());
            if (vcm == null) return;

            pnm = this.container.Resolve<PositionManager>(exch);
            if (pnm == null) return;

            dm = this.container.Resolve<DestrikerManager>(exch);
            if (dm == null) return;

            this.pricings = new Dictionary<Instrument, Pricing>();
            var hedgeUnderlyings = pm.GetHedgeUnderlyings();
            foreach (var underlying in hedgeUnderlyings)
            {
                var ps = psm.GetPricer(underlying.Id);
                if (ps != null)
                {
                    var pricing = new Pricing()
                        {
                            Spot = double.NaN,
                            PricingModel = new PricingModelWrapper(ps.Model == Proto.PricingModel.Bsm),
                            VolatilityModel = new VolatilityModelWrapper(),
                            Parameters = new Dictionary<DateTime, PricingParameters>(),
                        };
                    var options = pm.GetOptionsByHedgeUnderlying(underlying);
                    foreach (var option in options)
                    {
                        PricingParameters parameters = null;
                        if (pricing.Parameters.TryGetValue(option.Maturity, out parameters) == false)
                        {
                            parameters = new PricingParameters()
                                {
                                    Rate = rm.GetInterestRate(option.Maturity),
                                    SSRate = ssm.GetSSRate(option.HedgeUnderlying.Id, option.Maturity),
                                    Parameters = new Dictionary<Option, Parameter>(),
                                };
                            var vc = vcm.GetVolatilityCurve(option.HedgeUnderlying.Id, option.Maturity);
                            if (vc != null)
                            {
                                parameters.Volatility = new VolatilityParameterWrapper()
                                    {
                                        spot = vc.Spot,
                                        skew = vc.Skew,
                                        atm_vol = vc.AtmVol,
                                        call_convex = vc.CallConvex,
                                        put_convex = vc.PutConvex,
                                        call_slope = vc.CallSlope,
                                        put_slope = vc.PutSlope,
                                        call_cutoff = vc.CallCutoff,
                                        put_cutoff = vc.PutCutoff,
                                        vcr = vc.Vcr,
                                        scr = vc.Scr,
                                        ccr = vc.Ccr,
                                        spcr = vc.Spcr,
                                        sccr = vc.Sccr
                                    };
                            }
                            pricing.Parameters.Add(option.Maturity, parameters);
                        }
                        parameters.Parameters[option] = new Parameter()
                            {
                                Position = pnm.GetPosition(option.Id),
                                Destriker = dm.GetDestriker(option.Id),
                            };
                    }
                    this.pricings.Add(underlying, pricing);
                }
            }
        }

        private void OnPrice(Proto.Price p)
        {
            var inst = pm.FindId(p.Instrument);
            if (inst != null)
            {
                if (inst.Type == Proto.InstrumentType.Option)
                {
                    /// calculate IV
                    Pricing pricing = null;
                    if (this.pricings.TryGetValue(inst.HedgeUnderlying, out pricing))
                    {
                        Option option = inst as Option;
                        PricingParameters parameters = null; 
                        if (pricing.Parameters.TryGetValue(option.Maturity, out parameters))
                        {
                            Parameter param = null;
                            if (parameters.Parameters.TryGetValue(option, out param))
                            {
                                double t = this.epm.GetTimeValue(option.Maturity);
                                if (double.IsNaN(t) == false)
                                {
                                    CalculatePublishIV(option, p, pricing, parameters, param.Volatility, t);
                                }
                            }
                        }
                    }
                }
                else
                {
                    /// calculate theo
                    Pricing pricing = null;
                    if (this.pricings.TryGetValue(inst, out pricing))
                    {
                        pricing.Spot = p.AdjustedPrice;

                        foreach (var kvp in pricing.Parameters)
                        {
                            double t = epm.GetTimeValue(kvp.Key);
                            double t1 = epm.GetCharmTimeValue(kvp.Key);
                            if (double.IsNaN(t) == false)
                            {
                                //foreach(var option in kvp.Value.Volatilities.Keys.ToList())
                                //{
                                //    double v = 0;
                                //    if (CalculatePublishGreeks(option, pricing, kvp.Value, ref v, t, t1))
                                //    {
                                //        kvp.Value.Volatilities[option] = v;
                                //    }
                                //}
                                foreach (var param in kvp.Value.Parameters)
                                {
                                    CalculatePublishGreeks(param.Key, pricing, kvp.Value, param.Value, t, t1);
                                }
                            }
                        }
                    }
                    this.prices.AddOrUpdate(inst, p, (x, y) => p);
                }
            }
        }

        private void OnPricerReq(Proto.PricerReq req)
        {
            Debug.Assert(req.Exchange == this.exchange);
            if (req.Type == Proto.RequestType.Set)
            {
                foreach (var ps in req.Pricers)
                {
                    var underlying = pm.FindId(ps.Underlying);
                    if (underlying != null)
                    {
                        Pricing pricing = null;
                        if (this.pricings.TryGetValue(underlying, out pricing) == false)
                        {
                            pricing = new Pricing()
                                {
                                    Parameters = new Dictionary<DateTime, PricingParameters>(),
                                    VolatilityModel = new VolatilityModelWrapper(),
                                };
                            this.pricings.Add(underlying, pricing);
                        }
                        pricing.PricingModel = new PricingModelWrapper(ps.Model == Proto.PricingModel.Bsm);
                        //var options = new SortedSet<string>();
                        var options = pm.GetOptionsByHedgeUnderlying(underlying);
                        foreach (var option in options)
                        {
                            //var option = pm.FindId(op) as Option;
                            //if (option != null)
                            //{
                                //options.Add(option.Id);
                                PricingParameters parameters = null;
                                if (pricing.Parameters.TryGetValue(option.Maturity, out parameters) == false)
                                {
                                    parameters = new PricingParameters()
                                        {
                                            Rate = rm.GetInterestRate(option.Maturity),
                                            SSRate = ssm.GetSSRate(option.HedgeUnderlying.Id, option.Maturity),
                                            Parameters = new Dictionary<Option, Parameter>(),
                                        };
                                    var vc = vcm.GetVolatilityCurve(option.HedgeUnderlying.Id, option.Maturity);
                                    if (vc != null)
                                    {
                                        parameters.Volatility = new VolatilityParameterWrapper()
                                            {
                                                spot = vc.Spot,
                                                skew = vc.Skew,
                                                atm_vol = vc.AtmVol,
                                                call_convex = vc.CallConvex,
                                                put_convex = vc.PutConvex,
                                                call_slope = vc.CallSlope,
                                                put_slope = vc.PutSlope,
                                                call_cutoff = vc.CallCutoff,
                                                put_cutoff = vc.PutCutoff,
                                                vcr = vc.Vcr,
                                                scr = vc.Scr,
                                                ccr = vc.Ccr,
                                                spcr = vc.Spcr,
                                                sccr = vc.Sccr
                                            };
                                    }
                                    pricing.Parameters.Add(option.Maturity, parameters);
                                }
                                if (parameters.Parameters.ContainsKey(option) == false)
                                {
                                    parameters.Parameters.Add(option, new Parameter()
                                        {
                                            Position = pnm.GetPosition(option.Id),
                                            Destriker = dm.GetDestriker(option.Id),
                                        });
                                }
                            //}
                            //var itemsToRemove = new Dictionary<DateTime, List<Option>>();
                            //foreach (var kvp in pricing.Parameters)
                            //{
                            //    List<Option> items = new List<Option>();
                            //    foreach (var param in kvp.Value.Parameters)
                            //    {
                            //        if (options.Contains(param.Key.Id) == false)
                            //        {
                            //            items.Add(param.Key);
                            //        }
                            //    }
                            //    if (items.Count > 0)
                            //    {
                            //        itemsToRemove.Add(kvp.Key, items.Count < kvp.Value.Parameters.Count ? items : null);
                            //    }
                            //}
                            //foreach (var kvp in itemsToRemove)
                            //{
                            //    if (kvp.Value != null)
                            //    {
                            //        foreach (var opt in kvp.Value)
                            //        {
                            //            pricing.Parameters[kvp.Key].Parameters.Remove(opt);
                            //        }
                            //    }
                            //    else
                            //    {
                            //        pricing.Parameters.Remove(kvp.Key);
                            //    }
                            //}
                        }
                    }
                }
            }
            else if (req.Type == Proto.RequestType.Del)
            {
                foreach (var p in req.Pricers)
                {
                    var underlying = pm.FindId(p.Underlying);
                    if (underlying != null)
                    {
                        this.pricings.Remove(underlying);
                    }
                }
            }
        }

        private void OnExchangeParameterReq(Proto.ExchangeParameterReq req)
        {
            Debug.Assert(req.Exchange == this.exchange);
            if (req.Type == Proto.RequestType.Set)
            {
                foreach (var pricing in this.pricings)
                {
                    foreach (var kvp in pricing.Value.Parameters)
                    {
                        Calculate(pricing.Value, kvp.Key, kvp.Value);
                    }
                }
            }
        }

        private void OnInterestRateReq(Proto.InterestRateReq req)
        {
            Debug.Assert(req.Exchange == this.exchange);
            if (req.Type == Proto.RequestType.Set)
            {
                foreach (var pricing in this.pricings)
                {
                    foreach (var kvp in pricing.Value.Parameters)
                    {
                        kvp.Value.Rate = rm.GetInterestRate(kvp.Key);
                        Calculate(pricing.Value, kvp.Key, kvp.Value);
                    }
                }
            }
        }

        private void OnSSRateReq(Proto.SSRateReq req)
        {
            Debug.Assert(req.Exchange == this.exchange);
            if (req.Type == Proto.RequestType.Set)
            {
                foreach (var r in req.Rates)
                {
                    var underlying = pm.FindId(r.Underlying);
                    if (underlying != null)
                    {
                        Pricing pricing = null;
                        if (this.pricings.TryGetValue(underlying, out pricing))
                        {
                            try
                            {
                                var maturity = DateTime.ParseExact(r.Maturity, "yyyyMMdd", CultureInfo.InvariantCulture);
                                PricingParameters parameters = null;
                                if (pricing.Parameters.TryGetValue(maturity, out parameters))
                                {
                                    parameters.SSRate = r.Rate;
                                    Calculate(pricing, maturity, parameters);
                                }
                            }
                            catch (Exception) { }
                        }
                    }
                }
            }
        }

        private void OnVolatilityCurveReq(Proto.VolatilityCurveReq req)
        {
            Debug.Assert(req.Exchange == this.exchange);
            if (req.Type == Proto.RequestType.Set)
            {
                foreach (var curve in req.Curves)
                {
                    var underlying = pm.FindId(curve.Underlying);
                    if (underlying != null)
                    {
                        Pricing pricing = null;
                        if (this.pricings.TryGetValue(underlying, out pricing))
                        {
                            try
                            {
                                var maturity = DateTime.ParseExact(curve.Maturity, "yyyyMMdd", CultureInfo.InvariantCulture);
                                PricingParameters parameters = null;
                                if (pricing.Parameters.TryGetValue(maturity, out parameters))
                                {
                                    parameters.Volatility = new VolatilityParameterWrapper()
                                        {
                                            spot = curve.Spot,
                                            skew = curve.Skew,
                                            atm_vol = curve.AtmVol,
                                            call_convex = curve.CallConvex,
                                            put_convex = curve.PutConvex,
                                            call_slope = curve.CallSlope,
                                            put_slope = curve.PutSlope,
                                            call_cutoff = curve.CallCutoff,
                                            put_cutoff = curve.PutCutoff,
                                            vcr = curve.Vcr,
                                            scr = curve.Scr,
                                            ccr = curve.Ccr,
                                            spcr = curve.Spcr,
                                            sccr = curve.Sccr
                                        };

                                    Calculate(pricing, maturity, parameters);
                                }
                            }
                            catch (Exception) { }
                        }
                    }
                }
            }
        }

        private void OnDestrikerReq(Proto.DestrikerReq req)
        {
            Debug.Assert(req.Exchange == this.exchange);
            if (req.Type == Proto.RequestType.Set)
            {
                foreach (var d in req.Destrikers)
                {
                    var inst = pm.FindId(d.Instrument);
                    if (inst != null)
                    {
                        Pricing pricing = null;
                        if (this.pricings.TryGetValue(inst.HedgeUnderlying, out pricing))
                        {
                            Option option = inst as Option;
                            PricingParameters parameters = null;
                            if (pricing.Parameters.TryGetValue(option.Maturity, out parameters))
                            {
                                Parameter param = null;
                                if (parameters.Parameters.TryGetValue(option, out param))
                                {
                                    param.Destriker = d.Destriker_;
                                    double t = this.epm.GetTimeValue(option.Maturity);
                                    double t1 = this.epm.GetCharmTimeValue(option.Maturity);
                                    if (double.IsNaN(t) == false)
                                    {
                                        CalculatePublishGreeks(option, pricing, parameters, param, t, t1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnPositionReq(Proto.PositionReq req)
        {
            Debug.Assert(req.Exchange == this.exchange);
            if (req.Type == Proto.RequestType.Set)
            {
                foreach (var p in req.Positions)
                {
                    var inst = pm.FindId(p.Instrument);
                    if (inst != null && inst.Type == Proto.InstrumentType.Option)
                    {
                        Pricing pricing = null;
                        if (this.pricings.TryGetValue(inst.HedgeUnderlying, out pricing))
                        {
                            Option option = inst as Option;
                            PricingParameters parameters = null;
                            if (pricing.Parameters.TryGetValue(option.Maturity, out parameters))
                            {
                                Parameter param = null;
                                if (parameters.Parameters.TryGetValue(option, out param))
                                {
                                    param.Position = p.TotalLong - p.TotalShort;
                                    double t = this.epm.GetTimeValue(option.Maturity);
                                    double t1 = this.epm.GetCharmTimeValue(option.Maturity);
                                    if (double.IsNaN(t) == false)
                                    {
                                        CalculatePublishGreeks(option, pricing, parameters, param, t, t1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Calculate(Pricing pricing, DateTime maturity, PricingParameters parameters)
        {
            double t = epm.GetTimeValue(maturity);
            double t1 = epm.GetCharmTimeValue(maturity);
            if (double.IsNaN(t) == false)
            {
                foreach (var param in parameters.Parameters)
                {
                    if (CalculatePublishGreeks(param.Key, pricing, parameters, param.Value, t, t1))
                    {
                        Proto.Price p = null;
                        if (this.prices.TryGetValue(param.Key, out p))
                        {
                            CalculatePublishIV(param.Key, p, pricing, parameters, param.Value.Volatility, t);
                        }
                    }
                }
                //foreach (var option in parameters.Volatilities.Keys.ToList())
                //{
                //    double v = 0;
                //    if (CalculatePublishGreeks(option, pricing, parameters, ref v, t, t1))
                //    {
                //        parameters.Volatilities[option] = v;
                //        Proto.Price p = null;
                //        if (this.prices.TryGetValue(option, out p))
                //        {
                //            CalculatePublishIV(option, p, pricing, parameters, v, t);
                //        }
                //    }
                //}
            }
        }

        private bool CalculatePublishGreeks(Option option, Pricing pricing, PricingParameters parameters, Parameter parameter, double timeValue, double charmTimeValue)
        {
            if (double.IsNaN(pricing.Spot)) return false;
            double s = pricing.Spot;

            if (double.IsNaN(parameters.Rate)) return false;
            double r = parameters.Rate;

            if (double.IsNaN(parameters.SSRate)) return false;
            double q = parameters.SSRate;

            if (parameters.Volatility == null) return false;

            bool future = option.HedgeUnderlying.Type == Proto.InstrumentType.Future;

            //if (option.Id == "m1809-C-2500")
            //{
            //    int i = 9;
            //    ++i;
            //}

            parameters.Volatility.skew += 0.005;
            double v1 = pricing.VolatilityModel.Calculate(parameters.Volatility, future, s, option.Strike, r, q, timeValue);
            parameters.Volatility.skew -= 0.01;
            double v2 = pricing.VolatilityModel.Calculate(parameters.Volatility, future, s, option.Strike, r, q, timeValue);
            parameters.Volatility.skew += 0.005;
            double v = pricing.VolatilityModel.Calculate(parameters.Volatility, future, s, option.Strike, r, q, timeValue);
            parameter.Volatility = v;

            bool call = option.OptionType == Proto.OptionType.Call;
            GreeksDataWrapper greeks = null;
            double delta = 0, skewSensi = 0;
            double callConvexSensi = double.NaN, putConvexSensi = double.NaN;
            if (future)
            {
                greeks = pricing.PricingModel.CalculateGreeks(call, s + q, option.Strike, v, r, r, timeValue);
                if (charmTimeValue != 0)
                {
                    delta = pricing.PricingModel.CalculateDelta(call, s + q, option.Strike, v, r, r, charmTimeValue);
                }
                else if (option.SettlementType == Proto.SettlementType.PhysicalSettlement)
                {
                    if (call)
                    {
                        if (option.Strike < s + q)
                        {
                            delta = 1;
                        }
                    }
                    else if (option.Strike > s + q)
                    {
                        delta = -1;
                    }
                }
                skewSensi = pricing.PricingModel.Calculate(call, s + q, option.Strike, v1, r, r, timeValue) - pricing.PricingModel.Calculate(call, s + q, option.Strike, v2, r, r, timeValue);
                if (option.Strike > s + q)
                {
                    parameters.Volatility.call_convex += 0.005;
                    v1 = pricing.VolatilityModel.Calculate(parameters.Volatility, future, s, option.Strike, r, q, timeValue);
                    parameters.Volatility.call_convex -= 0.01;
                    v2 = pricing.VolatilityModel.Calculate(parameters.Volatility, future, s, option.Strike, r, q, timeValue);
                    parameters.Volatility.call_convex += 0.005;
                    callConvexSensi = pricing.PricingModel.Calculate(call, s + q, option.Strike, v1, r, r, timeValue) - pricing.PricingModel.Calculate(call, s + q, option.Strike, v2, r, r, timeValue);
                }
                else
                {
                    parameters.Volatility.put_convex += 0.005;
                    v1 = pricing.VolatilityModel.Calculate(parameters.Volatility, future, s, option.Strike, r, q, timeValue);
                    parameters.Volatility.put_convex -= 0.01;
                    v2 = pricing.VolatilityModel.Calculate(parameters.Volatility, future, s, option.Strike, r, q, timeValue);
                    parameters.Volatility.put_convex += 0.005;
                    putConvexSensi = pricing.PricingModel.Calculate(call, s + q, option.Strike, v1, r, r, timeValue) - pricing.PricingModel.Calculate(call, s + q, option.Strike, v2, r, r, timeValue);
                    //if (option.Strike == s + q)
                    //{
                    //    putConvexSensi /= 2;
                    //    callConvexSensi = putConvexSensi;
                    //}
                }
            }
            else
            {
                greeks = pricing.PricingModel.CalculateGreeks(call, s, option.Strike, v, r, q, timeValue);
                if (charmTimeValue != 0)
                {
                    delta = pricing.PricingModel.CalculateDelta(call, s, option.Strike, v, r, q, charmTimeValue);
                }
                else if (option.SettlementType == Proto.SettlementType.PhysicalSettlement)
                {
                    if (call)
                    {
                        if (option.Strike < s)
                        {
                            delta = 1;
                        }
                    }
                    else if (option.Strike > s)
                    {
                        delta = -1;
                    }
                }
                skewSensi = pricing.PricingModel.Calculate(call, s, option.Strike, v1, r, q, timeValue) - pricing.PricingModel.Calculate(call, s, option.Strike, v2, r, q, timeValue);

                double tmp = option.Strike * Math.Exp(-1 * ((option.Maturity - DateTime.Now).Days / 365.0) * (r - q));
                if (tmp > parameters.Volatility.spot)
                {
                    parameters.Volatility.call_convex += 0.005;
                    v1 = pricing.VolatilityModel.Calculate(parameters.Volatility, future, s, option.Strike, r, q, timeValue);
                    parameters.Volatility.call_convex -= 0.01;
                    v2 = pricing.VolatilityModel.Calculate(parameters.Volatility, future, s, option.Strike, r, q, timeValue);
                    parameters.Volatility.call_convex += 0.005;
                    callConvexSensi = pricing.PricingModel.Calculate(call, s, option.Strike, v1, r, q, timeValue) - pricing.PricingModel.Calculate(call, s, option.Strike, v2, r, q, timeValue);
                }
                else
                {
                    parameters.Volatility.put_convex += 0.005;
                    v1 = pricing.VolatilityModel.Calculate(parameters.Volatility, future, s, option.Strike, r, q, timeValue);
                    parameters.Volatility.put_convex -= 0.01;
                    v2 = pricing.VolatilityModel.Calculate(parameters.Volatility, future, s, option.Strike, r, q, timeValue);
                    parameters.Volatility.put_convex += 0.005;
                    putConvexSensi = pricing.PricingModel.Calculate(call, s, option.Strike, v1, r, q, timeValue) - pricing.PricingModel.Calculate(call, s, option.Strike, v2, r, q, timeValue);
                    //if (tmp == param.Volatility.spot)
                    //{
                    //    putConvexSensi /= 2;
                    //    callConvexSensi = putConvexSensi;
                    //}
                }
            }

            greeks.theo = Math.Max(greeks.theo - parameter.Position * parameter.Destriker, 0.0);
            var data = new GreeksData()
                {
                    Option = option,
                    Greeks = greeks,
                    Charm = delta - greeks.delta,
                    SkewSensi = skewSensi,
                    CallConvexSensi = callConvexSensi,
                    PutConvexSensi = putConvexSensi,
                };
            this.greeks.AddOrUpdate(option, data, (x, y) => data);
            this.container.Resolve<EventAggregator>().GetEvent<GreeksEvent>().Publish(data);
            return true;
        }

        private void CalculatePublishIV(Option option, Proto.Price price, Pricing pricing, PricingParameters param, double guessVol, double timeValue)
        {
            if (double.IsNaN(pricing.Spot)) return;
            double s = pricing.Spot;

            if (double.IsNaN(param.Rate)) return;
            double r = param.Rate;

            if (double.IsNaN(param.SSRate)) return;
            double q = param.SSRate;

            double lastIV = 0, bidIV = 0, askIV = 0;
            bool future = option.HedgeUnderlying.Type == Proto.InstrumentType.Future;
            if (future)
            {
                bool call = option.OptionType == Proto.OptionType.Call;
                if (price.Last.Price > 0)
                {
                    lastIV = pricing.PricingModel.CalculateIV(call, guessVol, price.Last.Price, s + q, option.Strike, r, r, timeValue);
                }
                if (price.Bids.Count > 0 && price.Bids[0].Price > 0)
                {
                    bidIV = pricing.PricingModel.CalculateIV(call, guessVol, price.Bids[0].Price, s + q, option.Strike, r, r, timeValue);
                }
                if (price.Asks.Count > 0 && price.Asks[0].Price > 0)
                {
                    askIV = pricing.PricingModel.CalculateIV(call, guessVol, price.Asks[0].Price, s + q, option.Strike, r, r, timeValue);
                }
            }
            else
            {
                bool call = option.OptionType == Proto.OptionType.Call;
                if (price.Last.Price > 0)
                {
                    lastIV = pricing.PricingModel.CalculateIV(call, guessVol, price.Last.Price, s, option.Strike, r, q, timeValue);
                }
                if (price.Bids.Count > 0 && price.Bids[0].Price > 0)
                {
                    bidIV = pricing.PricingModel.CalculateIV(call, guessVol, price.Bids[0].Price, s, option.Strike, r, q, timeValue);
                }
                if (price.Asks.Count > 0 && price.Asks[0].Price > 0)
                {
                    askIV = pricing.PricingModel.CalculateIV(call, guessVol, price.Asks[0].Price, s, option.Strike, r, q, timeValue);
                }
            }

            var data = new ImpliedVolatilityData() { Option = option, LastIV = lastIV, BidIV = bidIV, AskIV = askIV };
            this.impliedVolatilities.AddOrUpdate(option, data, (x, y) => data);
            this.container.Resolve<EventAggregator>().GetEvent<ImpliedVolatilityEvent>().Publish(data);
        }

        IUnityContainer container;
        Proto.Exchange exchange;

        class Parameter
        {
            public int Position { get; set; }
            public double Destriker { get; set; }
            public double Volatility { get; set; }
        }

        class PricingParameters
        {
            public double Rate { get; set; }
            public double SSRate { get; set; }
            public VolatilityParameterWrapper Volatility { get; set; }
            public Dictionary<Option, Parameter> Parameters { get; set; }
        }

        class Pricing
        {
            public PricingModelWrapper PricingModel { get; set; }
            public VolatilityModelWrapper VolatilityModel { get; set; }

            public double Spot { get; set; }
            public Dictionary<DateTime, PricingParameters> Parameters { get; set; }
        }

        ConcurrentDictionary<Instrument, Proto.Price> prices = new ConcurrentDictionary<Instrument,Proto.Price>();
        ConcurrentDictionary<Option, GreeksData> greeks = new ConcurrentDictionary<Option, GreeksData>();
        ConcurrentDictionary<Option, ImpliedVolatilityData> impliedVolatilities = new ConcurrentDictionary<Option, ImpliedVolatilityData>();

        Dictionary<Instrument, Pricing> pricings;
        Dictionary<Type, Action<IMessage>> actions;

        private BlockingCollection<IMessage> messages = new BlockingCollection<IMessage>();
        private volatile bool running = false;
        private Thread thread;

        private ProductManager pm = null;
        private ExchangeParameterManager epm = null;
        private InterestRateManager rm = null;
        private SSRateManager ssm = null;
        private VolatilityCurveManager vcm = null;
        private PositionManager pnm = null;
        private DestrikerManager dm = null;
    }
}
