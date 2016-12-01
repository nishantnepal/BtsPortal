using System.Collections.Generic;
using BtsPortal.Entities.Bam;

namespace BtsPortal.Repositories.Interface
{
    public interface IBtsPortalRepository
    {
        List<ActivityView> LoadActivitiesViews(bool activeOnly = false);
        void SaveActivityView(ActivityView view, string currentUser, List<ActivityViewFilterParameter> viewFilterParameters, bool currentFiltersCleared);
        ActivityView LoadActivityView(int id);
        void ToggleViewConfiguration(int id, bool currentState);
    }
}
