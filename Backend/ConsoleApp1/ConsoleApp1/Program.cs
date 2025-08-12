
using System.Text;


string inputStr = "Hello";
Console.WriteLine(Reverse(inputStr));

string Reverse(string inputStr)
{
    if (string.IsNullOrEmpty(inputStr))
        return inputStr;

    var outputStr = new StringBuilder(inputStr.Length);

    for (int i = inputStr.Length - 1; i >= 0; i--)
    {
        outputStr.Append(inputStr[i]);
    }

    return outputStr.ToString();
}

