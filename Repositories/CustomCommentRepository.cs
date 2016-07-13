using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace Sitecore.Feature.Repositories
{
    public class CustomCommentRepository
    {
        public void AddComment(string name, string email, string comment, String itemID)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["custom"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("usp_AddComment", con))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@FullName", SqlDbType.VarChar).Value = name;
                        cmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = email;
                        cmd.Parameters.Add("@Comment", SqlDbType.VarChar).Value = comment;
                        cmd.Parameters.Add("@BLogID", SqlDbType.VarChar).Value = itemID;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        
                    }
                    catch (Exception ex)
                    {
                        Sitecore.Diagnostics.Log.Info(ex.Message, this);
                    }
                }
            }
        }
    }
}