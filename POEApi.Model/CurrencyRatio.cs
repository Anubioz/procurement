namespace POEApi.Model
{
    public class CurrencyRatio
    {
        public OrbType OrbType { get; set; }
        public double OrbAmount { get; set; }
        public double GCPAmount { get; set; }

        public CurrencyRatio(OrbType orbType, double OrbAmount, double GCPAmount)
        {
            this.OrbType = orbType;
            this.OrbAmount = OrbAmount;
            this.GCPAmount = GCPAmount;
        }
    }
}
