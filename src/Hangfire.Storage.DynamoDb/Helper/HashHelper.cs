using System;
using System.Text;

namespace Hangfire.Storage.DynamoDb.Helper
{
    internal static class HashHelper
    {
        internal static string GenerateHash(this string input)
        {
            using System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);
            
            return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
        }
    }
}