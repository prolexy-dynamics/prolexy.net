using Tiba.Domain.Model.Uoms;

namespace Prolexy.Compiler.Tests.SchemaGenerators;

public class SimpleType
{
    public string Name { get; set; }
    public int Age { get; set; }
    public DateTime RegistrationDate { get; set; }
    public bool Accepted { get; set; }
}
public class SimpleTypeWithArray
{
    public string[] Name { get; set; }
    public int[] Age { get; set; }
    public DateTime[] RegistrationDate { get; set; }
    public bool[] Accepted { get; set; }
}
public class SimpleTypeWithMethod
{
    public string Name { get; set; }
    public int Age { get; set; }
    public DateTime GetBirthDay() => DateTime.Now;
    public void Register(string reason){}
}
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
    public AdditionalData Data { get; set; }
    public class AdditionalData
    {
        public string ImportRegNo { get; set; }
        public string ReferenceNumber { get; set; }
        public string PartyName { get; set; }
        public string EqExchangeRateTitle { get; set; }
        public string ExchangeRateTitle { get; set; }
        public string Reason { get; set; }
    }
}