using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Model
{
    public interface IModel
    {
        string Type { get; set; }
        string Path { get; set; }
        string Name { get; set; }
        string Size { get; set; }
        string Icon { get; set; }
    }
}
