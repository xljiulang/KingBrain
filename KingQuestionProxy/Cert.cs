using Fiddler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace KingQuestionProxy
{
    /// <summary>
    /// 提供证书安装
    /// </summary>
    public static class Cert
    {
        /// <summary>
        /// 客户端证书文件路径
        /// </summary>
        public const string Client_Cer = "Cert\\client.cer";

        /// <summary>
        /// 根端证
        /// </summary>
        public const string RootCert_Cer = "Cert\\rootCert.cer";
        /// <summary>
        /// 根端证
        /// </summary>
        public const string RootCert_Key = "Cert\\rootCert.key";

        /// <summary>
        /// 证书是否存在
        /// </summary>
        /// <returns></returns>
        public static bool Exists()
        {
            Directory.CreateDirectory("Cert");
            return File.Exists(Client_Cer) && File.Exists(RootCert_Cer) && File.Exists(RootCert_Key);
        }

        /// <summary>
        /// 设置证书
        /// </summary>
        public static void SetRootCertificate()
        {
            if (Cert.Exists() == false)
            {
                var certString = File.ReadAllText(Cert.RootCert_Cer, Encoding.ASCII);
                var keyString = File.ReadAllText(Cert.RootCert_Key, Encoding.ASCII);

                FiddlerApplication.Prefs.SetStringPref("fiddler.certmaker.bc.cert", certString);
                FiddlerApplication.Prefs.SetStringPref("fiddler.certmaker.bc.key", keyString);
            }
            else
            {
                CertMaker.createRootCert();
                var cert = CertMaker.GetRootCertificate();

                var clientCer = cert.Export(X509ContentType.Cert);
                File.WriteAllBytes(Cert.Client_Cer, clientCer);

                var certString = FiddlerApplication.Prefs.GetStringPref("fiddler.certmaker.bc.cert", null);
                var keyString = FiddlerApplication.Prefs.GetStringPref("fiddler.certmaker.bc.key", null);

                File.WriteAllText(Cert.RootCert_Cer, certString, Encoding.ASCII);
                File.WriteAllText(Cert.RootCert_Key, keyString, Encoding.ASCII);
            }
        }
    }
}
