using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppsSocket.Models;
using System.Data.Entity.Core.Objects;
namespace AppsSocket.SQLData
{
    public class ApplicantData
    {

        public PromptModel checkBooking(string fname, string lname, DateTime bdate)
        {
            PromptModel pr = new PromptModel();
            string applicantID = string.Empty;

            try
            {
                using (ApplicantDBEntities dbcontext = new ApplicantDBEntities())
                {

                    var qApplicantID = dbcontext.appool_applicant
                        .Where(aa => aa.first_name == fname && aa.last_name == lname && EntityFunctions.TruncateTime(aa.birth_date) == EntityFunctions.TruncateTime(bdate));

                    if (qApplicantID.Count() > 0)
                    {
                        applicantID = qApplicantID.First().applicant_id;//set applicant id
                        pr.applicantID = applicantID;//set applicantID in the model

                        if (dbcontext.appool_booking.Where(aa => aa.applicant_id == applicantID).Count() > 0)
                        {
                            var currentdate = DateTime.Now.Date;
                            var qCheckBookingExam = dbcontext.appool_booking.Where(aa => aa.applicant_id == applicantID && aa.exam_date == currentdate);
                            var qCheckBookingInterview = dbcontext.appool_booking.Where(aa => aa.applicant_id == applicantID && aa.interview_date == currentdate);

                            if (qCheckBookingExam.Count() > 0 && qCheckBookingInterview.Count() > 0)
                            {
                                //check if the user is already in logbox

                                var userLogboxInterview = dbcontext.appool_ApplicantCheckIn
                                    .Where(aa => EntityFunctions.TruncateTime(aa.login_date) == EntityFunctions.TruncateTime(currentdate) &&
                                    aa.applicant_id == applicantID && aa.purpose == "INTERVIEW")
                                    .Count();

                                var userLogboxExam = dbcontext.appool_ApplicantCheckIn
                                   .Where(aa => EntityFunctions.TruncateTime(aa.login_date) == EntityFunctions.TruncateTime(currentdate) &&
                                   aa.applicant_id == applicantID && aa.purpose == "EXAM")
                                   .Count();


                                if (userLogboxInterview > 0 && userLogboxExam > 0)
                                {
                                    pr.ifunctionVal = 1;
                                    pr.functionMsg = "Invalid: Duplicate log is not authorized!";
                                }
                                else
                                {
                                    postApplicantLogin(applicantID, "INTERVIEW");
                                    postApplicantLogin(applicantID, "EXAM");
                                    pr.ifunctionVal = 0;
                                    pr.functionMsg = "Thank's  for coming...";
                                }


                            }
                            else if (qCheckBookingExam.Count() > 0 && qCheckBookingInterview.Count() == 0)
                            {

                                var userLogboxExam = dbcontext.appool_ApplicantCheckIn
                                  .Where(aa => EntityFunctions.TruncateTime(aa.login_date) == EntityFunctions.TruncateTime(currentdate) &&
                                  aa.applicant_id == applicantID && aa.purpose == "EXAM")
                                  .Count();

                                if (userLogboxExam > 0)
                                {
                                    pr.ifunctionVal = 1;
                                    pr.functionMsg = "Invalid: Duplicate log is not authorized!";
                                }
                                else
                                {
                                    postApplicantLogin(applicantID, "EXAM");
                                    pr.ifunctionVal = 0;
                                    pr.functionMsg = "Thank's for coming...";
                                }


                            }
                            else if (qCheckBookingExam.Count() == 0 && qCheckBookingInterview.Count() > 0)
                            {

                                var userLogboxInterview = dbcontext.appool_ApplicantCheckIn
                                  .Where(aa => EntityFunctions.TruncateTime(aa.login_date) == EntityFunctions.TruncateTime(currentdate) &&
                                  aa.applicant_id == applicantID && aa.purpose == "INTERVIEW")
                                  .Count();

                                if (userLogboxInterview > 0)
                                {
                                    pr.ifunctionVal = 1;
                                    pr.functionMsg = "Invalid: Duplicate log is not authorized!";
                                }
                                else
                                {
                                    postApplicantLogin(applicantID, "INTERVIEW");
                                    pr.ifunctionVal = 0;
                                    pr.functionMsg = "Thank's for coming...";
                                }


                            }
                            else
                            {
                                pr.ifunctionVal = 1;
                                pr.functionMsg = "No Event.";
                            }
                        }
                        else
                        {
                            pr.ifunctionVal = 1;
                            pr.functionMsg = "Applicant Not Found!";
                        }
                    }
                    else
                    {
                        pr.ifunctionVal = 1;
                        pr.functionMsg = "Applicant Not Found!";
                    }

                  
                    
                    return pr;
                }
            }
            catch (Exception ex)
            {
                pr.ifunctionVal = 1;
                pr.functionMsg = ex.Message;
                return pr;
            }
            
        }

       /// <summary>
        /// postApplicantLogin
       /// </summary>
       /// <param name="p1">Applicant ID</param>
       /// <param name="p2">Purpose</param>
        public void postApplicantLogin(string p1,string p2)
        {
            try
            {
                using (ApplicantDBEntities dbcontext = new ApplicantDBEntities())
                {

                    dbcontext.Sproc_AppoolCheckIn(p1, p2);
           
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<appool_ApplicantCheckIn> getApplicantLoginbyCode(string p){
            try
            {
                using (ApplicantDBEntities dbcontext = new ApplicantDBEntities())
                {
                    var q = dbcontext.appool_ApplicantCheckIn.Where(aa => aa.applicant_id == p).ToList();

                    return q;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}