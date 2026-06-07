using System.Buffers.Text;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AspKnP231.Models.User
{
    public class JwtModel
    {
        public JwtHeader Header { get; set; } = new() { Alg = "HS256", Typ = "JWT" };
        public JwtPayload Payload { get; set; } = null!;

        private byte[]? _signature;
        public byte[] Signature { 
            get => _signature ??= Sign(SignedPart);
            set
            {
                _signature = value;
            } 
        }

        public static byte[] Sign(String input) => 
            System.Security.Cryptography.HMACSHA256.HashData(
                    Encoding.UTF8.GetBytes("secret"),
                    Encoding.UTF8.GetBytes(input));

        public static String Sign64(String input) => ToBase64(Sign(input));

        public override string ToString() => SignedPart + '.' + ToBase64(Signature);

        public String SignedPart => ToBase64(JsonSerializer.Serialize(Header, options))
                + '.' + ToBase64(JsonSerializer.Serialize(Payload, options));

        private static String ToBase64(String input) => ToBase64(
                    Encoding.UTF8.GetBytes(input));

        private static String ToBase64(byte[] input) => Encoding.UTF8.GetString(
                Base64Url.EncodeToUtf8(input));

        public static readonly JsonSerializerOptions options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }

    public class JwtHeader
    {
        public String Alg { get; set; } = null!;
        public String Typ { get; set; } = null!;
        public String? Cty { get; set; }
    }

    public class JwtPayload
    {
        public String? Iss { get; set; } = "AspKnP231";
        public String? Sub { get; set; }   // -- NameIdentifier == userAccess.Login
        public String? Aud { get; set; }   // -- Role
        public long? Exp { get; set; }
        public long? Nbf { get; set; }
        public long? Iat { get; set; }
        public String? Jti { get; set; }

        public String Name { get; set; } = null!;
        public String Email { get; set; } = null!;
        public String Dob { get; set; } = null!;   // Date of Birth
        public String? Ava { get; set; }   // Thumbprint == userAccess.AvatarFilename
    }
}
