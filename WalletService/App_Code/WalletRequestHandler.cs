using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace WalletService
{
    public static class WalletRequestHandler
    {

        //Private Methods
        private static bool ValidateRequest(string methodName, Request request, ref string message) 
        {
            message = "";

            if (request == null)
            {
                message += "ValidationError - Null Request Sent.";
            }
            else
            {

                switch (methodName)
                {
                    case "Register":

                        if (request.UserId == null || request.UserId.Trim().Length == 0)
                        {
                            message += "ValidationError - Request.UserId found empty.";
                        }
                        break;

                    case "GetWalletBalance":
                        if (request.AccountID == 0)
                        {
                            message += "ValidationError - Request.AccountID found empty.";
                        }
                        break;

                    case "Withdraw":
                    case "Deposit":
                        if (request.AccountID == 0)
                        {
                            message += " Request.AccountID found empty.";
                        }

                        if (request.Amount == 0)
                        {
                            message += " Request.Amount found empty.";
                        }
                        break;


                    case "AlterAccountState":
                        if (request.AccountID == 0)
                        {
                            message += " Request.AccountID found empty.";
                        }

                        if (request.NewAccountState != 2)
                        {
                            message += " Request.NewAccountState found invalid.";
                        }
                        break;
                }
            }


            if (message.Length > 0)
            {
                message = String.Concat("ValidationError -", message);
                return false;
            }

            return true;
        }

        private static int isValidActiveAccount(Int64 findAccountID,ref Account validAccount, ref string message)
        {
            try
            {
                using (var db = new WalletDBContext())
                {
                    validAccount = db.Accounts.SingleOrDefault(accountRecord => accountRecord.ID == findAccountID);

                    if (validAccount != null)
                    {
                        if (validAccount.AccountStateId == 2)
                        {
                            //Account Exists but is closed
                            message = "Account Found Closed.";
                            return 2;
                        }

                        //Account Found Valid
                        return 0;
                    }
                }

                message = "Account Does Not Exist.";
                return 1;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                message = "System Error. Checking if Account ID is valid.";
                return 3;
            }
        }


        //Methods used by SVC.
        //Methods were not coded into SVC directly so that these methods can be used and exposed by numerous other interfaces.

        //ResponseCode Meanings
        //0 = OK
        //1 = Request Validation Error
        //2 = UserId already exists
        //3 = System Error
        internal static RegisterResponse Register(Request request)
        {
            string message = "";
            
            //Validate Request
            if (!ValidateRequest("Register", request, ref message))
            {
                return new RegisterResponse(1, message, 0);
            }
            

            using (var db = new WalletDBContext())
            {
                //Check if UserID Already Exists
                var accountList = db.Accounts.Where(account => account.UserID == request.UserId);

                if (accountList.Count() > 0)
                {
                    return new RegisterResponse(2, "UserId provided already exists.", 0);
                }

                //Create Account
                var newAccount = new Account { UserID = request.UserId, AccountStateId = 1, DateCreated = DateTime.Now };
                db.Accounts.Add(newAccount);
                try
                {
                    db.SaveChanges();
                    return new RegisterResponse(0, "", newAccount.ID);
                }
                catch
                {
                    //Error occured while creating account in DB.
                    return new RegisterResponse(3, "System Error. Account Not Created.", 0);
                }
            }
        }


        //ResponseCode Meanings
        //0 = OK
        //1 = Request Validation Error
        //2 = Account does not exist
        //3 = Account Found Closed
        //4 = System Error
        internal static GetWalletBalanceResponse GetWalletBalance(Request request)
        {
            string message = "";
            Account resultAccount = null;

            //Validate Request
            if (!ValidateRequest("GetWalletBalance", request, ref message))
            {
                return new GetWalletBalanceResponse(1, message, 0);
            }

            //Check if Account Exists and in a Valid State
            switch (isValidActiveAccount(request.AccountID, ref resultAccount, ref message))
            {
                case 0:
                    //Account Found Valid
                    return new GetWalletBalanceResponse(0, "", resultAccount.Balance); //Returning Balance

                case 1:
                    //Account not found
                    return new GetWalletBalanceResponse(2, message, 0);

                case 2:
                    //Account found but closed
                    return new GetWalletBalanceResponse(3, message, 0);

                case 3:
                    //Error Accessing DB
                    return new GetWalletBalanceResponse(4, message, 0);

                default:
                    //Error Unknown
                    return new GetWalletBalanceResponse(4, "System Error - Unknown Error.", 0);
            }
        }


        //ResponseCode Meanings
        //0 = OK
        //1 = Request Validation Error
        //2 = Account does not exist
        //3 = Account Found Closed
        //4 = Not Enough Funds on Account
        //5 = System Error
        internal static WithdrawResponse Withdraw(Request request)
        {
            string message = "";
            Account resultAccount = null;

            //Validate Request
            if (!ValidateRequest("Withdraw", request, ref message))
            {
                return new WithdrawResponse(1, message, 0);
            }

            //Check if Account Exists and in a Valid State
            switch (isValidActiveAccount(request.AccountID, ref resultAccount, ref message))
            {
                case 1:
                    //Account not found
                    return new WithdrawResponse(2, message, 0);

                case 2:
                    //Account found but closed
                    return new WithdrawResponse(3, message, 0);

                case 3:
                    //Error Accessing DB
                    return new WithdrawResponse(5, message, 0);
            }

            //Check if Withdraw amount is larger then current blance
            if (resultAccount.Balance < request.Amount)
            {
                return new WithdrawResponse(4, "Withdrawl request amount is more then account current balance", resultAccount.Balance);
            }

            using (var db = new WalletDBContext())
            {
                resultAccount = db.Accounts.SingleOrDefault(accountRecord => accountRecord.ID == resultAccount.ID);

                if (resultAccount != null)
                {
                    resultAccount.Balance -= request.Amount; //Deducting withdrawal amount from wallet balance

                    WalletTransaction transaction = new WalletTransaction { AccountID = resultAccount.ID, Amount = request.Amount, DateCreated = DateTime.Now, TransactionTypeId = 2 }; //Creating Transaction Record

                    db.WalletTransactions.Add(transaction);

                    db.SaveChanges();

                    return new WithdrawResponse(0, "", resultAccount.Balance); //Returning Balance
                }
            }

            return new WithdrawResponse(5, "System Error - Unknown Error", 0);

        }

        //ResponseCode Meanings
        //0 = OK
        //1 = Request Validation Error
        //2 = Account does not exist
        //3 = Account Found Closed
        //4 = System Error
        internal static DepositResponse Deposit(Request request)
        {
            string message = "";
            Account resultAccount = null;

            //Validate Request
            if (!ValidateRequest("Deposit", request, ref message))
            {
                return new DepositResponse(1, message, 0);
            }

            //Check if Account Exists and in a Valid State
            switch (isValidActiveAccount(request.AccountID, ref resultAccount, ref message))
            {
                case 1:
                    //Account not found
                    return new DepositResponse(2, message, 0);

                case 2:
                    //Account found but closed
                    return new DepositResponse(3, message, 0);

                case 3:
                    //Error Accessing DB
                    return new DepositResponse(4, message, 0);
            }

            using (var db = new WalletDBContext())
            {
                resultAccount = db.Accounts.SingleOrDefault(accountRecord => accountRecord.ID == resultAccount.ID);

                if (resultAccount != null)
                {
                    resultAccount.Balance += request.Amount; //Adding Deposit Amount to Wallet

                    WalletTransaction transaction = new WalletTransaction { AccountID = resultAccount.ID, Amount = request.Amount, DateCreated = DateTime.Now, TransactionTypeId = 1 }; //Creating Transaction Record

                    db.WalletTransactions.Add(transaction);

                    db.SaveChanges();

                    return new DepositResponse(0, "", resultAccount.Balance); //Resutning Balance
                }
            }

            return new DepositResponse(4, "System Error - Unknown Error", 0);

        }

        //ResponseCode Meanings
        //0 = OK
        //1 = Request Validation Error
        //2 = Account does not exist
        //3 = Account Found Closed
        //4 = Account still has funds.
        //5 = System Error
        internal static AlterAccountStateResponse AlterAccountState(Request request)
        {
            string message = "";
            Account resultAccount = null;

            //Validate Request
            if (!ValidateRequest("AlterAccountState", request, ref message))
            {
                return new AlterAccountStateResponse(1, message);
            }

            //Check if Account Exists and in a Valid State
            switch (isValidActiveAccount(request.AccountID, ref resultAccount, ref message))
            {
                case 1:
                    //Account not found
                    return new AlterAccountStateResponse(2, message);

                case 2:
                    //Account found but closed
                    return new AlterAccountStateResponse(3, message);

                case 3:
                    //Error Accessing DB
                    return new AlterAccountStateResponse(5, message);
            }

            //Check if there is balance on the account
            if (resultAccount.Balance > 0)
            {
                return new AlterAccountStateResponse(4, "Account can not be closed because it still has a positive balance");
            }


            //Account can be closed.
            using (var db = new WalletDBContext())
            {
                resultAccount = db.Accounts.SingleOrDefault(accountRecord => accountRecord.ID == resultAccount.ID);

                if (resultAccount != null)
                {
                    resultAccount.AccountStateId = request.NewAccountState; //Setting Account new state
                    resultAccount.DateClosed = DateTime.Now; //Setting Account Close Date
                    db.SaveChanges();
                    return new AlterAccountStateResponse(0, "");
                }
            }

            return new AlterAccountStateResponse(5, "System Error - Unknown Error");

            
        }

    }
}