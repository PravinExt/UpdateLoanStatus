using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace UpdateLoanStatus
{
    public class Function
    {

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Loan FunctionHandler(Loan input, ILambdaContext context)
        {
            bool IsClosed = false;
            Loan loandetail = new Loan();

            //Check if the loan application has closed.
            IsClosed = loandetail.CheckIsClosed(input.CreditorAssigned_ID);

            if (IsClosed == false)
            {
                input.External_ID = loandetail.Get_ExternalID_by_LoanID(input.CreditorAssigned_ID);
                //Update Loan Status
                loandetail.Update_LoanInfo(input);

                if (input.LoanApplication_Status == 8)
                {
                    input.LoanApplication_BankerComment = "Closed by External Service";
                }
                if (input.LoanApplication_Status == 5)
                {
                    input.LoanApplication_BankerComment = "Approved";
                }
                if (input.LoanApplication_Status == 6)
                {
                    input.LoanApplication_BankerComment = "Sent To Decision Engine";
                }
            }
            else
            {
                input.LoanApplication_Status = 8;
                input.LoanApplication_BankerComment = "Closed by External Service";
            }


            return input;

        }
    }
}
