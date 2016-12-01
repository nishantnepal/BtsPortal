using System.Collections.Generic;
using BtsPortal.Entities.Bam;

namespace BtsPortal.Repositories.Interface
{
    public interface IBamRepository
    {
        bool BamInstalled();
        List<Activity> LoadActivities();
        BamResult LoadData(ActivityView activityView, int page, int defaultPagesize, Dictionary<string, string> parameters);
    }
}
