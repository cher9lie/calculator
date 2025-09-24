using System;
using System.Collections.Generic;
using System.Linq;

namespace WindowsFormsCalculator
{
    /// <summary>
    /// Represents a single history item in the calculation history
    /// Similar to the HISTORYITEM structure in the C++ implementation
    /// </summary>
    public class HistoryItem
    {
        public string Expression { get; set; }
        public string Result { get; set; }
        public DateTime Timestamp { get; set; }
        public CalculatorMode Mode { get; set; }

        public HistoryItem(string expression, string result, CalculatorMode mode)
        {
            Expression = expression;
            Result = result;
            Mode = mode;
            Timestamp = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{Expression} = {Result}";
        }
    }

    /// <summary>
    /// Calculation history management, inspired by the C++ CalculatorHistory class
    /// Maintains separate histories for different calculator modes
    /// </summary>
    public class CalculationHistory
    {
        private readonly List<HistoryItem> _standardHistory;
        private readonly List<HistoryItem> _scientificHistory;
        private const int MAX_HISTORY_ITEMS = 100;

        public CalculationHistory()
        {
            _standardHistory = new List<HistoryItem>();
            _scientificHistory = new List<HistoryItem>();
        }

        /// <summary>
        /// Add a calculation to the history
        /// </summary>
        public void AddCalculation(string expression, string result, CalculatorMode mode)
        {
            var historyItem = new HistoryItem(expression, result, mode);
            var targetHistory = GetHistoryForMode(mode);

            // Remove oldest item if we've reached the maximum
            if (targetHistory.Count >= MAX_HISTORY_ITEMS)
            {
                targetHistory.RemoveAt(0);
            }

            targetHistory.Add(historyItem);
        }

        /// <summary>
        /// Get history items for the specified mode
        /// </summary>
        public List<HistoryItem> GetHistory(CalculatorMode mode)
        {
            return new List<HistoryItem>(GetHistoryForMode(mode));
        }

        /// <summary>
        /// Get all history items regardless of mode
        /// </summary>
        public List<HistoryItem> GetAllHistory()
        {
            var allHistory = new List<HistoryItem>();
            allHistory.AddRange(_standardHistory);
            allHistory.AddRange(_scientificHistory);
            return allHistory.OrderBy(h => h.Timestamp).ToList();
        }

        /// <summary>
        /// Clear history for the specified mode
        /// </summary>
        public void ClearHistory(CalculatorMode mode)
        {
            GetHistoryForMode(mode).Clear();
        }

        /// <summary>
        /// Clear all history
        /// </summary>
        public void ClearAllHistory()
        {
            _standardHistory.Clear();
            _scientificHistory.Clear();
        }

        /// <summary>
        /// Remove a specific history item
        /// </summary>
        public bool RemoveHistoryItem(HistoryItem item)
        {
            return _standardHistory.Remove(item) || _scientificHistory.Remove(item);
        }

        /// <summary>
        /// Get the count of history items for the specified mode
        /// </summary>
        public int GetHistoryCount(CalculatorMode mode)
        {
            return GetHistoryForMode(mode).Count;
        }

        /// <summary>
        /// Get the most recent calculation for the specified mode
        /// </summary>
        public HistoryItem GetMostRecent(CalculatorMode mode)
        {
            var history = GetHistoryForMode(mode);
            return history.Count > 0 ? history[history.Count - 1] : null;
        }

        private List<HistoryItem> GetHistoryForMode(CalculatorMode mode)
        {
            return mode == CalculatorMode.Standard ? _standardHistory : _scientificHistory;
        }

        /// <summary>
        /// Export history as formatted text
        /// </summary>
        public string ExportHistory(CalculatorMode mode)
        {
            var history = GetHistoryForMode(mode);
            var modeText = mode == CalculatorMode.Standard ? "Standard" : "Scientific";
            var result = $"Calculator History - {modeText} Mode\n";
            result += new string('=', 40) + "\n";

            foreach (var item in history)
            {
                result += $"{item.Timestamp:yyyy-MM-dd HH:mm:ss} - {item}\n";
            }

            return result;
        }
    }
}