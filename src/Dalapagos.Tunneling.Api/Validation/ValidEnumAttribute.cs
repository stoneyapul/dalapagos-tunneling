namespace Dalapagos.Tunneling.Api.Validation;

using System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class ValidEnumAttribute<Tnum> : ValidationAttribute where Tnum : Enum
{
   public override bool IsValid(object? value)
     {
        var inputValue = value as string;
        if (string.IsNullOrWhiteSpace(inputValue))
        {
            SetErrorMessage();
            return false;
        }

        try
        {
            if (Enum.TryParse(typeof(Tnum), inputValue, true, out var _))
            {
                return true;
            }
        }
        catch (ArgumentException)
        {
            SetErrorMessage();
            return false;
        }

        SetErrorMessage();
        return false;
     }

     private void SetErrorMessage()
     {
         var vals = string.Join(",", Enum.GetValues(typeof(Tnum)).Cast<Tnum>());
         ErrorMessage = $"Invalid {typeof(Tnum).Name}. Valid values are: {vals}.";
     }

}
