namespace MyWarez.Core
{
    public interface ICsv : IPlainTextFile
    {
        // TODO: default methods to get rows and columns base on Text?
    }

    public class Csv : IPayload, ICsv
    {
        public Csv() { }
        public Csv(string csv)
        {
            Text = csv;
        }
        public PayloadType Type => PayloadType.Csv;
        public virtual string Text { get; }
    }
}
