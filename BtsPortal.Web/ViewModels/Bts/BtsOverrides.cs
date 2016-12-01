using BtsPortal.Entities.Bts;
using BtsPortal.Web.Infrastructure.Settings;

namespace BtsPortal.Web.ViewModels.Bts
{
    //public class BtsInstanceVm : BtsInstance
    //{
    //    public string DateSuspendedDisplay
    //    {
    //        get
    //        {
    //            if (DateSuspended.HasValue)
    //            {
    //                return DateSuspended.Value.ToLocalTime().ToString(AppSettings.DateTimeDisplay);
    //            }
    //            return string.Empty;
    //        }
    //    }
    //}

        /*
    public class BtsReceiveLocationVm : BtsReceiveLocation
    {
        public string StartEndDate
        {
            get
            {
                if (!StartEndDateEnabled)
                {
                    return string.Empty;
                }

                return string.Format("Schedule Dates <br/> From : '{0}' To: '{1}'",
                    StartDate.Value.ToString(AppSettings.DateDisplay),
                        EndDate.Value.ToString(AppSettings.DateDisplay));
            }
        }

        public string FromToTime
        {
            get
            {
                if (!ServiceWindowEnabled)
                {
                    return string.Empty;
                }

                return string.Format("Service Window <br/> From: '{0}' To: {1}",
                    FromTime.Value.ToString(AppSettings.TimeDisplay),
                        ToTime.Value.ToString(AppSettings.TimeDisplay));
            }
        }
    }

    public class BtsInstanceDetailVm : BtsInstanceDetail
    {
        public string SuspendDateTimeDisplay
        {
            get
            {
                if (SuspendDateTime.HasValue)
                {
                    return SuspendDateTime.Value.ToString(AppSettings.DateTimeDisplay);
                }
                return string.Empty;
            }
        }
    }

    */
}