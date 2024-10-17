using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary.Models;

public class VanID
{
    public VanID(string vanName, string vanId)
    {
        this.VanName = vanName.ToLower();
        this.VanId = vanId;
    }
    
    public VanID(string vanName)
    {
        this.VanName = vanName.ToLower();
        this.VanId = string.Empty;
    }
    
    // Dapper needs a blank constructor to parse data
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    internal VanID()
    {
        
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public string VanId { get; set; }
    public string VanName { get; init; }
    public bool Blocked { get; set; } = false;

}
