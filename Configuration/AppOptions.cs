using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteNetLibDebugApp;

public class OptionBase
{
    
}
public class AppOptions: OptionBase
{
    public bool ClientEnabled { get; set; } = true;
    public bool ServerEnabled { get; set; } = true;
}
