using System.Globalization;

namespace Kysect.GithubUtils.Contributions
{
    public class ContributionsInfo
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }

        public ContributionsInfo(string dateAsString, int count)
        {
            Count = count;
            Date = DateTime.Parse(dateAsString, CultureInfo.InvariantCulture);
        }

        public ContributionsInfo()
        {
        }
    }
}