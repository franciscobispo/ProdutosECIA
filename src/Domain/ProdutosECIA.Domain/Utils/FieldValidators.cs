using System.Text.RegularExpressions;

namespace ProdutosECIA.Domain.Utils;

public static class FieldValidators
{
    public static bool CnpjIsValid(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;
        
        string sanitizedCnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");

        //var regex = new Regex(@"^\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}$");
        //if (!regex.IsMatch(cnpj))
        //    return false;

        if (sanitizedCnpj.Length != 14)
            return false;

        int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        string tempCnpj = sanitizedCnpj.Substring(0, 12);
        int soma = 0;

        for (int i = 0; i < 12; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

        int resto = (soma % 11);
        resto = resto < 2 ? 0 : 11 - resto;
        string digito = resto.ToString();

        tempCnpj = tempCnpj + digito;
        soma = 0;

        for (int i = 0; i < 13; i++)
            soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

        resto = (soma % 11);
        resto = resto < 2 ? 0 : 11 - resto;
        digito = digito + resto.ToString();

        return sanitizedCnpj.EndsWith(digito);
    }
}
