using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WalletService
{
    //Service Interface
    [ServiceContract]
    public interface IWalletService
    {

        [OperationContract]
        RegisterResponse Register(Request request);

        [OperationContract]
        GetWalletBalanceResponse GetWalletBalance(Request request);

        [OperationContract]
        WithdrawResponse Withdraw(Request request);

        [OperationContract]
        DepositResponse Deposit(Request request);

        [OperationContract]
        AlterAccountStateResponse AlterAccountState(Request request);

    }



    //Service Data Models Used
    [DataContract]
    public class Request
    {

        [DataMember]
        public Int64 AccountID { get; set; }

        [DataMember]
        public Decimal Amount { get; set; }

        [DataMember]
        public int NewAccountState { get; set; }

        [DataMember]
        public string UserId { get; set; } 
    }

    [DataContract]
    public class Response
    {
        public Response()
        {
            ResponseCode = -1;
            Message = "";
        }

        public Response(int responseCode, string message)
        {
            ResponseCode = responseCode;
            Message = message;
        }

        [DataMember]
        public int ResponseCode { get; set; }

        [DataMember]
        public string Message { get; set; }
    }

    [DataContract]
    public class RegisterResponse : Response
    {
        public RegisterResponse()
        {
            NewAccountID = 0;
        }

        public RegisterResponse(int responseCode, string message, Int64 newAccountID)
        {
            ResponseCode = responseCode;
            Message = message;
            NewAccountID = newAccountID;
        }

        [DataMember]
        public Int64 NewAccountID { get; set; }
    }

    [DataContract]
    public class AlterAccountStateResponse : Response
    {
        public AlterAccountStateResponse()
        {
        }

        public AlterAccountStateResponse(int responseCode, string message)
        {
            ResponseCode = responseCode;
            Message = message;
        }
    }

    [DataContract]
    public class GetWalletBalanceResponse : Response
    {
        public GetWalletBalanceResponse()
        {
            WalletBalance = 0;
        }

        public GetWalletBalanceResponse(int responseCode, string message, Decimal walletBalance)
        {
            ResponseCode = responseCode;
            Message = message;
            WalletBalance = walletBalance;
        }

        [DataMember]
        public Decimal WalletBalance { get; set; }

    }

    [DataContract]
    public class WithdrawResponse : GetWalletBalanceResponse
    {
        public WithdrawResponse()
        {
        }

        public WithdrawResponse(int responseCode, string message, Decimal walletBalance)
        {
            ResponseCode = responseCode;
            Message = message;
            WalletBalance = walletBalance;
        }
    }

    [DataContract]
    public class DepositResponse : GetWalletBalanceResponse
    {
        public DepositResponse()
        {
        }

        public DepositResponse(int responseCode, string message, Decimal walletBalance)
        {
            ResponseCode = responseCode;
            Message = message;
            WalletBalance = walletBalance;
        }
    }


}
