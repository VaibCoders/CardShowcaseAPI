namespace CustomHTMLCardAPI.Configuration;

/// <summary>
/// Настройки валидации JWT токенов
/// </summary>
public class JwtValidationSettings
{
        /// <summary>
        /// Список допустимых аудиторий (AllowedAudiences)
        /// </summary>
        public string[] AllowedAudiences { get; set; } = null!;

        /// <summary>
        /// Издатель (Issuer) токена
        /// </summary>
        public string Issuer { get; set; } = null!;

        /// <summary>
        /// Секретная строка для создания симметричного ключа
        /// </summary>
        public string Secret { get; set; } = null!;

        /// <summary>
        /// Время жизни access-токена в секундах
        /// </summary>
        public long AccessTokenExpiration { get; set; }

        /// <summary>
        /// Время жизни refresh-токена в секундах
        /// </summary>
        public long RefreshTokenExpiration { get; set; }

        /// <summary>
        /// Допустимое расхождение времени (ClockSkew) в секундах при валидации
        /// </summary>
        public int ClockSkew { get; set; } = 0;
}