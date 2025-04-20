using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iMusic.DAL.Settings;

public class JwtSettings
{
    public string Issuer { get; set; }
    public string Audience { get; set; }

    public string Secret { get; set; }

    public int ExpirationInMinutes { get; set; }
    public int RefreshInMinutes { get; set; }
}
