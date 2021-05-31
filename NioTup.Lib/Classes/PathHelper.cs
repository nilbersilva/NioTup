using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NioTup.Lib.Classes
{

    public static class PathHelper
    {
        public static bool ValidatePath(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path).Delete();
                return true;
            }
            catch (Exception )
            {
            }
            return false;           
        }
    }
}
