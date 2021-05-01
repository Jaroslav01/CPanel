using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CPanel.MqttServer
{
    public class AuthOptions
    {
        public const string ISSUER = "CpanelAuthServer"; // издатель токена
        public const string AUDIENCE = "CpanelAuthClient"; // потребитель токена
        const string KEY = "|T].[rQoB;M<W%SzY%5iI!1D${aQ'>&ahX--}cg@_;?cJ|KD[n%'nmyj+YQ+'h0";   // ключ для шифрации
        public const int LIFETIME = 1440; // время жизни токена - 1 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
