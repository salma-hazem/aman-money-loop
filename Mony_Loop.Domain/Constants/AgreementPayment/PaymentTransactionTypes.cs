using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mony_Loop.Domain.Constants.Agreement___Payment
{
    public static class PaymentTransactionTypes
    {
        public const string Deposit = "Deposit";     // إيداع / دفع القسط
        public const string Withdrawal = "Withdrawal"; // سحب / استلام الجمعية
        public const string Penalty = "Penalty";       // غرامة تأخير
    }
}
