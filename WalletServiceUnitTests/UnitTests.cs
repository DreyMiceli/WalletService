using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WalletServiceUnitTests.WalletService;

namespace WalletServiceUnitTests
{
    [TestClass]
    public class UnitTests
    {
        #region Account ids which are used for testing, these are mapped to the accounts created from the RequiredData.SQL file in the DBScripts Folder

            private const Int64 accountActive = 1;

            private const Int64 accountClosed = 2;

            private const Int64 accountNonExisting = 99999999999;

        #endregion

        #region Register Method Tests

        [TestMethod]
        [Description("Sending unused UserId should yield a ResponseCode 0 and a number bigger then 0 for NewAccountID")]
        public void Register_CorrectRequest()
        {
            WalletServiceClient client = new WalletServiceClient();

            Request request = new Request();
            request.UserId = String.Concat(Guid.NewGuid(),"@email.com");
            
            RegisterResponse response =  client.Register(request);

            Assert.AreEqual(0, response.ResponseCode);

            Assert.IsTrue(response.NewAccountID > 0);

            Assert.IsTrue(response.Message.Length == 0);

        }

        [TestMethod]
        [Description("Sending empty request object should yield a ResponseCode 1 (ValidationError) and a message.")]
        public void Register_EmptyRequest()
        {
            WalletServiceClient client = new WalletServiceClient();

            Request request = new Request();

            RegisterResponse response = client.Register(request);

            Assert.AreEqual(1, response.ResponseCode);

            Assert.IsTrue(response.Message.Length > 0);

        }

        [TestMethod]
        [Description("Sending used UserId (test@email.com) request object should yield a ResponseCode 2 (UserID already used) and a message. If test fails the first time, try again because the userid was not found in DB.")]
        public void Register_AlreadyExistingUserId()
        {
            WalletServiceClient client = new WalletServiceClient();

            Request request = new Request();
            request.UserId = "test@email.com";

            RegisterResponse response = client.Register(request);

            Assert.AreEqual(2, response.ResponseCode);

            Assert.IsTrue(response.Message.Length > 0);

        }

        #endregion

        #region GetWalletBalance Method Tests

        [TestMethod]
        [Description("Sending existing AccountID should yield the balance of the account with ResponseCode 0.")]
        public void GetWalletBalance_CorrectRequest()
        {
            WalletServiceClient client = new WalletServiceClient();

            Request request = new Request();
            request.AccountID = accountActive;

            GetWalletBalanceResponse response = client.GetWalletBalance(request);

            Assert.AreEqual(0, response.ResponseCode);

            Assert.IsTrue(response.WalletBalance >= 0);

            Assert.AreEqual(0, response.Message.Length);

        }


        [TestMethod]
        [Description("Sending existing empty request should yield ResponseCode 1 (ValidationError)")]
        public void GetWalletBalance_EmptyRequest()
        {
            WalletServiceClient client = new WalletServiceClient();

            Request request = new Request();

            GetWalletBalanceResponse response = client.GetWalletBalance(request);

            Assert.AreEqual(1, response.ResponseCode);

            Assert.AreEqual(0, response.WalletBalance);

            Assert.IsTrue(response.Message.Length > 0);

        }

        [TestMethod]
        [Description("Sending non existing accountID should yield ResponseCode 2 (AccountNotFound).")]
        public void GetWalletBalance_NonExistingAccountRequest()
        {
            WalletServiceClient client = new WalletServiceClient();

            Request request = new Request();
            request.AccountID = accountNonExisting;

            GetWalletBalanceResponse response = client.GetWalletBalance(request);

            Assert.AreEqual(2, response.ResponseCode);

            Assert.AreEqual(0, response.WalletBalance);

            Assert.IsTrue(response.Message.Length > 0);

        }

        [TestMethod]
        [Description("Sending existing closed accountID should yield ResponseCode 3 (AccountFoundClosed).")]
        public void GetWalletBalance_ClosedAccountRequest()
        {
            WalletServiceClient client = new WalletServiceClient();

            Request request = new Request();
            request.AccountID = accountClosed;

            GetWalletBalanceResponse response = client.GetWalletBalance(request);

            Assert.AreEqual(3, response.ResponseCode);

            Assert.AreEqual(0, response.WalletBalance);

            Assert.IsTrue(response.Message.Length > 0);

        }


        #endregion

        #region Withdraw Method Tests

