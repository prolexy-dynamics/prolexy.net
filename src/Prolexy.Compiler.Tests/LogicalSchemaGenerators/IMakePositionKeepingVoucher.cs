using Tiba.Domain.Model.Uoms;

namespace Prolexy.Compiler.Tests.LogicalSchemaGenerators;

public interface IMakePositionKeepingVoucher
{
    public MoneyData Amount { get; set; }
    public string FromBankAccountCode { get; set; }
    public string OperationCode { get; set; }
    public string BranchCode { get; set; }
    public MoneyData FcAmount { get; set; }
    public MoneyData EqFcAmount { get; set; }
    public double ExchangeRate { get; set; }
    public double EqExchangeRate { get; set; }
    public MoneyData TotalAmount { get; set; }
   
}