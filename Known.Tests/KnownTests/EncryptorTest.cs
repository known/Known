namespace Known.Tests.KnownTests
{
    public class EncryptorTest
    {
        public static void TestToMd5()
        {
            var value = "test";
            var value1 = "test";
            Assert.IsEqual(Encryptor.ToMd5(value), Encryptor.ToMd5(value1));
        }

        public static void TestEncrypt()
        {
            var value = "test";
            var value1 = "test";
            Assert.IsEqual(Encryptor.Encrypt(value), Encryptor.Encrypt(value1));
        }

        public static void TestDecrypt()
        {
            var value = Encryptor.Encrypt("test");
            var value1 = Encryptor.Encrypt("test");
            Assert.IsEqual(Encryptor.Decrypt(value), Encryptor.Decrypt(value1));
        }
    }
}
