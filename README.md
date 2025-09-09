# QuantumSignals cBot

A sophisticated multi-timeframe trading algorithm for cTrader that combines technical indicators with advanced risk management and volatility filtering.

## 🚀 Overview

QuantumSignals is an automated trading robot (cBot) that uses a confluence of three powerful technical indicators across multiple timeframes to generate high-probability trading signals. The strategy incorporates ATR-based volatility filtering and dynamic risk management to adapt to changing market conditions.

## 📊 Trading Logic

### Core Strategy
The bot uses a **three-pillar approach** for signal generation:

1. **Bollinger Bands** - Identifies mean reversion opportunities
   - **Buy Signal**: Price touches or penetrates lower band (oversold conditions)
   - **Sell Signal**: Price touches or penetrates upper band (overbought conditions)

2. **MACD Crossover** - Confirms momentum direction
   - **Buy Signal**: MACD crosses above signal line with signal below threshold
   - **Sell Signal**: MACD crosses below signal line with signal above threshold

3. **Stochastic Oscillator** - Validates entry timing
   - **Buy Signal**: %K crosses above %D from oversold territory (<20)
   - **Sell Signal**: %K crosses below %D from overbought territory (>80)

### Signal Confluence
**All three conditions must align** for a trade signal to be generated, ensuring high-probability setups.

## 🎯 Key Features

### Trade Direction Control
- **Both**: Allow long and short trades (default)
- **Long Only**: Buy trades only
- **Short Only**: Sell trades only  
- **Disabled**: No trading (useful for analysis/maintenance)

### ATR Volatility Filtering
- **Volatility Range Filter**: Only trades when ATR is within specified multiples of average
- **Dynamic Stop Loss/Take Profit**: Automatically adjusts SL/TP based on market volatility
- **Market Adaptation**: Wider stops in volatile markets, tighter in calm conditions

### Advanced Risk Management
- **Trailing Stop Loss**: Protects profits as trades move favorably
- **Break-Even Stop**: Moves stop to entry + buffer after specified profit
- **Trade Cooldown**: Prevents overtrading with configurable time delays
- **Spread Protection**: Blocks trades during wide spread conditions
- **Position Sizing**: Configurable lot sizes with proper volume normalization

## 📋 Parameters

### Trade Control
- **Trade Direction**: Control allowed trade types (Both/Long Only/Short Only/Disabled)

### ATR Settings
- **Enable ATR Filter**: Toggle volatility-based trade filtering
- **ATR Periods**: Calculation period for Average True Range (default: 14)
- **Min/Max ATR Multiplier**: Acceptable volatility range (1.0x - 3.0x)
- **Dynamic SL/TP**: Use ATR-based stops instead of fixed pips
- **SL/TP ATR Multipliers**: Stop loss and take profit as ATR multiples

### Protection
- **Trade Cooldown**: Minimum minutes between trades (default: 15)
- **Quantity**: Trade size in lots (default: 0.01)
- **Stop Loss/Take Profit**: Fixed pip values (overridden if dynamic enabled)
- **Max Spread**: Maximum allowed spread for trade execution

### Indicator Settings
Independent configuration for buy and sell signals:
- **Bollinger Bands**: Periods, standard deviation, MA type, timeframe
- **MACD**: Long/short cycles, signal periods, thresholds, timeframe  
- **Stochastic**: %K/%D periods, slowing, MA type, levels, timeframe

## 🔧 Installation

1. Download the `QuantumSignals.cs` file
2. Open cTrader and navigate to Automate → cBots
3. Click "Add cBot" and select the downloaded file
4. Build the project (Ctrl+Shift+B)
5. Configure parameters and start trading

## 📈 Usage Recommendations

### Market Conditions
- **Trending Markets**: Use "Long Only" in uptrends, "Short Only" in downtrends
- **Ranging Markets**: Use "Both" for mean reversion opportunities
- **High Volatility**: Increase ATR max multiplier, use dynamic SL/TP
- **Low Volatility**: Decrease ATR min multiplier, consider tighter stops

### Optimization Tips
- Test different timeframe combinations for each indicator
- Optimize ATR multipliers for your specific market/timeframe
- Adjust cooldown periods based on market speed
- Compare performance with/without ATR filtering

### Risk Management
- Always use appropriate position sizing
- Monitor drawdown and adjust parameters accordingly
- Consider market sessions and news events
- Regular backtesting on different market periods

## 🤝 Contributing

This project welcomes collaboration from the trading community! Whether you're a developer, trader, or quantitative analyst, your contributions can help improve this strategy.

### Ways to Contribute
- **Fork the repository**: Create feature branch
- **Make your changes**: Submit pull request
- **Code Improvements**: Optimize algorithms, add new features
- **Strategy Enhancement**: Suggest new indicators or filters  
- **Bug Fixes**: Report and fix issues
- **Documentation**: Improve guides and explanations
- **Testing**: Share backtest results and optimization findings

### Development Guidelines
- Follow C# coding standards
- Test thoroughly before submitting changes
- Document new parameters and features
- Maintain backward compatibility where possible

## 📝 Version History

- **v1.2**: ATR integration with volatility filtering and dynamic SL/TP
- **v1.1**: Trade direction control implementation
- **v1.0**: Core multi-indicator strategy with risk management

## ⚖️ Disclaimer

This trading robot is provided for educational and research purposes. Trading carries significant risk of loss. Always test strategies thoroughly on demo accounts before live trading. Past performance does not guarantee future results.

## 🙏 Acknowledgments

Special thanks to **Claude AI** for assistance in code optimization, ATR integration, and strategy enhancement. The collaborative development process helped identify and resolve critical issues while implementing advanced features.

---

**Happy Trading!** 📊✨

*For questions, suggestions, or collaboration opportunities, please open an issue or submit a pull request.*