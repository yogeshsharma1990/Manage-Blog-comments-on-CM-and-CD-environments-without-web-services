using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Security.Accounts;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Sitecore.Feature.Scheduler
{
    public class CreateComments
    {
        public void Execute(Item[] items, Sitecore.Tasks.CommandItem command, Sitecore.Tasks.ScheduleItem schedule)
        {
            try
            {
                Sitecore.Security.Accounts.User user = Sitecore.Security.Accounts.User.FromName(@"extranet\Anonymous", true);
                using (new UserSwitcher(user))
                {
                    DataSet getUnprocessedComments = new DataSet();
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["custom"].ToString()))
                    {
                        using (SqlCommand cmd = new SqlCommand("usp_GetUnProcessedComments", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(getUnprocessedComments);
                            }
                        }

                        Database master = Sitecore.Configuration.Factory.GetDatabase("master");
                        TemplateItem template = master.GetTemplate("[Comment Data Template ID]"); //TODO: Please enter your comment data template ID
			string workflowID = template.StandardValues[Sitecore.FieldIDs.DefaultWorkflow];
                        bool IsSuccessfullyAdded = false;
                        foreach (DataRow DBValue in getUnprocessedComments.Tables[0].Rows)
                        {
                            Item parent = master.Items[DBValue["BlogID"].ToString()];
                            string validItemName = ItemUtil.ProposeValidItemName(DBValue["FullName"].ToString()).Trim();
                            Item item = parent.Add("comments by " + validItemName, template);
                            try
                            {
                                //TODO: Please update field name of comment template
				item.Editing.BeginEdit();
                                item.Fields["Name"].Value = DBValue["FullName"].ToString();
                                item.Fields["Email"].Value = DBValue["Email"].ToString();
                                item.Fields["Comment"].Value = DBValue["Comment"].ToString();
                                 if (!string.IsNullOrEmpty(workflowID))
                                {
                                    item.Editing.BeginEdit();
                                    item.Fields[Sitecore.FieldIDs.Workflow].Value = workflowID;
                                    IWorkflow wf = master.WorkflowProvider.GetWorkflow(workflowID);
                                    wf.Start(item);
                                }
                                item.Editing.EndEdit();
				IsSuccessfullyAdded = true;
                            }
                            catch (Exception ex)
                            {
                                IsSuccessfullyAdded = false;
                                Sitecore.Diagnostics.Log.Info("Comment Scheduler: Error in adding comment item under blog item " + ex.Message, this);
                                break;
                            }
                        }
                        if (IsSuccessfullyAdded == true)
                        {
                            using (SqlCommand cmd = new SqlCommand("usp_UpdateProcessedResults", con))
                            {
                                try
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@commentItems", getUnprocessedComments.Tables[0]);
                                    con.Open();
                                    cmd.ExecuteNonQuery();
                                    con.Close();
                                }
                                catch (Exception ex)
                                {
                                    Sitecore.Diagnostics.Log.Info("Comment Scheduler: Error in updating flag entry for added comment items " + ex.Message, this);
                                    con.Close();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Info("Comment Scheduler: Error in Create Comment Scheduler " + ex.Message, this);
            }
        }
    }
}