        //Tests

        //Correct Test

        //Withdraw with an empty request

        //Withdrawal from non existing account

        //Withdraw from Closed Account

        //Withdraw more funds then available in account


        [TestMethod]
        [Description("Sending an existing AccountID with 5 Amount should result in ResponseCode 0")]
        public void Withdraw_CorrectRequest()
        {
            WalletServiceClient client = new WalletServiceClient();

            //Get Account Balance
            Request request = new Request();
            request.AccountID = accountActive;
            GetWalletBalanceResponse getWalletBalanceResponse = client.GetWalletBalance(request);
            Decimal currentBalance = getWalletBalanceResponse.WalletBalance;


            request = new Request();
            request.AccountID = accountActive;
            request.Amount = 5;

            WithdrawResponse response = client.Withdraw(request);

            Assert.AreEqual(0, response.ResponseCode);

            Assert.AreEqual((currentBalance - request.Amount), response.WalletBalance);

            Assert.AreEqual(0, response.Message.Length);

        }


        [TestMethod]
        [Description("Sending an empty request should yield a ResponseCode 1 (ValidationError)")]
        public void Withdraw_EmptyRequest()
        {
            WalletServiceClient client = new WalletServiceClient();

            Request request = new Request();

            WithdrawResponse response = client.Withdraw(request);

            Assert.AreEqual(1, response.ResponseCode);

            Assert.AreEqual(0, response.WalletBalance);

            Assert.IsTrue(response.Message.Length > 0);

        }


        [TestMethod]
        [Description("Sending withdraw request to a non existing account should result in ResponseCode 2")]
        public void Withdraw_NonExistingAccount()
        {
            WalletServiceClient client = new WalletServiceClient();

            Request request = new Request();
            request.AccountID = accountNonExisting;
            request.Amount = 5;

            WithdrawResponse response = client.Withdraw(request);

            Assert.AreEqual(2, response.ResponseCode);

            Assert.AreEqual(0, response.WalletBalance);

            Assert.IsTrue(response.Message.Length > 0);

        }

        [TestMethod]
        [Description("Sending withdraw request to a closed account should result in ResponseCode 3")]
        public void Withdraw_FromClosedAccount()
        {
            WalletServiceClient client = new WalletServiceClient();

            Request request = new Request();
            request.AccountID = accountClosed;
            request.Amount = 5;

            WithdrawResponse response = client.Withdraw(request);

            Assert.AreEqual(3, response.ResponseCode);

            Assert.AreEqual(0, response.WalletBalance);

            Assert.IsTrue(response.Message.Length > 0);

        }

        [TestMethod]
        [Description("Sending withdraw request with amount higher then current balance should result in ResponseCode 4")]
        public void Withdraw_NotEnoughCredit()
        {
            WalletServiceClient client = new WalletServiceClient();

            Request request = new Request();
            request.AccountID = accountActive;
            request.Amount = 1000000;

            WithdrawResponse response = client.Withdraw(request);

            Assert.AreEqual(4, response.ResponseCode);

            Assert.IsTrue(response.WalletBalance >= 0);

            Assert.IsTrue(response.Message.Length > 0);

        }


        #endregion

        #region Deposit Method Tests

        //Tests

        //Correct Test

        //Deposit with an empty request

        //Deposit on non existing account

        //Deposit on Closed Account


        [TestMethod]
        [Description("Sending an existing AccountID with 5 Amount should result in ResponseCode 0")]
        public void Deposit_CorrectRequest()
        {
            WalletServiceClient client = new WalletServiceClient();

            //Get Account Balance
            Request request = new Request();
            request.AccountID = accountActive;
            GetWalletBalanceResponse getWalletBalanceResponse = client.GetWalletBalance(request);
            Decimal currentBalance = getWalletBalanceResponse.WalletBalance;


            request = new Request();
            request.AccountID = accountActive;
            request.Amount = 5;

            DepositResponse response = client.Deposit(request);

            Assert.AreEqual(0, response.ResponseCode);

            Assert.AreEqual((currentBalance + request.Amount), response.WalletBalance);

            Assert.AreEqual(0, response.Message.Length);

        }


