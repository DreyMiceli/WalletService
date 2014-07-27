using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WalletService;

namespace WalletService
{

    public class WalletService : IWalletService
    {
        /// <summary>
        /// Allows creation of an account for a user.
        /// </summary>
        /// <param name="request">UserID</param>
        /// <returns>RegisterResponse</returns>
        public RegisterResponse Register(Request request)
        {
            return WalletRequestHandler.Register(request);

        }

        /// <summary>
        /// Gets current account balance for account id provided.
        /// </summary>
        /// <param name="request">AccountID</param>
        /// <returns>GetWalletBalanceResponse</returns>
        public GetWalletBalanceResponse GetWalletBalance(Request request)
        {
            return WalletRequestHandler.GetWalletBalance(request);
            
        }

        /// <summary>
        /// Withdraw funds from account id provided.
        /// </summary>
        /// <param name="request">AccountID,Amount</param>
        /// <returns>WithdrawResponse</returns>
        public WithdrawResponse Withdraw(Request request)
        {
            return WalletRequestHandler.Withdraw(request);
        }

        /// <summary>
        /// Deposit funds in account id provided.
        /// </summary>
        /// <param name="request">AccountID,Amount</param>
        /// <returns>DepositResponse</returns>
        public DepositResponse Deposit(Request request)
        {
            return WalletRequestHandler.Deposit(request);
        }

        /// <summary>
        /// Close Account.
        /// </summary>
        /// <param name="request">AccountID</param>
        /// <returns>AlterAccountStateResponse</returns>
        public AlterAccountStateResponse AlterAccountState(Request request)
        {
            return WalletRequestHandler.AlterAccountState(request);
        }

    }
}
