using System;
using System.Collections.Generic;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;

/*
   Enhanced version of QuantumSignals with Trade Direction Logic
   - Added Trade Direction parameter to control trade types
   - Supports: Both, Long Only, Short Only, Disabled
   - Modified trade rules to respect direction settings
   
   QS Version 1.1 - Trade Direction Enhancement
*/

namespace cAlgo.Robots
{
    public enum TradeDirection
    {
        Both,
        LongOnly,
        ShortOnly,
        Disabled
    }

    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None, AddIndicators = true)]
    public class QuantumSignals: Robot
    {
        // =========================
        // Trade Direction Control
        // =========================
        [Parameter("Trade Direction", Group = "Trade Control", DefaultValue = TradeDirection.Both)]
        public TradeDirection Direction { get; set; }

        // =========================
        // Protection
        // =========================
        [Parameter("Trade Cooldown (minutes)", Group = "Protection", DefaultValue = 15, MinValue = 0)]
		public int CooldownMinutes { get; set; }		
		
		[Parameter("Quantity (Lots)", Group = "Protection", DefaultValue = 0.01, MinValue = 0.01, Step = 0.01)]
        public double Quantity { get; set; }

        [Parameter("Stop loss", Group = "Protection", DefaultValue = 100)]
        public int StopLoss { get; set; }

        [Parameter("Take profit", Group = "Protection", DefaultValue = 100)]
        public int TakeProfit { get; set; }

        [Parameter("Max Spread", Group = "Protection", DefaultValue = 2.5, MinValue = 0.0)]
        public double MaxSpread { get; set; }

        // =========================
        // Trailing Stop Loss
        // =========================
        [Parameter("Trigger (pips)", Group = "Trailing Stop Loss", DefaultValue = 14)]
        public int TrailingStopTrigger { get; set; }

        [Parameter("Step (pips)", Group = "Trailing Stop Loss", DefaultValue = 14)]
        public int TrailingStopStep { get; set; }

        // =========================
        // Break Even Stop
        // =========================
        [Parameter("Trigger (pips)", Group = "Break Even Stop", DefaultValue = 14, MinValue = 1)]
        public double TriggerPips { get; set; }

        [Parameter("Extra (pips)", Group = "Break Even Stop", DefaultValue = 0, MinValue = 0.0)]
        public double AddPips { get; set; }

        // =========================
        // Indicator settings for bullish signals
        // =========================
        [Parameter("Timeframe", DefaultValue = "h1", Group = "Bollinger Bands Buy #1")]
        public TimeFrame BollingerTimeFrameBuy1 { get; set; }

        [Parameter("Periods", DefaultValue = 20, Group = "Bollinger Bands Buy #1")]
        public int BollingerPeriodsBuy1 { get; set; }

        [Parameter("Standard Dev", DefaultValue = 2.0, Group = "Bollinger Bands Buy #1")]
        public double BollingerDeviationBuy1 { get; set; }

        [Parameter("Ma Type ", DefaultValue = MovingAverageType.Exponential, Group = "Bollinger Bands Buy #1")]
        public MovingAverageType BollingerMaTypeBuy1 { get; set; }

        [Parameter("Timeframe", DefaultValue = "h1", Group = "MACD Crossover Buy #2")]
        public TimeFrame MacdTimeFrameBuy2 { get; set; }

        [Parameter("Signal Periods", DefaultValue = 10, Group = "MACD Crossover Buy #2")]
        public int MacdPeriodsBuy2 { get; set; }

        [Parameter("MACD Lower", DefaultValue = -0.001, Group = "MACD Crossover Buy #2")]
        public double MacdLowerBuy2 { get; set; }

        [Parameter("Long Cycle", DefaultValue = 25, Group = "MACD Crossover Buy #2")]
        public int MacdLongCycleBuy2 { get; set; }

        [Parameter("Short Cycle", DefaultValue = 12, Group = "MACD Crossover Buy #2")]
        public int MacdShortCycleBuy2 { get; set; }

        [Parameter("Timeframe", DefaultValue = "h1", Group = "Stochastic Oscillator Buy #3")]
        public TimeFrame StochasticTimeFrameBuy3 { get; set; }

        [Parameter("%K Periods", DefaultValue = 9, Group = "Stochastic Oscillator Buy #3")]
        public int StochasticKPeriodsBuy3 { get; set; }

        [Parameter("%K Slowing", DefaultValue = 3, Group = "Stochastic Oscillator Buy #3")]
        public int StochasticKslowBuy3 { get; set; }

        [Parameter("%D Periods", DefaultValue = 9, Group = "Stochastic Oscillator Buy #3")]
        public int StochasticDperiodsBuy3 { get; set; }

        [Parameter("Ma Type ", DefaultValue = MovingAverageType.Exponential, Group = "Stochastic Oscillator Buy #3")]
        public MovingAverageType StochasticMaTypeBuy3 { get; set; }

        [Parameter("Lower Level", DefaultValue = 20, Group = "Stochastic Oscillator Buy #3")]
        public int StochasticLowerBuy3 { get; set; }

        // =========================
        // Indicator settings for bearish signals
        // =========================
        [Parameter("Timeframe", DefaultValue = "h1", Group = "Bollinger Bands Sell #1")]
        public TimeFrame BollingerTimeFrameSell1 { get; set; }

        [Parameter("Periods", DefaultValue = 20, Group = "Bollinger Bands Sell #1")]
        public int BollingerPeriodsSell1 { get; set; }

        [Parameter("Standard Dev", DefaultValue = 2.0, Group = "Bollinger Bands Sell #1")]
        public double BollingerDeviationSell1 { get; set; }

        [Parameter("Ma Type ", DefaultValue = MovingAverageType.Exponential, Group = "Bollinger Bands Sell #1")]
        public MovingAverageType BollingerMaTypeSell1 { get; set; }

        [Parameter("Timeframe", DefaultValue = "h1", Group = "MACD Crossover Sell #2")]
        public TimeFrame MacdTimeFrameSell2 { get; set; }

        [Parameter("Signal Periods", DefaultValue = 10, Group = "MACD Crossover Sell #2")]
        public int MacdPeriodsSell2 { get; set; }

        [Parameter("MACD Upper", DefaultValue = 0.001, Group = "MACD Crossover Sell #2")]
        public double MacdUpperSell2 { get; set; }

        [Parameter("Long Cycle", DefaultValue = 25, Group = "MACD Crossover Sell #2")]
        public int MacdLongCycleSell2 { get; set; }

        [Parameter("Short Cycle", DefaultValue = 12, Group = "MACD Crossover Sell #2")]
        public int MacdShortCycleSell2 { get; set; }

        [Parameter("Timeframe", DefaultValue = "h1", Group = "Stochastic Oscillator Sell #3")]
        public TimeFrame StochasticTimeFrameSell3 { get; set; }

        [Parameter("%K Periods", DefaultValue = 9, Group = "Stochastic Oscillator Sell #3")]
        public int StochasticKPeriodsSell3 { get; set; }

        [Parameter("%K Slowing", DefaultValue = 3, Group = "Stochastic Oscillator Sell #3")]
        public int StochasticKslowSell3 { get; set; }

        [Parameter("%D Periods", DefaultValue = 9, Group = "Stochastic Oscillator Sell #3")]
        public int StochasticDperiodsSell3 { get; set; }

        [Parameter("Ma Type ", DefaultValue = MovingAverageType.Exponential, Group = "Stochastic Oscillator Sell #3")]
        public MovingAverageType StochasticMaTypeSell3 { get; set; }

        [Parameter("Upper Level", DefaultValue = 80, Group = "Stochastic Oscillator Sell #3")]
        public int StochasticUpperSell3 { get; set; }

        // =========================
        // Indicators & Bars
        // =========================
        private BollingerBands _bollinger_buy1;
        private MacdCrossOver _macd_Buy2;
        private StochasticOscillator _stochastic_buy3;

        private BollingerBands _bollinger_sell1;
        private MacdCrossOver _macd_Sell2;
        private StochasticOscillator _stochastic_sell3;

        private Bars _bollingerBars_Buy1;
        private Bars _macdBars_Buy2;

        private Bars _bollingerBars_Sell1;
        private Bars _macdBars_Sell2;
		
        // =========================
        // State
        // =========================
        private string StrategyName { get; set; }
        bool IsBullish { get; set; }
        bool IsBearish { get; set; }

        // --- Track last trade time ---
		private DateTime _lastTradeTime = DateTime.MinValue;
		
		// =========================
        // Start
        // =========================
        protected override void OnStart()
        {
            StrategyName = "QuantumSignals";
            
            Print($"QuantumSignals started with Trade Direction: {Direction}");

            _bollingerBars_Buy1 = MarketData.GetBars(BollingerTimeFrameBuy1);
            _macdBars_Buy2 = MarketData.GetBars(MacdTimeFrameBuy2);

            _bollingerBars_Sell1 = MarketData.GetBars(BollingerTimeFrameSell1);
            _macdBars_Sell2 = MarketData.GetBars(MacdTimeFrameSell2);

            _bollinger_buy1 = Indicators.BollingerBands(_bollingerBars_Buy1.ClosePrices, BollingerPeriodsBuy1, BollingerDeviationBuy1, BollingerMaTypeBuy1);
            _macd_Buy2 = Indicators.MacdCrossOver(_macdBars_Buy2.ClosePrices, MacdLongCycleBuy2, MacdShortCycleBuy2, MacdPeriodsBuy2);
            _stochastic_buy3 = Indicators.StochasticOscillator(StochasticKPeriodsBuy3, StochasticKslowBuy3, StochasticDperiodsBuy3, StochasticMaTypeBuy3);

            _bollinger_sell1 = Indicators.BollingerBands(_bollingerBars_Sell1.ClosePrices, BollingerPeriodsSell1, BollingerDeviationSell1, BollingerMaTypeSell1);
            _macd_Sell2 = Indicators.MacdCrossOver(_macdBars_Sell2.ClosePrices, MacdLongCycleSell2, MacdShortCycleSell2, MacdPeriodsSell2);
            _stochastic_sell3 = Indicators.StochasticOscillator(StochasticKPeriodsSell3, StochasticKslowSell3, StochasticDperiodsSell3, StochasticMaTypeSell3);
        }

        // =========================
        // Tick
        // =========================
        protected override void OnTick()
        {
            SetTrailingStop();
            SetBreakEvenStop();
        }

        // =========================
        // On Bar Closed
        // =========================
        protected override void OnBarClosed()
        {
            // Skip all trading if disabled
            if (Direction == TradeDirection.Disabled)
                return;

            // Only check bullish signals if direction allows long trades
            if (Direction == TradeDirection.Both || Direction == TradeDirection.LongOnly)
            {
                TradeRulesBullish();
            }

            // Only check bearish signals if direction allows short trades
            if (Direction == TradeDirection.Both || Direction == TradeDirection.ShortOnly)
            {
                TradeRulesBearish();
            }
        }

        // =========================
        // Trade Rules
        // =========================
        private void TradeRulesBullish()
        {
            bool openBuy = IsBullishSignal();

            if (openBuy)
            {
                if (!IsBullish && !IsTradeOpen(TradeType.Buy))
                    OpenTrade(TradeType.Buy);

                IsBullish = true;
            }
            else
            {
                IsBullish = false;
            }
        }

        private void TradeRulesBearish()
        {
            bool openSell = IsBearishSignal();

            if (openSell)
            {
                if (!IsBearish && !IsTradeOpen(TradeType.Sell))
                    OpenTrade(TradeType.Sell);

                IsBearish = true;
            }
            else
            {
                IsBearish = false;
            }
        }

        // =========================
        // Signal Logic
        // =========================
        private bool IsBullishSignal()
        {
            // Bollinger: look for price probing/bouncing off lower band recently
            bool boll = Bars.LowPrices.Last(0) <= _bollinger_buy1.Bottom.Last(0) + (2 * Symbol.PipSize);
            if (!boll) return false;

            // MACD: bullish crossover region & below lower threshold (oversold momentum)
            bool macd = _macd_Buy2.MACD.LastValue > _macd_Buy2.Signal.LastValue
                        && _macd_Buy2.Signal.LastValue < MacdLowerBuy2;
            if (!macd) return false;

            // Stochastic: K crosses above D and prior K was below lower band
            bool stoch = _stochastic_buy3.PercentK.HasCrossedAbove(_stochastic_buy3.PercentD, 0)
                         && _stochastic_buy3.PercentK.Last(1) < StochasticLowerBuy3;
            if (!stoch) return false;

            return true;
        }

        private bool IsBearishSignal()
        {
            // Bollinger: look for price probing/bouncing off upper band recently
            bool boll = Bars.HighPrices.Last(0) >= _bollinger_sell1.Top.Last(0)
                        || Bars.HighPrices.Last(1) >= _bollinger_sell1.Top.Last(1);
            if (!boll) return false;

            // MACD: bearish crossover region & above upper threshold (overbought momentum)
            bool macd = _macd_Sell2.MACD.LastValue < _macd_Sell2.Signal.LastValue
                        && _macd_Sell2.Signal.LastValue > MacdUpperSell2;
            if (!macd) return false;

            // Stochastic: K crosses below D and prior K was above upper band
            bool stoch = _stochastic_sell3.PercentK.HasCrossedBelow(_stochastic_sell3.PercentD, 0)
                         && _stochastic_sell3.PercentK.Last(1) > StochasticUpperSell3;
            if (!stoch) return false;

            return true;
        }

        // =========================
        // Helpers
        // =========================
        private double GetSpread()
        {
            return (Symbol.Ask - Symbol.Bid) / Symbol.PipSize;
        }

        private void OpenTrade(TradeType type)
        {
            // Additional check: ensure trade type is allowed by direction setting
            if (!IsTradeTypeAllowed(type))
            {
                Print($"Trade type {type} not allowed with current direction setting: {Direction}");
                return;
            }

            if (GetSpread() > MaxSpread)
            {
                Print("No trade opened due to spread being greater than maximum allowed.");
                return;
            }

            if (CooldownMinutes > 0 && _lastTradeTime != DateTime.MinValue)
            {
			var minutesSince = (Server.Time - _lastTradeTime).TotalMinutes;

			if (minutesSince < CooldownMinutes)
			{
				Print($"Cooldown active. {CooldownMinutes - minutesSince:F1} minutes remaining.");
				return; // block trade if cooldown not expired
			}
		}	
			
			var volume = Symbol.QuantityToVolumeInUnits(Quantity);
            volume = Symbol.NormalizeVolumeInUnits(volume, RoundingMode.Down);

            ExecuteMarketOrder(type, SymbolName, volume, StrategyName, StopLoss, TakeProfit);
			_lastTradeTime = Server.Time;
            
            Print($"Opened {type} trade - Direction: {Direction}");
        }

        private bool IsTradeTypeAllowed(TradeType type)
        {
            switch (Direction)
            {
                case TradeDirection.Both:
                    return true;
                case TradeDirection.LongOnly:
                    return type == TradeType.Buy;
                case TradeDirection.ShortOnly:
                    return type == TradeType.Sell;
                case TradeDirection.Disabled:
                    return false;
                default:
                    return false;
            }
        }

        private bool IsTradeOpen(TradeType type)
        {
            var positions = Positions.FindAll(StrategyName, SymbolName, type);
            return positions.Any();
        }

        // =========================
        // Risk Management: Trailing Stop (Improved)
        // =========================
        private void SetTrailingStop()
        {
		double trigger = TrailingStopTrigger * Symbol.PipSize;
		double step = TrailingStopStep * Symbol.PipSize;

		// --- For SELL positions ---
		var sellPositions = Positions.FindAll(StrategyName, SymbolName, TradeType.Sell);
		foreach (var position in sellPositions)
		{
			double distance = position.EntryPrice - Symbol.Ask; // profit distance in price
			if (distance < trigger) continue;

			double newSL = Symbol.Ask + step; // SL should trail above current Ask
			// Only move SL if it improves (lower for shorts)
			if (position.StopLoss == null || newSL < position.StopLoss - Symbol.PipSize)
			{
				ModifyPosition(position, newSL, position.TakeProfit, ProtectionType.Absolute);
			}
		}
		
		// --- For BUY positions ---
		var buyPositions = Positions.FindAll(StrategyName, SymbolName, TradeType.Buy);
		foreach (var position in buyPositions)
		{
			double distance = Symbol.Bid - position.EntryPrice; // profit distance in price
			if (distance < trigger) continue;

			double newSL = Symbol.Bid - step; // SL should trail below current Bid
			// Only move SL if it improves (higher for longs)
			if (position.StopLoss == null || newSL > position.StopLoss + Symbol.PipSize)
			{
				ModifyPosition(position, newSL, position.TakeProfit, ProtectionType.Absolute);
				}
			}
		}

        // =========================
        // Risk Management: Break-even (Improved)
        // =========================
        private void SetBreakEvenStop()
		{
			double trigger = TriggerPips * Symbol.PipSize;
			double extra = AddPips * Symbol.PipSize;

			var allPositions = Positions.FindAll(StrategyName, SymbolName);
			foreach (var position in allPositions)
			{
				double entry = position.EntryPrice;
				double distance = position.TradeType == TradeType.Buy ? Symbol.Bid - entry : entry - Symbol.Ask;

				if (distance < trigger) continue;

				if (position.TradeType == TradeType.Buy)
				{
					double be = entry + extra; // BE + buffer
					// Only move SL if it's null or still below BE
					if (position.StopLoss == null || position.StopLoss < be - Symbol.PipSize)
					{
						ModifyPosition(position, be, position.TakeProfit, ProtectionType.Absolute);
					}
				}
				else
				{
					double be = entry - extra; // BE - buffer
					// Only move SL if it's null or still above BE
					if (position.StopLoss == null || position.StopLoss > be + Symbol.PipSize)
                    {
                        ModifyPosition(position, be, position.TakeProfit, ProtectionType.Absolute);
                    }
                }
            }
        }
    }
}