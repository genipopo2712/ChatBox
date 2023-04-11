﻿using System.Security.Cryptography;
using System.Text;

namespace Chatbox
{
    public class Helper
    {
        public static byte[] Hash(string plaintext)
        {
            HashAlgorithm hash = SHA512.Create();
            return hash.ComputeHash(Encoding.ASCII.GetBytes(plaintext));
        }
        public static string RandomString(int len)
        {
            char[] arr = new char[len];
            Random random = new Random();
            string pattern = "qweropaslzxvbnm1234567890";
            for (int i = 0; i < len; i++)
            {
                arr[i] = pattern[random.Next(pattern.Length)];
            }
            return string.Join("", arr);
        }
        public static string StringConv(string id1, string id2)
        {
            int ret = string.Compare(id1, id2);
            if (ret < 0)
            {
                return id1 + id2;
            }
            else
            {
                return id2 + id1;
            }
        }
    }
}
