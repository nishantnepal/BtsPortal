using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BtsPortal.Entities.Bam;
using BtsPortal.Repositories.Interface;
using BtsPortal.Repositories.Utilities;
using Dapper;

namespace BtsPortal.Repositories.Db
{
    public class BtsPortalRepository : IBtsPortalRepository
    {
        readonly IDbConnection _dbConnection;

        public BtsPortalRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public virtual List<ActivityView> LoadActivitiesViews(bool activeOnly = false)
        {
            string query = @"
                        SELECT [Id]
      ,[ActivityName]
      ,[ViewName]  ,[InsertedBy]
      ,[InsertedDate]
      ,[LastUpdatedBy]
      ,[LastUpdatedDate]   ,IsActive
  FROM [BamActivityView] (NOLOCK) ";

            if (activeOnly)
            {
                query = @"
                        SELECT [Id]
      ,[ActivityName]
      ,[ViewName]  ,[InsertedBy]
      ,[InsertedDate]
      ,[LastUpdatedBy]
      ,[LastUpdatedDate]   ,IsActive
  FROM [BamActivityView] (NOLOCK) where IsActive = 1";
            }

            var vm = _dbConnection.Query<ActivityView>(query).ToList();

            return vm;
        }

        public void SaveActivityView(ActivityView view, string currentUser, List<ActivityViewFilterParameter> viewFilterParameters, bool currentFiltersCleared)
        {
            const string query = @"
                        update BamActivityView
set ViewName = @viewName,
SqlToExecute =@sqlToExecute,
SqlNoOfRows=@sqlNoOfRows,
NoOfRowsPerPage = @noOfRowsPerPage,
sqlOrderBy = @sqlOrderBy,
filterParameters = @filterParameters,
InsertedBy = ISNULL(InsertedBy,@currentUser),
InsertedDate = ISNULL(InsertedDate,@now),
LastUpdatedBy = @currentUser,
LastUpdatedDate = @now
where id = @id

if @@rowcount = 0
insert into BamActivityView([ActivityName]
      ,[ViewName]
      ,[SqlToExecute]
      ,[SqlNoOfRows]
      ,[InsertedBy]
      ,[InsertedDate]
      ,[LastUpdatedBy]
      ,[LastUpdatedDate],noOfRowsPerPage,sqlOrderBy,filterParameters,isactive)
values(
@activityName,
@viewName,@sqlToExecute,@sqlNoOfRows,@currentUser,@now,@currentUser,@now,@noOfRowsPerPage,@sqlOrderBy,@filterParameters,'1'
)

if @currentFiltersCleared = 'true' or @currentFiltersCleared = 1
    delete from [dbo].[BamActivityViewParameter]
    where BamActivityViewId = @id

                        ";

            _dbConnection.Execute(query, new
            {
                viewName = view.ViewName.ToDbStringAnsi(),
                activityName = view.ActivityName.ToDbStringAnsi(),
                sqlToExecute = view.SqlToExecute,
                sqlNoOfRows = view.SqlNoOfRows,
                currentUser = currentUser.ToDbStringAnsi(),
                id = view.Id,
                sqlOrderBy = view.SqlOrderBy,
                now = DateTime.Now,
                noOfRowsPerPage = view.NoOfRowsPerPage,
                filterParameters = view.FilterParameters,
                currentFiltersCleared = currentFiltersCleared

            });

            foreach (var condition in view.ViewFilterParameters)
            {
                _dbConnection.Execute(@"
declare @viewId int
select top 1 @viewId = [Id]
from [dbo].[BamActivityView]
where [ViewName] = @viewName and [ActivityName] = @activityName

insert into [dbo].[BamActivityViewParameter](BamActivityViewId, Name, DisplayName, [Type]) values(@viewId,@name,@displayName,@paramType)
", new
                {
                    viewName = view.ViewName,
                    activityName = view.ActivityName,
                    name = condition.Name.ToDbStringAnsi(),
                    displayName = condition.DisplayName.ToDbStringAnsi(),
                    paramType = condition.ParameterType,

                }, null);
            }
        }

        public virtual ActivityView LoadActivityView(int id)
        {
            ActivityView vm = new ActivityView();
            const string query = @"
                        select * from [dbo].[BamActivityView] (NOLOCK) where [Id] = @id
                        select Id, BamActivityViewId
                                , Name
                                , DisplayName
                                , Type as ParameterType 
                                from [dbo].[BamActivityViewParameter] (NOLOCK) where [BamActivityViewId] = @id
                        ";

            using (var multi = _dbConnection.QueryMultiple(query,
                 new
                 {
                     @id = id

                 }))
            {
                vm = multi.Read<ActivityView>().FirstOrDefault();
                vm.ViewFilterParameters = multi.Read<ActivityViewFilterParameter>().ToList();
            }

            return vm;
        }

        public void ToggleViewConfiguration(int id, bool currentState)
        {
            const string query = @"
                       update BamActivityView set IsActive = @currentState where id=@id
                        ";

            _dbConnection.Execute(query, new
            {
                currentState = currentState,
                id = id,
            });
        }
    }
}
