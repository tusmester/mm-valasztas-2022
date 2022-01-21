namespace LakossagStat.Data
{
    public class DataAnalyzer
    {
        private LakossagData Data { get; }

        public DataAnalyzer(LakossagData data)
        {
            Data = data;
        }

        public void Analyze()
        {
            ComputeIncreaseDecrease();
        }

        private void ComputeIncreaseDecrease()
        {
            foreach (var item in Data.OevkItems)
            {
                // get the difference between the FIRST and LAST item
                var currentDiff = item.Difference;
                var currentDiffPercent = item.DifferencePercent;

                // if it is an increase
                if (currentDiff >= 0)
                {
                    // Memorize value and percentage separately, because these may
                    // refer to different places.
                    if (currentDiff > (Data.MaxIncrease?.Difference ?? 0))
                    {
                        Data.MaxIncrease = item;
                    }
                    if (currentDiffPercent > (Data.MaxIncreasePercent?.DifferencePercent ?? 0))
                    {
                        Data.MaxIncreasePercent = item;
                    }
                }
                else
                {
                    if (currentDiff < (Data.MaxDecrease?.Difference ?? 0))
                    {
                        Data.MaxDecrease = item;
                    }
                    if (currentDiffPercent < (Data.MaxDecreasePercent?.DifferencePercent ?? 0))
                    {
                        Data.MaxDecreasePercent = item;
                    }
                }
            }
        }
    }
}
