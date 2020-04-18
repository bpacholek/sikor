using System.Security.Cryptography;
using System.Text;

namespace Sikor.Util.Security
{
    /**
     * <summary>
     * Hashes generation utility.
     * </summary>
     */
    public class HashGenerator
    {
        /**
         * <summary>
         * Builds MD5 hash of an input string.
         * </summary>
         * <param name="input">String which should be hashed</param>
         * <returns>MD5 Hash string</returns>
         */
        public static string MD5(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}