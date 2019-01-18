namespace Known.Tests.Core
{
    public class EncryptorTest
    {
        public static void ToMd5()
        {
            var value = "test";
            var value1 = "test";
            TestAssert.AreEqual(Encryptor.ToMd5(value), Encryptor.ToMd5(value1));
        }

        public static void DESEncrypt()
        {
            var value = "test";
            var value1 = "test";
            TestAssert.AreEqual(Encryptor.DESEncrypt(value), Encryptor.DESEncrypt(value1));
        }

        public static void DESDecrypt()
        {
            var value = Encryptor.DESEncrypt("test");
            var value1 = Encryptor.DESEncrypt("test");
            TestAssert.AreEqual(Encryptor.DESDecrypt(value), Encryptor.DESDecrypt(value1));
        }
    }
}