        [TestMethod]
        [Description("Sending an empty request should yield a ResponseCode 1 (ValidationError)")]
        public void Deposit_EmptyRequest()
        {
            WalletServiceClient client = new WalletServiceClient();

            Request request = new Request();

            DepositResponse response = client.Deposit(request);

            Assert.AreEqual(1, response.ResponseCode);

            Assert.AreEqual(0, response.WalletBalance);

            Assert.IsTrue(response.Message.Length > 0);

        }

        [TestMethod]
        [Description("Sending deposit request to a non exisintg account should result in ResponseCode 2")]
        public void Deposit_NonExistingAccount()
        {
            WalletServiceClient client = new WalletServiceClient();

            Request request = new Request();
            request.AccountID = accountNonExisting;
            request.Amount = 5;

            DepositResponse response = client.Deposit(request);

            Assert.AreEqual(2, response.ResponseCode);

            Assert.AreEqual(0, response.WalletBalance);

            Assert.IsTrue(response.Message.Length > 0);

        }


        [TestMethod]
        [Description("Sending deposit request to a closed account should result in ResponseCode 3")]
        public void Deposit_ToClosedAccount()
        {
            WalletServiceClient client = new WalletServiceClient();

            Request request = new Request();
            request.AccountID = accountClosed;
            request.Amount = 5;

            DepositResponse response = client.Deposit(request);

            Assert.AreEqual(3, response.ResponseCode);

            Assert.AreEqual(0, response.WalletBalance);

            Assert.IsTrue(response.Message.Length > 0);

        }

        #endregion

        #region AlterAccountState Method Tests

        //Tests

        //Correct Request

        //Empty Request

        //Account which does not exist

        //Account Already Found In That Closed State

        //Account With Existing Balance and trying to Close it.


        [TestMethod]
        [Description("Creating a new account and then setting it as closed. ResponseCode 0")]
        public void AlterAccountState_CorrectRequest()
        {
            WalletServiceClient client = new WalletServiceClient();

            //Create Account
            Request request = new Request();
            request.UserId = String.Concat(Guid.NewGuid(), "@email.com");
            RegisterResponse registerResponse = client.Register(request);
            Int64 accountID = registerResponse.NewAccountID;


            request = new Request();
            request.AccountID = accountID;
            request.NewAccountState = 2;

            AlterAccountStateResponse response = client.AlterAccountState(request);

            Assert.AreEqual(0, response.ResponseCode);

            Assert.AreEqual(0, response.Message.Length);

        }


        [TestMethod]
        [Description("Sending an empty request should yield a ResponseCode 1 (ValidationError)")]
        public void AlterAccountState_EmptyRequest()
        {
            WalletServiceClient client = new WalletServiceClient();

            Request request = new Request();

            AlterAccountStateResponse response = client.AlterAccountState(request);

            Assert.AreEqual(1, response.ResponseCode);

            Assert.IsTrue(response.Message.Length > 0);

        }

        [TestMethod]
        [Description("Attempting to close an account which does not exist, should result in ResponseCode 2")]
        public void AlterAccountState_NonExistingAccount()
        {
            WalletServiceClient client = new WalletServiceClient();

            Request request = new Request();
            request.AccountID = accountNonExisting;
            request.NewAccountState = 2;

            AlterAccountStateResponse response = client.AlterAccountState(request);

            Assert.AreEqual(2, response.ResponseCode);

            Assert.IsTrue(response.Message.Length > 0);

        }

        [TestMethod]
        [Description("Attempting to close an account which is already closed, should result in ResponseCode 3")]
        public void AlterAccountState_AlreadyClosedAccount()
        {
            WalletServiceClient client = new WalletServiceClient();

            Request request = new Request();
            request.AccountID = accountClosed;
            request.NewAccountState = 2;

            AlterAccountStateResponse response = client.AlterAccountState(request);

            Assert.AreEqual(3, response.ResponseCode);

            Assert.IsTrue(response.Message.Length > 0);

        }

        [TestMethod]
        [Description("Attempting to close an account which a positive balance, should result in ResponseCode 4")]
        public void AlterAccountState_AccountWithPositiveBalance()
        {
            Deposit_CorrectRequest(); //Adding Funds to Account 1

            WalletServiceClient client = new WalletServiceClient();

            Request request = new Request();
            request.AccountID = accountActive;
            request.NewAccountState = 2;

            AlterAccountStateResponse response = client.AlterAccountState(request);

            Assert.AreEqual(4, response.ResponseCode);

            Assert.IsTrue(response.Message.Length > 0);

        }

        #endregion
    }
}
