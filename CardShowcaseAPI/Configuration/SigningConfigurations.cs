using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CardShowcaseAPI.Configuration
{
    /// <summary>
    /// Конфигурация подписи JWT — держит ключ и креденшелы
    /// </summary>
    public class SigningConfigurations
    {
        /// <summary>Симметричный ключ</summary>
        public SecurityKey SecurityKey { get; }

        /// <summary>Креденшелы подписи</summary>
        public SigningCredentials SigningCredentials { get; }

        /// <summary>
        /// Конструктор конфигурации подписи
        /// </summary>
        /// <param name="key">Строка-секрет для HMAC-SHA256</param>
        public SigningConfigurations(string key)
        {
            var keyBytes = Encoding.ASCII.GetBytes(key);
            SecurityKey = new SymmetricSecurityKey(keyBytes);
            SigningCredentials = new SigningCredentials(
                SecurityKey, 
                SecurityAlgorithms.HmacSha256Signature
            );
        }
    }
}
