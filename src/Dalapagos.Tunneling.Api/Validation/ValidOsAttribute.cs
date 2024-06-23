namespace Dalapagos.Tunneling.Api.Validation;

using System.ComponentModel.DataAnnotations;
using Core.Model;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class ValidOsAttribute : ValidationAttribute
 {
    public override bool IsValid(object? value)
     {
        var inputValue = value as string;
        if (string.IsNullOrWhiteSpace(inputValue))
        {
            SetErrorMessage();
            return false;
        }

        if (Enum.TryParse<Os>(inputValue, true, out var _))
        {
            return true;
        }

        SetErrorMessage();
        return false;
     }

     private void SetErrorMessage()
     {
         var oss = string.Join(",", Enum.GetValues(typeof(Os)).Cast<Os>());
         ErrorMessage = $"Invalid OS. Valid values are: {oss}.";
     }
 }