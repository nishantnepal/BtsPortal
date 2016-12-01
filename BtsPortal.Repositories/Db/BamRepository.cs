using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BtsPortal.Entities.Bam;
using BtsPortal.Repositories.Interface;
using Dapper;

namespace BtsPortal.Repositories.Db
{
    public class BamRepository : IBamRepository
    {
        readonly IDbConnection _dbConnection;

        public BamRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public bool BamInstalled()
        {
            const string query = @"
                        if db_id('BAMPrimaryImport') is not null
	                        select 1 as 'Exists'
                        else
	                        select 0 as 'Exists'
                        ";

            var vm = _dbConnection.Query<int>(query).FirstOrDefault();

            return vm == 1;
        }

        public virtual List<Activity> LoadActivities()
        {
            const string query = @"
                            select ActivityName--,DefinitionXml
             from bam_Metadata_Activities (NOLOCK)
             order by ActivityName
                        ";

            var vm = _dbConnection.Query<Activity>(query).ToList();
            return vm;
        }

        public BamResult LoadData(ActivityView activityView, int page, int defaultPagesize, Dictionary<string, string> parameters)
        {
            string sqlStatement = activityView.SqlToExecute;
            string orderBy = string.IsNullOrWhiteSpace(activityView.SqlOrderBy)
                ? " order by 1 desc"
                : " order by " + activityView.SqlOrderBy;
            if (!string.IsNullOrWhiteSpace(activityView.SqlNoOfRows))
            {
                sqlStatement = sqlStatement + Environment.NewLine
                        + orderBy + Environment.NewLine
                        + @" OFFSET  (@pageNum -1) * @pageSize ROWS FETCH NEXT @pageSize ROWS ONLY; " + Environment.NewLine
                        + activityView.SqlNoOfRows;
            }

            DataSet ds;
            int totalRows = 0;
            try
            {
                using (SqlCommand command = new SqlCommand(sqlStatement, (SqlConnection)_dbConnection))
                {
                    SqlDataAdapter da = new SqlDataAdapter { SelectCommand = command };
                    da.SelectCommand.Parameters.AddWithValue("pageNum", page);
                    da.SelectCommand.Parameters.AddWithValue("pageSize", activityView.NoOfRowsPerPage.GetValueOrDefault(defaultPagesize));
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> pair in parameters)
                        {
                            da.SelectCommand.Parameters.AddWithValue(pair.Key, pair.Value);
                        }
                    }

                    ds = new DataSet();
                    _dbConnection.Open();
                    da.Fill(ds);
                    _dbConnection.Close();
                }
            }
            finally
            {
                if (_dbConnection.State != ConnectionState.Closed)
                {
                    _dbConnection.Close();
                }
            }

            if (ds.Tables.Count > 1)
            {
                try
                {
                    totalRows = Convert.ToInt32(ds.Tables[1].Rows[0][0].ToString());
                }
                catch (Exception)
                {
                    throw new Exception("Unable to determine the total rows. Check the PagingSql statement.");
                }
            }

            var totalPage = (double)totalRows / activityView.NoOfRowsPerPage.GetValueOrDefault(defaultPagesize);
            return new BamResult()
            {
                PageSize = activityView.NoOfRowsPerPage.GetValueOrDefault(defaultPagesize),
                PageNumber = page,
                TotalRows = totalRows,
                TotalPage = (int?)Math.Ceiling(totalPage),
                ResultDataTable = ds.Tables[0],
                ViewId = activityView.Id.GetValueOrDefault(0),
                ViewName = activityView.ViewName,
                ActivityName = activityView.ActivityName
            };
        }


        //load xml
        /*
         use BAMPrimaryImport
SELECT * FROM sys.views
where name like '%_AllInstances'

        SELECT TOP 1000 [ActivityName]

      ,[DefinitionXml]
      ,[OnlineWindowTimeUnit]
      ,[OnlineWindowTimeLength]
      ,[Archive]
        FROM[BAMPrimaryImport].[dbo].[bam_Metadata_Activities]

*/
        //extract data types along the lines of Name and DataType
    }
}